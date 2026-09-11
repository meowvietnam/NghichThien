#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinInspector;
using UnityEditor;
using UnityEngine;
using UnityEngine.Profiling;

public class SpriteSize
{
    public long spriteSize;
    public long textureSize;

    public override string ToString()
    {
        float spriteMB = spriteSize / 1024f / 1024f;
        float textureMB = textureSize / 1024f / 1024f;
        return $"Sprite: {spriteMB:F2} MB, Texture: {textureMB:F2} MB";
    }
}

[Serializable]
public class TextureSize
{
    public long textureSize;
    public string name;
    public string path;
    [ShowInInspector] public string Total => ToString();

    [Button]
    public void Ping()
    {
        var obj = AssetDatabase.LoadAssetAtPath<Texture2D>(path);
        var refs = AssetDatabase.GetDependencies(path);
        Debug.Log(string.Join(",\n", refs));
        EditorGUIUtility.PingObject(obj);
    }

    public bool IsSingleDependencies()
    {
        var refs = AssetDatabase.GetDependencies(path, true);
        return refs.Length == 1;
    }

    public override string ToString()
    {
        float textureMB = textureSize / 1024f / 1024f;
        return $"{textureMB:F2} MB";
    }
}

public class AssetSize : MonoBehaviour
{
    public List<TextureSize> textures = new List<TextureSize>();

    public static SpriteSize CalculateSize(HashSet<Sprite> sprites)
    {
        var textures = sprites.Select(x => x.texture).ToHashSet();
        var spriteSize = sprites.Sum(x => Profiler.GetRuntimeMemorySizeLong(x));
        var textureSize = textures.Sum(x => Profiler.GetRuntimeMemorySizeLong(x));
        return new SpriteSize { spriteSize = spriteSize, textureSize = textureSize };
    }

    [Button]
    public void Get()
    {
        var guids = AssetDatabase.FindAssets("t:Texture2D", new[] { "Assets" });
        textures = guids.Select(x => AssetDatabase.GUIDToAssetPath(x))
            .Select(x => new { tex = AssetDatabase.LoadAssetAtPath<Texture2D>(x), path = x })
            .Where(x => x.tex != null)
            .Select(x => new TextureSize()
            {
                textureSize = Profiler.GetRuntimeMemorySizeLong(x.tex),
                path = x.path,
                name = x.tex.name,
            })
            .Where(x => x.textureSize > 2000_000)
            .Where(x => x.IsSingleDependencies())
            .OrderByDescending(x => x.textureSize)
            .ToList();
    }
}
#endif