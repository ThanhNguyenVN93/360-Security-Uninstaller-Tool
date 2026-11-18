using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;        // Cho Path và File
using System.Linq;
using System.Net.Http; // Cho HttpClient
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;
using System.Diagnostics;
using System.Threading;
using SevenZip;
using frm360security.Properties;
using System.Management;
using System.Runtime.InteropServices;
using System.ServiceProcess;


namespace frm360security
{
    public partial class Form1 : Form
    {
        [DllImport("user32.dll", SetLastError = true)]
        private static extern IntPtr FindWindow(string lpClassName, string lpWindowName);

        [DllImport("user32.dll", SetLastError = true)]
        private static extern bool PostMessage(IntPtr hWnd, uint Msg, IntPtr wParam, IntPtr lParam);

        private const uint WM_CLOSE = 0x0010;
        private const uint WM_QUIT = 0x0012;

        private static readonly string[] ProcessNames360 = new[]
        {
            "QHWatchdog", "QHActiveDefense", "QHSafeTray",
            "360Tray", "360tray", "360Safe", "360safe",
            "360rp", "360sd", "360leakfixer",
            "PopWndLog", "360speedld", "SoftMgrLite",
            "360InstantSetup", "360Chrome", "360SE",
            "360DeskAnti", "360Protect", "ZhuDongFangYu",
            "deepscan", "360NetSpeed",
            "LiveUpdate360", "360TsLiveUpd",
            "360LiveUpd", "360TS_Setup"
        };

        private static readonly string[] ServiceNames360 = new[]
        {
            "QHActiveDefense", "QHProtected", "QHWatchdog",
            "ZhuDongFangYu", "360rp", "QHWSCSVC",
            "360AntiHijacking", "360Safeguard",
            "360rps", "360SpeedLD", "QHSafeMain",
            "360netmon", "AviraPhantomVPN"
        };

        private static readonly HttpClient _httpClient = new HttpClient();
        private const int BufferSize = 40960; // 80 KB
        public Form1()
        {
            InitializeComponent();
            // --- Thiết lập UI ---
            progressBar1.Minimum = 0;
            progressBar1.Maximum = 100;
            progressBar1.Value = 0;
            lblstatus.Text = "Sẵn sàng...";

            // ĐẶT LINK MẶC ĐỊNH
            txturl.Text = "https://download.revouninstaller.com/download/revoregistrycleaner/RRC_FreePortable.zip";

            // 1. Lấy đường dẫn tới Desktop
            string desktopPath = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);

            // 2. Định nghĩa tên thư mục con
            string folderName = "360_Downloads";

            // 3. Tạo đường dẫn đầy đủ tới thư mục con
            string saveDirectory = Path.Combine(desktopPath, folderName);

            // 4. Đảm bảo thư mục con tồn tại. Nếu chưa có, nó sẽ được tạo
            Directory.CreateDirectory(saveDirectory);

            // 5. Lấy tên file từ URL
            string fileName = Path.GetFileName(txturl.Text);

