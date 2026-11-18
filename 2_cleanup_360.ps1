# ============================================================
# File 2: Xoa 360 Total Security
# Chay boi service 360Remover as SYSTEM trong Safe Mode
# ============================================================

$LOG    = "C:\360_removal_log.txt"
$SVC    = "360Remover"
$P2PATH = "C:\360_phase2_cleanup.ps1"

# Doc ngon ngu tu file tam
$Lang = "VI"
if (Test-Path "C:\360_lang.txt") {
    $raw = [System.IO.File]::ReadAllText("C:\360_lang.txt")
    if ($raw -match "LANG=EN") { $Lang = "EN" }
    Remove-Item "C:\360_lang.txt" -Force -ErrorAction SilentlyContinue
}

function Log {
    param([string]$msg, [string]$color = "White")
    $line = "$(Get-Date -Format 'HH:mm:ss') $msg"
    try { $line | Out-File $LOG -Append -Encoding UTF8 } catch {}
    Write-Host $line -ForegroundColor $color
}

function Remove-Force360 {
    param([string]$Path)
    if (-not (Test-Path $Path)) { return "SKIP" }

    # Use a GUID-based temp dir to avoid collisions
    $tmp = "C:\__empty_$([guid]::NewGuid().ToString('N'))"
    try {
        $qPath = "`"$Path`""
        $null = Start-Process -FilePath "takeown.exe" -ArgumentList "/f $qPath /r /d y" `
            -NoNewWindow -Wait -PassThru 2>&1
        $null = Start-Process -FilePath "icacls.exe" -ArgumentList "$qPath /grant Administrators:F /t /q" `
            -NoNewWindow -Wait -PassThru 2>&1

        $null = New-Item -ItemType Directory -Path $tmp -Force
        $null = Start-Process -FilePath "robocopy.exe" -ArgumentList "`"$tmp`" $qPath /MIR /NFL /NDL /NJH /NJS /NC /NS /R:1 /W:1" `
            -NoNewWindow -Wait -PassThru 2>&1

        Remove-Item $Path -Recurse -Force -ErrorAction SilentlyContinue
        if (Test-Path $Path) {
            $null = Start-Process -FilePath "cmd.exe" -ArgumentList "/c rd /s /q $qPath" `
                -NoNewWindow -Wait -PassThru 2>&1
        }
    }
    finally {
        if (Test-Path $tmp) {
            Remove-Item $tmp -Recurse -Force -ErrorAction SilentlyContinue
        }
    }

    # If still stuck, schedule remaining files for deletion on reboot
    if (Test-Path $Path) {
        Get-ChildItem $Path -Recurse -Force -ErrorAction SilentlyContinue | ForEach-Object {
            $movefile = @("[DllImport(`"kernel32.dll`", SetLastError=true, CharSet=CharSet.Unicode)]",
                "public static extern bool MoveFileEx(string lpExistingFileName, string lpNewFileName, int dwFlags);")
            try {
                $type = Add-Type -MemberDefinition ($movefile -join "`n") -Name "MoveFileUtils" -Namespace "Win32" -PassThru -ErrorAction SilentlyContinue
            } catch {
                $type = [Win32.MoveFileUtils]
            }
            $null = $type::MoveFileEx($_.FullName, $null, 4)
        }
        $null = ([Win32.MoveFileUtils])::MoveFileEx($Path, $null, 4)
        return "REBOOT"
    }
    return "OK"
}

