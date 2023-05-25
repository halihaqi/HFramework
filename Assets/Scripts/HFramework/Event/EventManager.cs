using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

namespace HFramework
{
    internal class EventManager : HModule, IEventManager
    {
        private class OnceEventInfo
        {
            public Dictionary<Delegate, Delegate> dels;

            public OnceEventInfo()
            {
                dels = new Dictionary<Delegate, Delegate>();
            }
        }
        //事件容器，不支持同种名称不同类型的事件监听
        private Dictionary<ClientEvent, Delegate> _eventDic;
        //用于单次监听除重
        private Dictionary<ClientEvent, OnceEventInfo> _onceEventDic;
        private static int ALL_EVENT_COUNT = 0;
        
        internal override int Priority => 7;

        internal override void Init()
        {
            _eventDic = new Dictionary<ClientEvent, Delegate>();
            _onceEventDic = new Dictionary<ClientEvent, OnceEventInfo>();
        }

        internal override void Update(float elapseSeconds, float realElapseSeconds)
        {
        }

        internal override void Shutdown()
        {
            _eventDic.Clear();
            _onceEventDic.Clear();
            ALL_EVENT_COUNT = 0;
        }
        

        #region 单次监听(触发一次自动移除)

        public void OnceListener(ClientEvent name, Action call)
        {
            //非法添加
            if(_eventDic.ContainsKey(name) && !(_eventDic[name] is Action))
                throw new Exception($"try AddListener funInfo:{call.GetMethodInfo().Name}, name:{name}");
            
            var oriCall = call;
            call += OnceRemoveFun;

            if (!_onceEventDic.TryGetValue(name, out var info))
            {
                info = new OnceEventInfo();
                _onceEventDic.Add(name, info);
            }
            else if(info.dels.ContainsKey(oriCall))//去重
                return;

            info.dels.Add(oriCall, call);
            AddListener(name, call);

            //内部方法
            void OnceRemoveFun()
            {
                if (_onceEventDic.ContainsKey(name) && 
                    _onceEventDic[name].dels.ContainsKey(oriCall))
                {
                    RemoveListener(name, oriCall);
                }
            }
        }
    
        public void OnceListener<T>(ClientEvent name, Action<T> call)
        {
            //非法添加
            if(_eventDic.ContainsKey(name) && !(_eventDic[name] is Action<T>))
                throw new Exception($"try AddListener funInfo:{call.GetMethodInfo().Name}, name:{name}");
            
            var oriCall = call;
            call += OnceRemoveFun;

            if (!_onceEventDic.TryGetValue(name, out var info))
            {
                info = new OnceEventInfo();
                _onceEventDic.Add(name, info);
            }
            else if(info.dels.ContainsKey(oriCall))//去重
                return;

            info.dels.Add(oriCall, call);
            AddListener(name, call);

            //内部方法
            void OnceRemoveFun(T t)
            {
                if (_onceEventDic.ContainsKey(name) && 
                    _onceEventDic[name].dels.ContainsKey(oriCall))
                {
                    RemoveListener(name, oriCall);
                }
            }
        }
    
        public void OnceListener<T, U>(ClientEvent name, Action<T, U> call)
        {
            //非法添加
            if(_eventDic.ContainsKey(name) && !(_eventDic[name] is Action<T, U>))
                throw new Exception($"try AddListener funInfo:{call.GetMethodInfo().Name}, name:{name}");
            
            var oriCall = call;
            call += OnceRemoveFun;

            if (!_onceEventDic.TryGetValue(name, out var info))
            {
                info = new OnceEventInfo();
                _onceEventDic.Add(name, info);
            }
            else if(info.dels.ContainsKey(oriCall))//去重
                return;

            info.dels.Add(oriCall, call);
            AddListener(name, call);

            //内部方法
            void OnceRemoveFun(T t, U u)
            {
                if (_onceEventDic.ContainsKey(name) && 
                    _onceEventDic[name].dels.ContainsKey(oriCall))
                {
                    RemoveListener(name, oriCall);
                }
            }
        }
        
        public void OnceListener<T, U, V>(ClientEvent name, Action<T, U, V> call)
        {
            //非法添加
            if(_eventDic.ContainsKey(name) && !(_eventDic[name] is Action<T, U, V>))
                throw new Exception($"try AddListener funInfo:{call.GetMethodInfo().Name}, name:{name}");
            
            var oriCall = call;
            call += OnceRemoveFun;

            if (!_onceEventDic.TryGetValue(name, out var info))
            {
                info = new OnceEventInfo();
                _onceEventDic.Add(name, info);
            }
            else if(info.dels.ContainsKey(oriCall))//去重
                return;

            info.dels.Add(oriCall, call);
            AddListener(name, call);

            //内部方法
            void OnceRemoveFun(T t, U u, V v)
            {
                if (_onceEventDic.ContainsKey(name) && 
                    _onceEventDic[name].dels.ContainsKey(oriCall))
                {
                    RemoveListener(name, oriCall);
                }
            }
        }
        
