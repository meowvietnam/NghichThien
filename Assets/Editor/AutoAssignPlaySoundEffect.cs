using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

public class AutoAssignPlaySoundEffect : EditorWindow
{
    [MenuItem("Tools/Assign PlaySoundEffect to Prefabs")]
    public static void ExecuteAssignScript()
    {
        // 1. Lấy danh sách thư mục đang được chọn ở cửa sổ Project
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
            Debug.LogWarning("Vui lòng bôi đen chọn chính xác các THƯ MỤC chứa Prefab!");
            return;
        }

        // 2. Tìm tất cả file .prefab trong các thư mục đã chọn
        string[] prefabGuids = AssetDatabase.FindAssets("t:Prefab", searchPaths.ToArray());

        List<string> assignedPrefabs = new List<string>();
        List<string> skippedPrefabs = new List<string>();

        int totalUpdated = 0;

        foreach (string prefabGuid in prefabGuids)
        {
            string prefabPath = AssetDatabase.GUIDToAssetPath(prefabGuid);
            GameObject prefabRoot = AssetDatabase.LoadAssetAtPath<GameObject>(prefabPath);
            if (prefabRoot == null) continue;

            string prefabName = prefabRoot.name;

            // Kiểm tra xem Prefab có child nào không
            if (prefabRoot.transform.childCount == 0)
            {
                skippedPrefabs.Add($"{prefabName} (Không có Child nào)");
                continue;
            }

            // Lấy Child đầu tiên (Index 0) - Nơi bạn đã gán AudioSource từ tool trước
            Transform firstChild = prefabRoot.transform.GetChild(0);

            // Tìm component AudioSource ở Child(0)
            AudioSource audioSource = firstChild.GetComponent<AudioSource>();

            if (audioSource != null)
            {
                // Kiểm tra xem đã có script PlaySoundEffect chưa, nếu chưa thì mới thêm để tránh trùng lặp
                PlaySoundEffect playSoundScript = firstChild.GetComponent<PlaySoundEffect>();

                if (playSoundScript == null)
                {
                    // Thêm Component mới vào Child(0)
                    playSoundScript = firstChild.gameObject.AddComponent<PlaySoundEffect>();

                    // Đồng bộ: Đảm bảo playOnAwake của AudioSource chuyển thành false để nhường quyền cho script xử lý tốt hơn
                    Undo.RecordObject(audioSource, "Disable Play On Awake");
                    audioSource.playOnAwake = false;

                    // Lưu thay đổi trực tiếp vào file Prefab cứng
                    PrefabUtility.SavePrefabAsset(prefabRoot);

                    assignedPrefabs.Add($"{prefabName} -> Đã gắn thành công [PlaySoundEffect] vào Child(0)");
                    totalUpdated++;
                }
                else
                {
                    skippedPrefabs.Add($"{prefabName} (Đã có sẵn script PlaySoundEffect từ trước)");
                }
            }
            else
            {
                skippedPrefabs.Add($"{prefabName} (Child(0) không chứa AudioSource)");
            }
        }

        // 3. In báo cáo chi tiết ra Console
        PrintReport(assignedPrefabs, skippedPrefabs);

        AssetDatabase.Refresh();

        EditorUtility.DisplayDialog("Hoàn thành!",
            $"Đã quét xong.\n- Gắn mới thành công: {totalUpdated} prefabs.\n- Bỏ qua / Không đổi: {skippedPrefabs.Count} prefabs.\nXem chi tiết trong bảng Console.", "OK");
    }

    private static void PrintReport(List<string> assigned, List<string> skipped)
    {
        System.Text.StringBuilder report = new System.Text.StringBuilder();

        report.AppendLine("==================================================");
        report.AppendLine($"🛠️ BÁO CÁO GẮN SCRIPT [PLAYSOUNDEFFECT] CHO PREFABS");
        report.AppendLine("==================================================");

        report.AppendLine($"\n✅ ĐÃ GẮN MỚI THÀNH CÔNG ({assigned.Count} prefabs):");
        if (assigned.Count == 0) report.AppendLine("- (Không có)");
        else
        {
            foreach (var item in assigned) report.AppendLine($" * {item}");
        }

        report.AppendLine($"\n⏭️ BỎ QUA / KHÔNG ĐỦ ĐIỀU KIỆN ({skipped.Count} prefabs):");
        if (skipped.Count == 0) report.AppendLine("- (Không có)");
        else
        {
            foreach (var item in skipped) report.AppendLine($" * {item}");
        }
        report.AppendLine("==================================================");

        Debug.Log(report.ToString());
    }
}