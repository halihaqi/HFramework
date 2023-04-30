using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using UnityEngine;

namespace HFramework
{
    internal class DataManager : HModule, IDataManager
    {
        //路径
        private static readonly string DATA_BINARY_PATH = $"{Application.streamingAssetsPath}/Binary/";//Excel生成二进制数据文件夹
        
        //Excel二进制数据容器，键为数据容器名
        private Dictionary<string, object> _dataDic;

        internal override int Priority => 1;

        internal override void Init()
        {
            _dataDic = new Dictionary<string, object>();
            InitTableData();
        }

        internal override void Update(float elapseSeconds, float realElapseSeconds)
        {
        }

        internal override void Shutdown()
        {
            _dataDic.Clear();
        }
        
        #region Data

        public void Save(string part, string name, object data)
        {
            string parentFile = $"{Application.persistentDataPath}/{part}";
            string path = $"{parentFile}/{name}.zxy";

            if (!Directory.Exists(parentFile))
                Directory.CreateDirectory(parentFile);
            
            using (FileStream fs = new FileStream(path, FileMode.OpenOrCreate))
            {
                BinaryFormatter bf = new BinaryFormatter();
                bf.Serialize(fs, data);
                fs.Flush();
                fs.Close();
            }
        }

        public void CustomSave(string part, string name, HDataBase data)
        {
            string parentFile = $"{Application.persistentDataPath}/{part}";
            string path = $"{parentFile}/{name}.zxy";

            if (!Directory.Exists(parentFile))
                Directory.CreateDirectory(parentFile);
            
            using (FileStream fs = new FileStream(path, FileMode.OpenOrCreate))
            {
                fs.Write(data.Serialize(), 0, data.GetBytesCount());
                fs.Flush();
                fs.Close();
            }
        }

        public T Load<T>(string part, string name) where T : new()
        {
            string path = $"{Application.persistentDataPath}/{part}/{name}.zxy";
            if (!File.Exists(path))
                return default;

            T obj;
            using (FileStream fs = new FileStream(path, FileMode.Open))
            {
                BinaryFormatter bf = new BinaryFormatter();
                obj = (T)bf.Deserialize(fs);
                fs.Close();
            }

            return obj;
        }

        public T CustomLoad<T>(string part, string name) where T : HDataBase, new()
        {
            string path = $"{Application.persistentDataPath}/{part}/{name}.zxy";
            if (!File.Exists(path))
                return default;

            T obj = new T();
            using (FileStream fs = new FileStream(path, FileMode.Open))
            {
                int len = obj.GetBytesCount();
                byte[] data = new byte[len];
                fs.Read(data, 0, len);
                obj.Deserialize(data);
                fs.Close();
            }

            return obj;
        }
        
        public bool TryLoad<T>(string part, string name, out T data) where T : new()
        {
            if (HasData(part, name))
            {
                data = Load<T>(part, name);
                return true;
            }

            data = default;
            return false;
        }

        public bool HasData(string part, string name)
        {
            return File.Exists($"{Application.persistentDataPath}/{part}/{name}.zxy");
        }

        #endregion

        #region Table

        /// <summary>
        /// 初始化Excel二进制数据到容器中
        /// </summary>
        private void InitTableData()
        {
            _dataDic.Clear();
            if (!Directory.Exists(DATA_BINARY_PATH))
                Directory.CreateDirectory(DATA_BINARY_PATH);
            FileInfo[] fileInfos = new DirectoryInfo(DATA_BINARY_PATH).GetFiles();
            foreach (FileInfo fileInfo in fileInfos)
            {
                if(fileInfo.Extension != ".zxy")
                    continue;
                var className = fileInfo.Name.Split('.')[0];
                var containerName = $"{className}Container";
                LoadTable(containerName, className);
            }
        }
        
