using System;

using UnityEngine;

namespace SPGame.Runtime
{
    /// <summary>
    /// 窗口逻辑基类
    /// </summary>
    public abstract class UIWindow
    {
        /// <summary>当前窗口预制体的实例对象</summary>
        public GameObject WindowObject { get; private set; }

        internal void Initialize(GameObject windowObject)
        {
            WindowObject = windowObject ?? throw new ArgumentNullException(nameof(windowObject));

            ScriptGenerator();
            BindMemberProperty();
            RegisterEvent();
            OnCreate();
        }

        internal void Refresh()
        {
            OnRefresh();
        }

        internal void NotifyVisible()
        {
            OnVisible();
        }

        internal void NotifyHidden()
        {
            OnHidden();
        }

        internal void DestroyWindow()
        {
            OnDestroy();
            WindowObject = null;
        }

        /// <summary>查找并绑定窗口控件</summary>
        protected virtual void ScriptGenerator() { }

        /// <summary>配置控件属性</summary>
        protected virtual void BindMemberProperty() { }

        /// <summary>注册窗口事件</summary>
        protected virtual void RegisterEvent() { }

        /// <summary>首次创建时执行</summary>
        protected virtual void OnCreate() { }

        /// <summary>刷新窗口数据</summary>
        protected virtual void OnRefresh() { }

        /// <summary>窗口变为可见时执行</summary>
        protected virtual void OnVisible() { }

        /// <summary>窗口变为不可见时执行</summary>
        protected virtual void OnHidden() { }

        /// <summary>销毁窗口时执行并解除事件注册</summary>
        protected virtual void OnDestroy() { }
    }
}
