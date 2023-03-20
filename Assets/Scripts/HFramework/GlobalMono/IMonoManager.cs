using System;
using System.Collections;
using UnityEngine;

namespace HFramework
{
    public interface IMonoManager
    {
        /// <summary>
        /// 添加Update事件监听
        /// </summary>
        /// <param name="action">事件</param>
        void AddUpdateListener(Action action);

        /// <summary>
        /// 添加Update事件监听
        /// </summary>
        /// <param name="action">参数为逻辑时间流逝，真实时间流逝</param>
        void AddUpdateListener(Action<float, float> action);

        /// <summary>
        /// 移除Update事件监听
        /// </summary>
        /// <param name="action">事件</param>
        void RemoveUpdateListener(Action action);

        /// <summary>
        /// 移除Update事件监听
        /// </summary>
        /// <param name="action">参数为逻辑时间流逝，真实时间流逝</param>
        void RemoveUpdateListener(Action<float, float> action);

        /// <summary>
        /// 开启协程
        /// </summary>
        /// <param name="routine"></param>
        /// <returns></returns>
        Coroutine StartCoroutine(IEnumerator routine);

        /// <summary>
        /// 开启协程
        /// </summary>
        /// <param name="methodName"></param>
        /// <returns></returns>
        Coroutine StartCoroutine(string methodName);

        /// <summary>
        /// 停止携程
        /// </summary>
        /// <param name="routine"></param>
        void StopCoroutine(IEnumerator routine);

        /// <summary>
        /// 停止携程
        /// </summary>
        /// <param name="methodName"></param>
        void StopCoroutine(string methodName);

        /// <summary>
        /// 停止所有协程
        /// </summary>
        void StopAllCoroutines();
    }
}