using System;

using UnityEngine;

using Cysharp.Threading.Tasks;

namespace SPGame.Runtime
{
    /// <summary>
    /// 初始化资源包并准备启动游戏所需的资源
    /// </summary>
    public sealed class ResourceInitProcedure : BaseProcedure
    {
        public override void OnEnter(IFsm<IProcedureModule> fsm)
        {
            PrepareResourcesAsync(fsm).Forget();
        }

        private async UniTask PrepareResourcesAsync(IFsm<IProcedureModule> fsm)
        {
            try
            {
                IResourceModule resourceModule = ModuleSystem.GetModule<IResourceModule>();

                await resourceModule.InitializeAsync(Settings.ResourceSettings);
                ResourceUpdatePlan plan = await resourceModule.CheckUpdateAsync();
                if (plan.TotalDownloadCount > 0)
                    await resourceModule.DownloadAsync(plan);
            }
            catch (Exception exception)
            {
                Debug.LogError($"游戏资源准备失败：{exception}");
                return;
            }

            fsm.Owner.SwitchState<StartGameProcedure>();
        }
    }
}
