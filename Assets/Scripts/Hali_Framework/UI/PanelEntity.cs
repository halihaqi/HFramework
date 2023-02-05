using System;
using UnityEngine;

namespace Hali_Framework
{
    /// <summary>
    /// 挂载继承PanelBase的类后自动添加，
    /// 将UI业务层和架构层分开
    /// </summary>
    public sealed class PanelEntity : MonoBehaviour
    {
        private int _serialId;
        private int _depth;
        private string _assetName;
        private UIGroup _uiGroup;
        private PanelBase _panelLogic;

        #region 属性

        /// <summary>
        /// 界面序列号
        /// </summary>
        public int SerialId => _serialId;

        /// <summary>
        /// 界面资源名
        /// </summary>
        public string AssetName => _assetName;
        
        /// <summary>
        /// 界面在组内深度
        /// </summary>
        public int Depth => _depth;

        /// <summary>
        /// 界面所属界面组
        /// </summary>
        public UIGroup UIGroup => _uiGroup;

        /// <summary>
        /// 界面逻辑
        /// </summary>
        public PanelBase Logic => _panelLogic;

        #endregion


        #region 生命周期

        public void OnInit(int serialId, string assetName, UIGroup uiGroup,
            bool isNewInstance, object userdata)
        {
            _serialId = serialId;
            _assetName = assetName;
            _uiGroup = uiGroup;
            _depth = 0;
            
            if(!isNewInstance) return;

            _panelLogic = GetComponent<PanelBase>();
            if (_panelLogic == null)
                throw new Exception($"Panel:{assetName} can not get panel logic.");

            _panelLogic.OnInit(userdata);
        }

        public void OnShow(object userData)
        {
            _panelLogic.OnShow(userData);
        }
        
        public void OnUpdate(float elapseSeconds, float realElapseSeconds)
        {
            _panelLogic.OnUpdate(elapseSeconds, realElapseSeconds);
        }

        public void OnHide(object userData)
        {
            _panelLogic.OnHide(userData);
        }

        public void OnRecycle()
        {
            _panelLogic.OnRecycle();

            _serialId = 0;
            _depth = 0;
        }

        public void OnPause()
        {
            _panelLogic.OnPause();
        }

        public void OnResume()
        {
            _panelLogic.OnResume();
        }

        public void OnCover()
        {
            _panelLogic.OnCover();
        }

        public void OnReveal()
        {
            _panelLogic.OnReveal();
        }

        public void OnRefocus(object userData)
        {
            _panelLogic.OnRefocus(userData);
        }

        public void OnDepthChanged(int uiGroupDepth, int depthInUIGroup)
        {
            _depth = depthInUIGroup;
            _panelLogic.OnDepthChanged(uiGroupDepth, depthInUIGroup);
        }

        #endregion
    }
}