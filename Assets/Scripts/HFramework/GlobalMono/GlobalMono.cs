using System;
using UnityEngine;

namespace HFramework
{
    /// <summary>
    /// 全局Mono实体
    /// </summary>
    internal class GlobalMono : MonoBehaviour
    {
        private event Action UpdateEvent;
        private event Action<float, float> UpdateEventWithTime;

        private void Start()
        {
            DontDestroyOnLoad(this.gameObject);
        }

        private void Update()
        {
            UpdateEvent?.Invoke();
            UpdateEventWithTime?.Invoke(Time.deltaTime, Time.unscaledDeltaTime);
        }
        
        internal void AddUpdateListener(Action action)
        {
            UpdateEvent -= action;
            UpdateEvent += action;
        }
        
        internal void AddUpdateListener(Action<float, float> action)
        {
            UpdateEventWithTime -= action;
            UpdateEventWithTime += action;
        }
        
        internal void RemoveUpdateListener(Action action)
        {
            UpdateEvent -= action;
        }
        
        internal void RemoveUpdateListener(Action<float, float> action)
        {
            UpdateEventWithTime -= action;
        }
        
        internal void Clear()
        {
            UpdateEvent = null;
            UpdateEventWithTime = null;
        }

        internal void Dispose()
        {
            Destroy(this);
        }
    }
}
