using System;
using UnityEngine.UI;

namespace Hali_Framework
{
    public class LoadingPanel : BasePanel
    {
        public Slider sliderProgress;
        private Action<int> progressEvent;

        public override void ShowMe(bool isFade = true)
        {
            base.ShowMe(isFade);
            //添加进度条变化事件监听
            progressEvent = (val) => { sliderProgress.value = (float)val / 100; };
            EventCenter.Instance.AddListener<int>(ClientEvent.LOADING, progressEvent);
        }

        public override void HideMe(bool isFade = true)
        {
            base.HideMe(isFade);
            EventCenter.Instance.RemoveListener<int>(ClientEvent.LOADING, progressEvent);
        }
    }
}
