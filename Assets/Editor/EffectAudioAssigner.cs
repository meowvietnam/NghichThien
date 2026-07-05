using UnityEngine;
using UnityEditor;
using System.IO;
using System.Collections.Generic;
using System.Text.RegularExpressions;

public class EffectAudioAssigner : EditorWindow
{
    [MenuItem("Tools/Assign Audio to Skill Prefabs")]
    public static void ExecuteAssignAudio()
    {
        // 1. Lấy tất cả các GUID của các thư mục đang được chọn ở cửa sổ Project
        string[] selectedGuids = Selection.assetGUIDs;
        if (selectedGuids.Length == 0)
        {
            Debug.LogWarning("Vui lòng chọn các thư mục chứa Prefab (như EffectHeHoa, EffectHeKhi...) trong cửa sổ Project trước!");
            return;
        }

        List<string> searchPaths = new List<string>();
        foreach (var guid in selectedGuids)
        {
            string path = AssetDatabase.GUIDToAssetPath(guid);
            if (AssetDatabase.IsValidFolder(path))
            {
                searchPaths.Add(path);
            }
        }

        if (searchPaths.Count == 0)
        {
            Debug.LogWarning("Vui lòng chọn chính xác THƯ MỤC chứa Prefab!");
            return;
        }

        // 2. Tìm kiếm tất cả file Prefab (.prefab) trong các thư mục đã chọn
        string[] prefabGuids = AssetDatabase.FindAssets("t:Prefab", searchPaths.ToArray());

        List<string> assignedPrefabs = new List<string>();
        List<string> unassignedPrefabs = new List<string>();

        int totalUpdated = 0;

        foreach (string prefabGuid in prefabGuids)
        {
            string prefabPath = AssetDatabase.GUIDToAssetPath(prefabGuid);

            GameObject prefabRoot = AssetDatabase.LoadAssetAtPath<GameObject>(prefabPath);
            if (prefabRoot == null) continue;

            string prefabName = prefabRoot.name;

            // Kiểm tra xem Prefab có child đầu tiên không
            if (prefabRoot.transform.childCount == 0)
            {
                unassignedPrefabs.Add($"{prefabName} (Không có Child ở index 0)");
                continue;
            }

            // Lấy Child đầu tiên (Index 0)
            Transform firstChild = prefabRoot.transform.GetChild(0);
            string childName = firstChild.name;

            // Lấy hoặc thêm AudioSource
            AudioSource audioSource = firstChild.GetComponent<AudioSource>();
            if (audioSource == null)
            {
                audioSource = firstChild.gameObject.AddComponent<AudioSource>();
            }

            AudioClip targetClip = null;
            string cleanedChildName = "";

            // TRƯỜNG HỢP 1: Logic "Chặt đuôi" từ ký tự đặc biệt đầu tiên trở đi
            // Tìm vị trí xuất hiện đầu tiên của bất kỳ ký tự nào KHÔNG PHẢI chữ hoặc số (bao gồm _, ., -, v.v.)
            Match specialCharMatch = Regex.Match(childName, @"[^a-zA-Z0-9]");

            if (specialCharMatch.Success)
            {
                // Cắt bỏ hoàn toàn từ vị trí ký tự đặc biệt đó trở về sau
                cleanedChildName = childName.Substring(0, specialCharMatch.Index);
            }
            else
            {
                // Nếu chuỗi không có ký tự đặc biệt nào thì giữ nguyên để xử lý tiếp
                cleanedChildName = childName;
            }

            // Sau khi chặt đuôi, lọc bỏ nốt các ký tự số còn sót lại ở phần phía trước, chỉ giữ lại CHỮ CÁI
            cleanedChildName = Regex.Replace(cleanedChildName, @"[^a-zA-Z]", "");

            // Tiến hành tìm kiếm Audio Clip dựa trên tên đã lọc sạch
            if (!string.IsNullOrEmpty(cleanedChildName))
            {
                targetClip = FindAudioClipByName(cleanedChildName);
            }

            // TRƯỜNG HỢP 2: Nếu TH1 không tìm thấy, tìm bằng chính tên của Prefab (Ví dụ: 408 hay 317)
            if (targetClip == null)
            {
                targetClip = FindAudioClipByName(prefabName);
            }

            // 4. Gán Clip vào AudioSource nếu tìm thấy
            if (targetClip != null)
            {
                Undo.RecordObject(audioSource, "Assign Audio Clip");

                audioSource.clip = targetClip;
                audioSource.playOnAwake = true;

                PrefabUtility.SavePrefabAsset(prefabRoot);

                assignedPrefabs.Add($"{prefabName} -> Gán thành công clip: [{targetClip.name}] (Từ khóa tìm kiếm: '{cleanedChildName}')");
                totalUpdated++;
            }
            else
            {
                string debugKeyword = string.IsNullOrEmpty(cleanedChildName) ? "Trống" : cleanedChildName;
                unassignedPrefabs.Add($"{prefabName} (Không tìm thấy Audio Clip trùng với từ khóa '{debugKeyword}' hoặc tên prefab '{prefabName}')");
            }
        }

        // 5. Xuất báo cáo Debug.Log ra Console
        PrintReport(assignedPrefabs, unassignedPrefabs);

        AssetDatabase.Refresh();

        EditorUtility.DisplayDialog("Hoàn thành!",
            $"Đã quét xong.\n- Gán thành công: {totalUpdated} prefabs.\n- Thất bại: {unassignedPrefabs.Count} prefabs.\nXem chi tiết ở bảng Console.", "OK");
    }

    // Hàm tìm kiếm AudioClip theo tên chính xác trên toàn bộ Project
    private static AudioClip FindAudioClipByName(string clipName)
    {
        string[] audioGuids = AssetDatabase.FindAssets($"{clipName} t:AudioClip");
        foreach (string audioGuid in audioGuids)
        {
            string path = AssetDatabase.GUIDToAssetPath(audioGuid);
            string fileNameWithoutExtension = Path.GetFileNameWithoutExtension(path);

            if (fileNameWithoutExtension.Equals(clipName, System.StringComparison.OrdinalIgnoreCase))
            {
                return AssetDatabase.LoadAssetAtPath<AudioClip>(path);
            }
        }
        return null;
    }

    private static void PrintReport(List<string> assigned, List<string> unassigned)
    {
        System.Text.StringBuilder report = new System.Text.StringBuilder();

        report.AppendLine("==================================================");
        report.AppendLine($"🔥 BÁO CÁO GÁN AUDIO (LOGIC CẮT ĐUÔI KÝ TỰ ĐẶC BIỆT)");
        report.AppendLine("==================================================");

        report.AppendLine($"\n✅ ĐÃ GÁN THÀNH CÔNG ({assigned.Count} prefabs):");
        if (assigned.Count == 0) report.AppendLine("- (Không có)");
        else
        {
            foreach (var item in assigned)
            {
                report.AppendLine($" * {item}");
            }
        }

        report.AppendLine($"\n❌ CHƯA ĐƯỢC GÁN / THẤT BẠI ({unassigned.Count} prefabs):");
        if (unassigned.Count == 0) report.AppendLine("- (Không có)");
        else
        {
            foreach (var item in unassigned)
            {
                report.AppendLine($" * {item}");
            }
        }
        report.AppendLine("==================================================");

        Debug.Log(report.ToString());
    }
}