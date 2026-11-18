using System;
using System.Diagnostics;
using System.IO;
using System.Management;
using System.Reflection;
using System.Windows.Forms;

namespace frm360security
{
    public partial class Form1 : Form
    {
        private const string DonateUrl = "https://nguyenthanhdev.com/donate/";
        private const string GitHubUrl = "https://github.com/thanhitma9x/360-Security-Uninstaller-Tool/issues";
        private const string LogPath = @"C:\360_removal_log.txt";
        private const string BootScript = "1_boot_safemode.ps1";
        private const string CleanupScript = "2_cleanup_360.ps1";

        private struct InstallInfo
        {
            public bool DirFound;
            public string UninstallExePath; // null if not found
        }

        private InstallInfo _lastInstallInfo;

        public Form1()
        {
            InitializeComponent();
            cmbLang.SelectedIndex = 0; // VI
        }

        private bool IsEnglish
        {
            get { return cmbLang.SelectedItem != null && cmbLang.SelectedItem.ToString() == "EN"; }
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            Program.Kill360Processes();
            AppendProgress(IsEnglish ? "Ready." : "San sang.");

            // He dieu hanh / Operating system
            try
            {
                string r = "";
                using (ManagementObjectSearcher searcher = new ManagementObjectSearcher("SELECT * FROM Win32_OperatingSystem"))
                {
                    foreach (ManagementObject obj in searcher.Get())
                    {
                        r = obj["Caption"].ToString() + " - " + obj["OSArchitecture"].ToString();
                    }
                }
                r = r.Replace("NT 5.1.2600", "XP").Replace("NT 5.2.3790", "Server 2003");
                lblinfo.Text = r;
            }
            catch
            {
                lblinfo.Text = Environment.OSVersion.VersionString;
            }

            _lastInstallInfo = DetectInstall();
            ApplyLanguage();
            ModeChanged(this, EventArgs.Empty);
        }

        private InstallInfo DetectInstall()
        {
            InstallInfo info = new InstallInfo();
            // Environment.SpecialFolder.ProgramFilesX86 doi hoi .NET 4.0+, nen
            // tu lay qua bien moi truong de tuong thich .NET 3.5 (co san tren Win7).
            string pfx86 = Environment.GetEnvironmentVariable("ProgramFiles(x86)");
            string pf = Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles);
            string[] roots = string.IsNullOrEmpty(pfx86)
                ? new[] { pf }
                : new[] { pfx86, pf };
            foreach (string root in roots)
            {
                if (string.IsNullOrEmpty(root)) continue;
                string folderPath = Path.Combine(Path.Combine(root, "360"), "Total Security");
                string filePath = Path.Combine(folderPath, "Uninstall.exe");
                if (Directory.Exists(folderPath))
                {
                    info.DirFound = true;
                    if (File.Exists(filePath))
                    {
                        info.UninstallExePath = filePath;
                        return info;
                    }
                }
            }
            return info;
        }

        // Cap nhat toan bo text giao dien theo ngon ngu dang chon
        private void ApplyLanguage()
        {
            bool en = IsEnglish;

            grbstatus.Text = en ? "STATUS" : "TRANG THAI";
            grbmode.Text = en ? "EXECUTION MODE" : "CHE DO THUC HIEN";
            rbAuto.Text = en ? "Automatic (recommended) - one click" : "Tu dong (khuyen nghi) - 1 cham xong";
            rbManual.Text = en ? "Manual - choose each step" : "Thu cong - tu chon tung buoc";
            grbactions.Text = en ? "ACTIONS" : "THUC HIEN";
            lblAutoHint.Text = en
                ? "One click: reboot into Safe Mode, completely remove all 360 files/registry, then reboot back to Normal Mode automatically."
                : "1 cham: khoi dong lai vao Safe Mode, xoa sach toan bo file/registry cua 360, tu dong quay lai Normal Mode.";
            btnAutoStart.Text = en ? "START REMOVING 360 TOTAL SECURITY" : "BAT DAU GO BO 360 TOTAL SECURITY";
            btnManualUninstall.Text = en ? "1. Run official uninstaller" : "1. Go cai dat chinh thuc";
            btnManualSafeClean.Text = en ? "2. Boot Safe Mode && Clean up" : "2. Khoi dong Safe Mode && Don dep";
            btnViewLog.Text = en ? "View Log" : "Xem Log";
            chkRestorePoint.Text = en
                ? "Create a system restore point before cleaning (recommended)"
                : "Tao diem khoi phuc he thong truoc khi don dep (khuyen nghi)";
            btnDonate.Text = en ? "Support the author" : "Ung ho tac gia";
            linkReport.Text = en ? "Report a bug / Contribute (GitHub)" : "Bao loi / Dong gop (GitHub)";
            grbprogress.Text = en ? "PROGRESS" : "TIEN TRINH";

            RefreshDetectLabel();
        }

