using System;

using UnityEngine.SceneManagement;

using Cysharp.Threading.Tasks;

namespace SPGame.Runtime
{
    /// <summary>
    /// 资源模块契约
    /// </summary>
    public interface IResourceModule : IModule
    {
        /// <summary>资源包是否初始化</summary>
        bool IsInitialized { get; }

        /// <summary>当前版本是否已准备，可以开始加载业务资源</summary>
        bool IsReady { get; }

        /// <summary>初始化资源模块</summary>
        /// <param name="settings">初始化设置</param>
        /// <returns></returns>
        UniTask InitializeAsync(ResourceSettings settings);

        /// <summary>准备目标版本的资源清单，传空表示加载当前运行环境的版本资源清单</summary>
        /// <param name="targetVersion">目标版本</param>
        /// <returns>资源更新计划</returns>
        UniTask<ResourceUpdatePlan> CheckUpdateAsync(string targetVersion = null);

        /// <summary>下载资源更新计划中的资源</summary>
        /// <param name="plan">资源更新计划</param>
        /// <param name="progress">下载进度</param>
        /// <returns></returns>
        UniTask DownloadAsync(ResourceUpdatePlan plan, IProgress<ResourceDownloadProgress> progress = null);

        /// <summary>加载普通资源</summary>
        /// <typeparam name="T">指定资源类型</typeparam>
        /// <param name="address">资源路径</param>
        /// <returns>资源句柄，用于卸载资源</returns>
        UniTask<ResourceHandle<T>> LoadAssetAsync<T>(string address) where T : UnityEngine.Object;

        /// <summary>加载场景资源</summary>
        /// <param name="address">场景路径</param>
        /// <param name="loadMode">加载模式</param>
        /// <returns>场景句柄，用于卸载场景</returns>
        UniTask<ResourceSceneHandle> LoadSceneAsync(string address, LoadSceneMode loadMode = LoadSceneMode.Single);
    }
}
