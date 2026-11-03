using System;

namespace SPGame.Runtime
{
    /// <summary>
    /// 一台有限状态机对外暴露的业务能力
    /// </summary>
    public interface IFsm<T> where T : class
    {
        /// <summary>
        /// 状态机持有者
        /// </summary>
        T Owner { get; }

        /// <summary>
        /// 当前状态，未启动或已销毁时为空
        /// </summary>
        BaseState<T> CurrentState { get; }

        /// <summary>
        /// 是否正在运行
        /// </summary>
        bool IsRunning { get; }

        /// <summary>
        /// 启动状态机
        /// </summary>
        void Start<TState>() where TState : BaseState<T>;

        /// <summary>
        /// 启动状态机
        /// </summary>
        void Start(Type stateType);

        /// <summary>
        /// 切换运行中的状态，同类型切换也会重新进入
        /// </summary>
        /// <remarks>
        /// 不支持在 OnLeave 回调中再次切换状态
        /// </remarks>
        void ChangeState<TState>() where TState : BaseState<T>;

        /// <summary>
        /// 查询是否注册了指定类型的状态
        /// </summary>
        bool HasState<TState>() where TState : BaseState<T>;

        /// <summary>
        /// 获取已注册的状态实例
        /// </summary>
        TState GetState<TState>() where TState : BaseState<T>;
    }
}
