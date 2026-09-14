using UnityEngine;
using UnityEditor;
using UnityEditor.AddressableAssets;
using UnityEditor.AddressableAssets.Settings;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Linq;

public class CheckSkillEffectTool : EditorWindow
{
    private Vector2 scrollPos;
    private List<SkillCheckResult> missingEffectList = new List<SkillCheckResult>();
    private bool hasChecked = false;

    private class SkillCheckResult
    {
        public int skillId;
        public string skillName;
        public string className;
        public string expectedAddressKey;
        public UnityEngine.Object assetObject;
    }

    [MenuItem("Tools/Thần Thống/Kiểm Tra Effect Addressable Missing")]
    public static void ShowWindow()
    {
        GetWindow<CheckSkillEffectTool>("Check Skill Effects");
    }

    private void OnGUI()
    {
        GUILayout.Label("Công cụ kiểm tra Addressable Effect của Thần Thống", EditorStyles.boldLabel);
        EditorGUILayout.HelpBox("Tool quét các file ScriptableObject Thần Thống trong project, kiểm tra override PlayVisualSkill và check Addressables.", MessageType.Info);

        if (GUILayout.Button("Kiểm Tra Ngay", GUILayout.Height(35)))
        {
            RunCheck();
        }

        EditorGUILayout.Space();

        if (!hasChecked) return;

        GUILayout.Label($"Kết quả kiểm tra: Tìm thấy {missingEffectList.Count} Thần Thống bị thiếu Effect.", EditorStyles.boldLabel);

        scrollPos = EditorGUILayout.BeginScrollView(scrollPos);
        foreach (var item in missingEffectList)
        {
            EditorGUILayout.BeginVertical("box");
            GUI.color = Color.red;
            EditorGUILayout.LabelField($"[ID: {item.skillId}] {item.skillName}", EditorStyles.boldLabel);
            GUI.color = Color.white;
            EditorGUILayout.LabelField($"Class: {item.className}");
            EditorGUILayout.LabelField($"Thiếu Key: {item.expectedAddressKey}", EditorStyles.miniLabel);

            if (GUILayout.Button("Chọn Asset trong Project", GUILayout.Width(180), GUILayout.Height(20)))
            {
                if (item.assetObject != null)
                {
                    Selection.activeObject = item.assetObject;
                    EditorGUIUtility.PingObject(item.assetObject);
                }
            }
            EditorGUILayout.EndVertical();
            EditorGUILayout.Space(2);
        }
        EditorGUILayout.EndScrollView();
    }

    private void RunCheck()
    {
        missingEffectList.Clear();
        hasChecked = true;

        // 1. Lấy Addressable settings để check key
        AddressableAssetSettings settings = AddressableAssetSettingsDefaultObject.Settings;
        if (settings == null)
        {
            Debug.LogError("Không tìm thấy AddressableAssetSettings trong project!");
            return;
        }

        HashSet<string> allAddressableKeys = new HashSet<string>();
        foreach (var entry in settings.groups.SelectMany(g => g.entries))
        {
            allAddressableKeys.Add(entry.address);
        }

        // 2. Tìm tất cả các file asset có kiểu BaseSkill (hoặc nằm trong folder ThanThong) trong Project
        string[] guids = AssetDatabase.FindAssets("t:BaseSkill");
        foreach (var guid in guids)
        {
            string path = AssetDatabase.GUIDToAssetPath(guid);
            BaseSkill skillAsset = AssetDatabase.LoadAssetAtPath<BaseSkill>(path);

            if (skillAsset == null) continue;

            Type type = skillAsset.GetType();

            // Bỏ qua nếu chính là class BaseSkill (không phải class con)
            if (type == typeof(BaseSkill)) continue;

            // Kiểm tra xem class này có override hàm PlayVisualSkill hay không
            MethodInfo playVisualMethod = type.GetMethod("PlayVisualSkill", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
            if (playVisualMethod != null && playVisualMethod.DeclaringType == type)
            {
                int id = skillAsset.Id;
                string skillName = skillAsset.nameItem;

                // Lấy giá trị addressEffectString nếu có override
                string customAddrString = "";
                PropertyInfo addrProp = type.GetProperty("addressEffectString", BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance);
                if (addrProp != null)
                {
                    try { customAddrString = addrProp.GetValue(skillAsset) as string; } catch { }
                }
                else
                {
                    FieldInfo addrField = type.GetField("addressEffectString", BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance);
                    if (addrField != null)
                    {
                        try { customAddrString = addrField.GetValue(skillAsset) as string; } catch { }
                    }
                }

                string effStr = string.IsNullOrEmpty(customAddrString) ? "PrefabEffectSkill" : customAddrString;
                string finalKey = $"{effStr}_{id}";

                // Kiểm tra xem key có tồn tại trong Addressables hay không
                if (!allAddressableKeys.Contains(finalKey))
                {
                    missingEffectList.Add(new SkillCheckResult
                    {
                        skillId = id,
                        skillName = string.IsNullOrEmpty(skillName) ? type.Name : skillName,
                        className = type.Name,
                        expectedAddressKey = finalKey,
                        assetObject = skillAsset
                    });

                    Debug.LogWarning($"[Missing Effect] Thần Thống: {skillName} (ID: {id}, Class: {type.Name}) override PlayVisualSkill nhưng thiếu Addressable Key: '{finalKey}'", skillAsset);
                }
            }
        }

        Debug.Log($"Đã quét xong! Tổng số Thần Thống thiếu effect: {missingEffectList.Count}");
    }
}