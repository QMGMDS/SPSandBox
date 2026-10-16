using UnityEngine;

namespace SPGame
{
    /// <summary>
    /// 模块运行时宿主
    /// </summary>
    [DisallowMultipleComponent]
    public sealed class RootModule : MonoBehaviour
    {
        private void Awake()
        {
        }

        private void Start()
        {
        }

        private void Update()
        {
            GameTime.StartFrame();
            ModuleSystem.Update(GameTime.DeltaTime, GameTime.UnscaledDeltaTime);
        }

        private void LateUpdate()
        {
        }

        private void OnDestroy()
        {
            ModuleSystem.Destroy();
        }

        private void FixedUpdate()
        {
        }
    }
}