            // 6. Gán đường dẫn lưu cuối cùng vào txtOutput
            txtoutput.Text = Path.Combine(saveDirectory, fileName);
        }
        private async Task Kill360ProcessesAsync()
        {
            await Task.Run(() =>
            {
                // Step 1: Try graceful exit via tray window messages
                try
                {
                    foreach (var proc in Process.GetProcessesByName("360Tray"))
                    {
                        if (proc.MainWindowHandle != IntPtr.Zero)
                        {
                            PostMessage(proc.MainWindowHandle, WM_CLOSE, IntPtr.Zero, IntPtr.Zero);
                        }
                    }
                    foreach (var proc in Process.GetProcessesByName("QHSafeTray"))
                    {
                        if (proc.MainWindowHandle != IntPtr.Zero)
                        {
                            PostMessage(proc.MainWindowHandle, WM_CLOSE, IntPtr.Zero, IntPtr.Zero);
                        }
                    }
                    Thread.Sleep(2000);
                }
                catch { }

                // Step 2: Stop all 360 services (they restart processes)
                foreach (string svcName in ServiceNames360)
                {
                    try
                    {
                        using (var sc = new ServiceController(svcName))
                        {
                            if (sc.Status == ServiceControllerStatus.Running ||
                                sc.Status == ServiceControllerStatus.Paused)
                            {
                                sc.Stop();
                                sc.WaitForStatus(ServiceControllerStatus.Stopped, TimeSpan.FromSeconds(10));
                            }
                        }
                    }
                    catch { }
                }

                // Step 3: Use sc.exe to disable services (prevents restart)
                foreach (string svcName in ServiceNames360)
                {
                    try
                    {
                        var psi = new ProcessStartInfo
                        {
                            FileName = "sc.exe",
                            Arguments = $"config \"{svcName}\" start= disabled",
                            UseShellExecute = false,
                            CreateNoWindow = true,
                            RedirectStandardOutput = true
                        };
                        using (var p = Process.Start(psi))
                            p.WaitForExit(5000);
                    }
                    catch { }
                }

                // Step 4: taskkill /F /T for each process
                foreach (string procName in ProcessNames360)
                {
                    try
                    {
                        var psi = new ProcessStartInfo
                        {
                            FileName = "taskkill",
                            Arguments = $"/F /IM {procName}.exe /T",
                            UseShellExecute = false,
                            CreateNoWindow = true,
                            RedirectStandardOutput = true,
                            RedirectStandardError = true
                        };
                        using (var p = Process.Start(psi))
                            p.WaitForExit(5000);
                    }
                    catch { }
                }

                // Step 5: Also try via WMI (bypasses some protections)
                try
                {
                    foreach (string procName in ProcessNames360)
                    {
                        using (var searcher = new ManagementObjectSearcher(
                            $"SELECT * FROM Win32_Process WHERE Name = '{procName}.exe'"))
                        {
                            foreach (ManagementObject obj in searcher.Get())
                            {
                                try { obj.InvokeMethod("Terminate", null); }
                                catch { }
                            }
                        }
                    }
                }
                catch { }

                // Step 6: Final aggressive kill with wmic (works on Win7+)
                try
                {
                    var psi = new ProcessStartInfo
                    {
                        FileName = "wmic",
                        Arguments = "process where \"name like '%360%' or name like '%QH%' or name like '%ZhuDong%'\" call terminate",
                        UseShellExecute = false,
                        CreateNoWindow = true,
                        RedirectStandardOutput = true,
                        RedirectStandardError = true
                    };
                    using (var p = Process.Start(psi))
                        p.WaitForExit(10000);
                }
                catch { }

                Thread.Sleep(1000);
            });
        }

