using System.Collections.Generic;
using HFramework;
using UnityEditor;
using UnityEngine;

namespace Editor.HFramework.Audio
{
    [CustomEditor(typeof(SoundMaster))]
    public class SoundMasterInspector : UnityEditor.Editor
    {
        private List<bool> bFoldOut = new List<bool>();

        public override void OnInspectorGUI()
        {
            var sm = (SoundMaster)target;
            if (bFoldOut.Count != sm.AudioList.Count)
            {
                bFoldOut.Clear();
                for (int i = 0; i < sm.AudioList.Count; i++)
                    bFoldOut.Add(false);
            }
            EditorGUILayout.LabelField($"总频道数：{sm.AudioList.Count}    空闲频道数：{HEntry.AudioMgr.GetEmptyChannelCount()}");
            for (int i = 0; i < sm.AudioList.Count; i++)
            {
                string title = $"频道{i + 1}";
                if (sm.AudioList[i].isPlaying)
                    title += "  (占用中)";
                bFoldOut[i] = EditorGUILayout.BeginFoldoutHeaderGroup(bFoldOut[i], title);
                if (bFoldOut[i])
                {
                    EditorGUI.indentLevel++;
                    EditorGUILayout.LabelField($"音效文件: {sm.AudioList[i].clip}");
                    GUILayout.Space(10);
                    EditorGUILayout.LabelField($"音效大小: {sm.AudioList[i].volume}");
                    string str = !sm.AudioList[i].mute ? "开" : "关";
                    EditorGUILayout.LabelField($"音效开关: {str}");
                    EditorGUI.indentLevel--;
                }
                EditorGUILayout.EndFoldoutHeaderGroup();
            }
        }
    }
}