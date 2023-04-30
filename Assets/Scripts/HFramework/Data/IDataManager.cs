namespace HFramework
{
    public interface IDataManager
    {
        /// <summary>
        /// 保存数据
        /// </summary>
        /// <param name="part">数据所在部分</param>
        /// <param name="name">数据名</param>
        /// <param name="data">数据</param>
        void Save(string part, string name, object data);

        /// <summary>
        /// 加载数据
        /// </summary>
        /// <param name="part">数据所在部分</param>
        /// <param name="name">数据名</param>
        /// <typeparam name="T">数据类型</typeparam>
        /// <returns></returns>
        T Load<T>(string part, string name) where T : new();

        /// <summary>
        /// 尝试加载数据
        /// </summary>
        /// <param name="part">数据所在部分</param>
        /// <param name="name">数据名</param>
        /// <param name="data">数据</param>
        /// <typeparam name="T">数据类型</typeparam>
        /// <returns>是否有该数据</returns>
        bool TryLoad<T>(string part, string name, out T data) where T : new();

        /// <summary>
        /// 是否有该数据
        /// </summary>
        /// <param name="part">数据所在部分</param>
        /// <param name="name">数据名</param>
        /// <returns></returns>
        bool HasData(string part, string name);

        /// <summary>
        /// 获取数据表容器
        /// </summary>
        /// <typeparam name="T">容器名</typeparam>
        /// <returns>容器类型</returns>
        T GetTable<T>() where T : class;

        /// <summary>
        /// 获取数据表容器中的某个数据
        /// </summary>
        /// <param name="index">info主键</param>
        /// <typeparam name="T">容器名</typeparam>
        /// <typeparam name="TKey">info主键类</typeparam>
        /// <typeparam name="TVal">info类</typeparam>
        /// <returns>info</returns>
        TVal GetInfo<T, TKey, TVal>(TKey index) where T : HContainerBase;
    }
}