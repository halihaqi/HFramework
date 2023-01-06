using System;
using UnityEngine;

namespace Hali_Framework
{
    /// <summary>
    /// 全局Mono脚本，外部不调用，通过MonoMgr使用
    /// </summary>
    public class GlobalMono : MonoBehaviour
    {
        private event Action UpdateEvent;

        private void Start()
        {
            DontDestroyOnLoad(this.gameObject);
        }

        private void Update()
        {
            UpdateEvent?.Invoke();
        }
        
        public void AddUpdateListener(Action action)
        {
            UpdateEvent -= action;
            UpdateEvent += action;
        }
        
        public void RemoveUpdateListener(Action action)
        {
            UpdateEvent -= action;
        }
        
        public void Clear()
        {
            UpdateEvent = null;
        }
    }
}
