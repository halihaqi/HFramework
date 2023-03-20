namespace HFramework
{
    public interface IReferencePoolManager
    {
        /// <summary>
        /// 从引用池取出
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        T PopRef<T>() where T : class, IReference, new();

        /// <summary>
        /// 压入引用池
        /// </summary>
        /// <param name="obj">引用实例</param>
        /// <typeparam name="T"></typeparam>
        void PushRef<T>(T obj) where T : class, IReference, new();

        /// <summary>
        /// 清空引用池
        /// </summary>
        void ClearReferencePool();
    }
}