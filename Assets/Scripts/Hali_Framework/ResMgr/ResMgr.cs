using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace Hali_Framework
{
    public class ResMgr : Singleton<ResMgr>
    {
        //资源容器
        //优点：避免重复加载，提升加载效率
        //缺点：内存占用，需要在合适时机释放
        private Dictionary<string, object> resDic = new Dictionary<string, object>();

        /// <summary>
        /// 清空资源容器，释放内存
        /// </summary>
        public void ClearResDic()
        {
            resDic.Clear();
        }

        /// <summary>
        /// 加载资源(同步)
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="path">资源路径</param>
        /// <returns></returns>
        public T Load<T>(string path) where T : Object
        {
            T res = null;
            //如果字典中有，就不用加载了
            if (resDic.ContainsKey(path))
                res = resDic[path] as T;
            else
                res = Resources.Load<T>(path);
            //如果是GameObject，先实例化再返回
            if (res is GameObject)
                return GameObject.Instantiate(res);
            else
                return res;
        }

        /// <summary>
        /// 加载资源(异步)
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="path">资源路径</param>
        /// <param name="callback">回调函数</param>
        public void LoadAsync<T>(string path, UnityAction<T> callback = null) where T : Object
        {
            MonoMgr.Instance.StartCoroutine(AsyncLoad(path, callback));
        }

        IEnumerator AsyncLoad<T>(string path, UnityAction<T> callback) where T : Object
        {
            T res = null;
            //如果字典中有，就不用加载了
            if (resDic.ContainsKey(path))
                res = resDic[path] as T;
            else
            {
                ResourceRequest rr = Resources.LoadAsync<T>(path);
                res = rr.asset as T;
            }
            yield return null;
            //如果是GameObject，先生成预制体再执行回调
            if (res is GameObject)
                callback?.Invoke(GameObject.Instantiate(res));
            else
                callback?.Invoke(res);
        }
    }
}
