using System;
using System.Collections.Generic;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Hali_Framework
{
    public class ObjectPoolMgr : Singleton<ObjectPoolMgr>
    {
        private readonly Dictionary<string, ObjectPool> _objectPoolCollection;
        private GameObject _poolCollectionObj;
        
        public ObjectPoolMgr()
            => _objectPoolCollection = new Dictionary<string, ObjectPool>();
        
        /// <summary>
        /// 从对象池取出
        /// </summary>
        /// <param name="path">物体路径</param>
        /// <param name="callback"></param>
        public void PopObj(string path, Action<GameObject> callback)
        {
            if (_objectPoolCollection.ContainsKey(path) && _objectPoolCollection[path].PoolNum > 0)
            {
                GameObject obj = _objectPoolCollection[path].Pop();
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
        /// 压入对象池
        /// </summary>
        /// <param name="path">物体路径</param>
        /// <param name="obj">物体实例</param>
        public void PushObj(string path, GameObject obj)
        {
            if (_objectPoolCollection.ContainsKey(path))
            {
                _objectPoolCollection[path].Push(obj);
            }
            else
            {
                if (_poolCollectionObj == null)
                    _poolCollectionObj = new GameObject("Pool");
                _objectPoolCollection.Add(path, new ObjectPool(obj, _poolCollectionObj));
            }
        }
        
        /// <summary>
        /// 清空对象池
        /// </summary>
        public void Clear()
        {
            _objectPoolCollection.Clear();
            Object.Destroy(_poolCollectionObj);
            _poolCollectionObj = null;
        }
    }
}