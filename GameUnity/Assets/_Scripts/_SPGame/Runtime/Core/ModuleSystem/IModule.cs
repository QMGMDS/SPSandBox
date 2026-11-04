namespace SPGame.Runtime
{
    /// <summary>
    /// 需要参与轮询的模块契约
    /// </summary>
    public interface IUpdateModule : IModule
    {
        /// <summary>模块轮询</summary>
        /// <param name="elapseSeconds">逻辑时间，单位为秒</param>
        /// <param name="realElapseSeconds">真实时间，单位为秒</param>
        void ThisModuleUpdate(float elapsedTime, float realElapsedTime);
    }

    /// <summary>
    /// 模块的基础契约
    /// </summary>
    public interface IModule
    {
        /// <summary>模块优先级</summary>
        /// <remarks>模块创建后保持不变，数值越大更新越早、销毁越晚</remarks>
        int Priority { get; }

        /// <summary>模块创建时执行</summary>
        void OnCreate();

        /// <summary>模块销毁时执行</summary>
        void OnDestroy();
    }
}
