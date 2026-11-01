using UnityEngine;

namespace SPGame
{
    public class Settings : MonoBehaviour
    {
        private static Settings s_instance;

        public static Settings Instance
        {
            get
            {
                if (s_instance == null)
                {
                    s_instance = FindObjectOfType<Settings>();

                    if (s_instance == null)
                    {
                        throw new System.InvalidOperationException("无法获取 Settings.Instance，未找到已激活的 Settings 组件，请确认组件已添加且所在场景已加载");
                    }
                }
                return s_instance;
            }
        }

        [SerializeField] private ProcedureSettings _procedureSettings;
        public static ProcedureSettings ProcedureSettings => Instance._procedureSettings;

        [SerializeField] private ResourceSettings _resourceSettings;
        public static ResourceSettings ResourceSettings => Instance._resourceSettings;
    }
}