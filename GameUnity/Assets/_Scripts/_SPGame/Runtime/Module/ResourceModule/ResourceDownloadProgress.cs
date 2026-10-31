using System;

namespace SPGame
{
    /// <summary>
    /// 本次下载尝试的进度，重试时仅统计尚未完成的文件
    /// </summary>
    public readonly struct ResourceDownloadProgress
    {
        public int TotalDownloadCount { get; }
        public int CurrentDownloadCount { get; }
        public long TotalDownloadBytes { get; }
        public long CurrentDownloadBytes { get; }

        /// <summary>
        /// 按字节计算的进度，无文件需要下载时为 1
        /// </summary>
        public float Progress => TotalDownloadBytes > 0
            ? (float)Math.Min(1d, (double)CurrentDownloadBytes / TotalDownloadBytes)
            : (TotalDownloadCount == 0 || CurrentDownloadCount == TotalDownloadCount ? 1f : 0f);

        internal ResourceDownloadProgress(int totalDownloadCount, int currentDownloadCount,
            long totalDownloadBytes, long currentDownloadBytes)
        {
            TotalDownloadCount = totalDownloadCount;
            CurrentDownloadCount = currentDownloadCount;
            TotalDownloadBytes = totalDownloadBytes;
            CurrentDownloadBytes = currentDownloadBytes;
        }
    }
}
