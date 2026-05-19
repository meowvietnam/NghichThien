using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class AssetOptimize
{
    public static void Move(Object asset, string newFolder)
    {
        var oldPath = AssetDatabase.GetAssetPath(asset);
        var fileName = System.IO.Path.GetFileName(oldPath);
        var newPath = $"{newFolder}/{fileName}";
        var error = AssetDatabase.MoveAsset(oldPath, newPath);
        if (!string.IsNullOrEmpty(error))
        {
            Debug.LogError(error);
        }
    }

    public static List<AnimationClip> GetAllClips()
    {
        var result = new List<AnimationClip>();

        string[] guids =
            AssetDatabase.FindAssets("t:AnimationClip");

        foreach (string guid in guids)
        {
            string path =
                AssetDatabase.GUIDToAssetPath(guid);

            var clip =
                AssetDatabase.LoadAssetAtPath<AnimationClip>(path);

            if (clip != null)
            {
                result.Add(clip);
            }
        }

        return result;
    }

    public static List<Sprite> GetSprites(AnimationClip clip)
    {
        var sprites = new List<Sprite>();

        var bindings = AnimationUtility.GetObjectReferenceCurveBindings(clip);

        foreach (var binding in bindings)
        {
            var keyframes =
                AnimationUtility.GetObjectReferenceCurve(clip, binding);

            foreach (var frame in keyframes)
            {
                if (frame.value is Sprite sprite)
                {
                    if (!sprites.Contains(sprite))
                        sprites.Add(sprite);
                }
            }
        }

        return sprites;
    }

    public static (Image[], SpriteRenderer[]) GetRenders(Scene scene)
    {
        var gameObjects = scene.GetRootGameObjects();
        var images = gameObjects.Select(x => x.GetComponentsInChildren<Image>(true)).SelectMany(x => x)
            .Where(x => x != null && x.sprite != null).ToArray();
        var sprites = gameObjects.Select(x => x.GetComponentsInChildren<SpriteRenderer>(true)).SelectMany(x => x)
            .Where(x => x != null && x.sprite != null).ToArray();
        return (images, sprites);
    }

    public static (Image[], SpriteRenderer[]) GetRenders(GameObject gameObject)
    {
        var images = gameObject.GetComponentsInChildren<Image>(true)
            .Where(x => x != null && x.sprite != null).ToArray();
        var sprites = gameObject.GetComponentsInChildren<SpriteRenderer>(true)
            .Where(x => x != null && x.sprite != null).ToArray();
        return (images, sprites);
    }

    public static bool IsOriginalSprite(Sprite sprite)
    {
        var path = AssetDatabase.GetAssetPath(sprite);
        return !path.EndsWith(".asset");
    }

    public static void AddTo(Image[] images, SpriteRenderer[] sprites)
    {
        var count = 0;
        foreach (var item in images)
        {
            var renderData = item.GetComponent<RenderData>();
            if (renderData == null)
            {
                renderData = item.gameObject.AddComponent<RenderData>();
                renderData.SetData(item.sprite);
                count++;
                Debug.Log($"Add to {item.name}", item);
            }
        }
        foreach (var item in sprites)
        {
            var renderData = item.GetComponent<RenderData>();
            if (renderData == null)
            {
                renderData = item.gameObject.AddComponent<RenderData>();
                renderData.SetData(item.sprite);
                count++;
                Debug.Log($"Add to {item.name}", item);
            }
        }
    }
    // [MenuItem("Tools/InjectRenderData")]
    public static void InjectRenderData()
    {
        var scene = SceneManager.GetActiveScene();
        var (images, sprites) = GetRenders(scene);
        AddTo(images, sprites);
        EditorSceneManager.MarkSceneDirty(
            scene
        );
        EditorSceneManager.SaveScene(
            scene
        );
    }

    public static (string, GameObject)[] GetAllPrefabs()
    {
        var guids = AssetDatabase.FindAssets("t:Prefab", new[] { "Assets" });
        var result = guids.Select(x => AssetDatabase.GUIDToAssetPath(x))
            .Select(x => (x, AssetDatabase.LoadAssetAtPath<GameObject>(x)))
            .ToArray();
        return result;
    }

    // [MenuItem("Tools/RemoveAllMissingScriptPrefabs")]
    public static void RemoveAllMissingScripts()
    {
        var prefabs = GetAllPrefabs();
        foreach (var (path, prefab) in prefabs)
        {
            GameObject root =
                PrefabUtility.LoadPrefabContents(path);
            RemoveMissingScript.Remove(prefab);
            PrefabUtility.SaveAsPrefabAsset(root, path);
            PrefabUtility.UnloadPrefabContents(root);
            AssetDatabase.SaveAssets();
        }
    }

    // [MenuItem("Tools/InjectToPrefabs")]
    public static void InjectToPrefabs()
    {
        var prefabs = GetAllPrefabs();
        foreach (var (path, prefab) in prefabs)
        {
            GameObject root =
                PrefabUtility.LoadPrefabContents(path);
            var (images, sprites) = GetRenders(root);
            AddTo(images, sprites);
            PrefabUtility.SaveAsPrefabAsset(root, path);
            PrefabUtility.UnloadPrefabContents(root);
            AssetDatabase.SaveAssets();
        }
    }
    // [MenuItem("Tools/ReplaceRefPrefabs")]
    public static void ReplaceRefPrefabs() {
        var prefabs = GetAllPrefabs();
        foreach (var (path, prefab) in prefabs)
        {
            GameObject root =
                PrefabUtility.LoadPrefabContents(path);
            var a = root.GetComponentsInChildren<RenderData>(true);
            foreach (var renderData in a)
            {
                // renderData.OnValidate();
            }
            PrefabUtility.SaveAsPrefabAsset(root, path);
            PrefabUtility.UnloadPrefabContents(root);
            AssetDatabase.SaveAssets();
        }
    }
    public static void InjectRenderDatas()
    {
        var guids = AssetDatabase.FindAssets("t:Scene");
        foreach (var guid in guids)
        {
            var scenePath = AssetDatabase.GUIDToAssetPath(guid);
            var scene = EditorSceneManager.OpenScene(scenePath);
            Debug.Log(scene.name);
            InjectRenderData();
        }
    }

    public static RenderData[] GetRenderDatas(Scene scene)
    {
        var gameObjects = scene.GetRootGameObjects();
        var renderDatas = gameObjects.SelectMany(x => x.GetComponentsInChildren<RenderData>(true));
        return renderDatas.ToArray();
    }
    public static Sprite[] GetOriginalSprites(Texture2D texture)
    {
        string path = AssetDatabase.GetAssetPath(texture);
        Object[] assets =
            AssetDatabase.LoadAllAssetRepresentationsAtPath(path);
        var sprites = assets
            .OfType<Sprite>()
            .ToArray();
        return sprites;
    }
    public static void ReplaceSprite(
        AnimationClip clip, Dictionary<Texture2D, Sprite[]> dict)
    {
        var bindings =
            AnimationUtility.GetObjectReferenceCurveBindings(clip);

        foreach (var binding in bindings)
        {
            if (binding.propertyName != "m_Sprite")
                continue;
            var frames =
                AnimationUtility.GetObjectReferenceCurve(
                    clip,
                    binding);
            bool changed = false;
            for (int i = 0; i < frames.Length; i++)
            {
                var frame = frames[i];
                var sprite = frame.value as Sprite;
                if (sprite == null)
                {
                    continue;
                }

                if (sprite.texture == null)
                {
                    Debug.Log(clip.name + $" has no texture {sprite.name}", sprite);
                    continue;
                }
                if(IsOriginalSprite(sprite)) continue;
                if (!dict.TryGetValue(sprite.texture, out Sprite[] sprites))
                {
                    sprites = GetOriginalSprites(sprite.texture);
                    dict.Add(sprite.texture, sprites);
                }
                var newSprite = sprites.FirstOrDefault(x => x.name == sprite.name);
                if (newSprite != null)
                {
                    frame.value = newSprite;
                    changed = true;
                }
            }
            if (changed)
            {
                AnimationUtility.SetObjectReferenceCurve(
                    clip,
                    binding,
                    frames);
            }
        }
        EditorUtility.SetDirty(clip);
        AssetDatabase.SaveAssets();
    }
    // [MenuItem("Tools/ValidateRenderDatas")]
    public static void ValidateRenderDatas()
    {
        var guids = AssetDatabase.FindAssets("t:Scene");
        foreach (var guid in guids)
        {
            var scenePath = AssetDatabase.GUIDToAssetPath(guid);
            var scene = EditorSceneManager.OpenScene(scenePath);
            var renderDatas = GetRenderDatas(scene);
            foreach (var renderData in renderDatas)
            {
                // renderData.OnValidate();
            }
            EditorSceneManager.MarkSceneDirty(
                scene
            );
            EditorSceneManager.SaveScene(
                scene
            );
        }
        Debug.Log("Complete");
    }

    // [MenuItem("Tools/ReplaceAnimationClips")]
    public static void ReplaceAnimationClips()
    {
        var guids = AssetDatabase.FindAssets("t:AnimationClip");
        var dict = new Dictionary<Texture2D, Sprite[]>();
        foreach (var guid in guids)
        {
            var path = AssetDatabase.GUIDToAssetPath(guid);
            var clip = AssetDatabase.LoadAssetAtPath<AnimationClip>(path);
            ReplaceSprite(clip, dict);
        }
        AssetDatabase.SaveAssets();
        Debug.Log("Complete");
    }
}