﻿using System;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Text;
using Excel;
using HFramework;
using UnityEditor;
using UnityEngine;
using Debug = UnityEngine.Debug;

namespace Editor.Excel
{
    public static class ExcelUtils
    {
        //Excel文件夹
        private static string EXCEL_PATH = $"{Application.dataPath}/Excel/";
        //Excel生成类文件夹
        private static string DATA_CLASS_PATH = $"{Application.dataPath}/Scripts/ExcelData/DataClass/";
        //Excel生成数据容器文件夹
        private static string DATA_CONTAINER_PATH = $"{Application.dataPath}/Scripts/ExcelData/Container/";
        //Excel生成二进制数据文件夹
        private static string DATA_BINARY_PATH = $"{Application.streamingAssetsPath}/Binary/";
        //从Excel第几行开始读数据
        private const int READ_INDEX = 4;
        

        [MenuItem("Tools/Excel/GenerateExcel")]
        private static void GenerateExcelInfo()
        {
            Stopwatch sw = Stopwatch.StartNew();

            //创建或获取源文件夹
            DirectoryInfo dirInfo = Directory.CreateDirectory(EXCEL_PATH);
            FileInfo[] fileInfos = dirInfo.GetFiles();
            
            //数据表容器
            DataTableCollection tableCollection;
            //获取文件夹下所有表数据
            for (var i = 0; i < fileInfos.Length; i++)
            {
                if (fileInfos[i].Extension != ".xlsx" && fileInfos[i].Extension != ".xls")
                    continue;

                using FileStream fs = fileInfos[i].Open(FileMode.Open, FileAccess.Read);
                IExcelDataReader eReader = ExcelReaderFactory.CreateOpenXmlReader(fs);
                tableCollection = eReader.AsDataSet().Tables;
                fs.Close();
                
                foreach (DataTable table in tableCollection)
                {
                    GenerateExcelDataClass(table);
                    GenerateExcelContainer(table);
                    GenerateExcelBinary(table);
                }
            }
            
            AssetDatabase.Refresh();
            Debug.Log($"数据生成完成，耗时：{sw.ElapsedMilliseconds}ms");
            sw.Stop();
        }

        
        
        /// <summary>
        /// 根据Excel表创建数据结构类
        /// </summary>
        /// <param name="table"></param>
        private static void GenerateExcelDataClass(DataTable table)
        {
            DataRow rowName = GetVariableNameRow(table);
            DataRow rowType = GetVariableTypeRow(table);
            
            //创建或获取源文件夹
            Directory.CreateDirectory(DATA_CLASS_PATH);
            
            StringBuilder content = new StringBuilder();
            content.Append("[System.Serializable]\n");
            content.Append($"public class {table.TableName}\n{{\n");
            //添加变量
            for (var i = 0; i < table.Columns.Count; i++)
            {
                content.Append($"    public {rowType[i]} {rowName[i]};\n");
            }
            content.Append("}");
            
            //创建数据结构类
            File.WriteAllText($"{DATA_CLASS_PATH}{table.TableName}.cs", content.ToString());
        }

        /// <summary>
        /// 根据Excel表创建数据容器类
        /// </summary>
        /// <param name="table"></param>
        private static void GenerateExcelContainer(DataTable table)
        {
            int keyIndex = GetKeyIndex(table);
            DataRow rowType = GetVariableTypeRow(table);
            
            //创建或获取源文件夹
            Directory.CreateDirectory(DATA_CONTAINER_PATH);
            
            StringBuilder content = new StringBuilder();
            content.Append("using System.Collections.Generic;\n");
            content.Append($"public class {table.TableName}Container : BaseContainer\n{{\n");
            content.Append($"   public Dictionary<{rowType[keyIndex]}, {table.TableName}> ");
            content.Append($"dataDic = new Dictionary<{rowType[keyIndex]}, {table.TableName}>();\n");
            content.Append("    public override object GetDic() => dataDic;\n");
            content.Append("}");
            
            //创建数据容器类
            File.WriteAllText($"{DATA_CONTAINER_PATH}{table.TableName}Container.cs", content.ToString());
        }

