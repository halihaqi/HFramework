namespace HFramework
{
    public interface IProcedureManager
    {
        /// <summary>
        /// 当前流程
        /// </summary>
        ProcedureBase CurProcedure { get; }
        
        /// <summary>
        /// 获取当前流程持续时间。
        /// </summary>
        float CurrentProcedureTime { get; }

        /// <summary>
        /// 初始化流程管理器。
        /// </summary>
        /// <param name="procedures">流程管理器包含的流程。</param>
        void Initialize(params ProcedureBase[] procedures);

        /// <summary>
        /// 开始流程。
        /// </summary>
        /// <typeparam name="T">要开始的流程类型。</typeparam>
        void StartProcedure<T>() where T : ProcedureBase;

        /// <summary>
        /// 是否存在流程。
        /// </summary>
        /// <typeparam name="T">要检查的流程类型。</typeparam>
        /// <returns>是否存在流程。</returns>
        bool HasProcedure<T>() where T : ProcedureBase;

        /// <summary>
        /// 获取流程。
        /// </summary>
        /// <typeparam name="T">要获取的流程类型。</typeparam>
        /// <returns>要获取的流程。</returns>
        ProcedureBase GetProcedure<T>() where T : ProcedureBase;
        
        /// <summary>
        /// 切换状态
        /// </summary>
        /// <typeparam name="T"></typeparam>
        void ChangeState<T>() where T : ProcedureBase;

        /// <summary>
        /// 设置流程数据。
        /// </summary>
        /// <typeparam name="TData">数据的类型。</typeparam>
        /// <param name="name">数据名称。</param>
        /// <param name="data">要获取的数据。</param>
        void SetData<TData>(string name, TData data);

        /// <summary>
        /// 移除流程数据。
        /// </summary>
        /// <param name="name">数据名称。</param>
        /// <returns>是否移除数据成功。</returns>
        bool RemoveData(string name);

        /// <summary>
        /// 获取流程数据。
        /// </summary>
        /// <param name="name">数据名称。</param>
        /// <returns>要获取的数据。</returns>
        TData GetData<TData>(string name);
    }
}