using UnityEngine;

namespace SPGame.Runtime
{
    /// <summary>
    /// 资源初始化配置 SO
    /// </summary>
    [CreateAssetMenu(fileName = "ResourceSettings", menuName = "SPGame/ResourceSettings")]
    public sealed class ResourceSettings : ScriptableObject
    {
        [SerializeField]
        [Tooltip("资源包名")]
        private string _packageName;

        [SerializeField]
        [Tooltip("资源运行模式")]
        private ResourcePlayMode _playMode = ResourcePlayMode.EditorSimulate;

        [SerializeField]
        [Tooltip("远端目录地址，仅联机模式使用")]
        private string _hostServerUrl;
        [SerializeField]
        [Min(1)]
        [Tooltip("最大同时下载文件数量")]
        private int _maxConcurrentDownloads = 4;
        [SerializeField]
        [Min(0)]
        [Tooltip("单个文件下载失败后的重试次数")]
        private int _downloadRetryCount = 3;

        [SerializeField]
        [Min(1)]
        [Tooltip("请求版本和资源清单的超时时间，单位为秒")]
        private int _manifestTimeoutSeconds = 60;

        /// <summary>与 YooAsset 收集器配置一致的资源包名</summary>
        public string PackageName => _packageName;

        /// <summary>资源运行模式</summary>
        public ResourcePlayMode PlayMode => _playMode;

        /// <summary>已按平台和安装包基线区分的远端目录地址</summary>
        public string HostServerUrl => _hostServerUrl?.TrimEnd('/');

        /// <summary>最大同时下载文件数量</summary>
        public int MaxConcurrentDownloads => _maxConcurrentDownloads;

        /// <summary>单个文件下载失败后的重试次数</summary>
        public int DownloadRetryCount => _downloadRetryCount;

        /// <summary>请求版本和资源清单的超时时间，单位为秒</summary>
        public int ManifestTimeoutSeconds => _manifestTimeoutSeconds;
    }
}
