using System;

namespace SPGame
{
    /// <summary>
    /// 资源初始化配置
    /// </summary>
    public sealed class ResourceSettings
    {
        /// <summary>与 YooAsset 收集器配置一致的资源包名</summary>
        public string PackageName { get; }

        /// <summary>资源运行模式</summary>
        public ResourcePlayMode PlayMode { get; }

        /// <summary>已按平台和安装包基线区分的远端目录地址</summary>
        public string HostServerUrl { get; }

        /// <summary>最大同时下载文件数量</summary>
        public int MaxConcurrentDownloads { get; }

        /// <summary>单个文件下载失败后的重试次数</summary>
        public int DownloadRetryCount { get; }

        /// <summary>请求版本和资源清单的超时时间，单位为秒</summary>
        public int ManifestTimeoutSeconds { get; }

        public ResourceSettings(string packageName, ResourcePlayMode playMode, string hostServerUrl = null,
            int maxConcurrentDownloads = 4, int downloadRetryCount = 3, int manifestTimeoutSeconds = 60)
        {
            #region 参数验证和处理

            if (string.IsNullOrWhiteSpace(packageName))
            {
                throw new ArgumentException("资源包名不能为空", nameof(packageName));
            }

            if (playMode == ResourcePlayMode.Host)
            {
                // 判断 Host 模式下资源服务器 URL 是否合法
                if (!Uri.TryCreate(hostServerUrl, UriKind.Absolute, out Uri uri)
                    || (uri.Scheme != Uri.UriSchemeHttp && uri.Scheme != Uri.UriSchemeHttps) // 检查协议是否为 HTTP 或 HTTPS
                    || !string.IsNullOrEmpty(uri.Query) || !string.IsNullOrEmpty(uri.Fragment)) // 检查是否包含查询参数或片段
                {
                    throw new ArgumentException("联机模式需要不带查询参数和片段的 HTTP 或 HTTPS 目录地址", nameof(hostServerUrl));
                }
                hostServerUrl = hostServerUrl.TrimEnd('/');
            }

            #endregion

            PackageName = packageName;
            PlayMode = playMode;
            HostServerUrl = hostServerUrl;
            MaxConcurrentDownloads = maxConcurrentDownloads;
            DownloadRetryCount = downloadRetryCount;
            ManifestTimeoutSeconds = manifestTimeoutSeconds;
        }
    }
}
