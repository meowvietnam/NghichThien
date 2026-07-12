using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEditor.AddressableAssets;
using UnityEditor.AddressableAssets.Settings;
using UnityEditor.AddressableAssets.Settings.GroupSchemas;
using UnityEngine;

public class AddressableSkillSetup : EditorWindow
{
    [MenuItem("Tools/Setup Skill Addressables")]
    public static void Setup()
    {
        var settings = AddressableAssetSettingsDefaultObject.Settings;

        // Quét thư mục CongPhap và tất cả folder con của nó
        ProcessFolderRecursive("Assets/SkillResource/Data/CongPhap", "congphap", settings);

        // Quét thư mục ThanThong và tất cả folder con của nó
        ProcessFolderRecursive("Assets/SkillResource/Data/ThanThong", "thanthong", settings);

        AssetDatabase.SaveAssets();
        Debug.Log("Đã setup xong Addressables cho toàn bộ thư mục con!");
    }

    static void ProcessFolderRecursive(string rootPath, string groupName, AddressableAssetSettings settings)
    {
        if (!Directory.Exists(rootPath)) return;

        // Lấy tất cả file .asset (ScriptableObject) trong thư mục gốc và mọi thư mục con
        string[] allGuids = AssetDatabase.FindAssets("t:ScriptableObject", new[] { rootPath });

        AddressableAssetGroup group = settings.FindGroup(groupName);
        if (group == null)
            group = settings.CreateGroup(groupName, false, false, true, null, typeof(BundledAssetGroupSchema));

        foreach (string guid in allGuids)
        {
            string path = AssetDatabase.GUIDToAssetPath(guid);

            // Đảm bảo chỉ xử lý file nằm trong cây thư mục mong muốn
            if (path.StartsWith(rootPath))
            {
                var asset = AssetDatabase.LoadAssetAtPath<BaseSkill>(path);

                if (asset != null)
                {
                    var entry = settings.CreateOrMoveEntry(guid, group);

                    // Format: congphap_101, thanthong_101
                    entry.address = $"{groupName}_{asset.Id}";
                }
            }
        }
    }
}