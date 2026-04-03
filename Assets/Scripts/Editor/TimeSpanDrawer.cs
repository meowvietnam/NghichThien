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
            int totalDays = value.Days;
            int years = totalDays / 365;
            int days = totalDays % 365;
            int hours = value.Hours;
            int minutes = value.Minutes;
            int seconds = value.Seconds;
            string display = $"{years}y {days}d {hours}h {minutes}m {seconds}s";

            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.PrefixLabel(label);
            EditorGUILayout.SelectableLabel(display, GUILayout.Height(EditorGUIUtility.singleLineHeight));
            EditorGUILayout.EndHorizontal();
        }
    }
}