using UnityEngine.Events;

namespace HFramework
{
    public interface ISceneManager
    {
        /// <summary>
        /// 是否是当前场景
        /// </summary>
        /// <param name="sceneName">场景名</param>
        /// <returns></returns>
        bool IsCurScene(string sceneName);

        /// <summary>
        /// 加载场景(同步)
        /// </summary>
        /// <param name="sceneName">场景名</param>
        void LoadScene(string sceneName);

        /// <summary>
        /// 加载场景(异步)
        /// 加载过程中会分发事件"Loading"
        /// 加载结束会分发无参事件"LoadComplete"
        /// </summary>
        /// <param name="sceneName">场景名</param>
        /// <param name="callback">完成回调函数</param>
        void LoadSceneAsync(string sceneName, UnityAction callback);

        /// <summary>
        /// 加载场景(显示LoadingUI界面)
        /// 加载过程中会分发有参事件"Loading",参数为加载进度
        /// 加载结束会分发无参事件"LoadComplete"，会自动隐藏加载面板
        /// </summary>
        /// <param name="sceneName">场景名</param>
        /// <param name="callback">加载完成的回调</param>
        /// <param name="isAutoHide">是否加载完成自动隐藏loading面板</param>
        void LoadSceneWithPanel<T>(string sceneName, UnityAction callback, bool isAutoHide = true)
            where T : PanelBase;

        /// <summary>
        /// 手动完成加载
        /// </summary>
        void ManualCompleteLoad();
    }
}