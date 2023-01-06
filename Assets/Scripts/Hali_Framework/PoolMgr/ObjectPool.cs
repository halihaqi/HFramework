using System.Collections.Generic;
using UnityEngine;

namespace Hali_Framework
{
        public class ObjectPool
        {
            private GameObject _parentObj;
            private readonly Queue<GameObject> _poolList;
            
            public int PoolNum { get; private set; }

            /// <summary>
            /// 对象池构造函数
            /// </summary>
            /// <param name="obj">第一个对象</param>
            /// <param name="poolObj">对象池</param>
            public ObjectPool(GameObject obj, GameObject poolObj)
            {
                this._parentObj = new GameObject(obj.name);
                _parentObj.transform.parent = poolObj.transform;
                _poolList = new Queue<GameObject>();
                Push(obj);
            }
            
            public GameObject Pop()
            {
                GameObject obj = _poolList.Dequeue();
                obj.SetActive(true);
                obj.transform.SetParent(null, false);
                --PoolNum;
                return obj;
            }
            
            public void Push(GameObject obj)
            {
                obj.SetActive(false);
                obj.transform.SetParent(_parentObj.transform, false);
                _poolList.Enqueue(obj);
                ++PoolNum;
            }
            
            public void Clear()
            {
                _parentObj = null;
                _poolList.Clear();
                PoolNum = 0;
            }
        }
}