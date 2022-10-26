using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class LoadingPanel : BasePanel
{
    public Slider sliderProgress;
    private UnityAction<int> progressEvent;

    public override void ShowMe(bool isFade = true)
    {
        base.ShowMe(isFade);
        //添加进度条变化事件监听
        progressEvent = (val) => { sliderProgress.value = (float)val / 100; };
        EventCenter.Instance.AddListener<int>("Loading", progressEvent);
    }

    public override void HideMe(bool isFade = true)
    {
        base.HideMe(isFade);
        EventCenter.Instance.RemoveListener<int>("Loading", progressEvent);
    }
}