        /// <summary>
        /// 根据Excel表生成二进制数据
        /// </summary>
        /// <param name="table"></param>
        private static void GenerateExcelBinary(DataTable table)
        {
            //创建或获取源文件夹
            Directory.CreateDirectory(DATA_BINARY_PATH);

            using (FileStream fs = new FileStream($"{DATA_BINARY_PATH}{table.TableName}.zxy", FileMode.OpenOrCreate))
            {
                //1.存储需要写多少行数据，用于读取数据
                fs.Write(BitConverter.GetBytes(table.Rows.Count - READ_INDEX), 0, GameConst.INT_SIZE);
                //2.存储主键的变量名
                string keyName = GetVariableNameRow(table)[GetKeyIndex(table)].ToString();
                byte[] bytes = Encoding.UTF8.GetBytes(keyName);
                fs.Write(BitConverter.GetBytes(bytes.Length), 0, GameConst.INT_SIZE);
                fs.Write(bytes, 0, bytes.Length);
                //3.存储所有数据
                DataRow row;
                DataRow rowType = GetVariableTypeRow(table);
                string[] arrStr;
                for (var i = READ_INDEX; i < table.Rows.Count; i++)
                {
                    row = table.Rows[i];
                    for (var j = 0; j < table.Columns.Count; j++)
                    {
                        switch (rowType[j].ToString())
                        {
                            case "int":
                                fs.Write(BitConverter.GetBytes(int.Parse(row[j].ToString())), 0, GameConst.INT_SIZE);
                                break;
                            case "long":
                                fs.Write(BitConverter.GetBytes(long.Parse(row[j].ToString())), 0, GameConst.LONG_SIZE);
                                break;
                            case "float":
                                fs.Write(BitConverter.GetBytes(float.Parse(row[j].ToString())), 0, GameConst.FLOAT_SIZE);
                                break;
                            case "bool":
                                fs.Write(BitConverter.GetBytes(bool.Parse(row[j].ToString())), 0, GameConst.BOOL_SIZE);
                                break;
                            case "string":
                                bytes = Encoding.UTF8.GetBytes(row[j].ToString());
                                fs.Write(BitConverter.GetBytes(bytes.Length), 0, GameConst.INT_SIZE);
                                fs.Write(bytes, 0, bytes.Length);
                                break;
                            
                            case "int[]":
                                //{1,2,3,4}
                                arrStr = row[j].ToString().TrimStart('{').TrimEnd('}').Split(',');
                                fs.Write(BitConverter.GetBytes(arrStr.Length), 0, GameConst.INT_SIZE);
                                for (int k = 0; k < arrStr.Length; k++)
                                    fs.Write(BitConverter.GetBytes(int.Parse(arrStr[k])), 0, GameConst.INT_SIZE);
                                break;
                            case "long[]":
                                arrStr = row[j].ToString().TrimStart('{').TrimEnd('}').Split(',');
                                fs.Write(BitConverter.GetBytes(arrStr.Length), 0, GameConst.INT_SIZE);
                                for (int k = 0; k < arrStr.Length; k++)
                                    fs.Write(BitConverter.GetBytes(long.Parse(arrStr[k])), 0, GameConst.LONG_SIZE);
                                break;
                            case "float[]":
                                arrStr = row[j].ToString().TrimStart('{').TrimEnd('}').Split(',');
                                fs.Write(BitConverter.GetBytes(arrStr.Length), 0, GameConst.INT_SIZE);
                                for (int k = 0; k < arrStr.Length; k++)
                                    fs.Write(BitConverter.GetBytes(float.Parse(arrStr[k])), 0, GameConst.FLOAT_SIZE);
                                break;
                            case "string[]":
                                arrStr = row[j].ToString().TrimStart('{').TrimEnd('}').Split(',');
                                fs.Write(BitConverter.GetBytes(arrStr.Length), 0, GameConst.INT_SIZE);
                                for (int k = 0; k < arrStr.Length; k++)
                                {
                                    bytes = Encoding.UTF8.GetBytes(arrStr[k]);
                                    fs.Write(BitConverter.GetBytes(bytes.Length), 0, GameConst.INT_SIZE);
                                    fs.Write(bytes, 0, bytes.Length);
                                }
                                break;
                            
                        }
                    }
                }
                
            }
        }

        /// <summary>
        /// 获取变量名所在行
        /// </summary>
        /// <param name="table"></param>
        /// <returns></returns>
        private static DataRow GetVariableNameRow(DataTable table) => table.Rows[0];

        /// <summary>
        /// 获取变量类型所在行
        /// </summary>
        /// <param name="table"></param>
        /// <returns></returns>
        private static DataRow GetVariableTypeRow(DataTable table) => table.Rows[1];

        /// <summary>
        /// 获取key键所在列
        /// </summary>
        /// <param name="table"></param>
        /// <returns></returns>
        private static int GetKeyIndex(DataTable table)
        {
            DataRow row = table.Rows[2];
            for (int i = 0; i < table.Columns.Count; i++)
            {
                if (row[i].ToString() == "key")
                    return i;
            }

            return 0;
        }
    }
}