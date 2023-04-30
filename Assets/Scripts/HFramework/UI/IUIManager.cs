using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace HFramework
{
    public interface IUIManager
    {
        /// <summary>
        /// 界面组数量
        /// </summary>
        public int UIGroupCount { get; }

        #region UIGroup

        /// <summary>
        /// 是否存在界面组
        /// </summary>
        /// <param name="groupName">组名</param>
        /// <returns></returns>
        public bool HasUIGroup(UILayer groupName);

        /// <summary>
        /// 获得界面组
        /// </summary>
        /// <param name="groupName">组名</param>
        /// <returns></returns>
        public UIGroup GetUIGroup(UILayer groupName);

        /// <summary>
        /// 获得所有界面组
        /// </summary>
        /// <returns></returns>
        public UIGroup[] GetAllUIGroups();

        /// <summary>
        /// 添加界面组
        /// </summary>
        /// <param name="groupName">组名</param>
        /// <param name="groupDepth">深度</param>
        /// <returns></returns>
        public bool AddUIGroup(UILayer groupName, int groupDepth);

        #endregion

        #region Panel

        /// <summary>
        /// 是否拥有界面
        /// </summary>
        /// <param name="serialId"></param>
        /// <returns></returns>
        public bool HasPanel(int serialId);

        /// <summary>
        /// 是否拥有界面，可能有两个相同类型的界面
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public bool HasPanel<T>() where T : PanelBase;

        /// <summary>
        /// 获得界面
        /// </summary>
        /// <param name="serialId"></param>
        /// <returns></returns>
        public PanelEntity GetPanel(int serialId);

        /// <summary>
        /// 获得界面，可能有两个相同类型的界面
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public PanelEntity GetPanel<T>() where T : PanelBase;

        /// <summary>
        /// 获得所有已加载界面
        /// </summary>
        /// <returns></returns>
        public List<PanelEntity> GetAllLoadedPanels();

        /// <summary>
        /// 界面是否正在加载
        /// </summary>
        /// <param name="serialId"></param>
        /// <returns></returns>
        public bool IsPanelLoading(int serialId);

        /// <summary>
        /// 获得所有正在加载的界面id
        /// </summary>
        /// <returns></returns>
        public int[] GetAllLoadingPanelIds();
        
        /// <summary>
        /// 显示界面
        /// </summary>
        /// <param name="callback">回调</param>
        /// <param name="uiGroupName">UI所在组名</param>
        /// <param name="userData">用户数据</param>
        /// <typeparam name="T">面板类名必须和资源名一致</typeparam>
        /// <returns>界面id</returns>
        public int ShowPanel<T>(UILayer uiGroupName = UILayer.Panel,
            object userData = null, Action<PanelBase> callback = null) where T : PanelBase;

        /// <summary>
        /// 显示界面
        /// </summary>
        /// <param name="assetName">UI资源名</param>
        /// <param name="callback">回调</param>
        /// <param name="uiGroupName">UI所在组名</param>
        /// <param name="userData">用户数据</param>
        /// <returns>界面id</returns>
        /// <exception cref="Exception"></exception>
        public int ShowPanel(string assetName, UILayer uiGroupName = UILayer.Panel, object userData = null,
            Action<PanelBase> callback = null);

        /// <summary>
        /// 隐藏界面
        /// </summary>
        /// <param name="serialId"></param>
        /// <param name="userData"></param>
        /// <param name="isShutdown"></param>
        public void HidePanel(int serialId, object userData = null, bool isShutdown = false);

        /// <summary>
        /// 隐藏界面
        /// </summary>
        /// <param name="panel"></param>
        /// <param name="userData"></param>
        /// <param name="isShutdown"></param>
        public void HidePanel(PanelBase panel, object userData = null, bool isShutdown = false);

        /// <summary>
        /// 隐藏界面
        /// </summary>
        /// <param name="panel"></param>
        /// <param name="userData"></param>
        /// <param name="isShutdown"></param>
        /// <exception cref="Exception"></exception>
        public bool HidePanel(PanelEntity panel, object userData = null, bool isShutdown = false);

        /// <summary>
        /// 隐藏UIGroup中的所有面板
        /// </summary>
        /// <param name="groupName"></param>
        /// <param name="userData"></param>
        public void HideUIGroupPanels(UILayer groupName, object userData = null);

        /// <summary>
        /// 隐藏UIGroup中的所有面板
        /// </summary>
        /// <param name="group"></param>
        /// <param name="userData"></param>
        public void HideUIGroupPanels(UIGroup group, object userData = null);

        /// <summary>
        /// 隐藏当前UIGroup上方的所有面板
        /// </summary>
        /// <param name="curGroupName">当前UIGroup名</param>
        /// <param name="userData"></param>
        public void HideUpperUIGroupPanels(UILayer curGroupName, object userData = null);

        /// <summary>
        /// 隐藏当前UIGroup上方的所有面板
        /// </summary>
        /// <param name="curGroup"></param>
        /// <param name="userData"></param>
        public void HideUpperUIGroupPanels(UIGroup curGroup, object userData = null);

        /// <summary>
        /// 隐藏所有已加载界面
        /// </summary>
        /// <param name="userData"></param>
        public void HideAllLoadedPanels(object userData = null);

        /// <summary>
        /// 隐藏所有正在加载界面
        /// </summary>
        public void HideAllLoadingPanels();

        /// <summary>
        /// 激活界面
        /// </summary>
        /// <param name="serialId"></param>
        /// <param name="userData"></param>
        public void RefocusPanel(int serialId, object userData = null);

        /// <summary>
        /// 激活界面
        /// </summary>
        /// <param name="panel"></param>
        /// <param name="userData"></param>
        public void RefocusPanel(PanelBase panel, object userData = null);

        /// <summary>
        /// 激活界面
        /// </summary>
        /// <param name="panel"></param>
        /// <param name="userData"></param>
        /// <exception cref="Exception"></exception>
        public void RefocusPanel(PanelEntity panel, object userData = null);

        #endregion

        #region UI Model

        /// <summary>
        /// 在uiStage显示模型，只能显示一个模型
        /// </summary>
        /// <param name="path"></param>
        /// <param name="callback"></param>
        public void ShowModel(string path, UnityAction<GameObject> callback);

        public void BindStageRT(RawImage raw);

        public void SetStageSize(float size);

        public void RecycleModel(string path);

        public void RecycleAllModel();

        public void ClearModel();

        public RenderTexture StageRT { get; }
        
        public Camera StageCamera { get; }

        #endregion
    }
}