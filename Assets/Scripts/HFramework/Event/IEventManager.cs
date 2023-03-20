using System;

namespace HFramework
{
    public interface IEventManager
    {
        #region 单次监听

        
        /// <summary>
        /// 添加单次事件监听
        /// </summary>
        /// <param name="name">事件枚举</param>
        /// <param name="call">回调</param>
        void OnceListener(ClientEvent name, Action call);

        /// <summary>
        /// 添加单次事件监听
        /// </summary>
        /// <param name="name">事件枚举</param>
        /// <param name="call">回调</param>
        /// <typeparam name="T">参数1</typeparam>
        void OnceListener<T>(ClientEvent name, Action<T> call);

        /// <summary>
        /// 添加单次事件监听
        /// </summary>
        /// <param name="name">事件枚举</param>
        /// <param name="call">回调</param>
        /// <typeparam name="T">参数1</typeparam>
        /// <typeparam name="U">参数2</typeparam>
        void OnceListener<T, U>(ClientEvent name, Action<T, U> call);

        /// <summary>
        /// 添加单次事件监听
        /// </summary>
        /// <param name="name">事件枚举</param>
        /// <param name="call">回调</param>
        /// <typeparam name="T">参数1</typeparam>
        /// <typeparam name="U">参数2</typeparam>
        /// <typeparam name="V">参数3</typeparam>
        void OnceListener<T, U, V>(ClientEvent name, Action<T, U, V> call);

        /// <summary>
        /// 添加单次事件监听
        /// </summary>
        /// <param name="name">事件枚举</param>
        /// <param name="call">回调</param>
        /// <typeparam name="T">参数1</typeparam>
        /// <typeparam name="U">参数2</typeparam>
        /// <typeparam name="V">参数3</typeparam>
        /// <typeparam name="W">参数4</typeparam>
        void OnceListener<T, U, V, W>(ClientEvent name, Action<T, U, V, W> call);

        #endregion

        #region 添加监听

        /// <summary>
        /// 添加事件监听
        /// </summary>
        /// <param name="name">事件枚举</param>
        /// <param name="call">回调</param>
        void AddListener(ClientEvent name, Action call);

        /// <summary>
        /// 添加事件监听
        /// </summary>
        /// <param name="name">事件枚举</param>
        /// <param name="call">回调</param>
        /// <typeparam name="T">参数1</typeparam>
        void AddListener<T>(ClientEvent name, Action<T> call);

        /// <summary>
        /// 添加事件监听
        /// </summary>
        /// <param name="name">事件枚举</param>
        /// <param name="call">回调</param>
        /// <typeparam name="T">参数1</typeparam>
        /// <typeparam name="U">参数2</typeparam>
        void AddListener<T, U>(ClientEvent name, Action<T, U> call);

        /// <summary>
        /// 添加事件监听
        /// </summary>
        /// <param name="name">事件枚举</param>
        /// <param name="call">回调</param>
        /// <typeparam name="T">参数1</typeparam>
        /// <typeparam name="U">参数2</typeparam>
        /// <typeparam name="V">参数3</typeparam>
        void AddListener<T, U, V>(ClientEvent name, Action<T, U, V> call);

        /// <summary>
        /// 添加事件监听
        /// </summary>
        /// <param name="name">事件枚举</param>
        /// <param name="call">回调</param>
        /// <typeparam name="T">参数1</typeparam>
        /// <typeparam name="U">参数2</typeparam>
        /// <typeparam name="V">参数3</typeparam>
        /// <typeparam name="W">参数4</typeparam>
        void AddListener<T, U, V, W>(ClientEvent name, Action<T, U, V, W> call);

        #endregion

        #region 移除监听

        /// <summary>
        /// 移除监听
        /// </summary>
        /// <param name="name">事件枚举</param>
        /// <param name="call">回调</param>
        void RemoveListener(ClientEvent name, Action call);

        /// <summary>
        /// 移除监听
        /// </summary>
        /// <param name="name">事件枚举</param>
        /// <param name="call">回调</param>
        /// <typeparam name="T">参数1</typeparam>
        void RemoveListener<T>(ClientEvent name, Action<T> call);

        /// <summary>
        /// 移除监听
        /// </summary>
        /// <param name="name">事件枚举</param>
        /// <param name="call">回调</param>
        /// <typeparam name="T">参数1</typeparam>
        /// <typeparam name="U">参数2</typeparam>
        void RemoveListener<T, U>(ClientEvent name, Action<T, U> call);

        /// <summary>
        /// 移除监听
        /// </summary>
        /// <param name="name">事件枚举</param>
        /// <param name="call">回调</param>
        /// <typeparam name="T">参数1</typeparam>
        /// <typeparam name="U">参数2</typeparam>
        /// <typeparam name="V">参数3</typeparam>
        void RemoveListener<T, U, V>(ClientEvent name, Action<T, U, V> call);

        /// <summary>
        /// 移除监听
        /// </summary>
        /// <param name="name">事件枚举</param>
        /// <param name="call">回调</param>
        /// <typeparam name="T">参数1</typeparam>
        /// <typeparam name="U">参数2</typeparam>
        /// <typeparam name="V">参数3</typeparam>
        /// <typeparam name="W">参数4</typeparam>
        void RemoveListener<T, U, V, W>(ClientEvent name, Action<T, U, V, W> call);

        #endregion

        #region 触发事件

        /// <summary>
        /// 触发事件
        /// </summary>
        /// <param name="name">事件枚举</param>
        void TriggerEvent(ClientEvent name);

        /// <summary>
        /// 触发事件
        /// </summary>
        /// <param name="name">事件枚举</param>
        /// <param name="param1">参数1</param>
        /// <typeparam name="T"></typeparam>
        void TriggerEvent<T>(ClientEvent name, T param1);

        /// <summary>
        /// 触发事件
        /// </summary>
        /// <param name="name">事件枚举</param>
        /// <param name="param1">参数1</param>
        /// <param name="param2">参数2</param>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="U"></typeparam>
        void TriggerEvent<T, U>(ClientEvent name, T param1, U param2);

        /// <summary>
        /// 触发事件
        /// </summary>
        /// <param name="name">事件枚举</param>
        /// <param name="param1">参数1</param>
        /// <param name="param2">参数2</param>
        /// <param name="param3">参数3</param>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="U"></typeparam>
        /// <typeparam name="V"></typeparam>
        void TriggerEvent<T, U, V>(ClientEvent name, T param1, U param2, V param3);

        /// <summary>
        /// 触发事件
        /// </summary>
        /// <param name="name">事件枚举</param>
        /// <param name="param1">参数1</param>
        /// <param name="param2">参数2</param>
        /// <param name="param3">参数3</param>
        /// <param name="param4">参数4</param>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="U"></typeparam>
        /// <typeparam name="V"></typeparam>
        /// <typeparam name="W"></typeparam>
        void TriggerEvent<T, U, V, W>(ClientEvent name, T param1, U param2, V param3, W param4);

        #endregion

        /// <summary>
        /// 移除事件所有监听，慎用
        /// </summary>
        /// <param name="name"></param>
        void RemoveEvent(ClientEvent name);

        /// <summary>
        /// 清空事件监听
        /// </summary>
        void Clear();
    }
}