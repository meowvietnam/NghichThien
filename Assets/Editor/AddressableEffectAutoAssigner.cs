using System;
using System.Reflection;
using UnityEditor;
using UnityEditor.AddressableAssets;
using UnityEditor.AddressableAssets.Settings;
using UnityEngine;

public class AddressableEffectAutoAssigner : EditorWindow
{
    private DefaultAsset soFolder;
    private string targetGroupName = "skilleffect";

    [MenuItem("Tools/Auto Assign Prefab Effect Addressables")]
    public static void ShowWindow()
    {
        GetWindow<AddressableEffectAutoAssigner>("Assign Prefab Effects");
    }

    private void OnGUI()
    {
        GUILayout.Label("Cấu hình Tool Quét Prefab Effect", EditorStyles.boldLabel);
        EditorGUILayout.Space();

        soFolder = (DefaultAsset)EditorGUILayout.ObjectField(
            "Thư mục SO Thần Thông:",
            soFolder,
            typeof(DefaultAsset),
            false
        );

        targetGroupName = EditorGUILayout.TextField("Tên Addressable Group:", targetGroupName);

        EditorGUILayout.Space();

        if (GUILayout.Button("Bắt đầu Quét & Gán Addressable", GUILayout.Height(35)))
        {
            if (soFolder == null)
            {
                EditorUtility.DisplayDialog("Lỗi", "Vui lòng kéo thư mục chứa SO Thần Thông vào!", "OK");
                return;
            }

            ProcessScriptableObjects();
        }
    }

    private void ProcessScriptableObjects()
    {
        string folderPath = AssetDatabase.GetAssetPath(soFolder);
        string[] guids = AssetDatabase.FindAssets("t:ScriptableObject", new[] { folderPath });

        if (guids.Length == 0)
        {
            Debug.LogWarning("Không tìm thấy ScriptableObject nào trong thư mục đã chọn!");
            return;
        }

        AddressableAssetSettings settings = AddressableAssetSettingsDefaultObject.Settings;
        if (settings == null)
        {
            Debug.LogError("Không tìm thấy Addressable Asset Settings trong Project!");
            return;
        }

        AddressableAssetGroup targetGroup = settings.FindGroup(targetGroupName);
        if (targetGroup == null)
        {
            targetGroup = settings.CreateGroup(targetGroupName, false, false, true, settings.DefaultGroup.Schemas);
        }

        int countSuccess = 0;
        int countSkipped = 0;

        foreach (string guid in guids)
        {
            string assetPath = AssetDatabase.GUIDToAssetPath(guid);
            ScriptableObject soObj = AssetDatabase.LoadAssetAtPath<ScriptableObject>(assetPath);

            if (soObj == null) continue;

            SerializedObject serializedSO = new SerializedObject(soObj);

            // 1. Quét tìm ID dựa trên tên hiển thị "Id" trên Inspector
            int skillId = ExtractIdFromInspector(serializedSO);

            // 2. Lấy Prefab GUID
            string prefabGuid = ExtractPrefabGuid(serializedSO);

            if (skillId != -1 && !string.IsNullOrEmpty(prefabGuid))
            {
                AddressableAssetEntry entry = settings.CreateOrMoveEntry(prefabGuid, targetGroup, false, false);
                string newAddressName = $"PrefabEffectSkill_{skillId}";
                entry.address = newAddressName;

                countSuccess++;
                string prefabPath = AssetDatabase.GUIDToAssetPath(prefabGuid);
                Debug.Log($"<color=green>[Thành công]</color> SO: '{soObj.name}' | ID: {skillId} -> Prefab: '{System.IO.Path.GetFileName(prefabPath)}' -> Addressable: <b>{newAddressName}</b>");
            }
            else
            {
                countSkipped++;
                Debug.LogWarning($"[Bỏ qua] SO: '{soObj.name}' | Skill ID: {(skillId == -1 ? "KHÔNG ĐỌC ĐƯỢC" : skillId.ToString())} | Prefab GUID: {(string.IsNullOrEmpty(prefabGuid) ? "KHÔNG TÌM THẤY" : prefabGuid)}");
            }
        }

        settings.SetDirty(AddressableAssetSettings.ModificationEvent.EntryMoved, null, true);
        AssetDatabase.SaveAssets();

        EditorUtility.DisplayDialog("Hoàn tất", $"Đã xử lý xong!\n- Thành công: {countSuccess} Prefab\n- Bỏ qua: {countSkipped} SO", "OK");
    }

    /// Lấy ID dựa theo đúng Nhãn (DisplayName) hiển thị trên Inspector
    private int ExtractIdFromInspector(SerializedObject serializedObject)
    {
        SerializedProperty iterator = serializedObject.GetIterator();
        bool enterChildren = true;

        while (iterator.NextVisible(enterChildren))
        {
            enterChildren = true; // Cho phép chui vào cả struct/class con nếu Id nằm bên trong

            // Nếu nhãn hiển thị đúng là "Id" hoặc "ID" và kiểu dữ liệu là Integer
            if ((iterator.displayName.Equals("Id", StringComparison.OrdinalIgnoreCase) ||
                 iterator.name.Equals("Id", StringComparison.OrdinalIgnoreCase) ||
                 iterator.name.Equals("id", StringComparison.OrdinalIgnoreCase) ||
                 iterator.name.Equals("_id", StringComparison.OrdinalIgnoreCase))
                && iterator.propertyType == SerializedPropertyType.Integer)
            {
                return iterator.intValue;
            }
        }

        // Phương án dự phòng: Nếu không tìm thấy chữ "Id", lấy số int dương đầu tiên xuất hiện ở đầu SO
        iterator = serializedObject.GetIterator();
        while (iterator.NextVisible(true))
        {
            if (iterator.propertyType == SerializedPropertyType.Integer && iterator.intValue > 0)
            {
                // Bỏ qua các trường delay hay time
                if (!iterator.name.ToLower().Contains("time") && !iterator.name.ToLower().Contains("delay"))
                {
                    return iterator.intValue;
                }
            }
        }

        return -1;
    }

    /// Trích xuất GUID của Prefab
    private string ExtractPrefabGuid(SerializedObject serializedObject)
    {
        SerializedProperty prop = serializedObject.GetIterator();
        bool enterChildren = true;

        while (prop.NextVisible(enterChildren))
        {
            enterChildren = true;

            if (prop.propertyType == SerializedPropertyType.ObjectReference && prop.objectReferenceValue != null)
            {
                string path = AssetDatabase.GetAssetPath(prop.objectReferenceValue);
                if (!string.IsNullOrEmpty(path) && path.EndsWith(".prefab"))
                {
                    return AssetDatabase.AssetPathToGUID(path);
                }
            }

            if (prop.hasChildren)
            {
                SerializedProperty pGuid = prop.FindPropertyRelative("m_AssetGUID");
                if (pGuid != null && !string.IsNullOrEmpty(pGuid.stringValue))
                {
                    return pGuid.stringValue;
                }
            }
        }

        return null;
    }
}