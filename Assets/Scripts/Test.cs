using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinInspector;
using UnityEditor;
using UnityEngine;

public class Test : MonoBehaviour
{
    [SerializeField] private int minSize = 4096;
    [SerializeField] private int target = 2048;
    public void Rescale(Sprite source, float scale)
    {
        SerializedObject so = new SerializedObject(source);
        var rect =
            so.FindProperty("m_Rect");
        var or = rect.rectValue;
        var newRect = new Rect(or.x / scale, or.y / scale, or.width / scale, or.height / scale);
        rect.rectValue = newRect;
        var offset = so.FindProperty("m_Offset");
        offset.vector2Value /= scale;
        var uv = so.FindProperty("m_RD.uvTransform");
        uv.vector4Value /= scale;
        rect = so.FindProperty("m_RD.textureRect");
        rect.rectValue = newRect;
        so.ApplyModifiedProperties();
    }

    public HashSet<Sprite> GetOriginalSprites(Texture2D texture)
    {
        string path = AssetDatabase.GetAssetPath(texture);
        Object[] assets =
            AssetDatabase.LoadAllAssetRepresentationsAtPath(path);
        var sprites = assets
            .OfType<Sprite>()
            .ToHashSet();
        return sprites;
    }


    [Button]
    public void Optimize()
    {
        string[] guids = AssetDatabase.FindAssets("t:Sprite");
        List<Sprite> sprites = new List<Sprite>();
        foreach (string guid in guids)
        {
            string path = AssetDatabase.GUIDToAssetPath(guid);
            Sprite sprite = AssetDatabase.LoadAssetAtPath<Sprite>(path);
            if (sprite != null && sprite.texture != null)
                sprites.Add(sprite);
        }

        var dict = sprites.GroupBy(x => x.texture).ToDictionary(x => x.Key, x => x.ToHashSet());
        Debug.Log(dict.Count);
        foreach (var pair in dict)
        {
            var originalSprites = GetOriginalSprites(pair.Key);
            pair.Value.ExceptWith(originalSprites);
        }
        foreach (var (texture, s) in dict)
        {
            ResizeTexture(texture, s);
        }
    }
    [Button]
    public void Optimize2()
    {
        string[] guids = AssetDatabase.FindAssets("t:Texture2D");
        foreach (var guid in guids)
        {
            string path = AssetDatabase.GUIDToAssetPath(guid);
            var texture = AssetDatabase.LoadAssetAtPath<Texture2D>(path);
            ResizeTexture(texture,new());
        }
    }
    public void ResizeTexture(Texture2D texture, HashSet<Sprite> sprites)
    {
        string path = AssetDatabase.GetAssetPath(texture);
        var importer = AssetImporter.GetAtPath(path) as TextureImporter;
        if (importer == null) return;
        int maxSize = GetMaxSize(importer);
        if (maxSize >= minSize)
        {
            Debug.Log($"Resize: {texture.name} ({maxSize} -> {target})");
            SetMaxSize(importer, target);
            var scale = maxSize / target;
            foreach (var sprite in sprites)
            {
                Rescale(sprite, scale);
            }
            importer.SaveAndReimport();
        }
    }

    static int GetMaxSize(TextureImporter importer)
    {
        return importer.maxTextureSize;
    }

    static void SetMaxSize(TextureImporter importer, int size)
    {
        importer.maxTextureSize = size;

        foreach (var platform in new[] { "Android", "iPhone", "Standalone" })
        {
            var settings = importer.GetPlatformTextureSettings(platform);
            settings.overridden = true;
            settings.maxTextureSize = size;
            importer.SetPlatformTextureSettings(settings);
        }
    }
}