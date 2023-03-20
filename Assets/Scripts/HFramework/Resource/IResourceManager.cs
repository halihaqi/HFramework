using UnityEngine;
using UnityEngine.Events;

namespace HFramework
{
    public interface IResourceManager
    {
        /// <summary>
        /// 加载资源(同步，不缓存)
        /// </summary>
        /// <param name="path"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        T Load<T>(string path) where T : Object;

        /// <summary>
        /// 加载资源(同步，缓存)
        /// </summary>
        /// <param name="part"></param>
        /// <param name="path"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        T Load<T>(string part, string path) where T : Object;

        /// <summary>
        /// 加载资源(异步)
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="path">资源路径</param>
        /// <param name="callback">回调函数</param>
        void LoadAsync<T>(string path, UnityAction<T> callback = null) where T : Object;

        /// <summary>
        /// 加载资源(异步，缓存)
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="part"></param>
        /// <param name="path">资源路径</param>
        /// <param name="callback">回调函数</param>
        void LoadAsync<T>(string part, string path, UnityAction<T> callback = null) where T : Object;

        /// <summary>
        /// 清除部分资源缓存
        /// </summary>
        /// <param name="part">部分名</param>
        void ClearPartRes(string part);

        /// <summary>
        /// 清空所有资源缓存
        /// </summary>
        void ClearAllRes();
    }
}