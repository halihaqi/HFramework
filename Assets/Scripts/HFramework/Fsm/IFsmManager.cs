using System.Collections.Generic;

namespace HFramework
{
    public interface IFsmManager
    {
        /// <summary>
        /// 获取有限状态机数量
        /// </summary>
        int Count { get; }

        /// <summary>
        /// 检查是否存在有限状态机
        /// </summary>
        /// <param name="name">有限状态机名</param>
        /// <returns></returns>
        bool HasFsm(string name);
        
        /// <summary>
        /// 获取有限状态机
        /// </summary>
        /// <param name="name">要获取的有限状态机</param>
        /// <typeparam name="T">状态机持有者类型</typeparam>
        /// <returns></returns>
        IFsm<T> GetFsm<T>(string name) where T : class;

        /// <summary>
        /// 创建有限状态机
        /// </summary>
        /// <typeparam name="T">有限状态机持有者类型</typeparam>
        /// <param name="name">有限状态机名称</param>
        /// <param name="owner">有限状态机持有者</param>
        /// <param name="states">有限状态机状态集合</param>
        /// <returns>要创建的有限状态机</returns>
        IFsm<T> CreateFsm<T>(string name, T owner, params FsmState<T>[] states) where T : class;

        /// <summary>
        /// 创建有限状态机。
        /// </summary>
        /// <typeparam name="T">有限状态机持有者类型</typeparam>
        /// <param name="name">有限状态机名称</param>
        /// <param name="owner">有限状态机持有者</param>
        /// <param name="states">有限状态机状态集合</param>
        /// <returns>要创建的有限状态机</returns>
        IFsm<T> CreateFsm<T>(string name, T owner, List<FsmState<T>> states) where T : class;

        /// <summary>
        /// 销毁有限状态机
        /// </summary>
        /// <param name="name">有限状态机名称</param>
        /// <returns>是否销毁有限状态机成功</returns>
        bool DestroyFsm(string name);
    }
}