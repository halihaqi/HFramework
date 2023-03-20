using System.Collections.Generic;
using UnityEngine;

namespace HFramework
{
    internal class FsmManager : HModule, IFsmManager
    {
        private Dictionary<string, FsmBase> _mFsms;
        private List<FsmBase> _mTempFsms;
        
        internal override int Priority => 6;

        /// <summary>
        /// 获取有限状态机数量。
        /// </summary>
        public int Count => _mFsms.Count;

        internal override void Init()
        {
            _mFsms = new Dictionary<string, FsmBase>();
            _mTempFsms = new List<FsmBase>();
        }

        internal override void Update(float elapseSeconds, float realElapseSeconds)
        {
            _mTempFsms.Clear();
            if(_mFsms.Count <= 0)
                return;
            foreach (var fsm in _mFsms.Values)
            {
                _mTempFsms.Add(fsm);
            }

            foreach (var fsm in _mTempFsms)
            {
                if(fsm.IsDestroyed)
                    continue;
                fsm.Update(elapseSeconds, realElapseSeconds);
            }
        }
        
        internal override void Shutdown()
        {
            foreach (var fsm in _mFsms.Values)
            {
                fsm.Shutdown();
            }

            _mFsms.Clear();
            _mTempFsms.Clear();
        }
        
        /// <summary>
        /// 检查是否存在有限状态机。
        /// </summary>
        /// <param name="name">有限状态机名</param>
        /// <returns></returns>
        public bool HasFsm(string name)
        {
            return _mFsms.ContainsKey(name);
        }
        
        /// <summary>
        /// 获取有限状态机。
        /// </summary>
        /// <param name="name">要获取的有限状态机。</param>
        /// <typeparam name="T">状态机持有者类型。</typeparam>
        /// <returns></returns>
        public IFsm<T> GetFsm<T>(string name) where T : class => GetFsm(name) as Fsm<T>;
        
        /// <summary>
        /// 获取有限状态机。
        /// </summary>
        /// <returns>要获取的有限状态机。</returns>
        public FsmBase GetFsm(string name)
        {
            if (_mFsms.TryGetValue(name, out var fsm))
                return fsm;
            return null;
        }
        
        /// <summary>
        /// 获取所有有限状态机。
        /// </summary>
        /// <returns>所有有限状态机。</returns>
        public FsmBase[] GetAllFsms()
        {
            int index = 0;
            FsmBase[] results = new FsmBase[_mFsms.Count];
            foreach (var fsm in _mFsms.Values)
            {
                results[index++] = fsm;
            }

            return results;
        }
        
        /// <summary>
        /// 创建有限状态机。
        /// </summary>
        /// <typeparam name="T">有限状态机持有者类型。</typeparam>
        /// <param name="name">有限状态机名称。</param>
        /// <param name="owner">有限状态机持有者。</param>
        /// <param name="states">有限状态机状态集合。</param>
        /// <returns>要创建的有限状态机。</returns>
        public IFsm<T> CreateFsm<T>(string name, T owner, params FsmState<T>[] states) where T : class
        {
            if (HasFsm(name))
                Debug.Log($"Already exist FSM '{typeof(T).Name}'.");

            Fsm<T> fsm = Fsm<T>.Create(name, owner, states);
            _mFsms.Add(name, fsm);
            return fsm;
        }
        
        /// <summary>
        /// 创建有限状态机。
        /// </summary>
        /// <typeparam name="T">有限状态机持有者类型。</typeparam>
        /// <param name="name">有限状态机名称。</param>
        /// <param name="owner">有限状态机持有者。</param>
        /// <param name="states">有限状态机状态集合。</param>
        /// <returns>要创建的有限状态机。</returns>
        public IFsm<T> CreateFsm<T>(string name, T owner, List<FsmState<T>> states) where T : class
        {
            if (HasFsm(name))
                Debug.Log($"Already exist FSM '{typeof(T).Name}'.");

            Fsm<T> fsm = Fsm<T>.Create(name, owner, states);
            _mFsms.Add(name, fsm);
            return fsm;
        }
        
        /// <summary>
        /// 销毁有限状态机。
        /// </summary>
        /// <param name="name">有限状态机名称。</param>
        /// <returns>是否销毁有限状态机成功。</returns>
        public bool DestroyFsm(string name)
        {
            if (_mFsms.TryGetValue(name, out var fsm))
            {
                fsm.Shutdown();
                return _mFsms.Remove(name);
            }

            return false;
        }
    }
}