        private void RefreshDetectLabel()
        {
            bool en = IsEnglish;
            if (_lastInstallInfo.DirFound && _lastInstallInfo.UninstallExePath != null)
            {
                lblDetect.ForeColor = System.Drawing.Color.FromArgb(192, 0, 0);
                lblDetect.Text = en
                    ? "360 Total Security is currently installed."
                    : "Da phat hien 360 Total Security dang cai dat tren may.";
            }
            else if (_lastInstallInfo.DirFound)
            {
                lblDetect.ForeColor = System.Drawing.Color.DarkOrange;
                lblDetect.Text = en
                    ? "Found a 360 folder but Uninstall.exe is missing (possibly already partially removed)."
                    : "Phat hien thu muc 360 nhung thieu Uninstall.exe (co the da go mot phan).";
            }
            else
            {
                lblDetect.ForeColor = System.Drawing.Color.FromArgb(0, 128, 0);
                lblDetect.Text = en
                    ? "No active 360 Total Security installation detected (leftovers may still remain)."
                    : "Khong phat hien 360 Total Security dang cai dat (co the chi con file rac).";
            }
        }

        private void cmbLang_SelectedIndexChanged(object sender, EventArgs e)
        {
            ApplyLanguage();
        }

        private void ModeChanged(object sender, EventArgs e)
        {
            bool auto = rbAuto.Checked;
            btnAutoStart.Visible = auto;
            lblAutoHint.Visible = auto;
            btnManualUninstall.Visible = !auto;
            btnManualSafeClean.Visible = !auto;
        }

        // Ghi 1 dong vao o "Tien trinh" tren form, kem gio:phut:giay
        private void AppendProgress(string message)
        {
            string line = DateTime.Now.ToString("HH:mm:ss") + "  " + message;
            txtProgress.AppendText((txtProgress.TextLength == 0 ? "" : Environment.NewLine) + line);
            txtProgress.SelectionStart = txtProgress.TextLength;
            txtProgress.ScrollToCaret();
            Application.DoEvents();
        }

        private string SelectedLang()
        {
            return cmbLang.SelectedItem != null ? cmbLang.SelectedItem.ToString() : "VI";
        }

        // Giai nen 2 script PowerShell (nhung san trong exe) ra thu muc tam,
        // de khong can keo theo file .ps1 rieng khi phat hanh.
        private string ExtractScripts()
        {
            string dir = Path.Combine(Path.GetTempPath(), "360UninstallerTool");
            Directory.CreateDirectory(dir);
            ExtractResource(BootScript, dir);
            ExtractResource(CleanupScript, dir);
            return Path.Combine(dir, BootScript);
        }

        private void ExtractResource(string logicalName, string targetDir)
        {
            Assembly asm = Assembly.GetExecutingAssembly();
            using (Stream stream = asm.GetManifestResourceStream(logicalName))
            {
                if (stream == null)
                {
                    throw new FileNotFoundException("Embedded resource not found: " + logicalName);
                }
                string outPath = Path.Combine(targetDir, logicalName);
                using (FileStream fs = new FileStream(outPath, FileMode.Create, FileAccess.Write))
                {
                    // Stream.CopyTo doi hoi .NET 4.0+, tu copy bang buffer de tuong thich .NET 3.5
                    byte[] buffer = new byte[8192];
                    int read;
                    while ((read = stream.Read(buffer, 0, buffer.Length)) > 0)
                    {
                        fs.Write(buffer, 0, read);
                    }
                }
            }
        }

        // Chay 1 tien trinh va cho hoan tat, tra ve true neu thanh cong.
        // hidden = an cua so console (dung cho powershell.exe chay ngam).
        private bool RunAndWait(string fileName, string arguments, bool asAdmin, bool hidden, out string errorMessage)
        {
            errorMessage = null;
            try
            {
                var startInfo = new ProcessStartInfo
                {
                    FileName = fileName,
                    Arguments = arguments,
                    UseShellExecute = true,
                    WindowStyle = hidden ? ProcessWindowStyle.Hidden : ProcessWindowStyle.Normal
                };
                if (asAdmin) startInfo.Verb = "runas";
                using (Process p = Process.Start(startInfo))
                {
                    p.WaitForExit();
                    return true;
                }
            }
            catch (Exception ex)
            {
                errorMessage = ex.Message;
                return false;
            }
        }