# ---- Strings ----
if ($Lang -eq "VI") {
    $s_title    = "=== [2/3] Don dep 360 Total Security ==="
    $s_safe     = "Phat hien Safe Mode"
    $s_warn     = "CANH BAO: Khong phat hien Safe Mode - van tiep tuc"
    $s_svc_hdr  = "[1] Xoa services 360..."
    $s_fld_hdr  = "[2] Xoa thu muc 360..."
    $s_reg_hdr  = "[3] Xoa registry..."
    $s_fw_hdr   = "[4] Xoa Firewall rules..."
    $s_cut_hdr  = "[5] Xoa shortcuts..."
    $s_cln_hdr  = "[6] Tu don service $SVC..."
    $s_done     = "=== DON DEP HOAN TAT ==="
    $s_reboot   = "Khoi dong lai vao Normal Mode trong 5 giay..."
    $s_finish   = "=== HOAN TAT TOAN BO QUA TRINH XOA 360 ==="
    $s_deleting = "Dang xoa"
    $s_noadmin  = "LOI: Script can quyen Administrator!"
} else {
    $s_title    = "=== [2/3] Cleaning up 360 Total Security ==="
    $s_safe     = "Safe Mode detected"
    $s_warn     = "WARNING: Safe Mode not detected - continuing anyway"
    $s_svc_hdr  = "[1] Removing 360 services..."
    $s_fld_hdr  = "[2] Removing 360 folders..."
    $s_reg_hdr  = "[3] Removing registry entries..."
    $s_fw_hdr   = "[4] Removing Firewall rules..."
    $s_cut_hdr  = "[5] Removing shortcuts..."
    $s_cln_hdr  = "[6] Cleaning up service $SVC..."
    $s_done     = "=== CLEANUP COMPLETE ==="
    $s_reboot   = "Rebooting to Normal Mode in 5 seconds..."
    $s_finish   = "=== 360 TOTAL SECURITY REMOVAL FINISHED ==="
    $s_deleting = "Deleting"
    $s_noadmin  = "ERROR: This script requires Administrator privileges!"
}

