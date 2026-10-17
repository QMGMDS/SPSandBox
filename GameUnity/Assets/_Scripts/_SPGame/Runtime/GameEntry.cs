using UnityEngine;
using SPGame;

public class GameEntry : MonoBehaviour
{
    private void Awake()
    {
        ModuleSystem.GetModule<IFsmModule>();
        Settings.ProcedureSettings.StartProcedure();
    }
}
