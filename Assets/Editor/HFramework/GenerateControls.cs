using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;
using HFramework;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
using Debug = UnityEngine.Debug;

namespace Editor.HFramework
{
    public class GenerateControlWindow : EditorWindow
    {
        private const string CONTROL_PATH = "Resources/UI/Controls";
        private const string GENERATE_PATH = "Scripts/Generate/UIControls";
        private const string CONTROL_KEY = "_";
        private static string _controlPath = CONTROL_PATH;
        private static string _generatePath = GENERATE_PATH;
        private static string _controlKey = CONTROL_KEY;
        private static string _cPath;
        private static string _gPath;
        private static string _cKey;

        public static string ControlPath => _controlPath;
        public static string GeneratePath => _generatePath;
        public static string ControlKey => _controlKey;
        
        [MenuItem("Tools/UI/生成控件设置")]
        public static void ShowWindow()
        {
            GetWindow<GenerateControlWindow>("GenerateControlOption");
            _cPath = _controlPath;
            _gPath = _generatePath;
            _cKey = _controlKey;
        }

        private void OnGUI()
        {
            EditorGUILayout.LabelField("控件预制体路径：");
            _cPath = EditorGUILayout.TextField(_cPath);
            GUILayout.Space(10);
            EditorGUILayout.LabelField("生成代码路径：");
            _gPath = EditorGUILayout.TextField(_gPath);
            GUILayout.Space(10);
            EditorGUILayout.LabelField("控件关键字：（名字拥有关键字的控件才会生成相应成员变量）");
            _cKey = EditorGUILayout.TextField(_cKey);
            GUILayout.FlexibleSpace();
            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("重置默认路径"))
            {
                GUIUtility.keyboardControl = 0;
                _controlPath = _cPath = CONTROL_PATH;
                _generatePath = _gPath = GENERATE_PATH;
                _controlKey = _cKey = CONTROL_KEY;
            }
            if (GUILayout.Button("确认"))
            {
                _controlPath = _cPath;
                _generatePath = _gPath;
                _controlKey = _cKey;
                Debug.Log("生成控件路径重定向成功。。。");
                Debug.Log($"控件资源路径：{_controlPath}");
                Debug.Log($"生成代码路径：{_generatePath}");
                Debug.Log($"控件关键字：{_controlKey}");
                Close();
            }
            EditorGUILayout.EndHorizontal();
        }
    }
    
    public static class GenerateControls
    {
        private class PrefabInfo
        {
            public string name;
            public Dictionary<string, Type> members = new Dictionary<string, Type>();
            public PrefabInfo(string name) => this.name = name;
        }

        [MenuItem("Tools/UI/生成控件")]
        public static void Execute()
        {
            Debug.Log($"开始生成控件类。。。");
            Stopwatch sw = Stopwatch.StartNew();
            var infos = GetAllPrefabInfo();
            var dir = Directory.CreateDirectory($"{Application.dataPath}/{GenerateControlWindow.ControlPath}");
            //先删除旧的
            foreach (var file in dir.GetFiles())
                file.Delete();
            foreach (var info in infos)
                GenerateControlClass(info);
            AssetDatabase.Refresh();
            Debug.Log($"生成控件类结束，耗时 {sw.ElapsedMilliseconds} ms。。。");
            sw.Stop();
        }

        private static void GenerateControlClass(PrefabInfo info)
        {
            StringBuilder content = new StringBuilder();
            content.Append("using Hali_Framework;\n");
            content.Append("using UnityEngine.UI;\n");
            content.Append("namespace Game.UI.Controls\n");
            content.Append("{\n");
            content.Append($"\tpublic partial class UI_{info.name} : ControlBase\n");
            content.Append("\t{\n");
            foreach (var member in info.members)
            {
                content.Append($"\t\tprivate {member.Value.Name} {member.Key};\n");
            }
            content.Append("\n");
            content.Append("\t\tprotected override void BindControls()\n");
            content.Append("\t\t{\n");
            content.Append("\t\t\tbase.BindControls();\n");
            foreach (var member in info.members)
            {
                content.Append($"\t\t\t{member.Key} = GetControl<{member.Value.Name}>(\"{member.Key}\");\n");
            }
            content.Append("\t\t}\n");
            content.Append("\t}\n");
            content.Append("}");
            
            File.WriteAllText($"{Application.dataPath}/{GenerateControlWindow.GeneratePath}/UI_{info.name}.cs", content.ToString());
        }
        
        private static List<PrefabInfo> GetAllPrefabInfo()
        {
            List<PrefabInfo> prefabInfos = new List<PrefabInfo>();
            string[] files = Directory.GetFiles($"{Application.dataPath}/{GenerateControlWindow.ControlPath}", "*.prefab");
            for (int i = 0; i < files.Length; i++)
            {
                //去除Asset前的路径
                int index = files[i].IndexOf("Asset", StringComparison.Ordinal);
                files[i] = files[i].Substring(index, files[i].Length - index);
                //创建info并添加
                GameObject prefab = AssetDatabase.LoadAssetAtPath<GameObject>(files[i]);
                var info = new PrefabInfo(prefab.name);
                info.members = new Dictionary<string, Type>();
                DeepFindMember(prefab.transform, ref info.members);
                prefabInfos.Add(info);
            }
            return prefabInfos;
        }

        private static void DeepFindMember(Transform parent, ref Dictionary<string, Type> members)
        {
            int count = parent.childCount;
            if(count <= 0) return;
            for (int i = 0; i < count; i++)
            {
                var child = parent.GetChild(i);
                if (!child.name.Contains(GenerateControlWindow.ControlKey))//只加入含指定关键字成员
                {
                    DeepFindMember(child, ref members);
                    continue;
                }
                
                bool stopRecursion = false;
                //加入成员
                //规则：挂载复数组件的成员，只能拥有一个组件的成员，获取if优先级最高的组件
                //优先级：自定义组件 -> 复杂组件 -> 基础组件
                //自定义组件
                if (child.TryGetComponent<ControlBase>(out var cb))
                {
                    members.Add(child.name, cb.GetType());
                    stopRecursion = true;
                }
                //复杂组件
                else if(child.TryGetComponent<ScrollRect>(out var sr))
                    members.Add(child.name, typeof(ScrollRect));
                else if(child.TryGetComponent<Slider>(out var sld))
                    members.Add(child.name, typeof(Slider));
                else if(child.TryGetComponent<Toggle>(out var tog))
                    members.Add(child.name, typeof(Toggle));
                else if(child.TryGetComponent<Button>(out var btn))
                    members.Add(child.name, typeof(Button));
                else if(child.TryGetComponent<InputField>(out var ifd))
                    members.Add(child.name, typeof(InputField));
                //基础组件
                else if (child.TryGetComponent<Text>(out var txt))
                    members.Add(child.name, typeof(Text));
                else if(child.TryGetComponent<Image>(out var img))
                    members.Add(child.name, typeof(Image));
                else if(child.TryGetComponent<RawImage>(out var rawImg))
                    members.Add(child.name, typeof(RawImage));
                
                if(!stopRecursion)
                    DeepFindMember(child, ref members);
            }
        }
    }
}