        public void OnceListener<T, U, V, W>(ClientEvent name, Action<T, U, V, W> call)
        {
            //非法添加
            if(_eventDic.ContainsKey(name) && !(_eventDic[name] is Action<T, U, V, W>))
                throw new Exception($"try AddListener funInfo:{call.GetMethodInfo().Name}, name:{name}");
            
            var oriCall = call;
            call += OnceRemoveFun;

            if (!_onceEventDic.TryGetValue(name, out var info))
            {
                info = new OnceEventInfo();
                _onceEventDic.Add(name, info);
            }
            else if(info.dels.ContainsKey(oriCall))//去重
                return;

            info.dels.Add(oriCall, call);
            AddListener(name, call);

            //内部方法
            void OnceRemoveFun(T t, U u, V v, W w)
            {
                if (_onceEventDic.ContainsKey(name) && 
                    _onceEventDic[name].dels.ContainsKey(oriCall))
                {
                    RemoveListener(name, oriCall);
                }
            }
        }

        #endregion

        #region 添加监听

        public void AddListener(ClientEvent name, Action call)
        {
            if (_eventDic.TryGetValue(name, out var fun))
            {
                if (fun is Action act)
                {
                    var len1 = act.GetInvocationList().Length;
                    act -= call;
                    act += call;
                    var len2 = act.GetInvocationList().Length;
                    if (len2 > len1) ALL_EVENT_COUNT += 1;
                    _eventDic[name] = act;
                }
                else
                    throw new Exception($"try AddListener funInfo:{fun.GetMethodInfo().Name}, name:{name}");
            }
            else
            {
                _eventDic.Add(name, call);
                ALL_EVENT_COUNT += 1;
            }
        }
    
        public void AddListener<T>(ClientEvent name, Action<T> call)
        {
            if (_eventDic.TryGetValue(name, out var fun))
            {
                if (fun is Action<T> act)
                {
                    var len1 = act.GetInvocationList().Length;
                    act -= call;
                    act += call;
                    var len2 = act.GetInvocationList().Length;
                    if (len2 > len1) ALL_EVENT_COUNT += 1;
                    _eventDic[name] = act;
                }
                else
                    throw new Exception($"try AddListener funInfo:{fun.GetMethodInfo().Name}, name:{name}");
            }
            else
            {
                _eventDic.Add(name, call);
                ALL_EVENT_COUNT += 1;
            }
        }
    
        public void AddListener<T, U>(ClientEvent name, Action<T, U> call)
        {
            if (_eventDic.TryGetValue(name, out var fun))
            {
                if (fun is Action<T, U> act)
                {
                    var len1 = act.GetInvocationList().Length;
                    act -= call;
                    act += call;
                    var len2 = act.GetInvocationList().Length;
                    if (len2 > len1) ALL_EVENT_COUNT += 1;
                    _eventDic[name] = act;
                }
                else
                    throw new Exception($"try AddListener funInfo:{fun.GetMethodInfo().Name}, name:{name}");
            }
            else
            {
                _eventDic.Add(name, call);
                ALL_EVENT_COUNT += 1;
            }
        }
        
        public void AddListener<T, U, V>(ClientEvent name, Action<T, U, V> call)
        {
            if (_eventDic.TryGetValue(name, out var fun))
            {
                if (fun is Action<T, U, V> act)
                {
                    var len1 = act.GetInvocationList().Length;
                    act -= call;
                    act += call;
                    var len2 = act.GetInvocationList().Length;
                    if (len2 > len1) ALL_EVENT_COUNT += 1;
                    _eventDic[name] = act;
                }
                else
                    throw new Exception($"try AddListener funInfo:{fun.GetMethodInfo().Name}, name:{name}");
            }
            else
            {
                _eventDic.Add(name, call);
                ALL_EVENT_COUNT += 1;
            }
        }
        
        public void AddListener<T, U, V, W>(ClientEvent name, Action<T, U, V, W> call)
        {
            if (_eventDic.TryGetValue(name, out var fun))
            {
                if (fun is Action<T, U, V, W> act)
                {
                    var len1 = act.GetInvocationList().Length;
                    act -= call;
                    act += call;
                    var len2 = act.GetInvocationList().Length;
                    if (len2 > len1) ALL_EVENT_COUNT += 1;
                    _eventDic[name] = act;
                }
                else
                    throw new Exception($"try AddListener funInfo:{fun.GetMethodInfo().Name}, name:{name}");
            }
            else
            {
                _eventDic.Add(name, call);
                ALL_EVENT_COUNT += 1;
            }
        }

