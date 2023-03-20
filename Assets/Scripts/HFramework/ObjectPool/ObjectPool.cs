﻿using System;
using System.Collections.Generic;
using UnityEngine;
using Object = UnityEngine.Object;

namespace HFramework
{
    internal class ObjectPool
    {
            private GameObject _parentObj;
            private readonly Queue<GameObject> _poolList;

            /// <summary>
            /// 池中对象缓存数
            /// </summary>
            public int CacheNum => _poolList?.Count ?? -1;
            
            /// <summary>
            /// 对象池名
            /// </summary>
            public string PoolName { get; private set; }

            /// <summary>
            /// 对象池构造函数
            /// </summary>
            /// <param name="poolName">对象池名</param>
            /// <param name="poolObj">对象池实例</param>
            /// <param name="firstObj">第一个对象</param>
            public ObjectPool(string poolName, GameObject poolObj, GameObject firstObj = null)
            {
                this._parentObj = new GameObject(poolName);
                PoolName = poolName;
                _parentObj.transform.parent = poolObj.transform;
                _poolList = new Queue<GameObject>();
                if(firstObj != null)
                    Push(firstObj);
            }

            public GameObject Pop()
            {
                GameObject obj = _poolList.Dequeue();
                obj.SetActive(true);
                obj.transform.SetParent(null, false);
                return obj;
            }
            
            public void Push(GameObject obj)
            {
                if (_poolList.Contains(obj))
                    throw new Exception($"Can not push {obj} twice in pool:{PoolName}.");
                obj.SetActive(false);
                obj.transform.SetParent(_parentObj.transform, false);
                _poolList.Enqueue(obj);
            }
            
            public void Clear()
            {
                Object.Destroy(_parentObj);
                _parentObj = null;
                _poolList.Clear();
            }
    }
}