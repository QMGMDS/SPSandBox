namespace SPGame.Runtime
{
    /// <summary>
    /// 一次检查生成的下载计划
    /// </summary>
    public sealed class ResourceUpdatePlan
    {
        /// <summary>目标资源版本号</summary>
        public string TargetVersion { get; }
        /// <summary>需要下载的资源文件总数</summary>
        public int TotalDownloadCount { get; }
        /// <summary>需要下载的资源文件总大小（字节）</summary>
        public long TotalDownloadBytes { get; }

        internal ResourceUpdatePlan(string targetVersion, int totalDownloadCount, long totalDownloadBytes)
        {
            TargetVersion = targetVersion;
            TotalDownloadCount = totalDownloadCount;
            TotalDownloadBytes = totalDownloadBytes;
        }
    }
}
