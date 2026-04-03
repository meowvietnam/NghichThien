using System;
using Sirenix.OdinInspector.Editor;
using UnityEditor;
using UnityEngine;

namespace Editor
{
    public class TimeSpanDrawer : OdinValueDrawer<TimeSpan>
    {
        protected override void DrawPropertyLayout(GUIContent label)
        {
            var value = ValueEntry.SmartValue;

            int hours = value.Hours + value.Days * 24;
            int minutes = value.Minutes;
            int seconds = value.Seconds;

            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.PrefixLabel(label);
            EditorGUI.BeginChangeCheck();
            int newHours = EditorGUILayout.IntField(hours, GUILayout.Width(50));
            GUILayout.Label(":", GUILayout.Width(10));
            int newMinutes = EditorGUILayout.IntField(minutes, GUILayout.Width(30));
            GUILayout.Label(":", GUILayout.Width(10));
            int newSeconds = EditorGUILayout.IntField(seconds, GUILayout.Width(30));
            if (EditorGUI.EndChangeCheck())
            {
                newMinutes = Mathf.Clamp(newMinutes, 0, 59);
                newSeconds = Mathf.Clamp(newSeconds, 0, 59);
                ValueEntry.SmartValue = new TimeSpan(newHours, newMinutes, newSeconds);
            }
            
            EditorGUILayout.EndHorizontal();
        }
    }
}