        // Tao diem khoi phuc he thong (best-effort, khong chan luong neu loi)
        private void TryCreateRestorePoint()
        {
            bool en = IsEnglish;
            AppendProgress(en ? "Creating system restore point..." : "Dang tao diem khoi phuc he thong...");
            string args = "-ExecutionPolicy Bypass -NoProfile -WindowStyle Hidden -Command \"Checkpoint-Computer -Description '360 Uninstaller Tool' -RestorePointType 'MODIFY_SETTINGS'\"";
            string err;
            bool ok = RunAndWait("powershell.exe", args, false, true, out err);
            AppendProgress(ok
                ? (en ? "Restore point step finished." : "Da xu ly xong buoc tao diem khoi phuc.")
                : (en ? "Could not create restore point: " + err : "Khong tao duoc diem khoi phuc: " + err));
        }

        // Chay trinh go cai dat chinh thuc cua 360 (silent)
        private bool RunOfficialUninstall()
        {
            bool en = IsEnglish;
            InstallInfo info = DetectInstall();
            if (info.UninstallExePath == null)
            {
                AppendProgress(en ? "No official uninstaller found - skipped." : "Khong tim thay trinh go cai dat chinh thuc - bo qua.");
                MessageBox.Show(
                    en
                        ? "Official uninstaller (Uninstall.exe) not found. 360 may already be partially removed - use \"Boot Safe Mode & Clean up\" to remove the remaining leftovers."
                        : "Khong tim thay trinh go cai dat chinh thuc (Uninstall.exe). Co the 360 da bi go mot phan - hay dung chuc nang \"Khoi dong Safe Mode & Don dep\" de xoa sach con sot.",
                    en ? "Notice" : "Thong bao", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return false;
            }

            AppendProgress(en ? "Running official uninstaller..." : "Dang go cai dat chinh thuc...");
            string err;
            bool ok = RunAndWait(info.UninstallExePath, "/S", true, true, out err);
            if (ok)
            {
                AppendProgress(en ? "Official uninstall step finished." : "Da go cai dat chinh thuc xong.");
            }
            else
            {
                AppendProgress((en ? "Uninstall error: " : "Loi go cai dat: ") + err);
                MessageBox.Show((en ? "Error while uninstalling: " : "Loi khi go cai dat: ") + err,
                    en ? "Error" : "Loi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            return ok;
        }

        // Khoi dong pipeline Safe Mode: giai nen 2 script, chay 1_boot_safemode.ps1
        // (no se tu reboot may, chay 2_cleanup_360.ps1 as SYSTEM trong Safe Mode,
        // roi tu reboot lai Normal Mode).
        private bool LaunchSafeModeCleanup()
        {
            bool en = IsEnglish;

            string scriptPath;
            try
            {
                AppendProgress(en ? "Preparing cleanup scripts..." : "Dang chuan bi script don dep...");
                scriptPath = ExtractScripts();
            }
            catch (Exception ex)
            {
                AppendProgress((en ? "Error preparing scripts: " : "Loi chuan bi script: ") + ex.Message);
                MessageBox.Show((en ? "Error preparing scripts: " : "Loi chuan bi script: ") + ex.Message,
                    en ? "Error" : "Loi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }

            string args = string.Format(
                "-ExecutionPolicy Bypass -NoProfile -WindowStyle Hidden -File \"{0}\" -Lang {1}",
                scriptPath, SelectedLang());

            AppendProgress(en ? "Starting Safe Mode cleanup (the computer will reboot)..." : "Dang khoi dong don dep Safe Mode (may se khoi dong lai)...");
            string err;
            bool ok = RunAndWait("powershell.exe", args, true, true, out err);
            if (!ok)
            {
                AppendProgress((en ? "Error starting cleanup: " : "Loi khi khoi chay don dep: ") + err);
                MessageBox.Show((en ? "Error starting cleanup: " : "Loi khi khoi chay don dep: ") + err,
                    en ? "Error" : "Loi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }

            AppendProgress(en ? "Safe Mode cleanup triggered. Waiting for automatic reboot..." : "Da kich hoat don dep Safe Mode. Dang cho may tu khoi dong lai...");
            MessageBox.Show(
                en
                    ? "Cleanup process started. The computer will automatically reboot into Safe Mode, remove 360 Total Security, then reboot back to Normal Mode.\n\nPlease do not turn off the computer during this process."
                    : "Da khoi dong qua trinh don dep. May se tu dong khoi dong lai vao Safe Mode, don sach 360 Total Security, roi tu dong khoi dong lai Normal Mode.\n\nVui long khong tat may trong qua trinh nay.",
                en ? "Processing" : "Dang xu ly", MessageBoxButtons.OK, MessageBoxIcon.Information);
            return true;
        }

        private void btnAutoStart_Click(object sender, EventArgs e)
        {
            bool en = IsEnglish;
            string msg = en
                ? "AUTOMATIC mode will:\n" +
                  (chkRestorePoint.Checked ? "  1) Create a system restore point\n" : "") +
                  (chkRestorePoint.Checked ? "  2)" : "  1)") + " Reboot into Safe Mode to completely remove all 360 files, folders and registry entries\n" +
                  (chkRestorePoint.Checked ? "  3)" : "  2)") + " Automatically reboot back to Normal Mode\n\n" +
                  "The computer will REBOOT. Please save your work first.\n" +
                  "Are you sure you want to continue?"
                : "Che do TU DONG se thuc hien theo thu tu:\n" +
                  (chkRestorePoint.Checked ? "  1) Tao diem khoi phuc he thong\n" : "") +
                  (chkRestorePoint.Checked ? "  2)" : "  1)") + " Khoi dong lai vao Safe Mode de xoa sach toan bo file, thu muc va registry cua 360\n" +
                  (chkRestorePoint.Checked ? "  3)" : "  2)") + " Tu dong khoi dong lai Normal Mode\n\n" +
                  "May se KHOI DONG LAI. Hay luu cong viec truoc khi tiep tuc.\n" +
                  "Ban co chac chan muon bat dau?";

            var confirm = MessageBox.Show(msg, en ? "Confirm" : "Xac nhan", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
            if (confirm != DialogResult.Yes) return;

            btnAutoStart.Enabled = false;
            try
            {
                if (chkRestorePoint.Checked)
                {
                    TryCreateRestorePoint();
                }

                LaunchSafeModeCleanup();
            }
            finally
            {
                btnAutoStart.Enabled = true;
            }
        }

        private void btnManualUninstall_Click(object sender, EventArgs e)
        {
            bool en = IsEnglish;
            var confirm = MessageBox.Show(
                en
                    ? "Run the official 360 Total Security uninstaller (no Safe Mode reboot)?"
                    : "Chay trinh go cai dat chinh thuc cua 360 Total Security (khong khoi dong lai Safe Mode)?",
                en ? "Confirm" : "Xac nhan", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (confirm != DialogResult.Yes) return;

            btnManualUninstall.Enabled = false;
            try
            {
                RunOfficialUninstall();
            }
            finally
            {
                btnManualUninstall.Enabled = true;
            }
        }

        private void btnManualSafeClean_Click(object sender, EventArgs e)
        {
            bool en = IsEnglish;
            var confirm = MessageBox.Show(
                en
                    ? "This will reboot into Safe Mode to remove remaining 360 files/registry entries, then automatically reboot back to Normal Mode.\n\nThe computer will REBOOT. Please save your work first.\nAre you sure you want to continue?"
                    : "Thao tac nay se khoi dong lai may vao Safe Mode de don sach file/registry con sot cua 360, sau do tu dong khoi dong lai Normal Mode.\n\nMay se KHOI DONG LAI. Hay luu cong viec truoc khi tiep tuc.\nBan co chac chan muon tiep tuc?",
                en ? "Confirm" : "Xac nhan", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
            if (confirm != DialogResult.Yes) return;

            btnManualSafeClean.Enabled = false;
            try
            {
                if (chkRestorePoint.Checked)
                {
                    TryCreateRestorePoint();
                }
                LaunchSafeModeCleanup();
            }
            finally
            {
                btnManualSafeClean.Enabled = true;
            }
        }

        private void btnViewLog_Click(object sender, EventArgs e)
        {
            bool en = IsEnglish;
            if (!File.Exists(LogPath))
            {
                MessageBox.Show(
                    en
                        ? "No log file yet (" + LogPath + "). Run \"Boot Safe Mode & Clean up\" first."
                        : "Chua co file log (" + LogPath + "). Hay chay chuc nang \"Khoi dong Safe Mode & Don dep\" truoc.",
                    en ? "Notice" : "Thong bao", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            try
            {
                Process.Start("notepad.exe", LogPath);
            }
            catch (Exception ex)
            {
                MessageBox.Show((en ? "Could not open log: " : "Khong mo duoc log: ") + ex.Message,
                    en ? "Error" : "Loi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnDonate_Click(object sender, EventArgs e)
        {
            OpenUrl(DonateUrl);
        }

        private void linkReport_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            OpenUrl(GitHubUrl);
        }

        private void OpenUrl(string url)
        {
            bool en = IsEnglish;
            try
            {
                Process.Start(new ProcessStartInfo(url) { UseShellExecute = true });
            }
            catch (Exception ex)
            {
                MessageBox.Show((en ? "Could not open link: " : "Khong mo duoc lien ket: ") + ex.Message,
                    en ? "Error" : "Loi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
