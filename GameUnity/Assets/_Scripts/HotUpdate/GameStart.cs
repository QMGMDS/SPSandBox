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
            ModuleSystem.GetModule<ICameraModule>().StartRotation(Vector3.up, 2f);
            ModuleSystem.GetModule<IUIModule>().ShowWindowAsync<MainWindow>().Forget();
        }
    }
}
