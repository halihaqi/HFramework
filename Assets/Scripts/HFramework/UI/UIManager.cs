using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace HFramework
{
    internal class UIManager : HModule, IUIManager
    {
        private Dictionary<string, UIGroup> _uiGroups;
        private Dictionary<int, string> _loadingPanels;
        private List<int> _hidingPanels;
        private HashSet<int> _loadingPanelsToRelease;
        private Queue<PanelEntity> _recycleQueue;
        private int _cachedSerialId;
        
        private CanvasEntity _canvas;
        private EventSystemEntity _eventSystem;
        private UICameraEntity _uiCamera;
        private UIStageEntity _uiStage; 

        public const string PANEL_PATH = "UI/";
        public const string CONTROL_PATH = "UI/Controls/";
        
        internal override int Priority => 10;

        /// <summary>
        /// 界面组数量
        /// </summary>
        public int UIGroupCount => _uiGroups.Count;

        internal override void Init()
        {
            _uiGroups = new Dictionary<string, UIGroup>();
            _loadingPanels = new Dictionary<int, string>();
            _hidingPanels = new List<int>();
            _loadingPanelsToRelease = new HashSet<int>();
            _recycleQueue = new Queue<PanelEntity>();
            _cachedSerialId = 0;
        }

        internal override void Update(float elapseSeconds, float realElapseSeconds)
        {
            while (_recycleQueue.Count > 0)
            {
                PanelEntity panelEntity = _recycleQueue.Dequeue();
                panelEntity.OnRecycle();
                HEntry.ObjectPoolMgr.PushObj(PANEL_PATH + panelEntity.AssetName, panelEntity.gameObject);
            }

            foreach (var group in _uiGroups.Values)
            {
                group.Update(elapseSeconds, realElapseSeconds);
            }
        }

        internal override void Shutdown()
        {
            _uiGroups.Clear();
            _loadingPanels.Clear();
            _loadingPanelsToRelease.Clear();
            _recycleQueue.Clear();
            _cachedSerialId = 0;
        }
        
        
        private void InitCanvas()
        {
            _canvas ??= new GameObject("Canvas").AddComponent<CanvasEntity>();
            _eventSystem ??= new GameObject("EventSystem").AddComponent<EventSystemEntity>();
            _uiCamera ??= new GameObject("UICamera").AddComponent<UICameraEntity>();
            _canvas.renderCamera = _uiCamera.Camera;
            _canvas.UpdateConfig();
            _uiStage ??= HEntry.ResMgr.Load<GameObject>("UI/UIStage").GetComponent<UIStageEntity>();
        }

        #region UIGroup

        /// <summary>
        /// 是否存在界面组
        /// </summary>
        /// <param name="groupName">组名</param>
        /// <returns></returns>
        public bool HasUIGroup(string groupName) => _uiGroups.ContainsKey(groupName);

        /// <summary>
        /// 获得界面组
        /// </summary>
        /// <param name="groupName">组名</param>
        /// <returns></returns>
        public UIGroup GetUIGroup(string groupName)
        {
            return _uiGroups.TryGetValue(groupName, out var group) ? group : null;
        }

        /// <summary>
        /// 获得所有界面组
        /// </summary>
        /// <returns></returns>
        public UIGroup[] GetAllUIGroups()
        {
            int index = 0;
            UIGroup[] allGroups = new UIGroup[_uiGroups.Count];
            foreach (var group in _uiGroups.Values)
                allGroups[index++] = group;
            return allGroups;
        }

        /// <summary>
        /// 添加界面组
        /// </summary>
        /// <param name="groupName">组名</param>
        /// <param name="groupDepth">深度</param>
        /// <returns></returns>
        public bool AddUIGroup(string groupName, int groupDepth)
        {
            if (HasUIGroup(groupName))
                return false;

            _uiGroups.Add(groupName, new UIGroup(groupName, groupDepth));
            InitCanvas();
            GameObject obj = new GameObject(groupName);
            obj.AddComponent<UIGroupEntity>().BindUIGroup(_uiGroups[groupName]);
            obj.transform.SetParent(_canvas.transform, false);
            return true;
        }

        #endregion


        #region Panel

        /// <summary>
        /// 是否拥有界面
        /// </summary>
        /// <param name="serialId"></param>
        /// <returns></returns>
        public bool HasPanel(int serialId)
        {
            foreach (var group in _uiGroups.Values)
            {
                if (group.HasPanel(serialId))
                    return true;
            }

            return false;
        }

        public bool HasPanel<T>() where T : PanelBase
        {
            foreach (var group in _uiGroups.Values)
            {
                if (group.HasPanel<T>())
                    return true;
            }

            return false;
        }

        /// <summary>
        /// 获得界面
        /// </summary>
        /// <param name="serialId"></param>
        /// <returns></returns>
        public PanelEntity GetPanel(int serialId)
        {
            foreach (var group in _uiGroups.Values)
            {
                var panel = group.GetPanel(serialId);
                if (panel != null)
                    return panel;
            }

            return null;
        }
        
        public PanelEntity GetPanel<T>() where T : PanelBase
        {
            foreach (var group in _uiGroups.Values)
            {
                var panel = group.GetPanel<T>();
                if (panel != null)
                    return panel;
            }

            return null;
        }

        /// <summary>
        /// 获得所有已加载界面
        /// </summary>
        /// <returns></returns>
        public List<PanelEntity> GetAllLoadedPanels()
        {
            List<PanelEntity> panelList = new List<PanelEntity>(_uiGroups.Count);
            foreach (var group in _uiGroups.Values)
                panelList.AddRange(group.GetAllPanels());

            return panelList;
        }

        /// <summary>
        /// 界面是否正在加载
        /// </summary>
        /// <param name="serialId"></param>
        /// <returns></returns>
        public bool IsPanelLoading(int serialId) => _loadingPanels.ContainsKey(serialId);

        /// <summary>
        /// 获得所有正在加载的界面id
        /// </summary>
        /// <returns></returns>
        public int[] GetAllLoadingPanelIds()
        {
            int num = 0;
            int[] loadingIds = new int[_loadingPanels.Count];
            foreach (var id in _loadingPanels.Keys)
                loadingIds[num++] = id;

            return loadingIds;
        }

        /// <summary>
        /// 显示界面
        /// </summary>
        /// <param name="callback">回调</param>
        /// <param name="uiGroupName">UI所在组名</param>
        /// <param name="userData">用户数据</param>
        /// <typeparam name="T">面板类名必须和资源名一致</typeparam>
        /// <returns>界面id</returns>
        public int ShowPanel<T>(string uiGroupName = GameConst.UIGROUP_PANEL,
            object userData = null, Action<PanelBase> callback = null) where T : PanelBase =>
            ShowPanel(typeof(T).Name, uiGroupName, userData, callback);

        /// <summary>
        /// 显示界面
        /// </summary>
        /// <param name="assetName">UI资源名</param>
        /// <param name="callback">回调</param>
        /// <param name="uiGroupName">UI所在组名</param>
        /// <param name="userData">用户数据</param>
        /// <returns>界面id</returns>
        /// <exception cref="Exception"></exception>
        public int ShowPanel(string assetName, string uiGroupName = GameConst.UIGROUP_PANEL, object userData = null,
            Action<PanelBase> callback = null)
        {
            if (string.IsNullOrEmpty(assetName))
                throw new Exception("Show panel failed. Asset name is invalid.");

            UIGroup group = GetUIGroup(uiGroupName);
            if (group == null)
                throw new Exception($"Show panel failed. dont contains {uiGroupName} UIGroup.");

            int id = ++_cachedSerialId;

            bool isNew = false;
            //如果需要加载，就加入加载列表
            if (HEntry.ObjectPoolMgr.GetPoolCacheNum(PANEL_PATH + assetName) <= 0)
            {
                _loadingPanels.Add(id, assetName);
                isNew = true;
            }

            HEntry.ObjectPoolMgr.PopObj(PANEL_PATH + assetName,
                obj =>
                {
                    //如果在加载途中被Shutdown，加载完直接回收
                    if (_loadingPanelsToRelease.Contains(id))
                    {
                        HEntry.ObjectPoolMgr.PushObj(PANEL_PATH + assetName, obj);
                        _loadingPanelsToRelease.Remove(id);
                        return;
                    }

                    InternalShowPanel(id, assetName, group, obj, isNew, userData, callback);
                });
            return id;
        }

        /// <summary>
        /// 隐藏界面
        /// </summary>
        /// <param name="serialId"></param>
        /// <param name="userData"></param>
        /// <param name="isShutdown"></param>
        public void HidePanel(int serialId, object userData = null, bool isShutdown = false)
        {
            //如果正在加载，加载完再隐藏
            if (IsPanelLoading(serialId))
            {
                _loadingPanelsToRelease.Add(serialId);
                _loadingPanels.Remove(serialId);
                return;
            }
            
            HidePanel(GetPanel(serialId), userData, isShutdown);
        }

        /// <summary>
        /// 隐藏界面
        /// </summary>
        /// <param name="panel"></param>
        /// <param name="userData"></param>
        /// <param name="isShutdown"></param>
        public void HidePanel(PanelBase panel, object userData = null, bool isShutdown = false)
            => HidePanel(panel.PanelEntity, userData, isShutdown);

        /// <summary>
        /// 隐藏界面
        /// </summary>
        /// <param name="panel"></param>
        /// <param name="userData"></param>
        /// <param name="isShutdown"></param>
        /// <exception cref="Exception"></exception>
        public bool HidePanel(PanelEntity panel, object userData = null, bool isShutdown = false)
        {
            if (panel == null)
                return false;
            UIGroup group = panel.UIGroup;
            if (group == null)
                throw new Exception("UI group is invalid.");

            if (_hidingPanels.Contains(panel.SerialId)) return false;
            
            _hidingPanels.Add(panel.SerialId);
            group.RemovePanel(panel);
            panel.OnHide(isShutdown, userData);
            
            if (isShutdown)
            {
                group.Refresh();
                _recycleQueue.Enqueue(panel);
                _hidingPanels.Remove(panel.SerialId);
            }
            else
                panel.AddHideCompleteListener(OnPanelHideComplete);

            return true;
        }

        /// <summary>
        /// 隐藏UIGroup中的所有面板
        /// </summary>
        /// <param name="groupName"></param>
        /// <param name="userData"></param>
        public void HideUIGroupPanels(string groupName, object userData = null)
        {
            var panels = GetUIGroup(groupName).GetAllPanels();
            foreach (var panel in panels)
                HidePanel(panel, userData);
        }
        
        /// <summary>
        /// 隐藏UIGroup中的所有面板
        /// </summary>
        /// <param name="group"></param>
        /// <param name="userData"></param>
        public void HideUIGroupPanels(UIGroup group, object userData = null)
        {
            var panels = group.GetAllPanels();
            foreach (var panel in panels)
                HidePanel(panel, userData);
        }

        /// <summary>
        /// 隐藏当前UIGroup上方的所有面板
        /// </summary>
        /// <param name="curGroupName">当前UIGroup名</param>
        /// <param name="userData"></param>
        public void HideUpperUIGroupPanels(string curGroupName, object userData = null)
        {
            int curDepth = GetUIGroup(curGroupName).Depth;
            var groups = GetAllUIGroups();
            foreach (var group in groups)
            {
                if (group.Depth > curDepth)
                    HideUIGroupPanels(group, userData);
            }
        }
        
        public void HideUpperUIGroupPanels(UIGroup curGroup, object userData = null)
        {
            int curDepth = curGroup.Depth;
            var groups = GetAllUIGroups();
            foreach (var group in groups)
            {
                if (group.Depth > curDepth)
                    HideUIGroupPanels(group, userData);
            }
        }

        /// <summary>
        /// 隐藏所有已加载界面
        /// </summary>
        /// <param name="userData"></param>
        public void HideAllLoadedPanels(object userData = null)
        {
            foreach (var panel in GetAllLoadedPanels())
                HidePanel(panel, userData);
        }

        /// <summary>
        /// 隐藏所有正在加载界面
        /// </summary>
        public void HideAllLoadingPanels()
        {
            foreach (var id in _loadingPanels.Keys)
                _loadingPanelsToRelease.Add(id);
            _loadingPanels.Clear();
        }

        /// <summary>
        /// 激活界面
        /// </summary>
        /// <param name="serialId"></param>
        /// <param name="userData"></param>
        public void RefocusPanel(int serialId, object userData = null)
            => RefocusPanel(GetPanel(serialId), userData);
        
        /// <summary>
        /// 激活界面
        /// </summary>
        /// <param name="panel"></param>
        /// <param name="userData"></param>
        public void RefocusPanel(PanelBase panel, object userData = null)
            => RefocusPanel(panel.PanelEntity, userData);

        /// <summary>
        /// 激活界面
        /// </summary>
        /// <param name="panel"></param>
        /// <param name="userData"></param>
        /// <exception cref="Exception"></exception>
        public void RefocusPanel(PanelEntity panel, object userData = null)
        {
            UIGroup group = panel != null ? panel.UIGroup : null;
            if (group == null)
                throw new Exception("UI group is invalid.");
            
            group.RefocusPanel(panel);
            group.Refresh();
            panel.OnRefocus(userData);
        }
        
        
        private void OnPanelHideComplete(PanelEntity panelEntity)
        {
            panelEntity.UIGroup.Refresh();
            _recycleQueue.Enqueue(panelEntity);
            panelEntity.RemoveHideCompleteListener(OnPanelHideComplete);
            _hidingPanels.Remove(panelEntity.SerialId);
        }

        private void InternalShowPanel(int serialId, string assetName, UIGroup uiGroup, GameObject obj, bool isNew,
            object userData, Action<PanelBase> callback)
        {
            //移除加载列表
            if (isNew)
                _loadingPanels.Remove(serialId);
            
            obj.name = assetName;
            obj.transform.SetParent(uiGroup.UIGroupEntity.transform, false);
            if (!obj.TryGetComponent(out PanelEntity panel))
            {
                Debug.LogError("panel has no panel entity.");
                return;
            }

            panel.OnInit(serialId, assetName, uiGroup, isNew, userData);
            uiGroup.AddPanel(panel);
            panel.OnShow(userData);
            uiGroup.Refresh();
            callback?.Invoke(panel.Logic);
        }

        #endregion


        #region Custom Event
        
        public static void AddCustomEventListener(UIBehaviour control, EventTriggerType type,
            UnityAction<BaseEventData> callback)
        {
            if (control == null)
                throw new Exception($"Add custom listener control is null");
            if (!control.TryGetComponent(out EventTrigger trigger))
                trigger = control.gameObject.AddComponent<EventTrigger>();
            
            var entry = trigger.triggers.Find(e => e.eventID == type);
            if (entry != null)
            {
                entry.callback.RemoveListener(callback);
                entry.callback.AddListener(callback);
                return;
            }

            entry = new EventTrigger.Entry { eventID = type };
            entry.callback.AddListener(callback);
            trigger.triggers.Add(entry);
        }

        public static void AddCustomEventListener(UIBehaviour[] controls, EventTriggerType type,
            UnityAction<BaseEventData> callback)
        {
            for (int i = 0; i < controls.Length; i++)
                AddCustomEventListener(controls[i], type, callback);
        }
        
        public static void AddCustomEventListener<T>(List<T> controls, EventTriggerType type,
            UnityAction<BaseEventData> callback) where T : UIBehaviour
        {
            for (int i = 0; i < controls.Count; i++)
                AddCustomEventListener(controls[i], type, callback);
        }

        public static bool RemoveCustomEventListener(UIBehaviour control, EventTriggerType type,
            UnityAction<BaseEventData> callback)
        {
            if (control == null)
                throw new Exception($"Remove custom listener control is null");
            if (control.TryGetComponent(out EventTrigger trigger))
            {
                var entry = trigger.triggers.Find(e => e.eventID == type);
                if (entry != null)
                {
                    entry.callback.RemoveListener(callback);
                    return true;
                }
            }

            return false;
        }

        public static void RemoveCustomEventListener(UIBehaviour[] controls, EventTriggerType type,
            UnityAction<BaseEventData> callback)
        {
            for (int i = 0; i < controls.Length; i++)
                RemoveCustomEventListener(controls[i], type, callback);
        }
        
        public static void RemoveCustomEventListener<T>(List<T> controls, EventTriggerType type,
            UnityAction<BaseEventData> callback) where T : UIBehaviour
        {
            for (int i = 0; i < controls.Count; i++)
                RemoveCustomEventListener(controls[i], type, callback);
        }

        public static bool RemoveCustomEvent(UIBehaviour control, EventTriggerType type)
        {
            if (control == null)
                throw new Exception($"Remove custom listener control is null");
            if (control.TryGetComponent(out EventTrigger trigger))
            {
                var entry = trigger.triggers.Find(e => e.eventID == type);
                return trigger.triggers.Remove(entry);
            }

            return false;
        }
        
        public static void RemoveCustomEvent(UIBehaviour[] controls, EventTriggerType type)
        {
            for (int i = 0; i < controls.Length; i++)
                RemoveCustomEvent(controls[i], type);
        }

        public static void RemoveCustomEvent<T>(List<T> controls, EventTriggerType type) where T : UIBehaviour
        {
            for (int i = 0; i < controls.Count; i++)
                RemoveCustomEvent(controls[i], type);
        }

        public static bool RemoveAllCustomEvents(UIBehaviour control)
        {
            if (control == null)
                throw new Exception($"Remove custom listener control is null");
            if (control.TryGetComponent(out EventTrigger trigger))
            {
                trigger.triggers.Clear();
                return true;
            }

            return false;
        }
        
        public static void RemoveAllCustomEvents(UIBehaviour[] controls)
        {
            for (int i = 0; i < controls.Length; i++)
                RemoveAllCustomEvents(controls[i]);
        }
        
        public static void RemoveAllCustomEvents<T>(List<T> controls) where T : UIBehaviour
        {
            for (int i = 0; i < controls.Count; i++)
                RemoveAllCustomEvents(controls[i]);
        }

        #endregion

        #region UI Model

        /// <summary>
        /// 在uiStage显示模型，只能显示一个模型
        /// </summary>
        /// <param name="path"></param>
        /// <param name="callback"></param>
        public void ShowModel(string path, UnityAction<GameObject> callback) => _uiStage.ShowObj(path, callback);

        public void BindStageRT(RawImage raw)
        {
            var rect = raw.rectTransform.rect;
            raw.texture = _uiStage.BindRT((int)rect.width, (int)rect.height, 1);
        }

        public void SetStageSize(float size)
        {
            _uiStage.SetCameraSize(size);
        }

        public void RecycleModel(string path) => _uiStage.RecycleObj(path);

        public void RecycleAllModel() => _uiStage.RecycleAll();

        public void ClearModel() => _uiStage.Clear();

        public RenderTexture StageRT => _uiStage.RT;

        public Camera StageCamera => _uiStage.StageCamera;

        #endregion
    }
}