using System;
using UnityEngine.UI;

namespace Hali_Framework
{
    public class LoadingPanel : BasePanel
    {
        public Slider sliderProgress;
        private Action<int> progressEvent;

        public override void OnShow(bool isFade = true)
        {
            base.OnShow(isFade);
            //添加进度条变化事件监听
            progressEvent = (val) => { sliderProgress.value = (float)val / 100; };
            EventMgr.Instance.AddListener<int>(ClientEvent.LOADING, progressEvent);
        }

        public override void OnHide(bool isFade = true)
        {
            base.OnHide(isFade);
            EventMgr.Instance.RemoveListener<int>(ClientEvent.LOADING, progressEvent);
        }
    }
}