        /// <summary>
        /// 加载Excel表的二进制数据存入数据字典
        /// </summary>
        /// <param name="containerName">容器名</param>
        /// <param name="className">数据结构名</param>
        private void LoadTable(string containerName, string className)
        {
            Type containerType = Type.GetType(containerName);
            Type classType = Type.GetType(className);
            if (containerType == null || classType == null)
            {
                Debug.LogError($"{containerName} 类不存在 或 {className} 类不存在");
                return;
            }
        
            //1.读取二进制数据
            byte[] bytes;
            using (FileStream fs = File.Open($"{DATA_BINARY_PATH}{classType.Name}.zxy", FileMode.Open, FileAccess.Read))
            {
                bytes = new byte[fs.Length];
                fs.Read(bytes, 0, bytes.Length);
                fs.Close();
            }
            var index = 0;
        
            //2.读取数据行数
            int count = BitConverter.ToInt32(bytes, index);
            index += sizeof(int);
            
            //3.读取主键名
            int keyNameLength = BitConverter.ToInt32(bytes, index);
            index += sizeof(int);
            string keyName = Encoding.UTF8.GetString(bytes, index, keyNameLength);
            index += keyNameLength;
            
            //4.读取每一行信息
            //实例化容器类
            object containerObj = Activator.CreateInstance(containerType);
            FieldInfo[] infos = classType.GetFields();
            for (var i = 0; i < count; i++)
            {
                //实例化数据结构类
                object dataObj = Activator.CreateInstance(classType);
                //填充数据结构类
                foreach (FieldInfo info in infos)
                {
                    if (info.FieldType == typeof(int))
                    {
                        DataUtils.DecryptData(bytes, index, sizeof(int));//解密
                        info.SetValue(dataObj, BitConverter.ToInt32(bytes, index));
                        index += sizeof(int);
                    }
                    else if (info.FieldType == typeof(long))
                    {
                        DataUtils.DecryptData(bytes, index, sizeof(long));//解密
                        info.SetValue(dataObj, BitConverter.ToInt64(bytes, index));
                        index += sizeof(long);
                    }
                    else if (info.FieldType == typeof(float))
                    {
                        DataUtils.DecryptData(bytes, index, sizeof(float));//解密
                        info.SetValue(dataObj, BitConverter.ToSingle(bytes, index));
                        index += sizeof(float);
                    }
                    else if (info.FieldType == typeof(bool))
                    {
                        DataUtils.DecryptData(bytes, index, sizeof(bool));//解密
                        info.SetValue(dataObj, BitConverter.ToBoolean(bytes, index));
                        index += sizeof(bool);
                    }
                    else if (info.FieldType == typeof(string))
                    {
                        int len = BitConverter.ToInt32(bytes, index);
                        index += sizeof(int);
                        DataUtils.DecryptData(bytes, index, len);//解密
                        info.SetValue(dataObj, Encoding.UTF8.GetString(bytes, index, len));
                        index += len;
                    }
                    //复杂类型
                    else
                    {
                        int arrLength;
                        if (info.FieldType == typeof(int[]))
                        {
                            arrLength = BitConverter.ToInt32(bytes, index);
                            index += sizeof(int);
                            int[] arrInt = new int[arrLength];
                            for (int j = 0; j < arrLength; j++)
                            {
                                DataUtils.DecryptData(bytes, index, sizeof(int));//解密
                                arrInt[j] = BitConverter.ToInt32(bytes, index);
                                index += sizeof(int);
                            }
                            info.SetValue(dataObj, arrInt);
                        }
                        else if (info.FieldType == typeof(long[]))
                        {
                            arrLength = BitConverter.ToInt32(bytes, index);
                            index += sizeof(int);
                            long[] arrLong = new long[arrLength];
                            for (int j = 0; j < arrLength; j++)
                            {
                                DataUtils.DecryptData(bytes, index, sizeof(long));//解密
                                arrLong[j] = BitConverter.ToInt64(bytes, index);
                                index += sizeof(long);
                            }
                            info.SetValue(dataObj, arrLong);
                        }
                        else if (info.FieldType == typeof(float[]))
                        {
                            arrLength = BitConverter.ToInt32(bytes, index);
                            index += sizeof(int);
                            float[] arrFloat = new float[arrLength];
                            for (int j = 0; j < arrLength; j++)
                            {
                                DataUtils.DecryptData(bytes, index, sizeof(float));//解密
                                arrFloat[j] = BitConverter.ToSingle(bytes, index);
                                index += sizeof(float);
                            }
                            info.SetValue(dataObj, arrFloat);
                        }
                        else if (info.FieldType == typeof(string[]))
                        {
                            arrLength = BitConverter.ToInt32(bytes, index);
                            index += sizeof(int);
                            string[] arrStr = new string[arrLength];
                            for (int j = 0; j < arrLength; j++)
                            {
                                int len = BitConverter.ToInt32(bytes, index);
                                index += sizeof(int);
                                DataUtils.DecryptData(bytes, index, len);//解密
                                arrStr[j] = Encoding.UTF8.GetString(bytes, index, len);
                                index += len;
                            }
                            info.SetValue(dataObj, arrStr);
                        }
                    }
                }
            
                //添加数据结构类到容器类
                object dicObj = containerType.GetField("dataDic").GetValue(containerObj);
                MethodInfo mInfo = dicObj.GetType().GetMethod("Add");
                object keyValue = classType.GetField(keyName).GetValue(dataObj);
                mInfo.Invoke(dicObj, new[] { keyValue, dataObj });
            }
        
            //记录读取的数据
            _dataDic.Add(containerType.Name, containerObj);
        }

        /// <summary>
        /// 获取一张表信息
        /// </summary>
        /// <typeparam name="T">容器名</typeparam>
        /// <returns></returns>
        public T GetTable<T>() where T : class
        {
            string tableName = typeof(T).Name;
            if (_dataDic.ContainsKey(tableName))
                return _dataDic[tableName] as T;
            return null;
        }

        /// <summary>
        /// 获取表中元素
        /// </summary>
        /// <param name="index">item主键</param>
        /// <typeparam name="T">容器名</typeparam>
        /// <typeparam name="TKey">item主键类</typeparam>
        /// <typeparam name="TInfo">item类</typeparam>
        /// <returns></returns>
        public TInfo GetInfo<T, TKey, TInfo>(TKey index) where T : HContainerBase
        {
            Dictionary<TKey,TInfo> dic = GetTable<T>().GetDic() as Dictionary<TKey, TInfo>;
            
            if (dic == null) return default;
            if (dic.TryGetValue(index, out TInfo val))
                return val;
            return default;
        }

        #endregion
    }
}