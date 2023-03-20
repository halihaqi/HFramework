using UnityEngine;

namespace HFramework
{
    public static class GameConst
    {
        //基础类型字节长度
        public const int INT_SIZE = sizeof(int);
        public const int LONG_SIZE = sizeof(long);
        public const int FLOAT_SIZE = sizeof(float);
        public const int BOOL_SIZE = sizeof(bool);
        
        //路径
        public static string DATA_BINARY_PATH = $"{Application.streamingAssetsPath}/Binary/";//Excel生成二进制数据文件夹
        
        //数据密钥
        public const byte KEY = 233;
        
        //UI
        //UI组名
        public const string UIGROUP_WORLD = "World";
        public const string UIGROUP_PANEL = "Panel";
        public const string UIGROUP_POP = "Pop";
        public const string UIGROUP_TIP = "Tip";
        public const string UIGROUP_SYS = "System";
    }
}