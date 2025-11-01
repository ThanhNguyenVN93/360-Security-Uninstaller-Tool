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


namespace frm360security
{
    public partial class Form1 : Form
    {
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
        private void btnuninstall_Click(object sender, EventArgs e)
        {
            // Sử dụng thư viện System.IO; và System.Diagnostics;
            string programFilesPath = Environment.GetFolderPath(Environment.SpecialFolder.ProgramFilesX86);

            // 1. Dùng Path.Combine để ghép đường dẫn an toàn
            string folderPath = Path.Combine(programFilesPath, "360", "Total Security");
            string filePath = Path.Combine(folderPath, "Uninstall.exe"); // Phát hiện file đã tồn tại
            try
            {
                // Kiểm tra ĐỦ CẢ 2 điều kiện: Thư mục tồn tại VÀ File Uninstall.exe tồn tại
                if (Directory.Exists(folderPath) && File.Exists(filePath))
                {
                    // 2. CHƯƠNG TRÌNH ĐÃ CÀI ĐẶT
                    // Khởi chạy quá trình gỡ cài đặt
                    ProcessStartInfo startInfo = new ProcessStartInfo
                    {
                        FileName = filePath,
                        Arguments = "/S", // Tham số gỡ cài đặt im lặng (nếu hỗ trợ)
                        UseShellExecute = true,
                        Verb = "runas" // Chạy với quyền quản trị nếu cần
                    };
                    Process uninstallProcess = Process.Start(startInfo);
                    uninstallProcess.WaitForExit(); // Chờ quá trình gỡ cài đặt kết thúc
                    //MessageBox.Show("Gỡ cài đặt hoàn tất!", "Thành công", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    //Application.Exit();
                }
                else
                {
                    // 3. CHƯƠNG TRÌNH CHƯA CÀI ĐẶT
                    //MessageBox.Show("Chưa cài đặt 360 Total Security.", "Thông Báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            catch (Exception ex)
            {
                // Xử lý lỗi nếu có vấn đề về quyền truy cập hoặc hệ thống
                MessageBox.Show($"Lỗi khi cố gắng gỡ cài đặt: {ex.Message}", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                // ...
            }

        }

        private void btnclean_Click(object sender, EventArgs e)
        {
            //đường dẫn cài đặt 360 Total Security
            string filePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ProgramFilesX86), "360");
            try
            {
                if (Directory.Exists(filePath))
                {
                    // Khởi chạy quá trình dọn dẹp
                    ProcessStartInfo startInfo = new ProcessStartInfo
                    {
                        //xoá folder cài đặt 360 Total Security
                        FileName = "cmd.exe",
                        Arguments = $"/C rmdir /S /Q \"{filePath}\"",// Lệnh xoá thư mục và tất cả nội dung bên trong
                        Verb = "runas" // Chạy với quyền quản trị nếu cần
                    };
                    Process cleanProcess = Process.Start(startInfo);
                    cleanProcess.WaitForExit(); // Chờ quá trình dọn dẹp kết thúc
                                                // 🌟 THAY THẾ CHO THÔNG BÁO THÀNH CÔNG:
                                                // CŨ: MessageBox.Show("Dọn dẹp hoàn tất!", "Thành công", ...
                    MessageBox.Show(Properties._string.CleanupCompleteMessage,Properties._string.SuccessTitle,MessageBoxButtons.OK,MessageBoxIcon.Information);
                    Application.Exit();
                }
                else
                {
                }
            }
            catch (Exception ex)
            {
                // Xử lý lỗi nếu có vấn đề về quyền truy cập hoặc hệ thống
                // CŨ: MessageBox.Show("Lỗi khi cố gắng dọn dẹp: " + ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);

                // MỚI: SỬ DỤNG _string
                string fullErrorMessage = Properties._string.ErrorWhenTryingToCleanup + " " + ex.Message;

                MessageBox.Show(fullErrorMessage,
                                Properties._string.ErrorTitle,
                                MessageBoxButtons.OK, MessageBoxIcon.Error);
                Application.Exit();
                // ...
            }
        }
    }
}
