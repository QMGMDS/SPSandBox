using System;
using System.Collections.Generic;

using UnityEngine.SceneManagement;

using Cysharp.Threading.Tasks;
using YooAsset;

namespace SPGame.Runtime
{
    /// <summary>
    /// 资源模块
    /// </summary>
    public sealed class ResourceModule : IResourceModule
    {
        private ResourceSettings _settings;
        private ResourcePackage _package;
        private ResourceDownloaderOperation _downloader;

        public ResourceModule()
        {
            YooAssets.Initialize();
        }

        #region IModule

        // 高于流程模块，保证流程先销毁，资源系统后销毁
        public int Priority => 1;

        public void Destroy()
        {
            _settings = null;
            _package = null;
            _downloader = null;
        }

        #endregion

        #region IResourceModule

        public bool IsInitialized => false;
        public bool IsReady => false;

        public async UniTask InitializeAsync(ResourceSettings settings)
        {
            _settings = settings;

            InitializePackageOptions options;
            switch (settings.PlayMode)
            {
                case ResourcePlayMode.EditorSimulate:
                    PackageBuildResult result = EditorSimulateBuildInvoker.Build(settings.PackageName, (int)EBundleType.VirtualAssetBundle);
                    options = new EditorSimulateModeOptions
                    {
                        EditorFileSystemParameters = FileSystemParameters.CreateDefaultEditorFileSystemParameters(result.PackageRootDirectory),
                        AutoUnloadBundleWhenUnused = true
                    };
                    break;

                case ResourcePlayMode.Offline:
                    options = new OfflinePlayModeOptions
                    {
                        BuiltinFileSystemParameters = FileSystemParameters.CreateDefaultBuiltinFileSystemParameters(),
                        AutoUnloadBundleWhenUnused = true
                    };
                    break;

                case ResourcePlayMode.Host:
                    options = new HostPlayModeOptions
                    {
                        BuiltinFileSystemParameters = FileSystemParameters.CreateDefaultBuiltinFileSystemParameters(),
                        CacheFileSystemParameters = FileSystemParameters.CreateDefaultSandboxFileSystemParameters(new RemoteService(settings.HostServerUrl)),
                        AutoUnloadBundleWhenUnused = true
                    };
                    break;

                default:
                    throw new ArgumentOutOfRangeException(nameof(settings.PlayMode));
            }

            _package = YooAssets.CreatePackage(settings.PackageName);
            await _package.InitializePackageAsync(options);
        }

        public async UniTask<ResourceUpdatePlan> CheckUpdateAsync(string targetVersion = null)
        {
            // 无目标版本则获取当前环境的资源版本，作为目标版本
            if (targetVersion == null)
            {
                var versionOperation = _package.RequestPackageVersionAsync(new RequestPackageVersionOptions(true, _settings.ManifestTimeoutSeconds));
                await versionOperation;

                targetVersion = versionOperation.PackageVersion;
            }

            // 加载目标版本的资源清单
            var manifestOptions = new LoadPackageManifestOptions(targetVersion, _settings.ManifestTimeoutSeconds);
            await _package.LoadPackageManifestAsync(manifestOptions);

            // 创建资源下载器，此时统计需要下载的文件
            _downloader = _package.CreateResourceDownloader(new ResourceDownloaderOptions(_settings.MaxConcurrentDownloads, _settings.DownloadRetryCount));

            // 生成资源更新计划
            return new ResourceUpdatePlan(targetVersion, _downloader.TotalDownloadCount, _downloader.TotalDownloadBytes);
        }

        public async UniTask DownloadAsync(ResourceUpdatePlan plan, IProgress<ResourceDownloadProgress> progress = null)
        {
            ResourceDownloaderOperation downloader = _downloader;

            downloader.DownloadProgressChanged += args =>
            {
                progress?.Report(new ResourceDownloadProgress(
                    args.TotalDownloadCount,
                    args.CurrentDownloadCount,
                    args.TotalDownloadBytes,
                    args.CurrentDownloadBytes
                ));
            };

            downloader.StartDownload();
            await downloader;
        }

        public async UniTask<ResourceHandle<T>> LoadAssetAsync<T>(string address) where T : UnityEngine.Object
        {
            AssetHandle handle = _package.LoadAssetAsync<T>(address);
            await handle;
            return new ResourceHandle<T>(handle);
        }

        public async UniTask<ResourceSceneHandle> LoadSceneAsync(string address, LoadSceneMode loadMode = LoadSceneMode.Single)
        {
            SceneHandle handle = _package.LoadSceneAsync(address, loadMode);
            await handle;
            return new ResourceSceneHandle(handle);
        }

        #endregion

        #region 内部工具

        private sealed class RemoteService : IRemoteService
        {
            private readonly string _hostServerUrl;

            public RemoteService(string hostServerUrl)
            {
                _hostServerUrl = hostServerUrl;
            }

            public IReadOnlyList<string> GetRemoteUrls(string fileName)
            {
                return new[] { $"{_hostServerUrl}/{fileName}" };
            }
        }

        #endregion
    }
}
