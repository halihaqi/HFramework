namespace HFramework
{
    public static class int_Expand
    {
        /// <summary>
        /// 将int转换为time格式字符串 00:00:00
        /// </summary>
        /// <param name="i"></param>
        /// <returns></returns>
        public static string ToTime(this int i)
        {
            if (i < 0)
                return "0";
            int s = i % 60;
            i = i / 60;
            int m = i % 60;
            i = i / 60;
            int h = i;
            return h + ":" + m + ":" + s;
        }

        /// <summary>
        /// 将int转换为 "x00" 格式
        /// </summary>
        /// <param name="i"></param>
        /// <returns></returns>
        public static string ToXNum(this int i) => $"x{i}";

        public static string ToLv(this int i) => $"Lv.{i}";
    }
}
