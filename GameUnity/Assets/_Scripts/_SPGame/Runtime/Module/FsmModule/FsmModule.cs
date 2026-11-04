using System.Collections.Generic;

namespace SPGame.Runtime
{
    /// <summary>
    /// 有限状态机模块
    /// </summary>
    public sealed class FsmModule : IFsmModule
    {
        private readonly List<BaseFsm> _fsms;
        private readonly List<BaseFsm> _updateFsms;

        public FsmModule()
        {
            _fsms = new List<BaseFsm>();
            _updateFsms = new List<BaseFsm>();
        }

        #region IUpdateModule

        public int Priority => 0;

        public void OnCreate() { }

        public void OnDestroy()
        {
            while (_fsms.Count > 0)
            {
                int lastIndex = _fsms.Count - 1;
                BaseFsm fsm = _fsms[lastIndex];
                _fsms.RemoveAt(lastIndex);
                fsm.Destroy();
            }
        }

        public void ThisModuleUpdate(float elapsedTime, float realElapsedTime)
        {
            // 固定本轮更新范围，新建的状态机从下一轮开始更新
            _updateFsms.Clear();
            _updateFsms.AddRange(_fsms);
            for (int i = 0; i < _updateFsms.Count; i++)
            {
                _updateFsms[i].Update(elapsedTime, realElapsedTime);
            }
        }

        #endregion

        #region IFsmModule

        public IFsm<T> CreateFsm<T>(T owner, params BaseState<T>[] states) where T : class
        {
            Fsm<T> fsm = new Fsm<T>(owner, states);
            _fsms.Add(fsm);
            return fsm;
        }

        public void DestroyFsm<T>(IFsm<T> fsm) where T : class
        {
            Fsm<T> instance = (Fsm<T>)fsm;
            if (_fsms.Remove(instance))
            {
                instance.Destroy();
            }
        }

        #endregion
    }
}
