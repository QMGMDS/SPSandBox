using UnityEngine;

using SPGame.Runtime;

namespace Game.Runtime
{
    /// <summary>
    /// 摄像机模块契约
    /// </summary>
    public interface ICameraModule : IUpdateModule
    {
        /// <summary>在原地绕指定的世界空间轴持续旋转</summary>
        /// <remarks>重复调用会替换当前旋转参数</remarks>
        /// <param name="worldAxis">非零旋转轴</param>
        /// <param name="degreesPerSecond">每秒旋转角度，负值表示反向</param>
        void StartRotation(Vector3 worldAxis, float degreesPerSecond);

        /// <summary>停止旋转并保留当前朝向</summary>
        void StopRotation();
    }
}
