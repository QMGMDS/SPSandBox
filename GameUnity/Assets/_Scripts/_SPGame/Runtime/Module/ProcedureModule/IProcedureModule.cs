using System;

namespace SPGame
{
    /// <summary>
    /// 流程模块契约
    /// </summary>
    public interface IProcedureModule : IModule
    {
        /// <summary>
        /// 当前流程，未启动或已销毁时为空
        /// </summary>
        BaseProcedure CurrentProcedure { get; }

        /// <summary>
        /// 开始流程
        /// </summary>
        void StartProcedure(Type entryProcedureType, params BaseProcedure[] procedures);

        /// <summary>
        /// 切换到已注册的流程
        /// </summary>
        void SwitchState<TProcedure>() where TProcedure : BaseProcedure;
    }
}
