namespace SPGame
{
    /// <summary>
    /// 具体状态的基类，每个实例由一台状态机独占
    /// </summary>
    public abstract class BaseState<T> where T : class
    {
        /// <summary>
        /// 所有状态注册完成后初始化
        /// </summary>
        public virtual void OnInit(IFsm<T> fsm)
        {
        }

        /// <summary>
        /// 进入状态
        /// </summary>
        public virtual void OnEnter(IFsm<T> fsm)
        {
        }

        /// <summary>
        /// 更新当前状态
        /// </summary>
        /// <param name="fsm">所属状态机</param>
        /// <param name="elapsedTime">逻辑时间，单位为秒</param>
        /// <param name="realElapsedTime">真实时间，单位为秒</param>
        public virtual void OnUpdate(IFsm<T> fsm, float elapsedTime, float realElapsedTime)
        {
        }

        /// <summary>
        /// 离开状态
        /// </summary>
        public virtual void OnLeave(IFsm<T> fsm)
        {
        }

        /// <summary>
        /// 状态机销毁时释放状态持有的资源
        /// </summary>
        public virtual void OnDestroy(IFsm<T> fsm)
        {
        }
    }
}
