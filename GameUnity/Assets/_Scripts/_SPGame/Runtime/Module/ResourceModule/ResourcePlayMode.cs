namespace SPGame.Runtime
{
    /// <summary>
    /// 支持的资源运行模式
    /// </summary>
    public enum ResourcePlayMode
    {
        /// <summary>编辑器模拟模式</summary>
        EditorSimulate,

        /// <summary>离线模式，只使用安装包内的资源</summary>
        Offline,

        /// <summary>联机模式，使用内置资源、本地缓存和远端服务器</summary>
        Host
    }
}
