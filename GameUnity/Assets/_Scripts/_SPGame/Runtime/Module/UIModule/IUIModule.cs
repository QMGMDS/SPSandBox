using Cysharp.Threading.Tasks;

namespace SPGame.Runtime
{
    /// <summary>
    /// UI 模块契约
    /// </summary>
    public interface IUIModule : IModule
    {
        /// <summary>异步显示窗口</summary>
        /// <remarks>排到最上方</remarks>
        /// <typeparam name="T">窗口类型</typeparam>
        UniTask ShowWindowAsync<T>() where T : UIWindow, new();

        /// <summary>关闭窗口或取消正在加载的窗口</summary>
        /// <typeparam name="T">窗口类型</typeparam>
        /// <returns>是否成功关闭</returns>
        bool CloseWindow<T>() where T : UIWindow;

        /// <summary>刷新窗口</summary>
        /// <typeparam name="T">窗口类型</typeparam>
        /// <returns>是否成功刷新</returns>
        bool RefreshWindow<T>() where T : UIWindow;
    }
}
