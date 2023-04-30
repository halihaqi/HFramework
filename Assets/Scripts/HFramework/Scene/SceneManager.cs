using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

namespace HFramework
{
    public class SceneManager : HModule, ISceneManager
    {
        private bool _isManualControlLoad = false;
        private bool _isManualCompleteLoad = false;
        
        internal override int Priority => 5;
        
        internal override void Init()
        {
        }

        internal override void Update(float elapseSeconds, float realElapseSeconds)
        {
        }

        internal override void Shutdown()
        {
        }

        public bool IsCurScene(string sceneName) =>
            UnityEngine.SceneManagement.SceneManager.GetActiveScene().name == sceneName;

        /// <summary>
        /// 加载场景(同步)
        /// </summary>
        /// <param name="sceneName">场景名</param>
        public void LoadScene(string sceneName)
        {
            //切换下一个场景前，清空对象池和资源字典，释放空间
            HEntry.ResMgr.ClearAllRes();
            HEntry.ObjectPoolMgr.Clear();
            UnityEngine.SceneManagement.SceneManager.LoadScene(sceneName);
        }

        /// <summary>
        /// 加载场景(异步)
        /// 加载过程中会分发事件"Loading"
        /// 加载结束会分发无参事件"LoadComplete"
        /// </summary>
        /// <param name="sceneName">场景名</param>
        /// <param name="callback">完成回调函数</param>
        public void LoadSceneAsync(string sceneName, UnityAction callback)
        {
            HEntry.ResMgr.ClearAllRes();
            HEntry.ObjectPoolMgr.Clear();
            HEntry.MonoMgr.StartCoroutine(AsyncLoad(sceneName, callback));
        }

        /// <summary>
        /// 加载场景(显示LoadingUI界面)
        /// 加载过程中会分发有参事件"Loading",参数为加载进度
        /// 加载结束会分发无参事件"LoadComplete"，会自动隐藏加载面板
        /// </summary>
        /// <param name="sceneName">场景名</param>
        /// <param name="callback">加载完成的回调</param>
        /// <param name="isAutoHide">是否加载完成自动隐藏loading面板</param>
        public void LoadSceneWithPanel<T>(string sceneName, UnityAction callback, bool isAutoHide = true) where T : PanelBase
        {
            _isManualControlLoad = !isAutoHide;
            //先显示LoadingUI
            HEntry.UIMgr.ShowPanel<T>(UILayer.System, callback: panel =>
            {
                //显示完后开始切换场景
                HEntry.ResMgr.ClearAllRes();
                HEntry.ObjectPoolMgr.Clear();
                HEntry.MonoMgr.StartCoroutine(AsyncLoadWithPanel(sceneName, panel, callback, isAutoHide));
            });
        }
        
        public void ManualCompleteLoad()
        {
            if(!_isManualControlLoad) return;
            _isManualCompleteLoad = true;
        }

        //异步加载场景协程
        IEnumerator AsyncLoad(string name,UnityAction action)
        {
            AsyncOperation ao = UnityEngine.SceneManagement.SceneManager.LoadSceneAsync(name);
            while (!ao.isDone)
            {
                yield return ao.progress;
            }
            //等一帧，为界面更新提供时机
            yield return null;
            HEntry.EventMgr.TriggerEvent(ClientEvent.SCENE_LOAD_COMPLETE);
            action?.Invoke();
        }

        //异步加载场景协程(有UI遮挡)
        IEnumerator AsyncLoadWithPanel(string name, PanelBase panel, UnityAction callback, bool isAutoHide)
        {
            //申明toProgress表示假的加载进度
            //因为ao.progress在加载小场景时变化太快，效果不好
            int toProgress = 0;
            AsyncOperation ao = UnityEngine.SceneManagement.SceneManager.LoadSceneAsync(name);
            ao.allowSceneActivation = false;

            //在allowSceneActivation = true之前progress只会加载到0.9
            //需要手动设置
            while (ao.progress < 0.9f)
            {
                //这里表示的假的进度变化，如果小于ao.progress，每帧加1
                while (toProgress < (int)(ao.progress * 100))
                {
                    toProgress++;
                    HEntry.EventMgr.TriggerEvent(ClientEvent.SCENE_LOADING, toProgress);
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
                HEntry.EventMgr.TriggerEvent(ClientEvent.SCENE_LOADING, toProgress);
                yield return toProgress;
            }
            //等一帧，为界面更新提供时机
            yield return null;

            toProgress = 100;
            HEntry.EventMgr.TriggerEvent(ClientEvent.SCENE_LOADING, toProgress);
            callback?.Invoke();
            HEntry.EventMgr.TriggerEvent(ClientEvent.SCENE_LOAD_COMPLETE);
            
            if (!isAutoHide)
            {
                while (!_isManualCompleteLoad)
                    yield return null;
            }
            _isManualControlLoad = false;
            _isManualCompleteLoad = false;
            HEntry.UIMgr.HidePanel(panel);
        }
    }
}
