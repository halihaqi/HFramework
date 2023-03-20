using System;
using System.Collections;
using UnityEngine;

namespace HFramework
{
    internal class MonoManager : HModule, IMonoManager
    {
        private GlobalMono _globalMono;

        internal override int Priority => -100;//最先初始化，最后轮询

        internal override void Init()
        {
            GameObject obj = new GameObject("GlobalMono");
            _globalMono = obj.AddComponent<GlobalMono>();
        }

        internal override void Update(float elapseSeconds, float realElapseSeconds)
        {
        }

        internal override void Shutdown()
        {
            _globalMono.Clear();
            _globalMono.Dispose();
            _globalMono = null;
        }

        #region Update相关公共方法
        /// <summary>
        /// 添加Update事件监听
        /// </summary>
        /// <param name="action">事件</param>
        public void AddUpdateListener(Action action)
        {
            _globalMono.AddUpdateListener(action);
        }
        
        /// <summary>
        /// 添加Update事件监听
        /// </summary>
        /// <param name="action">参数为逻辑时间流逝，真实时间流逝</param>
        public void AddUpdateListener(Action<float, float> action)
        {
            _globalMono.AddUpdateListener(action);
        }

        /// <summary>
        /// 移除Update事件监听
        /// </summary>
        /// <param name="action">事件</param>
        public void RemoveUpdateListener(Action action)
        {
            _globalMono.RemoveUpdateListener(action);
        }
        
        /// <summary>
        /// 移除Update事件监听
        /// </summary>
        /// <param name="action">参数为逻辑时间流逝，真实时间流逝</param>
        public void RemoveUpdateListener(Action<float, float> action)
        {
            _globalMono.RemoveUpdateListener(action);
        }

        /// <summary>
        /// 清空Update事件监听
        /// </summary>
        internal void Clear()
        {
            _globalMono.Clear();
        }
        #endregion

        #region Coroutine相关公共方法
        //开启协程
        public Coroutine StartCoroutine(IEnumerator routine)
        {
            return _globalMono.StartCoroutine(routine);
        }

        public Coroutine StartCoroutine(string methodName)
        {
            return _globalMono.StartCoroutine(methodName);
        }

        //停止协程
        public void StopCoroutine(IEnumerator routine)
        {
            _globalMono.StopCoroutine(routine);
        }

        public void StopCoroutine(string methodName)
        {
            _globalMono.StopCoroutine(methodName);
        }

        //停止所有协程
        public void StopAllCoroutines()
        {
            _globalMono.StopAllCoroutines();
        }
        #endregion
        
    }
}
