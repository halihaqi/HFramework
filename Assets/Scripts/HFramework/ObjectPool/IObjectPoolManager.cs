using System;
using UnityEngine;

namespace HFramework
{
    public interface IObjectPoolManager
    {
        /// <summary>
        /// 从对象池取出
        /// </summary>
        /// <param name="path">物体路径</param>
        /// <param name="callback"></param>
        void PopObj(string path, Action<GameObject> callback);

        /// <summary>
        /// 压入对象池
        /// </summary>
        /// <param name="path">物体路径</param>
        /// <param name="obj">物体实例</param>
        void PushObj(string path, GameObject obj);

        /// <summary>
        /// 获得对象池缓存的对象数
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        int GetPoolCacheNum(string path);

        /// <summary>
        /// 清空单个对象池
        /// </summary>
        /// <param name="path"></param>
        void ClearPool(string path);

        /// <summary>
        /// 清空所有对象池
        /// </summary>
        void Clear();
    }
}