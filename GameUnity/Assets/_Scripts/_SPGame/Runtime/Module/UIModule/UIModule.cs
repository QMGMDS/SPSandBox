using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace SPGame.Runtime
{
    /// <summary>
    /// UI 模块
    /// </summary>
    public sealed class UIModule : IUIModule
    {
        // 最大窗口数量（Canvas 的最大排序范围为 32767）
        private const int MAX_SORTING_ORDER = 100;

        private readonly Dictionary<Type, string> _addresses = new();
        private readonly Dictionary<Type, WindowRecord> _windows = new();
        private readonly List<Type> _displayOrder = new();

        private IResourceModule _resourceModule;
        private GameObject _uiRoot;
        private Canvas _rootCanvas;

        #region IModule

        public int Priority => 0;

        public void OnCreate()
        {
            _resourceModule = ModuleSystem.GetModule<IResourceModule>();
            CreateUIRoot();
        }

        public void OnDestroy()
        {
            DestroyAllWindows();
            _addresses.Clear();

            _uiRoot = null;
            _rootCanvas = null;
            _resourceModule = null;
        }

        #endregion

        #region IUIModule

        public void RegisterWindow<T>(string address) where T : UIWindow, new()
        {
            if (string.IsNullOrWhiteSpace(address))
                throw new ArgumentException("窗口资源地址不能为空", nameof(address));

            Type type = typeof(T);

            if (!_addresses.TryAdd(type, address))
                throw new InvalidOperationException($"窗口类型已注册：{type.FullName}");
        }

        public async UniTask ShowWindowAsync<T>() where T : UIWindow, new()
        {
            Type type = typeof(T);

            if (!_addresses.TryGetValue(type, out string address))
                throw new InvalidOperationException($"窗口类型尚未注册：{type.FullName}");

            // 已加载的窗口直接显示
            if (_windows.TryGetValue(type, out WindowRecord existing))
            {
                ShowExistingWindow(type, existing);
                return;
            }

            ResourceHandle<GameObject> handle = null;
            GameObject instance = null;
            T window = null;
            bool added = false;

            try
            {
                // 加载预制体
                handle = await _resourceModule.LoadAssetAsync<GameObject>(address);

                if (handle == null || handle.Asset == null)
                    throw new InvalidOperationException($"窗口资源加载失败：{address}");

                // 实例化窗口
                instance = UnityEngine.Object.Instantiate(handle.Asset, _rootCanvas.transform, false);

                Canvas canvas = instance.GetComponent<Canvas>();
                if (canvas == null)
                    throw new InvalidOperationException($"窗口缺少 Canvas：{address}");

                canvas.enabled = false;
                instance.SetActive(true);
                canvas.overrideSorting = true;
                canvas.sortingLayerID = _rootCanvas.sortingLayerID;

                // 初始化窗口逻辑
                window = new T();
                window.Initialize(instance);
                window.Refresh();

                // 注册窗口
                var record = new WindowRecord(window, canvas, handle);
                _windows.Add(type, record);
                added = true;

                _displayOrder.Add(type);
                RecalculateSortingOrders();

                // 显示窗口
                canvas.enabled = true;
                window.NotifyVisible();

                // 所有权转交给 WindowRecord
                handle = null;
                instance = null;
                window = null;
            }
            catch
            {
                if (added)
                {
                    _windows.Remove(type);
                    _displayOrder.Remove(type);
                    RecalculateSortingOrders();
                }

                throw;
            }
            finally
            {
                // 失败时释放尚未转交所有权的资源
                try
                {
                    window?.DestroyWindow();
                }
                finally
                {
                    if (instance != null)
                        UnityEngine.Object.Destroy(instance);

                    handle?.Release();
                }
            }
        }

        public bool CloseWindow<T>() where T : UIWindow
        {
            Type type = typeof(T);

            if (!_windows.TryGetValue(type, out WindowRecord record))
                return false;

            CloseLoadedWindow(type, record);
            return true;
        }

        public bool RefreshWindow<T>() where T : UIWindow
        {
            if (!_windows.TryGetValue(typeof(T), out WindowRecord record))
                return false;

            record.Window.Refresh();
            return true;
        }

        #endregion

        private void CreateUIRoot()
        {
            _uiRoot = GameObject.Find("UIRoot");

            if (_uiRoot == null)
                throw new InvalidOperationException("场景中不存在 UIRoot");

            _rootCanvas = _uiRoot.GetComponentInChildren<Canvas>();

            if (_rootCanvas == null)
                throw new InvalidOperationException("UIRoot 缺少 Canvas");

            UnityEngine.Object.DontDestroyOnLoad(_uiRoot);
        }

        private void ShowExistingWindow(Type type, WindowRecord record)
        {
            _displayOrder.Remove(type);
            _displayOrder.Add(type);

            RecalculateSortingOrders();
            record.Window.Refresh();
        }

        private void CloseLoadedWindow(Type type, WindowRecord record)
        {
            _windows.Remove(type);
            _displayOrder.Remove(type);
            RecalculateSortingOrders();

            GameObject instance = record.Window.WindowObject;

            try
            {
                record.Window.NotifyHidden();
            }
            finally
            {
                try
                {
                    record.Window.DestroyWindow();
                }
                finally
                {
                    if (record.Canvas != null)
                        record.Canvas.enabled = false;

                    if (instance != null)
                        UnityEngine.Object.Destroy(instance);

                    record.Handle.Release();
                }
            }
        }

        private void RecalculateSortingOrders()
        {
            if (_displayOrder.Count > MAX_SORTING_ORDER)
                throw new InvalidOperationException("窗口数量超出上限");

            for (int i = 0; i < _displayOrder.Count; i++)
            {
                _windows[_displayOrder[i]].Canvas.sortingOrder = i + 1;
            }
        }

        private void DestroyAllWindows()
        {
            while (_displayOrder.Count > 0)
            {
                Type type = _displayOrder[^1];

                try
                {
                    CloseLoadedWindow(type, _windows[type]);
                }
                catch (Exception exception)
                {
                    Debug.LogException(exception);
                }
            }
        }

        private sealed class WindowRecord
        {
            public UIWindow Window { get; }
            public Canvas Canvas { get; }
            public ResourceHandle<GameObject> Handle { get; }

            public WindowRecord(
                UIWindow window,
                Canvas canvas,
                ResourceHandle<GameObject> handle)
            {
                Window = window;
                Canvas = canvas;
                Handle = handle;
            }
        }
    }
}