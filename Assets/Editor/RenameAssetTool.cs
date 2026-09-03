using UnityEngine;
using UnityEditor;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Globalization;

public class RenameAssetTool : EditorWindow
{
    [MenuItem("Tools/Chuan Hoa Ten Asset Khong Dau")]
    public static void ShowWindow()
    {
        GetWindow<RenameAssetTool>("Đổi Tên Asset Không Dấu");
    }

    private void OnGUI()
    {
        GUILayout.Label("Công cụ đổi tên ScriptableObject không dấu", EditorStyles.boldLabel);
        EditorGUILayout.Space();

        if (GUILayout.Button("Chạy đổi tên thư mục CongPhap & ThanThong", GUILayout.Height(40)))
        {
            ProcessFolder("Assets/SkillResource/Data/CongPhap");
            ProcessFolder("Assets/SkillResource/Data/ThanThong");
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            Debug.Log("<color=green>Đã đổi tên và chuẩn hóa toàn bộ asset thành công!</color>");
        }
    }

    private static void ProcessFolder(string folderPath)
    {
        if (!Directory.Exists(folderPath))
        {
            Debug.LogWarning("Không tìm thấy đường dẫn: " + folderPath);
            return;
        }

        string[] guids = AssetDatabase.FindAssets("t:ScriptableObject", new[] { folderPath });
        int count = 0;

        foreach (string guid in guids)
        {
            string assetPath = AssetDatabase.GUIDToAssetPath(guid);
            string fileName = Path.GetFileNameWithoutExtension(assetPath);

            string newFileName = RemoveDiacritics(fileName);
            // Thay thế khoảng trắng và ký tự đặc biệt thành dấu gạch dưới
            newFileName = Regex.Replace(newFileName, @"[^a-zA-Z0-9_]", "_");
            // Thu gọn các dấu gạch dưới liên tiếp
            newFileName = Regex.Replace(newFileName, @"_+", "_").Trim('_');

            if (fileName != newFileName)
            {
                string result = AssetDatabase.RenameAsset(assetPath, newFileName);
                if (string.IsNullOrEmpty(result))
                {
                    count++;
                    Debug.Log($"Đổi tên thành công: {fileName} -> {newFileName}");
                }
                else
                {
                    Debug.LogError($"Lỗi đổi tên file {fileName}: {result}");
                }
            }
        }
        Debug.Log($"Đã xử lý xong thư mục {folderPath}. Tổng số file đổi tên: {count}");
    }

    private static string RemoveDiacritics(string text)
    {
        if (string.IsNullOrEmpty(text)) return text;

        // Xử lý riêng các ký tự tiếng Việt hay bị sót
        text = text.Replace("Đ", "D").Replace("đ", "d");

        NormalizationForm normForm = NormalizationForm.FormD;
        string normalizedString = text.Normalize(normForm);
        StringBuilder sb = new StringBuilder();

        foreach (char c in normalizedString)
        {
            UnicodeCategory unicodeCategory = CharUnicodeInfo.GetUnicodeCategory(c);
            if (unicodeCategory != UnicodeCategory.NonSpacingMark)
            {
                sb.Append(c);
            }
        }

        return sb.ToString().Normalize(NormalizationForm.FormC);
    }
}