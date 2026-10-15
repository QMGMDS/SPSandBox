using System;
using UnityEngine;

namespace SPGame
{
    /// <summary>
    /// 流程配置 SO
    /// </summary>
    [CreateAssetMenu(fileName = "ProcedureSettings", menuName = "SPGame/Procedure Settings")]
    public sealed class ProcedureSettings : ScriptableObject
    {
        [SerializeField]
        [Tooltip("注册的流程类型全名")]
        private string[] _procedureTypeNames =
        {
            typeof(LaunchProcedure).FullName,
            typeof(SplashProcedure).FullName
        };

        [SerializeField]
        [Tooltip("入口流程的类型全名，必须包含在注册集合中")]
        private string _entryProcedureTypeName = typeof(LaunchProcedure).FullName;

        /// <summary>
        /// 启动流程
        /// </summary>
        public void StartProcedure()
        {
            BaseProcedure[] procedures = new BaseProcedure[_procedureTypeNames.Length];
            for (int i = 0; i < _procedureTypeNames.Length; i++)
            {
                Type procedureType = Type.GetType(_procedureTypeNames[i], true);
                procedures[i] = (BaseProcedure)Activator.CreateInstance(procedureType);
            }

            Type entryProcedureType = Type.GetType(_entryProcedureTypeName, true);
            ModuleSystem.GetModule<IProcedureModule>().StartProcedure(entryProcedureType, procedures);
        }
    }
}