        #endregion

        #region 移除监听

        public void RemoveListener(ClientEvent name, Action call)
        {
            if (call == null) return;
            if (!_eventDic.TryGetValue(name, out var fun)) return;
            if (fun is Action act)
            {
                var len1 = act.GetInvocationList().Length;
                act -= call;
                if (act == null)
                {
                    _eventDic.Remove(name);
                    ALL_EVENT_COUNT -= 1;
                }
                else
                {
                    _eventDic[name] = act;
                    var len2 = act.GetInvocationList().Length;
                    if (len2 < len1) ALL_EVENT_COUNT -= 1;
                }

                //如果Once里面有，移出Once里的和Event中Once多添加的
                if (_onceEventDic.ContainsKey(name) && 
                    _onceEventDic[name].dels.TryGetValue(call, out var removeFun))
                {
                    _onceEventDic[name].dels.Remove(call);
                    if (_onceEventDic[name].dels.Count == 0)
                        _onceEventDic.Remove(name);
                    RemoveListener(name, (Action)removeFun);
                }
            }
        }
    
        public void RemoveListener<T>(ClientEvent name, Action<T> call)
        {
            if (call == null) return;
            if (!_eventDic.TryGetValue(name, out var fun)) return;
            if (fun is Action<T> act)
            {
                var len1 = act.GetInvocationList().Length;
                act -= call;
                if (act == null)
                {
                    _eventDic.Remove(name);
                    ALL_EVENT_COUNT -= 1;
                }
                else
                {
                    _eventDic[name] = act;
                    var len2 = act.GetInvocationList().Length;
                    if (len2 < len1) ALL_EVENT_COUNT -= 1;
                }
                
                //如果Once里面有，移出Once里的和Event中Once多添加的
                if (_onceEventDic.ContainsKey(name) && 
                    _onceEventDic[name].dels.TryGetValue(call, out var removeFun))
                {
                    _onceEventDic[name].dels.Remove(call);
                    if (_onceEventDic[name].dels.Count == 0)
                        _onceEventDic.Remove(name);
                    RemoveListener(name, (Action<T>)removeFun);
                }
            }
        }
    
        public void RemoveListener<T, U>(ClientEvent name, Action<T, U> call)
        {
            if (call == null) return;
            if (!_eventDic.TryGetValue(name, out var fun)) return;
            if (fun is Action<T, U> act)
            {
                var len1 = act.GetInvocationList().Length;
                act -= call;
                if (act == null)
                {
                    _eventDic.Remove(name);
                    ALL_EVENT_COUNT -= 1;
                }
                else
                {
                    _eventDic[name] = act;
                    var len2 = act.GetInvocationList().Length;
                    if (len2 < len1) ALL_EVENT_COUNT -= 1;
                }
                
                //如果Once里面有，移出Once里的和Event中Once多添加的
                if (_onceEventDic.ContainsKey(name) && 
                    _onceEventDic[name].dels.TryGetValue(call, out var removeFun))
                {
                    _onceEventDic[name].dels.Remove(call);
                    if (_onceEventDic[name].dels.Count == 0)
                        _onceEventDic.Remove(name);
                    RemoveListener(name, (Action<T, U>)removeFun);
                }
            }
        }
        
        public void RemoveListener<T, U, V>(ClientEvent name, Action<T, U, V> call)
        {
            if (call == null) return;
            if (!_eventDic.TryGetValue(name, out var fun)) return;
            if (fun is Action<T, U, V> act)
            {
                var len1 = act.GetInvocationList().Length;
                act -= call;
                if (act == null)
                {
                    _eventDic.Remove(name);
                    ALL_EVENT_COUNT -= 1;
                }
                else
                {
                    _eventDic[name] = act;
                    var len2 = act.GetInvocationList().Length;
                    if (len2 < len1) ALL_EVENT_COUNT -= 1;
                }
                
                //如果Once里面有，移出Once里的和Event中Once多添加的
                if (_onceEventDic.ContainsKey(name) && 
                    _onceEventDic[name].dels.TryGetValue(call, out var removeFun))
                {
                    _onceEventDic[name].dels.Remove(call);
                    if (_onceEventDic[name].dels.Count == 0)
                        _onceEventDic.Remove(name);
                    RemoveListener(name, (Action<T, U, V>)removeFun);
                }
            }
        }
        
