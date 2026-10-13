namespace SPGame
{
    /// <summary>
    /// 有限状态机模块契约
    /// </summary>
    public interface IFsmModule : IUpdateModule
    {
        /// <summary>
        /// 创建有限状态机
        /// </summary>
        /// <typeparam name="T">持有者类型</typeparam>
        /// <param name="owner">持有者</param>
        /// <param name="states">状态集合</param>
        IFsm<T> CreateFsm<T>(T owner, params BaseState<T>[] states) where T : class;

        /// <summary>
        /// 销毁有限状态机
        /// </summary>
        void DestroyFsm<T>(IFsm<T> fsm) where T : class;
    }
}
