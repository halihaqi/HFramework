using System;
using System.Collections.Generic;
using UnityEngine;

namespace Hali_Framework
{
    /// <summary>
    /// 对象池管理器
    /// </summary>
    public class PoolMgr : Singleton<PoolMgr>
    {
        //缓存池容器
        private readonly Dictionary<string, PoolData> _goPoolDic = new Dictionary<string, PoolData>();
        private GameObject _poolObj;

        #region 对象池公共方法
        /// <summary>
        /// 从缓存池取出
        /// </summary>
        /// <param name="path">物体路径</param>
        /// <param name="callback">取完回调</param>
        public void PopObj(string path, Action<GameObject> callback)
        {
            //如果有这个池并且池里有未使用的物体
            if (_goPoolDic.ContainsKey(path) && _goPoolDic[path].poolList.Count > 0)
            {
                GameObject obj = _goPoolDic[path].Pop();
                callback?.Invoke(obj);
            }
            else
            {
                ResMgr.Instance.LoadAsync<GameObject>(path, (obj) =>
                {
                    obj.name = path;
                    callback?.Invoke(obj);
                });
            }
        }


        /// <summary>
        /// 压入缓存池
        /// </summary>
        /// <param name="path">物体路径</param>
        /// <param name="obj">物体实例</param>
        public void PushObj(string path, GameObject obj)
        {
            //如果有这个缓存池
            if (_goPoolDic.ContainsKey(path))
            {
                _goPoolDic[path].Push(obj);
            }
            else
            {
                //如果没有就新建一个缓存池
                if (_poolObj == null)
                    _poolObj = new GameObject("Pool");
                _goPoolDic.Add(path, new PoolData(obj, _poolObj));
            }
        }


        /// <summary>
        /// 清空对象池
        /// </summary>
        public void Clear()
        {
            _goPoolDic.Clear();
            _poolObj = null;
        }
        #endregion




        #region 对象池数据结构类(内部类)
        private class PoolData
        {
            private GameObject _parentObj;    //父对象池
            public readonly Queue<GameObject> poolList;  //对象池对象列表

            /// <summary>
            /// 对象池构造函数
            /// </summary>
            /// <param name="obj">第一个对象</param>
            /// <param name="poolObj">对象池</param>
            public PoolData(GameObject obj, GameObject poolObj)
            {
                this._parentObj = new GameObject(obj.name);
                _parentObj.transform.parent = poolObj.transform;
                poolList = new Queue<GameObject>();
                Push(obj);
            }

            /// <summary>
            /// 从对象池取出对象
            /// </summary>
            /// <returns>拿出的对象</returns>
            public GameObject Pop()
            {
                GameObject obj = poolList.Dequeue();
                obj.SetActive(true);
                obj.transform.SetParent(null, false);
                return obj;
            }

            /// <summary>
            /// 向对象池压入对象
            /// </summary>
            /// <param name="obj">要压入的对象</param>
            public void Push(GameObject obj)
            {
                obj.SetActive(false);
                obj.transform.SetParent(_parentObj.transform, false);
                poolList.Enqueue(obj);
            }

            /// <summary>
            /// 清空对象池
            /// </summary>
            public void Clear()
            {
                _parentObj = null;
                poolList.Clear();
            }
        }
        #endregion

    }
}
