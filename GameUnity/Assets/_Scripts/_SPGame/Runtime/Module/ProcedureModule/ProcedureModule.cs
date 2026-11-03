using System;

namespace SPGame.Runtime
{
    /// <summary>
    /// 流程模块
    /// </summary>
    public sealed class ProcedureModule : IProcedureModule
    {
        private readonly IFsmModule _fsmModule;
        private IFsm<IProcedureModule> _fsm;

        public ProcedureModule()
        {
            _fsmModule = ModuleSystem.GetModule<IFsmModule>();
        }

        #region IModule

        // 低于状态机模块，保证流程先销毁
        public int Priority => -1;

        public void Destroy()
        {
            if (_fsm != null)
            {
                _fsmModule.DestroyFsm(_fsm);
                _fsm = null;
            }
        }

        #endregion

        #region IProcedureModule

        public BaseProcedure CurrentProcedure => (BaseProcedure)_fsm?.CurrentState;

        public void StartProcedure(Type entryProcedureType, params BaseProcedure[] procedures)
        {
            Destroy();
            _fsm = _fsmModule.CreateFsm<IProcedureModule>(this, procedures);
            _fsm.Start(entryProcedureType);
        }

        public void SwitchState<TProcedure>() where TProcedure : BaseProcedure
        {
            _fsm.ChangeState<TProcedure>();
        }

        #endregion
    }
}
