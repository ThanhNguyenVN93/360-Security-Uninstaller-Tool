# ============================================================
# File 1: Boot to Safe Mode
# Chay o Normal Mode - tu dong UAC elevation
# ============================================================

param([string]$Lang = "VI")

# PS 2.0 compatible script directory (PSScriptRoot is PS 3.0+)
$selfPath = $MyInvocation.MyCommand.Path
if (-not $selfPath) { $selfPath = $MyInvocation.MyCommand.Definition }
$scriptDir = Split-Path -Parent $selfPath
if (-not $scriptDir) { $scriptDir = $PWD.Path }

# Tu dong UAC elevation
if (-not ([Security.Principal.WindowsPrincipal][Security.Principal.WindowsIdentity]::GetCurrent()).IsInRole(
        [Security.Principal.WindowsBuiltInRole]"Administrator")) {
    Start-Process powershell -Verb RunAs -ArgumentList "-ExecutionPolicy Bypass -File `"$selfPath`" -Lang $Lang"
    exit
}

$SVC    = "360Remover"
$P2PATH = "C:\360_phase2_cleanup.ps1"
$BATPATH = "C:\360_run_cleanup.bat"
$LOG    = "C:\360_removal_log.txt"

# ---- Strings theo ngon ngu ----
if ($Lang -eq "VI") {
    $s_title    = "=== [1/3] Khoi dong vao Safe Mode ==="
    $s_copy     = "[1] Sao chep script don dep"
    $s_nosrc    = "LOI: Khong tim thay 2_cleanup_360.ps1 cung thu muc!"
    $s_svc      = "[2] Dang ky RunOnce (chay ca trong Safe Mode)"
    $s_bcd      = "[3] Cai dat Safe Mode boot"
    $s_done1    = "HOAN TAT BUOC 1!"
    $s_reboot   = "Khoi dong lai vao Safe Mode trong 10 giay..."
    $s_info1    = "Sau khi may vao Safe Mode, HAY DANG NHAP vao tai khoan Windows"
    $s_info2    = "cua ban - script don dep se TU DONG chay ngay sau khi dang nhap"
    $s_info3    = "Sau do tu dong boot lai vao Normal Mode"
    $s_cancel   = "(Nhan Ctrl+C de huy)"
} else {
    $s_title    = "=== [1/3] Booting into Safe Mode ==="
    $s_copy     = "[1] Copying cleanup script"
    $s_nosrc    = "ERROR: Cannot find 2_cleanup_360.ps1 in the same folder!"
    $s_svc      = "[2] Registering RunOnce (runs even in Safe Mode)"
    $s_bcd      = "[3] Setting Safe Mode boot"
    $s_done1    = "STEP 1 COMPLETE!"
    $s_reboot   = "Rebooting into Safe Mode in 10 seconds..."
    $s_info1    = "Once the PC is in Safe Mode, LOG IN to your Windows account"
    $s_info2    = "- the cleanup script will run AUTOMATICALLY right after you log in"
    $s_info3    = "Then it will automatically reboot back to Normal Mode"
    $s_cancel   = "(Press Ctrl+C to cancel)"
}

# Khoi tao log
"=== 360 Removal Log - $(Get-Date) | Lang=$Lang ===" | Out-File $LOG -Encoding UTF8

Write-Host ""
Write-Host $s_title -ForegroundColor Cyan
Write-Host ""

# [1] Copy file cleanup sang C:\
$cleanupSrc = Join-Path $scriptDir "2_cleanup_360.ps1"
if (-not (Test-Path $cleanupSrc)) {
    Write-Host $s_nosrc -ForegroundColor Red
    "[FAIL] $s_nosrc" | Out-File $LOG -Append
    pause; exit 1
}
Copy-Item $cleanupSrc $P2PATH -Force
# Ghi lang vao dong dau file de phase2 biet
"# LANG=$Lang" | Set-Content "C:\360_lang.txt" -Encoding UTF8
"$s_copy -> $P2PATH" | Out-File $LOG -Append
Write-Host "$s_copy -> $P2PATH" -ForegroundColor Green

# [2] Don dep entry cu (tu lan chay truoc)
$null = sc.exe stop  $SVC 2>&1
$null = sc.exe delete $SVC 2>&1
$safeBootPath = "HKLM:\SYSTEM\CurrentControlSet\Control\SafeBoot\Minimal\$SVC"
if (Test-Path $safeBootPath) { Remove-Item $safeBootPath -Force -ErrorAction SilentlyContinue }
# Xoa RunOnce cu bang .NET (tranh PowerShell wildcard * issue)
try {
    $roKey = [Microsoft.Win32.Registry]::LocalMachine.OpenSubKey("SOFTWARE\Microsoft\Windows\CurrentVersion\RunOnce", $true)
    if ($roKey) { $roKey.DeleteValue("*$SVC", $false); $roKey.Close() }
} catch {}
Remove-Item $BATPATH -Force -ErrorAction SilentlyContinue
# Xoa Startup folder fallback cu
$startupDir = "C:\ProgramData\Microsoft\Windows\Start Menu\Programs\Startup"
$startupBat = Join-Path $startupDir "360_run_cleanup.bat"
Remove-Item $startupBat -Force -ErrorAction SilentlyContinue

# [3] Tao bat file wrapper + dang ky nhieu co che tu dong chay
$batContent = "@echo off`r`nstart `"`" powershell.exe -ExecutionPolicy Bypass -NonInteractive -File `"$P2PATH`"`r`ndel `"%~f0`"`r`n"
[System.IO.File]::WriteAllText($BATPATH, $batContent)
"[2] Created $BATPATH" | Out-File $LOG -Append

# --- Co che 1: RunOnce voi tien to * (chay ca trong Safe Mode) ---
# Dung .NET Registry.SetValue() truc tiep de tranh PowerShell xu ly * thanh wildcard
try {
    [Microsoft.Win32.Registry]::SetValue(
        "HKEY_LOCAL_MACHINE\SOFTWARE\Microsoft\Windows\CurrentVersion\RunOnce",
        "*$SVC",
        $BATPATH,
        [Microsoft.Win32.RegistryValueKind]::String
    )
    "[3a] RunOnce OK via .NET Registry.SetValue" | Out-File $LOG -Append
    Write-Host "  [OK] RunOnce *$SVC" -ForegroundColor Green
} catch {
    "[3a] RunOnce FAIL: $_" | Out-File $LOG -Append
    Write-Host "  [FAIL] RunOnce: $_" -ForegroundColor Red
}

# Verify RunOnce bang .NET (doc lai gia tri)
try {
    $roKey2 = [Microsoft.Win32.Registry]::LocalMachine.OpenSubKey("SOFTWARE\Microsoft\Windows\CurrentVersion\RunOnce", $false)
    if ($roKey2) {
        $val = $roKey2.GetValue("*$SVC")
        $roKey2.Close()
        "[3a] Verify: *$SVC = $val" | Out-File $LOG -Append
        Write-Host "  Verify: *$SVC = $val" -ForegroundColor Gray
    }
} catch {}

# --- Co che 2 (fallback): Copy bat vao Startup folder ---
# Explorer.exe load cac file trong Startup folder khi shell khoi dong,
# KE CA trong Safe Mode. Day la fallback neu RunOnce khong hoat dong.
if (Test-Path $startupDir) {
    Copy-Item $BATPATH $startupBat -Force
    "[3b] Startup folder OK: $startupBat" | Out-File $LOG -Append
    Write-Host "  [OK] Startup folder fallback" -ForegroundColor Green
} else {
    # Win7 co the dung duong dan khac
    $startupDir2 = [System.Environment]::GetFolderPath("CommonStartup")
    if ($startupDir2 -and (Test-Path $startupDir2)) {
        $startupBat2 = Join-Path $startupDir2 "360_run_cleanup.bat"
        Copy-Item $BATPATH $startupBat2 -Force
        "[3b] Startup folder OK (CommonStartup): $startupBat2" | Out-File $LOG -Append
        Write-Host "  [OK] Startup folder fallback (CommonStartup)" -ForegroundColor Green
    } else {
        "[3b] Startup folder: directory not found" | Out-File $LOG -Append
        Write-Host "  [--] Startup folder not found" -ForegroundColor Yellow
    }
}

Write-Host $s_svc -ForegroundColor Green

# [4] Set safeboot
$r = bcdedit /set "{current}" safeboot minimal 2>&1
"$s_bcd : $r" | Out-File $LOG -Append
Write-Host "$s_bcd : $r" -ForegroundColor Green

Write-Host ""
Write-Host "==========================================" -ForegroundColor Yellow
Write-Host "  $s_done1" -ForegroundColor Yellow
Write-Host "  $s_info1" -ForegroundColor White
Write-Host "  $s_info2" -ForegroundColor White
Write-Host "  $s_info3" -ForegroundColor White
Write-Host "==========================================" -ForegroundColor Yellow
Write-Host ""
Write-Host "  $s_reboot" -ForegroundColor Red
Write-Host "  $s_cancel" -ForegroundColor Gray
Write-Host ""
Start-Sleep 10
shutdown.exe /r /t 0 /f
