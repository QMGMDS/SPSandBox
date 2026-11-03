using System;

using YooAsset;

namespace SPGame.Runtime
{
    /// <summary>
    /// 普通资源的使用句柄
    /// </summary>
    public sealed class ResourceHandle<T> : IDisposable where T : UnityEngine.Object
    {
        private readonly AssetHandle _handle;

        /// <summary>当前资源句柄是否有效</summary>
        public bool IsValid => _handle.IsValid;

        /// <summary>当前句柄持有的资源对象</summary>
        public T Asset
        {
            get
            {
                if (!IsValid)
                {
                    throw new ObjectDisposedException(nameof(ResourceHandle<T>));
                }

                return _handle.GetAssetObject<T>();
            }
        }

        internal ResourceHandle(AssetHandle handle)
        {
            _handle = handle;
        }

        /// <summary>释放当前资源引用</summary>
        public void Release()
        {
            if (IsValid)
            {
                _handle.Release();
            }
        }

        public void Dispose()
        {
            Release();
        }
    }
}
