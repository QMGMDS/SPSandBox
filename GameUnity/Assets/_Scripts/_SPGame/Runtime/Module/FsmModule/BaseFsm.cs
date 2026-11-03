namespace SPGame.Runtime
{
    /// <summary>
    /// 状态机的管理入口，仅供模块统一更新和销毁
    /// </summary>
    internal abstract class BaseFsm
    {
        /// <summary>
        /// 更新本台有限状态机
        /// </summary>
        internal abstract void Update(float elapsedTime, float realElapsedTime);

        /// <summary>
        /// 销毁本台有限状态机
        /// </summary>
        internal abstract void Destroy();
    }
}
