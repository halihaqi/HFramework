using System;

/// <summary>
/// 单例模式基类(不继承Mono)
/// </summary>
/// <typeparam name="T"></typeparam>
public abstract class Singleton<T> where T : Singleton<T>, new()
{
    private static T _instance;
    public static T Instance
    {
        get
        {
            if (_instance == null)
                _instance = new T();
            return _instance;
        }
    }
}


#region Pro版，有多线程锁，有私有构造函数(不常用)
///// <summary>
///// 单例模式基类(不继承Mono)
///// 有多线程锁 有私有构造函数
///// </summary>
///// <typeparam name="T"></typeparam>
//public abstract class Singleton<T> where T : Singleton<T>
//{
//    private static T instance;
//    private static readonly object obj = new object();
//    public static T Instance
//    {
//        get
//        {
//            if (instance == null)
//            {
//                lock (obj)
//                {
//                    if (instance == null)
//                    {
//                        instance = Activator.CreateInstance<T>();
//                    }
//                }
//            }
//            return instance;
//        }
//    }
//}
#endregion
