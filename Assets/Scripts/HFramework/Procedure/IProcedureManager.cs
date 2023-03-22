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
    }
}