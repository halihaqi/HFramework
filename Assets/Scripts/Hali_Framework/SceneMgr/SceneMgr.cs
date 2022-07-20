using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

public class SceneMgr : Singleton<SceneMgr>
{
    /// <summary>
    /// 加载场景(同步)
    /// </summary>
    /// <param name="name">场景名</param>
    public void LoadScene(string name)
    {
        //切换下一个场景前，清空对象池和资源字典，释放空间
        //对象池如果不清空会导致字典中有对象但场景中被销毁
        ResMgr.Instance.ClearResDic();
        PoolMgr.Instance.Clear();
        SceneManager.LoadScene(name);
        EventCenter.Instance.Clear();
    }

    /// <summary>
    /// 加载场景(异步)
    /// 加载过程中会分发事件"Loading"
    /// 加载结束会分发无参事件"LoadComplete"
    /// </summary>
    /// <param name="name">场景名</param>
    /// <param name="action">完成回调函数</param>
    public void LoadSceneAsync(string name, UnityAction callback)
    {
        ResMgr.Instance.ClearResDic();
        PoolMgr.Instance.Clear();
        MonoMgr.Instance.StartCoroutine(AsyncLoad(name, callback));
    }

    /// <summary>
    /// 加载场景(显示LoadingUI界面)
    /// 加载过程中会分发有参事件"Loading",参数为加载进度
    /// 加载结束会分发无参事件"LoadComplete"，会自动隐藏加载面板
    /// </summary>
    /// <param name="name">场景名</param>
    /// <param name="panelName">加载界面Panel预设体名</param>
    /// <param name="callback">加载完成的回调</param>
    public void LoadSceneAsync<T>(string name, string panelName, UnityAction callback) where T : BasePanel
    {
        //先显示LoadingUI
        UIMgr.Instance.ShowPanel<T>(panelName, E_UI_Layer.System, (panel) =>
        {
            //显示完后开始切换场景
            ResMgr.Instance.ClearResDic();
            PoolMgr.Instance.Clear();
            MonoMgr.Instance.StartCoroutine(AsyncLoad<T>(name, panelName, callback));
        });
    }

    //异步加载场景协程
    IEnumerator AsyncLoad(string name,UnityAction action)
    {
        AsyncOperation ao = SceneManager.LoadSceneAsync(name);
        while (!ao.isDone)
        {
            yield return ao.progress;
            EventCenter.Instance.PostEvent("Loading");
        }
        //等一帧，为界面更新提供时机
        yield return null;
        EventCenter.Instance.PostEvent("LoadComplete");
        action?.Invoke();
    }

    //异步加载场景协程(有UI遮挡)
    IEnumerator AsyncLoad<T>(string name, string panelName, UnityAction callback) where T : BasePanel
    {
        //申明toProgress表示假的加载进度
        //因为ao.progress在加载小场景时变化太快，效果不好
        int toProgress = 0;
        AsyncOperation ao = SceneManager.LoadSceneAsync(name);
        ao.allowSceneActivation = false;

        //在allowSceneActivation = true之前progress只会加载到0.9
        //需要手动设置
        while (ao.progress < 0.9f)
        {
            //这里表示的假的进度变化，如果小于ao.progres，每帧加1
            while (toProgress < (int)(ao.progress * 100))
            {
                toProgress++;
                EventCenter.Instance.PostEvent("Loading", toProgress);
                yield return toProgress;
            }
        }
        //加载到0.9后手动设置为true
        //会继续加载0.9~1的资源并开启场景
        //此时还不应该显示场景
        ao.allowSceneActivation = true;
        //isDone只有在allowSceneActivation为true并且加载完后才为true
        //可以通过这个判断是否加载完成
        while (!ao.isDone)
        {
            toProgress = 95;
            EventCenter.Instance.PostEvent("Loading", toProgress);
            yield return toProgress;
        }
        toProgress = 100;
        EventCenter.Instance.PostEvent("Loading", toProgress);
        //等一帧，为界面更新提供时机
        yield return null;
        callback?.Invoke();
        UIMgr.Instance.HidePanel(panelName);
        EventCenter.Instance.PostEvent("LoadComplete");
    }
}
