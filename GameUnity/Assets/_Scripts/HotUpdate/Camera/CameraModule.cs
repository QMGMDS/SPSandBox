using System;

using UnityEngine;

using SPGame.Runtime;

namespace Game.Runtime
{
    /// <summary>
    /// 控制主摄像机的业务模块
    /// </summary>
    public sealed class CameraModule : ICameraModule
    {
        private Transform _cameraTransform;
        private Vector3 _worldAxis;
        private float _degreesPerSecond;

        #region IUpdateModule

        public int Priority => 0;

        public void OnCreate()
        {
            Camera camera = Camera.main;
            if (camera == null)
            {
                throw new InvalidOperationException("摄像机模块未找到主摄像机");
            }

            _cameraTransform = camera.transform;
        }

        public void OnDestroy()
        {
            StopRotation();
            _cameraTransform = null;
        }

        #endregion

        #region ICameraModule

        public void StartRotation(Vector3 worldAxis, float degreesPerSecond)
        {
            _worldAxis = worldAxis;
            _degreesPerSecond = degreesPerSecond;
        }

        public void StopRotation()
        {
            _degreesPerSecond = 0f;
        }

        void IUpdateModule.ThisModuleUpdate(float elapsedTime, float realElapsedTime)
        {
            float angle = _degreesPerSecond * elapsedTime;
            if (_cameraTransform == null || angle == 0f)
            {
                return;
            }

            _cameraTransform.Rotate(_worldAxis, angle, Space.World);
        }

        #endregion
    }
}