# Tu dong UAC elevation neu chua co quyen Admin
# (can thiet khi chay tu Startup folder — user context, khong phai Admin)
$currentPrincipal = [Security.Principal.WindowsPrincipal][Security.Principal.WindowsIdentity]::GetCurrent()
if (-not $currentPrincipal.IsInRole([Security.Principal.WindowsBuiltInRole]::Administrator)) {
    $selfPath = $MyInvocation.MyCommand.Path
    if (-not $selfPath) { $selfPath = $MyInvocation.MyCommand.Definition }
    if ($selfPath) {
        Start-Process powershell -Verb RunAs -ArgumentList "-ExecutionPolicy Bypass -NonInteractive -File `"$selfPath`""
        exit
    }
    Log $s_noadmin "Red"
    exit 1
}

# Doi Windows load xong trong Safe Mode
$safeMode = Test-Path "HKLM:\SYSTEM\CurrentControlSet\Control\SafeBoot\Option"
if ($safeMode) { Start-Sleep 20 }

Log "" ; Log $s_title "Cyan"
Log "  User: $env:USERNAME | ComputerName: $env:COMPUTERNAME"

# Phat hien Safe Mode
Log "  $s_safe : $safeMode" $(if ($safeMode) { "Green" } else { "Yellow" })
if (-not $safeMode) { Log "  $s_warn" "Yellow" }

$explorerKilled = $false

# ---- [0] Kill 360 processes ----
foreach ($proc in @("360Tray","360Safe","360sd","360rps","360rp","QHSafeMain",
                    "QHActiveDefense","QHSafeTray","QHWatchdog",
                    "360leakfixer","360TSLiveUpd8","360TsLiveUpd",
                    "LiveUpdate360","PopWndLog")) {
    $null = Start-Process -FilePath "taskkill.exe" -ArgumentList "/f /im $proc.exe" `
        -NoNewWindow -Wait -PassThru 2>&1
}

# ---- [1] Services ----
Log "" ; Log $s_svc_hdr "Cyan"
foreach ($s in @("QHActiveDefense","QHProtected","360rp","360Safe","360TS",
                 "360FsFlt","360AvFlt","360netmon","360Box64","qutmdrv",
                 "360TsLiveUpd","360TSLiveUpd8")) {
    $null = sc.exe stop $s 2>&1
    $r = sc.exe delete $s 2>&1
    if ("$r" -match "SUCCESS") { Log "  [OK] $s" "Green" }
    else { Log "  [--] $s" }
}

# ---- [2] Folders ----
Log "" ; Log $s_fld_hdr "Cyan"
$folders = New-Object System.Collections.Generic.List[string]
foreach ($f in @(
    "C:\Program Files\360\Total Security",
    "C:\Program Files\360",
    "C:\Program Files (x86)\360\Total Security",
    "C:\Program Files (x86)\360",
    "C:\ProgramData\360Safe",
    "C:\ProgramData\360",
    "C:\ProgramData\360TotalSecurity",
    "C:\ProgramData\360Quarant",
    "C:\360SANDBOX",
    "C:\Windows\Tasks\360Disabled",
    "C:\Windows\SysWOW64\config\systemprofile\AppData\Roaming\360safe",
    "C:\Windows\SysWOW64\config\systemprofile\AppData\LocalLow\360WD"
)) { $folders.Add($f) }

Get-ChildItem "C:\Users" -ErrorAction SilentlyContinue | Where-Object { $_.PSIsContainer } | ForEach-Object {
    $u = $_.FullName
    # Roaming
    $folders.Add("$u\AppData\Roaming\360Safe")
    $folders.Add("$u\AppData\Roaming\360TotalSecurity")
    $folders.Add("$u\AppData\Roaming\360TotalSecurityLow")
    $folders.Add("$u\AppData\Roaming\360DesktopLite")
    $folders.Add("$u\AppData\Roaming\h8n_360ts")
    # Desktop
    $folders.Add("$u\Desktop\360_Downloads")
    # Local
    $folders.Add("$u\AppData\Local\360Safe")
    $folders.Add("$u\AppData\Local\360TotalSecurity")
    $folders.Add("$u\AppData\Local\360TotalSecurityLow")
    $folders.Add("$u\AppData\Local\360DesktopLite")
    # LocalLow
    $folders.Add("$u\AppData\LocalLow\360MenuMgr")
    $folders.Add("$u\AppData\LocalLow\360WD")
    $folders.Add("$u\AppData\LocalLow\360Safe")
    $folders.Add("$u\AppData\LocalLow\360TotalSecurity")
}
# Temp install folders: 360_install_* in all user Temp dirs
Get-ChildItem "C:\Users" -ErrorAction SilentlyContinue | Where-Object { $_.PSIsContainer } | ForEach-Object {
    $tmp = "$($_.FullName)\AppData\Local\Temp"
    if (Test-Path $tmp) {
        Get-ChildItem $tmp -Filter "360_install_*" -ErrorAction SilentlyContinue | Where-Object { $_.PSIsContainer } |
            ForEach-Object { $folders.Add($_.FullName) }
    }
}
foreach ($f in $folders) {
    if (Test-Path $f) {
        Log "  $s_deleting : $f" "Yellow"
        $r = Remove-Force360 -Path $f
        if ($r -eq "FAIL" -or $r -eq "REBOOT") {
            if (-not $explorerKilled) {
                Log "  Killing explorer.exe to release DLLs..." "Yellow"
                $null = Start-Process -FilePath "taskkill.exe" -ArgumentList "/f /im explorer.exe" `
                    -NoNewWindow -Wait -PassThru 2>&1
                $explorerKilled = $true
                Start-Sleep 2
            }
            $r = Remove-Force360 -Path $f
        }
        if     ($r -eq "OK")     { Log "  [OK]     $f" "Green" }
        elseif ($r -eq "REBOOT") { Log "  [REBOOT] $f (scheduled for deletion on reboot)" "Yellow" }
        elseif ($r -eq "SKIP")   { Log "  [--]     $f" }
        else                     { Log "  [FAIL]   $f" "Red" }
    }
}

# ---- [3] Registry ----
Log "" ; Log $s_reg_hdr "Cyan"

# [3a] Known 360 keys (explicit, high-confidence)
foreach ($k in @(
    "HKLM:\SOFTWARE\Microsoft\Security Center\Provider\Av\{FFDC234A-CE9B-08F9-406B-F876951CE066}",
    "HKLM:\SOFTWARE\WOW6432Node\360TotalSecurity",
    "HKLM:\SOFTWARE\WOW6432Node\Microsoft\Windows\CurrentVersion\Uninstall\360TotalSecurity",
    "HKLM:\SOFTWARE\Microsoft\Windows\CurrentVersion\Uninstall\360TotalSecurity",
    "HKLM:\SOFTWARE\360Safe",
    "HKLM:\SOFTWARE\WOW6432Node\360Safe",
    "HKCU:\SOFTWARE\360",
    "HKCU:\SOFTWARE\360Safe",
    "HKCU:\SOFTWARE\360se6",
    "HKCU:\SOFTWARE\360desktoplite",
    "HKCU:\SOFTWARE\360SoftMgr",
    "HKCU:\SOFTWARE\360TotalSecurity",
    "HKCU:\SOFTWARE\LiveUpdate360",
    "HKCU:\SOFTWARE\Classes\.TotalSecurity",
    "HKCU:\SOFTWARE\Classes\360TotalSecurity.ext.1",
    "HKCU:\SOFTWARE\Classes\ixnloginserver-360ts",
    # SD360 shell-extension registrations
    "HKLM:\SOFTWARE\Classes\*\shellex\ContextMenuHandlers\SD360",
    "HKLM:\SOFTWARE\Classes\Directory\shellex\ContextMenuHandlers\SD360",
    "HKLM:\SOFTWARE\Classes\Folder\shellex\ContextMenuHandlers\SD360",
    "HKLM:\SOFTWARE\WOW6432Node\Classes\*\shellex\ContextMenuHandlers\SD360",
    "HKLM:\SOFTWARE\WOW6432Node\Classes\Directory\shellex\ContextMenuHandlers\SD360",
    "HKLM:\SOFTWARE\WOW6432Node\Classes\Folder\shellex\ContextMenuHandlers\SD360"
)) {
    if (Test-Path -LiteralPath $k) {
        Remove-Item -LiteralPath $k -Recurse -Force -ErrorAction SilentlyContinue
        Log "  [OK] $k" "Green"
    }
}

Log "  [3b] Pattern check..."
# [3b] Direct check for known 360 subkey names (fast, no hive enumeration)
$360KeyNames = @("360Safe","360TotalSecurity","360TS","360rp","360se6","360desktoplite",
    "360SoftMgr","360Speedld","360WD","360MenuMgr","360softmgr","360netmon")
foreach ($root in @("HKLM:\SOFTWARE", "HKLM:\SOFTWARE\WOW6432Node", "HKCU:\SOFTWARE")) {
    foreach ($name in $360KeyNames) {
        $kpath = "$root\$name"
        if (Test-Path $kpath) {
            Remove-Item $kpath -Recurse -Force -ErrorAction SilentlyContinue
            Log "  [OK] $kpath" "Green"
        }
    }
}

Log "  [3c] Uninstall entries..."
# [3c] Uninstall entries — stricter match to avoid removing unrelated software (e.g. "Xbox 360")
$360UninstallPatterns = @(
    '360 Total Security',
    '360 Safe',
    '360TotalSecurity',
    '360Safe',
    '360 Security',
    '360 Desktop Lite',
    '360DesktopLite'
)
foreach ($uroot in @(
    "HKLM:\SOFTWARE\Microsoft\Windows\CurrentVersion\Uninstall",
    "HKLM:\SOFTWARE\WOW6432Node\Microsoft\Windows\CurrentVersion\Uninstall",
    "HKCU:\SOFTWARE\Microsoft\Windows\CurrentVersion\Uninstall")) {
    if (-not (Test-Path $uroot)) { continue }
    Get-ChildItem $uroot -ErrorAction SilentlyContinue | ForEach-Object {
        $dn = (Get-ItemProperty $_.PSPath -ErrorAction SilentlyContinue).DisplayName
        if ($dn) {
            $matched = $false
            foreach ($pattern in $360UninstallPatterns) {
                if ($dn -like "*$pattern*") { $matched = $true; break }
            }
            if ($matched) {
                Remove-Item $_.PSPath -Recurse -Force -ErrorAction SilentlyContinue
                Log "  [OK] Uninstall entry: $dn" "Green"
            }
        }
    }
}

Log "  [3d] MuiCache..."
# [3d] MUI cache values referencing 360 paths
foreach ($mc in @(
    "HKLM:\SOFTWARE\Classes\Local Settings\Software\Microsoft\Windows\Shell\MuiCache",
    "HKCU:\SOFTWARE\Classes\Local Settings\Software\Microsoft\Windows\Shell\MuiCache")) {
    if (Test-Path $mc) {
        $item = Get-Item $mc -ErrorAction SilentlyContinue
        if ($item) {
            $item.GetValueNames() | Where-Object { $_ -match '\\360\\' -or $_ -match '\\360(Safe|Total|TS)' } | ForEach-Object {
                Remove-ItemProperty -Path $mc -Name $_ -ErrorAction SilentlyContinue
                Log "  [OK] MuiCache: $_" "Green"
            }
        }
    }
}

Log "  [3e] UFH\\SHC..."
# [3e] UFH\SHC — User File History shortcut cache (binary values embedding 360 paths)
foreach ($shcRoot in @(
    "HKCU:\SOFTWARE\Microsoft\Windows\CurrentVersion\UFH\SHC")) {
    if (Test-Path $shcRoot) {
        $shcItem = Get-Item $shcRoot -ErrorAction SilentlyContinue
        if ($shcItem) {
            $shcItem.GetValueNames() | ForEach-Object {
                $val = $shcItem.GetValue($_)
                $isMatch = $false
                if ($val -is [byte[]]) {
                    $text = [System.Text.Encoding]::Unicode.GetString($val)
                    if ($text -match '360(Safe|Total|TS|\\)') { $isMatch = $true }
                } elseif ($val -is [string]) {
                    if ($val -match '360(Safe|Total|TS|\\)') { $isMatch = $true }
                }
                if ($isMatch) {
                    Remove-ItemProperty -Path $shcRoot -Name $_ -ErrorAction SilentlyContinue
                    Log "  [OK] UFH\SHC: $_" "Green"
                }
            }
        }
    }
}

Log "  [3f] PendingFileRenameOperations..."
# [3f] PendingFileRenameOperations — filter out 360 entries, keep the rest
$pfroPath = "HKLM:\SYSTEM\CurrentControlSet\Control\Session Manager"
if (Test-Path $pfroPath) {
    try {
        $pfroVal = (Get-ItemProperty $pfroPath -Name "PendingFileRenameOperations" -ErrorAction Stop).PendingFileRenameOperations
        if ($pfroVal) {
            $cleaned = New-Object System.Collections.Generic.List[string]
            $removed = 0
            for ($i = 0; $i -lt $pfroVal.Count; $i += 2) {
                $src = $pfroVal[$i]
                $dst = if ($i + 1 -lt $pfroVal.Count) { $pfroVal[$i + 1] } else { "" }
                if ($src -match '360(Safe|Total|TS|\\360\\)' -or $dst -match '360(Safe|Total|TS|\\360\\)') {
                    $removed++
                } else {
                    $cleaned.Add($src)
                    $cleaned.Add($dst)
                }
            }
            if ($removed -gt 0) {
                if ($cleaned.Count -eq 0) {
                    Remove-ItemProperty -Path $pfroPath -Name "PendingFileRenameOperations" -ErrorAction SilentlyContinue
                } else {
                    Set-ItemProperty -Path $pfroPath -Name "PendingFileRenameOperations" -Value ([string[]]$cleaned.ToArray()) -ErrorAction SilentlyContinue
                }
                Log "  [OK] PendingFileRenameOperations: removed $removed 360 entries" "Green"
            }
        }
    } catch {}
}

# ---- [4] Firewall ----
Log "" ; Log $s_fw_hdr "Cyan"
$fwPath = "HKLM:\SYSTEM\CurrentControlSet\Services\SharedAccess\Parameters\FirewallPolicy\FirewallRules"
if (Test-Path $fwPath) {
    $fwItem = Get-Item $fwPath -ErrorAction SilentlyContinue
    foreach ($g in @("{2FFADE84-AD17-4DA3-85CF-27CFA3EAF9E8}",
                     "{7B1321C8-56D0-446A-A217-6300341FBAAB}",
                     "{A0C2022A-8869-46B0-A058-FB734FD11C5B}",
                     "{129BA0C1-212B-487F-A840-8B847FEBB7E7}")) {
        if ($fwItem -and ($fwItem.GetValueNames() -contains $g)) {
            Remove-ItemProperty -Path $fwPath -Name $g -ErrorAction SilentlyContinue
            Log "  [OK] $g" "Green"
        } else {
            Log "  [--] $g (not found)"
        }
    }
}

# ---- [5] Shortcuts ----
Log "" ; Log $s_cut_hdr "Cyan"
$cuts = New-Object System.Collections.Generic.List[string]
$cuts.Add("C:\ProgramData\Microsoft\Windows\Start Menu\Programs\360 Security Center")
$cuts.Add("C:\Users\Public\Desktop\360 Total Security.lnk")
Get-ChildItem "C:\Users" -ErrorAction SilentlyContinue | Where-Object { $_.PSIsContainer } | ForEach-Object {
    $cuts.Add("$($_.FullName)\AppData\Roaming\Microsoft\Windows\Start Menu\Programs\360 Security Center")
    $cuts.Add("$($_.FullName)\Desktop\360 Total Security.lnk")
}
foreach ($s in $cuts) {
    if (Test-Path $s) {
        Remove-Item $s -Recurse -Force -ErrorAction SilentlyContinue
        Log "  [OK] $s" "Green"
    }
}

# ---- [6] Tu don RunOnce / service cu / file tam ----
Log "" ; Log $s_cln_hdr "Cyan"
$null = Start-Process -FilePath "reg.exe" -ArgumentList @("delete", "HKLM\SOFTWARE\Microsoft\Windows\CurrentVersion\RunOnce", "/v", "*$SVC", "/f") `
    -NoNewWindow -Wait -PassThru 2>&1
$null = Start-Process -FilePath "reg.exe" -ArgumentList @("delete", "HKLM\SYSTEM\CurrentControlSet\Control\SafeBoot\Minimal\$SVC", "/f") `
    -NoNewWindow -Wait -PassThru 2>&1
$null = sc.exe stop $SVC 2>&1
$null = sc.exe delete $SVC 2>&1
Remove-Item $P2PATH -Force -ErrorAction SilentlyContinue
Remove-Item "C:\360_run_cleanup.bat" -Force -ErrorAction SilentlyContinue
# Xoa Startup folder fallback
$startupDir = "C:\ProgramData\Microsoft\Windows\Start Menu\Programs\Startup"
Remove-Item (Join-Path $startupDir "360_run_cleanup.bat") -Force -ErrorAction SilentlyContinue
$startupDir2 = [System.Environment]::GetFolderPath("CommonStartup")
if ($startupDir2) { Remove-Item (Join-Path $startupDir2 "360_run_cleanup.bat") -Force -ErrorAction SilentlyContinue }
Log "  [OK] $SVC RunOnce/Startup/bat/temp files removed" "Green"

# ---- Reboot Normal Mode ----
Log "" ; Log $s_done "Green"
# Xoa safeboot flag de Windows boot lai Normal Mode
# Thu bcdedit truoc, neu fail thu reg delete truc tiep
$r = bcdedit /deletevalue "{current}" safeboot 2>&1
Log "  bcdedit deletevalue: $r" $(if ("$r" -match "success") { "Green" } else { "Yellow" })
if ("$r" -notmatch "success") {
    # Fallback: xoa truc tiep registry key SafeBoot
    $null = Start-Process -FilePath "reg.exe" `
        -ArgumentList "delete `"HKLM\SYSTEM\CurrentControlSet\Control\SafeBoot`" /v `"Option`" /f" `
        -NoNewWindow -Wait -PassThru 2>&1
    $r2 = bcdedit /deletevalue safeboot 2>&1
    Log "  bcdedit fallback: $r2" $(if ("$r2" -match "success") { "Green" } else { "Red" })
}
Log "  $s_reboot" "Yellow"
Log $s_finish "Green"
# Restart explorer if it was killed (before reboot, in case reboot fails)
if ($explorerKilled) {
    $null = Start-Process -FilePath "explorer.exe" -ErrorAction SilentlyContinue
}
Start-Sleep 5
$null = shutdown.exe /r /t 0 /f 2>&1
if ($LASTEXITCODE -ne 0) {
    $null = Start-Process -FilePath "cmd.exe" -ArgumentList "/c shutdown /r /t 0 /f" `
        -NoNewWindow -Wait -PassThru 2>&1
}