        public void RemoveListener<T, U, V, W>(ClientEvent name, Action<T, U, V, W> call)
        {
            if (call == null) return;
            if (!_eventDic.TryGetValue(name, out var fun)) return;
            if (fun is Action<T, U, V, W> act)
            {
                var len1 = act.GetInvocationList().Length;
                act -= call;
                if (act == null)
                {
                    _eventDic.Remove(name);
                    ALL_EVENT_COUNT -= 1;
                }
                else
                {
                    _eventDic[name] = act;
                    var len2 = act.GetInvocationList().Length;
                    if (len2 < len1) ALL_EVENT_COUNT -= 1;
                }
                
                //如果Once里面有，移出Once里的和Event中Once多添加的
                if (_onceEventDic.ContainsKey(name) && 
                    _onceEventDic[name].dels.TryGetValue(call, out var removeFun))
                {
                    _onceEventDic[name].dels.Remove(call);
                    if (_onceEventDic[name].dels.Count == 0)
                        _onceEventDic.Remove(name);
                    RemoveListener(name, (Action<T, U, V, W>)removeFun);
                }
            }
        }

        #endregion

        #region 触发事件

        public void TriggerEvent(ClientEvent name)
        {
            if(!_eventDic.TryGetValue(name, out var fun)) return;
            var list = fun.GetInvocationList();
            for (var i = 0; i < list.Length; i++)
            {
                if(!(list[i] is Action act)) continue;
                try
                {
                    act.Invoke();
                }
                catch (Exception e)
                {
                    Debug.LogErrorFormat("响应事件异常，事件名({0})\n{1}\n{2}", name, e.Message, e.StackTrace);
                }
            }
        }
    
        public void TriggerEvent<T>(ClientEvent name, T param1)
        {
            if(!_eventDic.TryGetValue(name, out var fun)) return;
            var list = fun.GetInvocationList();
            for (var i = 0; i < list.Length; i++)
            {
                if(!(list[i] is Action<T> act)) continue;
                try
                {
                    act.Invoke(param1);
                }
                catch (Exception e)
                {
                    Debug.LogErrorFormat("响应事件异常，事件名({0})\n{1}\n{2}", name, e.Message, e.StackTrace);
                }
            }
        }
    
        public void TriggerEvent<T, U>(ClientEvent name, T param1, U param2)
        {
            if(!_eventDic.TryGetValue(name, out var fun)) return;
            var list = fun.GetInvocationList();
            for (var i = 0; i < list.Length; i++)
            {
                if(!(list[i] is Action<T, U> act)) continue;
                try
                {
                    act.Invoke(param1, param2);
                }
                catch (Exception e)
                {
                    Debug.LogErrorFormat("响应事件异常，事件名({0})\n{1}\n{2}", name, e.Message, e.StackTrace);
                }
            }
        }
        
        public void TriggerEvent<T, U, V>(ClientEvent name, T param1, U param2, V param3)
        {
            if(!_eventDic.TryGetValue(name, out var fun)) return;
            var list = fun.GetInvocationList();
            for (var i = 0; i < list.Length; i++)
            {
                if(!(list[i] is Action<T, U, V> act)) continue;
                try
                {
                    act.Invoke(param1, param2, param3);
                }
                catch (Exception e)
                {
                    Debug.LogErrorFormat("响应事件异常，事件名({0})\n{1}\n{2}", name, e.Message, e.StackTrace);
                }
            }
        }
        
        public void TriggerEvent<T, U, V, W>(ClientEvent name, T param1, U param2, V param3, W param4)
        {
            if(!_eventDic.TryGetValue(name, out var fun)) return;
            var list = fun.GetInvocationList();
            for (var i = 0; i < list.Length; i++)
            {
                if(!(list[i] is Action<T, U, V, W> act)) continue;
                try
                {
                    act.Invoke(param1, param2, param3, param4);
                }
                catch (Exception e)
                {
                    Debug.LogErrorFormat("响应事件异常，事件名({0})\n{1}\n{2}", name, e.Message, e.StackTrace);
                }
            }
        }

        #endregion
    

        /// <summary>
        /// 移除事件所有监听，慎用
        /// </summary>
        /// <param name="name"></param>
        public void RemoveEvent(ClientEvent name)
        {
            if (_eventDic.TryGetValue(name, out var fun))
            {
                ALL_EVENT_COUNT -= fun.GetInvocationList().Length;
                _eventDic.Remove(name);
                _onceEventDic.Remove(name);
            }
        }
    
        public void Clear()
        {
            _eventDic.Clear();
            _onceEventDic.Clear();
        }
    }
}
