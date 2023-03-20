namespace HFramework
{
    public abstract class HModule
    {
        /// <summary>
        /// 游戏模块优先级，优先级高的先轮询
        /// </summary>
        internal virtual int Priority => 0;

        /// <summary>
        /// 游戏模块初始化，优先级低的先初始化
        /// </summary>
        internal abstract void Init();

        /// <summary>
        /// 游戏模块轮询
        /// </summary>
        /// <param name="elapseSeconds">逻辑时间</param>
        /// <param name="realElapseSeconds">真实时间</param>
        internal abstract void Update(float elapseSeconds, float realElapseSeconds);

        /// <summary>
        /// 关闭并清理游戏模块，优先级低的先关闭
        /// </summary>
        internal abstract void Shutdown();
    }
}