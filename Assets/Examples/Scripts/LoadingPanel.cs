using System;
using Example;
using UnityEngine.UI;

namespace Hali_Framework
{
    public class LoadingPanel : PanelBase
    {
        private Slider _progress;
        private Action<int> progressEvent;

        protected internal override void OnInit(object userData)
        {
            base.OnInit(userData);
            _progress = GetControl<Slider>("slider");
        }

        protected internal override void OnShow(object userData)
        {
            base.OnShow(userData);
            //添加进度条变化事件监听
            progressEvent = (val) => { _progress.value = (float)val / 100; };
            EventMgr.Instance.AddListener(ClientEvent.LOADING, progressEvent);
        }

        protected internal override void OnHide(bool isShutdown, object userData)
        {
            base.OnHide(isShutdown, userData);
            EventMgr.Instance.RemoveListener<int>(ClientEvent.LOADING, progressEvent);
        }
    }
}
