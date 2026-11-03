using System;

using UnityEngine.SceneManagement;

using Cysharp.Threading.Tasks;
using YooAsset;

namespace SPGame.Runtime
{
    /// <summary>
    /// 场景资源的使用句柄
    /// </summary>
    public sealed class ResourceSceneHandle
    {
        private readonly SceneHandle _handle;

        /// <summary>当前资源句柄是否有效</summary>
        public bool IsValid => _handle.IsValid;

        /// <summary>当前句柄持有的资源对象</summary>
        public Scene Scene
        {
            get
            {
                if (!IsValid)
                {
                    throw new ObjectDisposedException(nameof(ResourceSceneHandle));
                }

                return _handle.SceneObject;
            }
        }

        internal ResourceSceneHandle(SceneHandle handle)
        {
            _handle = handle;
        }

        /// <summary>卸载场景并释放引用，已经被场景切换卸载时直接返回</summary>
        public async UniTask UnloadAsync()
        {
            if (IsValid)
            {
                UnloadSceneOperation operation = _handle.UnloadSceneAsync();
                await operation;
            }
        }
    }
}
