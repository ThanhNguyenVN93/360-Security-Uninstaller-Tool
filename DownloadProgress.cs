using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace frm360security
{
    public class DownloadProgress
    {
        // Tổng dung lượng file (có thể là null nếu server không gửi Content-Length)
        public long? TotalBytes { get; set; }

        // Số byte đã tải
        public long BytesDownloaded { get; set; }

        // Phần trăm tiến trình (từ 0 đến 100)
        public int? ProgressPercentage { get; set; }
    }
}
