#if UNITY_EDITOR
using AllMaps;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Clan.Editor
{
    public static class ClanStrongholdPrefabSetup
    {
        private const string PrefabPath = "Assets/Scripts/Clan/Prefabs/ClanStronghold.prefab";
        private const string ScenePath = "Assets/Scripts/AllMaps/AllMaps.unity";

        [MenuItem("Tools/Clan/Configure Clan Stronghold")]
        public static void Configure()
        {
            ConfigurePrefab();
            ConfigureMapManager();
            AssetDatabase.SaveAssets();
            Debug.Log("ClanStronghold prefab và MapManager đã được cấu hình.");
        }

        private static void ConfigurePrefab()
        {
            var root = PrefabUtility.LoadPrefabContents(PrefabPath);
            try
            {
                var oldStronghold = root.GetComponent<Stronghold>();
                if (oldStronghold != null) Object.DestroyImmediate(oldStronghold, true);

                var stronghold = root.GetComponent<ClanStronghold>();
                if (stronghold == null) stronghold = root.AddComponent<ClanStronghold>();

                var serialized = new SerializedObject(stronghold);
                serialized.FindProperty("<MapType>k__BackingField").enumValueIndex = (int)MapType.Stronghold;
                serialized.FindProperty("iconVisualAction").objectReferenceValue = null;
                serialized.FindProperty("clanNameText").objectReferenceValue = FindNamed<Text>(root, "Text");
                serialized.FindProperty("enter").objectReferenceValue = FindNamed<Transform>(root, "enter")?.gameObject;
                serialized.FindProperty("enterButton").objectReferenceValue = FindNamed<Button>(root, "Button");
                serialized.ApplyModifiedPropertiesWithoutUndo();
                PrefabUtility.SaveAsPrefabAsset(root, PrefabPath);
            }
            finally
            {
                PrefabUtility.UnloadPrefabContents(root);
            }
        }

        private static void ConfigureMapManager()
        {
            var previousScenePath = SceneManager.GetActiveScene().path;
            var scene = EditorSceneManager.OpenScene(ScenePath, OpenSceneMode.Single);
            var manager = Object.FindFirstObjectByType<MapManager>();
            var prefab = AssetDatabase.LoadAssetAtPath<GameObject>(PrefabPath)?.GetComponent<ClanStronghold>();
            if (manager == null || prefab == null)
                throw new System.InvalidOperationException("Không tìm thấy MapManager hoặc ClanStronghold prefab.");

            var serialized = new SerializedObject(manager);
            serialized.FindProperty("clanStrongholdPrefab").objectReferenceValue = prefab;
            serialized.ApplyModifiedPropertiesWithoutUndo();
            EditorUtility.SetDirty(manager);
            EditorSceneManager.MarkSceneDirty(scene);
            EditorSceneManager.SaveScene(scene);

            if (!string.IsNullOrEmpty(previousScenePath) && previousScenePath != ScenePath)
                EditorSceneManager.OpenScene(previousScenePath, OpenSceneMode.Single);
        }

        private static T FindNamed<T>(GameObject root, string objectName) where T : Component
        {
            foreach (var component in root.GetComponentsInChildren<T>(true))
                if (component.name == objectName) return component;
            return null;
        }
    }
}
#endif
