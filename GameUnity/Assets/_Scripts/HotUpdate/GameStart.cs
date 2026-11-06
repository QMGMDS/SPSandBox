using UnityEngine;

using Cysharp.Threading.Tasks;

using SPGame.Runtime;

namespace Game.Runtime
{
    /// <summary>
    /// 热更业务启动入口
    /// </summary>
    public static class GameStart
    {
        /// <summary>由启动流程通过反射调用</summary>
        public static void Entrance()
        {
            // 在此编排热更业务初始化

            // 测试主菜单窗口
            // ModuleSystem.GetModule<ICameraModule>().StartRotation(Vector3.up, 2f);
            // ModuleSystem.GetModule<IUIModule>().ShowWindowAsync<MainWindow>().Forget();

            // 测试加载窗口
            // ModuleSystem.GetModule<IUIModule>().ShowWindowAsync<LoadingWindow>().Forget();
        }
    }
}
