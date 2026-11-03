using System;
using System.Collections.Generic;

namespace SPGame.Runtime
{
    /// <summary>
    /// 模块系统，统一管理所有模块
    /// </summary>
    public static class ModuleSystem
    {
        private static readonly Dictionary<Type, IModule> s_modules = new Dictionary<Type, IModule>();

        private static readonly LinkedList<IModule> s_sortedModules = new LinkedList<IModule>();
        private static readonly List<IUpdateModule> s_updateModules = new List<IUpdateModule>();
        private static bool s_updateModulesDirty; // 脏标记

        /// <summary>
        /// 模块轮询
        /// </summary>
        /// <param name="elapsedTime">逻辑时间，单位为秒</param>
        /// <param name="realElapsedTime">真实时间，单位为秒</param>
        public static void Update(float elapsedTime, float realElapsedTime)
        {
            if (s_updateModulesDirty)
            {
                s_updateModulesDirty = false;
                BuildUpdateModulesList();
            }

            for (int i = 0; i < s_updateModules.Count; i++)
            {
                s_updateModules[i].ThisModuleUpdate(elapsedTime, realElapsedTime);
            }
        }

        private static void BuildUpdateModulesList()
        {
            s_updateModules.Clear();
            foreach (IModule module in s_sortedModules)
            {
                if (module is IUpdateModule updateModule)
                {
                    s_updateModules.Add(updateModule);
                }
            }
        }

        /// <summary>
        /// 获取模块
        /// </summary>
        /// <remarks>
        /// 接口与实现必须是同一程序集、同一命名空间中的顶层非泛型类型，
        /// 接口名称以 I 开头，实现类名称为接口名称去掉首个 I，
        /// 实现必须是非抽象的普通 C# 类，模块间的依赖由调用方管理
        /// </remarks>
        public static T GetModule<T>() where T : class, IModule
        {
            Type type = typeof(T);
            if (s_modules.TryGetValue(type, out IModule module))
            {
                return (T)module;
            }

            if (!type.IsInterface)
            {
                throw new ArgumentException("获取的模块必须是一个接口类型的：", type.FullName);
            }

            string moduleName = $"{type.Namespace}.{type.Name.Substring(1)}, {type.Assembly.GetName().Name}";
            Type moduleType = Type.GetType(moduleName);
            if (moduleType == null)
            {
                throw new TypeLoadException("找不到该模块：" + moduleName);
            }

            T instance = (T)Activator.CreateInstance(moduleType);
            RegisterModule(type, instance);
            return instance;
        }

        private static void RegisterModule(Type type, IModule module)
        {
            int priority = module.Priority;
            LinkedListNode<IModule> node = s_sortedModules.Last;
            while (node != null && node.Value.Priority < priority)
            {
                node = node.Previous;
            }

            if (node == null)
            {
                s_sortedModules.AddFirst(module);
            }
            else
            {
                s_sortedModules.AddAfter(node, module);
            }

            s_modules.Add(type, module);
            s_updateModulesDirty = true;
        }

        /// <summary>
        /// 销毁并清理所有的模块
        /// </summary>
        public static void Destroy()
        {
            for (LinkedListNode<IModule> node = s_sortedModules.Last; node != null; node = node.Previous)
            {
                node.Value.Destroy();
            }

            s_modules.Clear();
            s_sortedModules.Clear();
            s_updateModules.Clear();
            s_updateModulesDirty = false;
        }
    }
}
