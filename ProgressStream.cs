using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace frm360security
{
    public class ProgressStream : Stream
    {
        private readonly Stream _innerStream;
        private readonly IProgress<DownloadProgress> _progress;
        private readonly long? _totalBytes;
        private long _bytesRead;

        public ProgressStream(Stream innerStream, long? totalBytes, IProgress<DownloadProgress> progress)
        {
            _innerStream = innerStream ?? throw new ArgumentNullException(nameof(innerStream));
            _totalBytes = totalBytes;
            _progress = progress;
            _bytesRead = 0;
        }

        // PHẦN QUAN TRỌNG: Ghi đè ReadAsync để tính toán và báo cáo tiến trình
        public override async Task<int> ReadAsync(byte[] buffer, int offset, int count, CancellationToken cancellationToken)
        {
            // Thực hiện việc đọc thực tế từ luồng mạng
            var bytesRead = await _innerStream.ReadAsync(buffer, offset, count, cancellationToken);

            if (bytesRead > 0)
            {
                _bytesRead += bytesRead;
                ReportProgress();
            }
            return bytesRead;
        }

        private void ReportProgress()
        {
            int? percentage = null;
            if (_totalBytes.HasValue && _totalBytes.Value > 0)
            {
                // Tính phần trăm
                percentage = (int)((double)_bytesRead * 100 / _totalBytes.Value);
            }

            // Báo cáo tiến trình về UI thread thông qua IProgress<T>
            _progress?.Report(new DownloadProgress
            {
                BytesDownloaded = _bytesRead,
                TotalBytes = _totalBytes,
                ProgressPercentage = percentage
            });
        }

        // GHI ĐÈ CÁC PHƯƠNG THỨC STREAM CẦN THIẾT KHÁC (Chỉ đơn thuần gọi lại innerStream)
        public override bool CanRead => _innerStream.CanRead;
        public override bool CanSeek => _innerStream.CanSeek;
        public override bool CanWrite => _innerStream.CanWrite;
        public override long Length => _innerStream.Length;
        public override long Position { get => _innerStream.Position; set => _innerStream.Position = value; }
        public override void Flush() => _innerStream.Flush();
        public override int Read(byte[] buffer, int offset, int count) => _innerStream.Read(buffer, offset, count);
        public override long Seek(long offset, SeekOrigin origin) => _innerStream.Seek(offset, origin);
        public override void SetLength(long value) => _innerStream.SetLength(value);
        public override void Write(byte[] buffer, int offset, int count) => _innerStream.Write(buffer, offset, count);
    }
}

