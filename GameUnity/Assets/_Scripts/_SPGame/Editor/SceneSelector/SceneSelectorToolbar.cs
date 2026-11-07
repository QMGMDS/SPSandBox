
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

namespace SPGame.Editor
{
    /// <summary>
    /// Unity 编辑器全局工具栏场景选择器。
    /// </summary>
    [InitializeOnLoad]
    public static class SceneSelectorToolbar
    {
        #region Constants

        /// <summary>
        /// 自定义控件的唯一名称，用于防止重复挂载。
        /// </summary>
        private const string ElementName = "SceneSelector";

        /// <summary>
        /// Unity 编辑器内部工具栏类型名称。
        /// </summary>
        private const string ToolbarTypeName = "UnityEditor.Toolbar";

        /// <summary>
        /// Unity 内部工具栏根元素字段名称。
        /// </summary>
        private const string ToolbarRootFieldName = "m_Root";

        /// <summary>
        /// Unity 内部播放控制区域名称。
        /// </summary>
        private const string PlayModeZoneName = "ToolbarZonePlayMode";

        private const float MenuWidth = 160f;

        #endregion

        #region Fields

        /// <summary>
        /// 缓存反射得到的内部 Toolbar 类型。
        /// </summary>
        private static readonly Type s_toolbarType =
            typeof(UnityEditor.Editor).Assembly.GetType(ToolbarTypeName);

        /// <summary>
        /// 缓存内部字段信息，避免每次查找都重新反射。
        /// </summary>
        private static readonly FieldInfo s_toolbarRootField =
            s_toolbarType?.GetField(
                ToolbarRootFieldName,
                BindingFlags.Instance | BindingFlags.NonPublic);

        /// <summary>
        /// 自定义控件的容器。
        /// </summary>
        private static VisualElement s_container;

        /// <summary>
        /// 工具栏场景菜单。
        /// </summary>
        private static ToolbarMenu s_sceneMenu;

        #endregion

        #region Initialization

        /// <summary>
        /// 编辑器加载程序集后执行。
        /// </summary>
        static SceneSelectorToolbar()
        {
            // 等待 Unity 全局工具栏创建完成。
            EditorApplication.update += TryAttach;

            // 项目资源发生变化时刷新菜单。
            EditorApplication.projectChanged += OnProjectChanged;

            // 编辑模式下切换当前场景时，更新菜单显示。
            EditorSceneManager.activeSceneChangedInEditMode +=
                OnActiveSceneChanged;

            // 场景保存后，更新菜单显示。
            EditorSceneManager.sceneSaved += OnSceneSaved;
        }

        #endregion

        #region Toolbar Reflection

        /// <summary>
        /// 获取 Unity 全局工具栏的根 VisualElement。
        ///
        /// 反射职责被封装在这里，外部方法不需要了解：
        /// - UnityEditor.Toolbar
        /// - m_Root
        /// - BindingFlags
        ///
        /// 返回 true 表示已经找到可用的工具栏根元素。
        /// </summary>
        private static bool TryGetToolbarRoot(out VisualElement root)
        {
            root = null;

            if (s_toolbarType == null || s_toolbarRootField == null)
            {
                return false;
            }

            // UnityEditor.Toolbar 不是公开类型，
            // 因此通过 Type 查找已经创建的实例。
            UnityEngine.Object[] toolbarObjects =
                Resources.FindObjectsOfTypeAll(s_toolbarType);

            if (toolbarObjects == null || toolbarObjects.Length == 0)
            {
                return false;
            }

            foreach (UnityEngine.Object toolbarObject in toolbarObjects)
            {
                // 读取内部字段 m_Root。
                var candidateRoot =
                    s_toolbarRootField.GetValue(toolbarObject)
                    as VisualElement;

                // 等待工具栏 UI 真正挂载到面板。
                if (candidateRoot == null || candidateRoot.panel == null)
                {
                    continue;
                }

                root = candidateRoot;

                return true;
            }

            return false;
        }

        #endregion

        #region Toolbar Attachment

        /// <summary>
        /// 尝试把场景选择器挂到 Unity 全局工具栏。
        ///
        /// Unity 工具栏可能晚于脚本程序集初始化，
        /// 所以在 EditorApplication.update 中尝试挂载。
        /// </summary>
        private static void TryAttach()
        {
            // 已挂载到有效面板，不需要重复操作。
            if (s_container != null && s_container.panel != null)
            {
                return;
            }

            // 获取 Unity 工具栏根节点。
            if (!TryGetToolbarRoot(out VisualElement toolbarRoot))
            {
                return;
            }

            // 查找是否已经存在同名控件，防止重复挂载。
            VisualElement existingElement =
                toolbarRoot.Q<VisualElement>(ElementName);

            if (existingElement != null)
            {
                s_container = existingElement;
                s_sceneMenu = existingElement.Q<ToolbarMenu>();

                SyncSceneMenuText();

                return;
            }

            // 查找播放按钮所在区域。
            VisualElement playModeZone =
                toolbarRoot.Q<VisualElement>(PlayModeZoneName);

            if (playModeZone == null || playModeZone.parent == null)
            {
                return;
            }

            // 创建容器。
            s_container = CreateContainer();

            // 创建菜单。
            s_sceneMenu = CreateSceneMenu();

            // 将菜单放入容器。
            s_container.Add(s_sceneMenu);

            // 插入到播放控制区域之后。
            // 具体位置依赖 Unity 2022.3 的内部布局结构。
            VisualElement parent = playModeZone.parent;

            int playModeZoneIndex = parent.IndexOf(playModeZone);

            parent.Insert(playModeZoneIndex + 1, s_container);
        }

