using System;
using Sirenix.OdinInspector.Editor;
using UnityEditor;
using UnityEngine;

namespace Editor
{
    public class DateTimeDrawer : OdinValueDrawer<DateTime>
    {
        protected override void DrawPropertyLayout(GUIContent label)
        {
            var value = ValueEntry.SmartValue;
            string local = value.ToLocalTime().ToString("dd-MM-yyyy HH:mm:ss");
            EditorGUILayout.LabelField(label, new GUIContent(local));
        }
    }
}