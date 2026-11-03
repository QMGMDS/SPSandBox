using UnityEngine;
using SPGame.Runtime;

public class GameEntry : MonoBehaviour
{
    private void Awake()
    {
        ModuleSystem.GetModule<IFsmModule>();
        Settings.ProcedureSettings.StartProcedure();
    }
}
