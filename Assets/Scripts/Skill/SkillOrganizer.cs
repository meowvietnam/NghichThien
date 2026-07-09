#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using System;
using System.IO;
using System.Linq;
using Skill;

public class SkillOrganizer : EditorWindow
{
    [MenuItem("Tools/Organize Skills")]
    public static void Organize()
    {
        // Đường dẫn folder gốc chứa các script (theo ảnh của bạn)
        string sourcePath = "Assets/Scripts/Skill";
        string pathThanThong = sourcePath + "/ScriptThanThong";
        string pathCongPhap = sourcePath + "/ScriptCongPhap";

        // Tạo folder nếu chưa có
        if (!AssetDatabase.IsValidFolder(pathThanThong)) AssetDatabase.CreateFolder(sourcePath, "ScriptThanThong");
        if (!AssetDatabase.IsValidFolder(pathCongPhap)) AssetDatabase.CreateFolder(sourcePath, "ScriptCongPhap");

        // Lấy tất cả các file .cs trong folder Skill (không quét subfolders để tránh bị lặp)
        string[] guids = AssetDatabase.FindAssets("t:Script", new[] { sourcePath });

        int count = 0;
        foreach (string guid in guids)
        {
            string assetPath = AssetDatabase.GUIDToAssetPath(guid);
            MonoScript script = AssetDatabase.LoadAssetAtPath<MonoScript>(assetPath);

            if (script != null)
            {
                Type scriptType = script.GetClass();
                if (scriptType == null) continue;

                string destination = "";

                // Kiểm tra kế thừa
                if (typeof(ThanThong).IsAssignableFrom(scriptType) && scriptType != typeof(ThanThong))
                {
                    destination = pathThanThong + "/" + Path.GetFileName(assetPath);
                }
                else if (typeof(CongPhap).IsAssignableFrom(scriptType) && scriptType != typeof(CongPhap))
                {
                    destination = pathCongPhap + "/" + Path.GetFileName(assetPath);
                }

                // Thực hiện di chuyển
                if (!string.IsNullOrEmpty(destination) && assetPath != destination)
                {
                    string error = AssetDatabase.MoveAsset(assetPath, destination);
                    if (string.IsNullOrEmpty(error)) count++;
                    else Debug.LogError($"Lỗi khi di chuyển {script.name}: {error}");
                }
            }
        }

        AssetDatabase.Refresh();
        Debug.Log($"Successfully organized {count} skill scripts!");
    }
}
#endif