        // 1. Phương thức download chính (Logic)
        public async Task DownloadFileAsync(string url, string outputPath, IProgress<DownloadProgress> progress)
        {
            // Sử dụng ResponseHeadersRead để stream file, không tải toàn bộ vào bộ nhớ
            using (var response = await _httpClient.GetAsync(url, HttpCompletionOption.ResponseHeadersRead))
            {
                // Kiểm tra lỗi HTTP (404, 500,...)
                response.EnsureSuccessStatusCode();

                // Lấy tổng dung lượng file (nếu server hỗ trợ)
                long? totalBytes = response.Content.Headers.ContentLength;

                using (var contentStream = await response.Content.ReadAsStreamAsync())
                // Dùng ProgressStream để bao bọc ContentStream và báo cáo tiến trình
                using (var progressStream = new ProgressStream(contentStream, totalBytes, progress))
                // Dùng FileStream để ghi vào ổ đĩa
                using (var fileStream = new FileStream(outputPath, FileMode.Create, FileAccess.Write, FileShare.None, BufferSize, useAsync: true))
                {
                    // Sao chép dữ liệu từ mạng sang file. ProgressStream sẽ báo cáo tiến trình
                    await progressStream.CopyToAsync(fileStream, BufferSize);
                }
            }
        }
        private async void btnDownload_Click(object sender, EventArgs e)
        {
            string fileUrl = txturl.Text.Trim();
            string saveDirectory = txtoutput.Text.Trim();
            string zipFilePath = txtoutput.Text.Trim();

            // 1. Kiểm tra đầu vào
            if (string.IsNullOrWhiteSpace(fileUrl) || string.IsNullOrWhiteSpace(saveDirectory))
            {
                MessageBox.Show("Lỗi: Vui lòng điền đầy đủ LINK và LINK OUT.", "Thông Báo", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            // Khởi tạo Progress để nhận báo cáo từ luồng nền và cập nhật UI (lblStatus, progressBar)
            var progress = new Progress<DownloadProgress>(UpdateProgressUI);

            try
            {
                // Vô hiệu hóa nút để tránh nhấp đúp khi đang tải
                btnDownload.Enabled = false;

                // Thiết lập trạng thái ban đầu cho UI
                progressBar1.Value = 0;
                lblstatus.Text = Properties._string.StartingDownload;

                // BẮT ĐẦU DOWNLOAD BẤT ĐỒNG BỘ
                await DownloadFileAsync(fileUrl, saveDirectory, progress);

                // HIỂN THỊ KHI HOÀN TẤT
                lblstatus.Text = Properties._string.DownloadComplete; ;
                MessageBox.Show(Properties._string.DownloadComplete, Properties._string.SuccessTitle, MessageBoxButtons.OK, MessageBoxIcon.Information);
                // Đường dẫn thư mục đích (thư mục giải nén sẽ nằm cùng chỗ với file zip)
                string destinationFolder = Path.GetDirectoryName(zipFilePath);

                // Tên thư mục con (ví dụ: RRC_FreePortable)
                string zipFileNameWithoutExtension = Path.GetFileNameWithoutExtension(zipFilePath);
                string extractPath = Path.Combine(destinationFolder, zipFileNameWithoutExtension);

                lblstatus.Text = Properties._string.StartingExtraction;

                // Nếu thư mục giải nén đã tồn tại, xóa nó đi để tránh lỗi
                if (Directory.Exists(extractPath))
                {
                    Directory.Delete(extractPath, true);
                }
                // ... (Các định nghĩa extractPath và xóa folder cũ giữ nguyên) ...

                // ----------------------------------------------------
                // LOGIC GIẢI NÉN SỬ DỤNG SevenZipSharp
                // ----------------------------------------------------

                lblstatus.Text = Properties._string.Extracting7z;

                try
                {
                    // 1. Xác định thư mục DLL (x64 hoặc x86)
                    string archFolder = (Environment.Is64BitProcess) ? "x64" : "x86";

                    // 2. Tên file DLL: Hầu hết các thư viện gọi nó là "7z.dll"
                    string nativeDllName = "7z.dll";
                    // Gói NuGet của 2GIS ERM thường đặt 7z.dll vào cả x64 và x86

                    // 3. Tạo đường dẫn đầy đủ: [App_Path]\[x64/x86]\7z.dll
                    string dllPath = Path.Combine(Application.StartupPath, archFolder, nativeDllName);

                    // 4. KIỂM TRA ĐƯỜNG DẪN TRƯỚC KHI GỌI: (Để debug chính xác)
                    if (!File.Exists(dllPath))
                    {
                        string dllErrorMessage = string.Format(Properties._string.ErrorDLLNotFound, dllPath);
                        MessageBox.Show(dllErrorMessage, Properties._string.ErrorDLLTitle, MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }
                    // 4. Thiết lập đường dẫn thư viện
                    SevenZipBase.SetLibraryPath(dllPath);
                    SevenZipExtractor extractor = new SevenZipExtractor(zipFilePath);

                    // Đảm bảo thư mục đích tồn tại
                    Directory.CreateDirectory(extractPath);

                    // Giải nén tất cả các mục vào thư mục đích
                    extractor.ExtractArchive(extractPath);
                }
                catch (Exception ex)
                {
                    // Bắt lỗi giải nén (vì 7z.dll có thể không tìm thấy hoặc lỗi)
                    string extractErrorMessage = Properties._string.ErrorAdvancedExtraction + $" {ex.Message}";
                    MessageBox.Show(extractErrorMessage, Properties._string.ErrorTitle, MessageBoxButtons.OK, MessageBoxIcon.Error);
                    lblstatus.Text = Properties._string.ErrorExtractionStatus;
                    return;
                }  
                // 3. MỞ FOLDER
                Process.Start(extractPath);

                // HIỂN THỊ KHI HOÀN TẤT
                lblstatus.Text = Properties._string.CompleteExtractionStatus;
                btnclean.Enabled = true;
                btnclean.PerformClick();
                //MessageBox.Show("Download và giải nén hoàn tất!", "Thành công", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (HttpRequestException)
            {
                lblstatus.Text = Properties._string.ErrorHTTPRequest; // Lỗi HTTP: Kiểm tra URL hoặc kết nối.
                MessageBox.Show(Properties._string.ErrorHTTPRequest, // Lỗi kết nối hoặc HTTP: Không thể tải file.
                                Properties._string.ErrorTitle,
                                MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            catch (Exception ex)
            {
                lblstatus.Text = Properties._string.ErrorDownloadProcess; // Lỗi xảy ra trong quá trình download.
                MessageBox.Show(Properties._string.ErrorTitle + $": {ex.Message}",
                                Properties._string.ErrorTitle,
                                MessageBoxButtons.OK, MessageBoxIcon.Error);

            }
            finally
            {
                // Khôi phục UI sau khi download hoàn tất hoặc thất bại
                btnDownload.Enabled = true;
                progressBar1.Value = progressBar1.Maximum; // Đảm bảo thanh tiến trình đầy
            }
        }
        // Phương thức cập nhật UI (được gọi tự động trên UI thread nhờ Progress<T>)
        private void UpdateProgressUI(DownloadProgress p)
        {
            if (p.ProgressPercentage.HasValue)
            {
                int percentageValue = p.ProgressPercentage.Value;

                // Đảm bảo không bị kẹt ở 0% nếu đã tải được một chút
                if (percentageValue == 0 && p.BytesDownloaded > 0 && p.TotalBytes > 0)
                {
                    percentageValue = 1; // Đặt tối thiểu là 1%
                }

                // Cập nhật ProgressBar và Label
                progressBar1.Value = percentageValue;

                lblstatus.Text = $"Đang tải... {percentageValue}% ({p.BytesDownloaded:N0} / {p.TotalBytes:N0} bytes)";
            }
            // ... (phần else giữ nguyên)
        }
        private void Form1_Load(object sender, EventArgs e)
        {
            //hệ điều hành
            string r = "";
            using (ManagementObjectSearcher searcher = new ManagementObjectSearcher("SELECT * FROM Win32_OperatingSystem"))
            {
                ManagementObjectCollection information = searcher.Get();
                if (information != null)
                {
                    foreach (ManagementObject obj in information)
                    {
                        r = obj["Caption"].ToString() + " - " + obj["OSArchitecture"].ToString();
                    }
                }
                r = r.Replace("NT 5.1.2600", "XP");
                r = r.Replace("NT 5.2.3790", "Server 2003");
                lblinfo.Text = r;
            }

            // Sử dụng thư viện System.IO; và System.Diagnostics;
            string programFilesPath = Environment.GetFolderPath(Environment.SpecialFolder.ProgramFilesX86);

            // 1. Dùng Path.Combine để ghép đường dẫn an toàn
            string folderPath = Path.Combine(programFilesPath, "360", "Total Security");
            string filePath = Path.Combine(folderPath, "Uninstall.exe"); // Phát hiện file đã tồn tại
            //string desktopPath = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
            //string folder=Path.Combine(desktopPath, "360_Downloads");

            try
            {
                // Kiểm tra ĐỦ CẢ 2 điều kiện: Thư mục tồn tại VÀ File Uninstall.exe tồn tại
                if (Directory.Exists(folderPath) && File.Exists(filePath))
                {
                    // 2. CHƯƠNG TRÌNH ĐÃ CÀI ĐẶT
                    btnuninstall.Enabled = true;    // Cho phép gỡ cài đặt
                    btnuninstall.PerformClick();      // Tự động nhấn nút Uninstall
                    //btnDownload.Enabled = true;    // Bật hóa nút Download
                    //btnDownload.PerformClick();
                    // KHÔNG sử dụng PerformClick() ở đây
                }
                else if (Directory.Exists(folderPath) && !File.Exists(filePath))
                    {
                    
                        // 2.5 TRƯỜNG HỢP THƯ MỤC TỒN TẠI NHƯNG KHÔNG CÓ FILE UNINSTALL.EXE
                        btnDownload.Enabled = true;    // Bật hóa nút Download
                        btnDownload.PerformClick();
                       // btnclean.Enabled = true;
                        //tnclean.PerformClick();
                    }
                    // 3. CHƯƠNG TRÌNH CHƯA CÀI ĐẶT
                    else
                {
                    //MessageBox.Show("Chưa cài đặt 360 Total Security.", "Thông Báo", MessageBoxButtons.OK, MessageBoxIcon.Information);

                    btnclean.Enabled = true;
                            btnclean.PerformClick();
                            btnDownload.Enabled = false;
                   
                    // Bạn có thể kích hoạt tự động download nếu muốn:
                    // btnDownload.PerformClick(); 
                }
            }
            catch (Exception ex)
            {
                // Xử lý lỗi nếu có vấn đề về quyền truy cập hoặc hệ thống
                MessageBox.Show($"Lỗi kiểm tra trạng thái: {ex.Message}", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                // ...
            }
        }
        private async void btnuninstall_Click(object sender, EventArgs e)
        {
            string programFilesPath = Environment.GetFolderPath(Environment.SpecialFolder.ProgramFilesX86);
            string folderPath = Path.Combine(programFilesPath, "360", "Total Security");
            string filePath = Path.Combine(folderPath, "Uninstall.exe");
            try
            {
                if (Directory.Exists(folderPath) && File.Exists(filePath))
                {
                    btnuninstall.Enabled = false;

                    // Kill all 360 processes and services first
                    lblstatus.Text = "Đang tắt các tiến trình 360...";
                    await Kill360ProcessesAsync();

                    lblstatus.Text = Properties._string.Uninstalling;
                    ProcessStartInfo startInfo = new ProcessStartInfo
                    {
                        FileName = filePath,
                        Arguments = "/S",
                        UseShellExecute = true,
                        Verb = "runas"
                    };
                    Process uninstallProcess = Process.Start(startInfo);
                    await Task.Run(() => uninstallProcess.WaitForExit());
                    lblstatus.Text = Properties._string.UninstallComplete;
                    btnclean.Enabled = true;
                    btnclean.PerformClick();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi cố gắng gỡ cài đặt: {ex.Message}", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                btnuninstall.Enabled = true;
            }
        }

        private async void btnclean_Click(object sender, EventArgs e)
        {
            try
            {
                btnclean.Enabled = false;
                lblstatus.Text = "Đang tắt các tiến trình 360 còn lại...";
                await Kill360ProcessesAsync();

                lblstatus.Text = Properties._string.CleaningUp;

                var foldersToDelete = new System.Collections.Generic.List<string>();
                foldersToDelete.Add(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ProgramFilesX86), "360"));
                foldersToDelete.Add(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData), "360Safe"));
                foldersToDelete.Add(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData), "360"));
                foldersToDelete.Add(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData), "360TotalSecurity"));
                foldersToDelete.Add(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData), "360Quarant"));
                foldersToDelete.Add(@"C:\360SANDBOX");

