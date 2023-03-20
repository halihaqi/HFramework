using System;
using System.Collections.Generic;

namespace HFramework
{
    public static class HEntry
    {
        private static readonly LinkedList<HModule> _modules = new LinkedList<HModule>();
        private static LinkedListNode<HModule> _cachedNode = null;

        public static IAudioManager AudioMgr { get; private set; }
        public static IDataManager DataMgr { get; private set; }
        public static IEventManager EventMgr { get; private set; }
        public static IFsmManager FsmMgr { get; private set; }
        public static IMonoManager MonoMgr { get; private set; }
        public static IInputManager InputMgr { get; private set; }
        public static IObjectPoolManager ObjectPoolMgr { get; private set; }
        public static IProcedureManager ProcedureMgr { get; private set; }
        public static IReferencePoolManager ReferencePoolMgr { get; private set; }
        public static IResourceManager ResMgr { get; private set; }
        public static ISceneManager SceneMgr { get; private set; }
        public static IUIManager UIMgr { get; private set; }



        public static void Init()
        {
            AudioMgr = CreateModule<AudioManager>();
            DataMgr = CreateModule<DataManager>();
            EventMgr = CreateModule<EventManager>();
            FsmMgr = CreateModule<FsmManager>();
            MonoMgr = CreateModule<MonoManager>();
            InputMgr = CreateModule<InputManager>();
            ObjectPoolMgr = CreateModule<ObjectPoolManager>();
            ProcedureMgr = CreateModule<ProcedureManager>();
            ReferencePoolMgr = CreateModule<ReferencePoolManager>();
            ResMgr = CreateModule<ResourceManager>();
            SceneMgr = CreateModule<SceneManager>();
            UIMgr = CreateModule<UIManager>();
            
            _cachedNode = _modules.First;
            while (_cachedNode != null)
            {
                _cachedNode.Value.Init();
                _cachedNode = _cachedNode.Next;
            }
        }

        /// <summary>
        /// 游戏模块轮询
        /// </summary>
        /// <param name="elapseSeconds">逻辑时间</param>
        /// <param name="realElapseSeconds">实际时间</param>
        private static void Update(float elapseSeconds, float realElapseSeconds)
        {
            foreach (var module in _modules)
                module.Update(elapseSeconds, realElapseSeconds);
        }

        /// <summary>
        /// 关闭并清理所有模块
        /// </summary>
        public static void Shutdown()
        {
            foreach (var module in _modules)
                module.Shutdown();
            _modules.Clear();
        }

        public static T GetModule<T>() where T : HModule
        {
            foreach (var module in _modules)
            {
                if (module is T)
                    return module as T;
            }

            var m = CreateModule<T>();
            m.Init();
            return m;
        }

        private static T CreateModule<T>() where T : HModule
            => CreateModule(typeof(T)) as T;

        private static HModule CreateModule(Type moduleType)
        {
            HModule module = (HModule)Activator.CreateInstance(moduleType);
            if (module == null)
                throw new Exception($"Can not create module {moduleType.FullName}");

            _cachedNode = _modules.First;
            while (_cachedNode != null)
            {
                if(module.Priority > _cachedNode.Value.Priority)
                    break;
                _cachedNode = _cachedNode.Next;
            }

            if (_cachedNode != null)
                _modules.AddBefore(_cachedNode, module);
            else
                _modules.AddLast(module);

            return module;
        }
    }
}