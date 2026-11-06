using UnityEngine;

using SPGame.Runtime;

namespace Game.Runtime
{
    public class MainWindow : UIWindow
    {
        protected override void OnCreate()
        {
            Debug.Log("MainWindow OnCreate");
        }

        protected override void OnRefresh()
        {
            Debug.Log("MainWindow OnRefresh");
        }

        protected override void OnVisible()
        {
            Debug.Log("MainWindow OnVisible");
        }

        protected override void OnHidden()
        {
            Debug.Log("MainWindow OnHidden");
        }

        protected override void OnDestroy()
        {
            Debug.Log("MainWindow OnDestroy");
        }
    }
}
