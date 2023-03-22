using System;
using UnityEngine;

namespace HFramework
{
    /// <summary>
    /// 游戏流程管理类
    /// </summary>
    internal class ProcedureManager : HModule, IProcedureManager
    {
        private IFsm<IProcedureManager> _procedureFsm;
        
        internal override int Priority => 9;

        internal override void Init()
        {
        }

        internal override void Update(float elapseSeconds, float realElapseSeconds)
        {
        }

        internal override void Shutdown()
        {
            _procedureFsm = null;
        }

        public ProcedureBase CurProcedure
        {
            get
            {
                if (_procedureFsm == null)
                {
                    Debug.LogError("You must initialize procedure first.");
                    return null;
                }

                return (ProcedureBase)_procedureFsm.CurrentState;
            }
        }
        
        /// <summary>
        /// 获取当前流程持续时间。
        /// </summary>
        public float CurrentProcedureTime
        {
            get
            {
                if (_procedureFsm == null)
                {
                    Debug.LogError("You must initialize procedure first.");
                    return -1;
                }

                return _procedureFsm.CurrentStateTime;
            }
        }
        
        /// <summary>
        /// 初始化流程管理器。
        /// </summary>
        /// <param name="procedures">流程管理器包含的流程。</param>
        public void Initialize(params ProcedureBase[] procedures)
        {
            _procedureFsm = HEntry.FsmMgr.CreateFsm("ProcedureFsm", this, procedures);
        }
        
        /// <summary>
        /// 开始流程。
        /// </summary>
        /// <typeparam name="T">要开始的流程类型。</typeparam>
        public void StartProcedure<T>() where T : ProcedureBase
        {
            if (_procedureFsm == null)
            {
                Debug.LogError("You must initialize procedure first.");
                return;
            }

            _procedureFsm.Start<T>();
        }
        
        /// <summary>
        /// 是否存在流程。
        /// </summary>
        /// <typeparam name="T">要检查的流程类型。</typeparam>
        /// <returns>是否存在流程。</returns>
        public bool HasProcedure<T>() where T : ProcedureBase
        {
            if (_procedureFsm == null)
            {
                Debug.LogError("You must initialize procedure first.");
                return false;
            }

            return _procedureFsm.HasState<T>();
        }
        
        /// <summary>
        /// 获取流程。
        /// </summary>
        /// <typeparam name="T">要获取的流程类型。</typeparam>
        /// <returns>要获取的流程。</returns>
        public ProcedureBase GetProcedure<T>() where T : ProcedureBase
        {
            if (_procedureFsm == null)
            {
                Debug.LogError("You must initialize procedure first.");
                return null;
            }

            return _procedureFsm.GetState<T>();
        }
        
        public void ChangeState<T>() where T : ProcedureBase
        {
            _procedureFsm.ChangeState<T>();
        }
    }
}