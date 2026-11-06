using UnityEngine;

using SPGame.Runtime;

namespace Game.Runtime
{
    public class MainWindow : UIWindow
    {
        protected override void OnCreate()
        {
            base.OnCreate();
            Debug.Log("MainWindow OnCreate");
        }

        protected override void OnRefresh()
        {
            base.OnRefresh();
            Debug.Log("MainWindow OnRefresh");
        }

        protected override void OnVisible()
        {
            base.OnVisible();
            Debug.Log("MainWindow OnVisible");
        }

        protected override void OnHidden()
        {
            base.OnHidden();
            Debug.Log("MainWindow OnHidden");
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            Debug.Log("MainWindow OnDestroy");
        }
    }
}
