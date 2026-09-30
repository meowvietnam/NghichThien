using System.Linq;
using AllMaps;
using AllMaps.TongMonChien;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

/// <summary>
/// Thiết lập scene Tông môn chiến (bản sao AllMaps): bật chế độ chiến cho MapManager, gắn controller,
/// thêm scene vào Build Settings. Chạy lại nhiều lần vẫn an toàn.
/// </summary>
public static class TongMonChienSceneSetup
{
    private const string ScenePath = "Assets/Scripts/AllMaps/TongMonChien.unity";

    [MenuItem("Tools/Tông môn chiến/Thiết lập scene")]
    public static void Setup()
    {
        if (!EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo()) return;
        AddToBuildSettings();
        var scene = EditorSceneManager.OpenScene(ScenePath, OpenSceneMode.Single);
        var mapManager = Object.FindFirstObjectByType<MapManager>(FindObjectsInactive.Include);
        if (mapManager == null)
        {
            Debug.LogError("Không tìm thấy MapManager trong scene TongMonChien");
            return;
        }

        var so = new SerializedObject(mapManager);
        so.FindProperty("isWarMode").boolValue = true;
        so.ApplyModifiedPropertiesWithoutUndo();

        var controller = Object.FindFirstObjectByType<TongMonChienController>(FindObjectsInactive.Include);
        if (controller == null)
        {
            var go = new GameObject("TongMonChienController");
            controller = go.AddComponent<TongMonChienController>();
        }

        var cso = new SerializedObject(controller);
        cso.FindProperty("mapManager").objectReferenceValue = mapManager;
        // Prefab HUD: dùng bản có sẵn (giữ chỉnh sửa), chưa có thì dựng bản mặc định.
        var hud = AssetDatabase.LoadAssetAtPath<TongMonChienHud>(TongMonChienController.HudPrefabPath)
                  ?? TongMonChienHudPrefabBuilder.Build();
        cso.FindProperty("hudPrefab").objectReferenceValue = hud;
        cso.ApplyModifiedPropertiesWithoutUndo();
        EditorSceneManager.MarkSceneDirty(scene);
        EditorSceneManager.SaveScene(scene);

        // Liệt kê các hệ thống của bản đồ thường còn trong scene để người dựng scene quyết định tắt/xoá.
        var suspects = Object.FindObjectsByType<MonoBehaviour>(FindObjectsInactive.Include, FindObjectsSortMode.None)
            .Where(x => x != null)
            .Select(x => x.GetType().Name)
            .Where(n => n.Contains("Quest") || n.Contains("ActionEnterMap") || n.Contains("NPC") || n.Contains("Boss"))
            .Distinct()
            .ToArray();
        Debug.Log("Đã thiết lập scene Tông môn chiến. Xem xét tắt các hệ thống bản đồ thường còn trong scene: " +
                  string.Join(", ", suspects));
    }

    private static void AddToBuildSettings()
    {
        var scenes = EditorBuildSettings.scenes.ToList();
        if (scenes.Any(x => x.path == ScenePath)) return;
        scenes.Add(new EditorBuildSettingsScene(ScenePath, true));
        EditorBuildSettings.scenes = scenes.ToArray();
    }
}
