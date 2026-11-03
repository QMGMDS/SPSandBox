using System;
using System.Collections.Generic;

namespace SPGame.Runtime
{
    /// <summary>
    /// 状态机实现，由状态机模块创建和管理
    /// </summary>
    internal sealed class Fsm<T> : BaseFsm, IFsm<T> where T : class
    {
        private T _owner;
        private readonly Dictionary<Type, BaseState<T>> _states = new Dictionary<Type, BaseState<T>>();
        private BaseState<T> _currentState;

        #region IFsm<T>

        public T Owner => _owner;
        public BaseState<T> CurrentState => _currentState;
        public bool IsRunning => _currentState != null;

        internal Fsm(T owner, BaseState<T>[] states)
        {
            _owner = owner;
            foreach (BaseState<T> state in states)
            {
                _states.Add(state.GetType(), state);
            }

            foreach (BaseState<T> state in _states.Values)
            {
                state.OnInit(this);
            }
        }

        public void Start<TState>() where TState : BaseState<T>
        {
            Start(typeof(TState));
        }

        public void Start(Type stateType)
        {
            if (IsRunning)
            {
                throw new InvalidOperationException("状态机已经启动");
            }

            _currentState = _states[stateType];
            _currentState.OnEnter(this);
        }

        public void ChangeState<TState>() where TState : BaseState<T>
        {
            if (!IsRunning)
            {
                throw new InvalidOperationException("状态机尚未启动");
            }

            BaseState<T> nextState = GetState<TState>();
            _currentState.OnLeave(this);
            _currentState = nextState;
            _currentState.OnEnter(this);
        }

        public bool HasState<TState>() where TState : BaseState<T>
        {
            return _states.ContainsKey(typeof(TState));
        }

        public TState GetState<TState>() where TState : BaseState<T>
        {
            return (TState)_states[typeof(TState)];
        }

        #endregion

        #region BaseFsm

        internal override void Update(float elapsedTime, float realElapsedTime)
        {
            _currentState?.OnUpdate(this, elapsedTime, realElapsedTime);
        }

        internal override void Destroy()
        {
            _currentState?.OnLeave(this);
            _currentState = null;
            foreach (BaseState<T> state in _states.Values)
            {
                state.OnDestroy(this);
            }

            _states.Clear();
            _owner = null;
        }

        #endregion
    }
}