                string usersRoot = Path.GetDirectoryName(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile));
                if (Directory.Exists(usersRoot))
                {
                    foreach (var userDir in Directory.GetDirectories(usersRoot))
                    {
                        string roaming = Path.Combine(userDir, "AppData", "Roaming");
                        string local = Path.Combine(userDir, "AppData", "Local");
                        string localLow = Path.Combine(userDir, "AppData", "LocalLow");
                        foreach (var sub in new[] { "360Safe", "360TotalSecurity", "360TotalSecurityLow", "360DesktopLite" })
                        {
                            foldersToDelete.Add(Path.Combine(roaming, sub));
                            foldersToDelete.Add(Path.Combine(local, sub));
                        }
                        foreach (var sub in new[] { "360WD", "360MenuMgr", "360Safe", "360TotalSecurity" })
                            foldersToDelete.Add(Path.Combine(localLow, sub));
                    }
                }

                await Task.Run(() =>
                {
                    foreach (var folder in foldersToDelete)
                    {
                        try
                        {
                            if (Directory.Exists(folder))
                                Directory.Delete(folder, true);
                        }
                        catch { }
                    }
                });

                lblstatus.Text = Properties._string.CompleteExtractionStatus;
                MessageBox.Show(Properties._string.CleanupCompleteMessage, Properties._string.SuccessTitle, MessageBoxButtons.OK, MessageBoxIcon.Information);
                Application.Exit();
            }
            catch (Exception ex)
            {
                string fullErrorMessage = Properties._string.ErrorWhenTryingToCleanup + " " + ex.Message;
                MessageBox.Show(fullErrorMessage, Properties._string.ErrorTitle, MessageBoxButtons.OK, MessageBoxIcon.Error);
                btnclean.Enabled = true;
            }
        }
    }
}
