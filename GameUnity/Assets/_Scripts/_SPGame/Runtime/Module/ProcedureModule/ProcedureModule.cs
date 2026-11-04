using System;

namespace SPGame.Runtime
{
    /// <summary>
    /// 流程模块
    /// </summary>
    public sealed class ProcedureModule : IProcedureModule
    {
        private IFsmModule _fsmModule;
        private IFsm<IProcedureModule> _fsm;

        #region IModule

        // 低于状态机模块，保证流程先销毁
        public int Priority => -1;

        public void OnCreate()
        {
            _fsmModule = ModuleSystem.GetModule<IFsmModule>();
        }

        public void OnDestroy()
        {
            StopCurrentProcedure();
        }

        #endregion

        #region IProcedureModule

        public BaseProcedure CurrentProcedure => (BaseProcedure)_fsm?.CurrentState;

        public void StartProcedure(Type entryProcedureType, params BaseProcedure[] procedures)
        {
            StopCurrentProcedure();
            _fsm = _fsmModule.CreateFsm<IProcedureModule>(this, procedures);
            _fsm.Start(entryProcedureType);
        }

        public void SwitchState<TProcedure>() where TProcedure : BaseProcedure
        {
            _fsm.ChangeState<TProcedure>();
        }

        #endregion

        private void StopCurrentProcedure()
        {
            if (_fsm != null)
            {
                _fsmModule.DestroyFsm(_fsm);
                _fsm = null;
            }
        }
    }
}