        /// <summary>
        /// 创建自定义 UI 容器。
        /// </summary>
        private static VisualElement CreateContainer()
        {
            var container = new VisualElement
            {
                name = ElementName
            };

            container.style.flexDirection = FlexDirection.Row;
            container.style.alignItems = Align.Center;
            container.style.marginLeft = 8;
            container.style.flexShrink = 0;

            return container;
        }

        #endregion

        #region Menu Creation

        /// <summary>
        /// 创建用于选择场景的 ToolbarMenu。
        ///
        /// ToolbarMenu 不同于 DropdownField：
        /// - 没有 choices 属性。
        /// - 不通过 RegisterValueChangedCallback 处理选择。
        /// - 使用 menu.AppendAction 注册菜单命令。
        /// </summary>
        private static ToolbarMenu CreateSceneMenu()
        {
            var sceneMenu = new ToolbarMenu
            {
                tooltip = "快速切换项目场景"
            };

            sceneMenu.style.width = MenuWidth;
            sceneMenu.style.flexShrink = 0;

            // 根据当前真实场景设置菜单文字。
            sceneMenu.text = GetActiveSceneDisplayName();

            // 获取项目中的所有场景路径。
            List<string> scenePaths = EditorAssetFinder.FindScenePaths();

            if (scenePaths.Count == 0)
            {
                sceneMenu.text = "无可用场景";
                sceneMenu.SetEnabled(false);

                return sceneMenu;
            }

            foreach (string scenePath in scenePaths)
            {
                string path = scenePath;

                string sceneName =
                    Path.GetFileNameWithoutExtension(path);

                // 每个菜单项都对应一个具体的场景路径。
                sceneMenu.menu.AppendAction(
                    sceneName,
                    action => SwitchScene(path)
                );
            }

            return sceneMenu;
        }

        /// <summary>
        /// 获取当前场景的展示名称。
        ///
        /// 当前场景可能尚未保存，此时路径为空。
        /// </summary>
        private static string GetActiveSceneDisplayName()
        {
            string scenePath = SceneManager.GetActiveScene().path;

            if (string.IsNullOrEmpty(scenePath))
            {
                return "选择场景";
            }

            return Path.GetFileNameWithoutExtension(scenePath);
        }

        #endregion

        #region Scene Switching

        /// <summary>
        /// 在 Unity 编辑器中切换场景。
        ///
        /// 只允许编辑模式切换。
        /// 切换前询问用户是否保存修改。
        /// </summary>
        private static void SwitchScene(string scenePath)
        {
            if (string.IsNullOrEmpty(scenePath))
            {
                return;
            }

            // 不允许在播放模式及其切换过程中执行。
            if (EditorApplication.isPlayingOrWillChangePlaymode)
            {
                Debug.LogWarning("请先退出 Play Mode，再切换编辑器场景。");

                return;
            }

            // 当前已经是目标场景，不必重新打开。
            string currentPath = SceneManager.GetActiveScene().path;

            if (string.Equals(
                    currentPath,
                    scenePath,
                    StringComparison.Ordinal))
            {
                return;
            }

            // 询问是否保存当前场景修改。
            // 如果选择 Cancel，则不切换。
            if (!EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo())
            {
                return;
            }

            try
            {
                EditorSceneManager.OpenScene(
                    scenePath,
                    OpenSceneMode.Single);
            }
            catch (Exception exception)
            {
                Debug.LogException(exception);
            }
            finally
            {
                // 以 Unity 实际打开的场景为准。
                SyncSceneMenuText();
            }
        }

        #endregion

        #region State Synchronization

        /// <summary>
        /// 同步工具栏显示的场景名称。
        ///
        /// 这里不修改场景本身，只更新 UI。
        /// </summary>
        private static void SyncSceneMenuText()
        {
            if (s_sceneMenu == null)
            {
                return;
            }

            s_sceneMenu.text = GetActiveSceneDisplayName();
        }

        /// <summary>
        /// 项目资源发生变化时，重新生成场景菜单。
        /// </summary>
        private static void RebuildSceneMenu()
        {
            if (s_container == null || s_container.panel == null)
            {
                return;
            }

            // 重新创建菜单，旧菜单会从容器中移除。
            ToolbarMenu newMenu = CreateSceneMenu();

            s_container.Clear();

            s_container.Add(newMenu);

            s_sceneMenu = newMenu;
        }

        #endregion

        #region Editor Events

        /// <summary>
        /// 项目资源发生变化时触发。
        /// 例如新增、删除、移动或重命名场景资源。
        /// </summary>
        private static void OnProjectChanged()
        {
            RebuildSceneMenu();
        }

        /// <summary>
        /// 当前编辑场景发生变化时触发。
        /// </summary>
        private static void OnActiveSceneChanged(
            Scene previousScene,
            Scene newScene)
        {
            SyncSceneMenuText();
        }

        /// <summary>
        /// 场景保存时触发。
        /// </summary>
        private static void OnSceneSaved(Scene scene)
        {
            SyncSceneMenuText();
        }

        #endregion
    }
}
