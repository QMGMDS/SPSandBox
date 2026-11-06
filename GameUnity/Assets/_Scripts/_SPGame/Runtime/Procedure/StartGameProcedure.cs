using System;
using System.Reflection;

namespace SPGame.Runtime
{
    /// <summary>
    /// 启动热更业务的流程
    /// </summary>
    public sealed class StartGameProcedure : BaseProcedure
    {
        private const string ASSEMBLY_NAME = "Game.Runtime";
        private const string ENTRY_TYPE_NAME = "Game.Runtime.GameStart";
        private const string ENTRY_METHOD_NAME = "Entrance";

        public override void OnEnter(IFsm<IProcedureModule> fsm)
        {
            Assembly gameAssembly = null;
            foreach (Assembly assembly in AppDomain.CurrentDomain.GetAssemblies())
            {
                if (assembly.GetName().Name == ASSEMBLY_NAME)
                {
                    gameAssembly = assembly;
                    break;
                }
            }

            Type entryType = gameAssembly.GetType(ENTRY_TYPE_NAME);
            MethodInfo entrance = entryType.GetMethod(ENTRY_METHOD_NAME, BindingFlags.Public | BindingFlags.Static);
            entrance.Invoke(null, null);
        }
    }
}
