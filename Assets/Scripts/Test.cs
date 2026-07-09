#if UNITY_EDITOR
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinInspector;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;


public class Test : MonoBehaviour
{
    private Texture GetTexture(string name)
    {
        string[] guids =
            AssetDatabase.FindAssets($"{name} t:Texture2D");
        foreach (string guid in guids)
        {
            string path =
                AssetDatabase.GUIDToAssetPath(guid);

            Texture2D tex =
                AssetDatabase.LoadAssetAtPath<Texture2D>(path);

            if (tex != null && tex.name == name)
            {
                return tex;
            }
        }

        return null;
    }

    [Button]
    public void Check()
    {
        var clips = AssetOptimize.GetAllClips();
        foreach (var clip in clips)
        {
            var sprites = AssetOptimize.GetSprites(clip);
            var paths = sprites.Select(x => AssetDatabase.GetAssetPath(x)).ToArray();
            if (paths.Any())
            {
                if (paths.Any(x => x.EndsWith(".asset")))
                {
                    Debug.Log(clip.name, clip);
                    continue;
                }
            }
        }
    }
    
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
    public void Optimize2()
    {
        string[] guids = AssetDatabase.FindAssets("t:Texture2D");
        foreach (var guid in guids)
        {
            string path = AssetDatabase.GUIDToAssetPath(guid);
            var texture = AssetDatabase.LoadAssetAtPath<Texture2D>(path);
            ResizeTexture(texture, new());
        }
    }

    [Button]
    public void RemoveSprites()
    {
        var sprites = Resources.LoadAll<Sprite>("");
        foreach (var sprite in sprites)
        {
            var path = AssetDatabase.GetAssetPath(sprite);
            if (path.EndsWith(".asset"))
            {
                AssetDatabase.DeleteAsset(path);
            }
        }

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
    }

    private List<Sprite> GetAllSprites()
    {
        string[] guids = AssetDatabase.FindAssets("t:Sprite", new []{"Assets"});
        List<Sprite> sprites = new List<Sprite>();
        HashSet<Sprite> set = new HashSet<Sprite>();
        foreach (string guid in guids)
        {
            string path = AssetDatabase.GUIDToAssetPath(guid);

            Object[] assets =
                AssetDatabase.LoadAllAssetsAtPath(path);

            foreach (Object obj in assets)
            {
                if (obj is Sprite sprite)
                {
                    if (set.Add(sprite))
                    {
                        sprites.Add(sprite);
                    }
                }
            }
        }

        return sprites;
    }

    [Button]
    public void NewCheck()
    {
        var sprites = GetAllSprites();
        var dict = sprites.GroupBy(x => x.texture).ToDictionary(x => x.Key, x => x.ToHashSet());
        foreach (var pair in dict)
        {
            var originalSprites = GetOriginalSprites(pair.Key);
            var value = pair.Value;
            value.ExceptWith(originalSprites);
            var a = originalSprites.Select(x => x.name).ToHashSet();
            var b =  value.Select(x => x.name).ToHashSet();
            b.ExceptWith(a);
            foreach (var se in b)
            {
                Debug.Log($"Missing {se}",pair.Key);
            }
        }

    }
    [Button]
    public void Optimize()
    {
        var sprites = GetAllSprites();
        var dict = sprites.GroupBy(x => x.texture).ToDictionary(x => x.Key, x => x.ToHashSet());
        Debug.Log(dict.Count);
        foreach (var pair in dict)
        {
            var originalSprites = GetOriginalSprites(pair.Key);
            pair.Value.ExceptWith(originalSprites);
        }

        Resize(dict);
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
    }

    private void Resize(Dictionary<Texture2D, HashSet<Sprite>> dict)
    {
        foreach (var (texture, s) in dict)
        {
            ResizeTexture(texture, s);
        }
    }

    public void ResizeTexture(Texture2D texture, HashSet<Sprite> sprites)
    {
        string path = AssetDatabase.GetAssetPath(texture);
        var importer = AssetImporter.GetAtPath(path) as TextureImporter;
        if (importer == null) return;
        int maxSize = GetMaxSize(importer);
        var minSize = 4096;
        var target = 2048;
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

    Vector2 GetPivot(Rect rect, Vector2 pixelPivot)
    {
        return new Vector2(
            pixelPivot.x / rect.width,
            pixelPivot.y / rect.height
        );
    }

    public void Copy(Sprite source, Sprite dest)
    {
        var so = new SerializedObject(source);
        var sd = new SerializedObject(dest);

        sd.FindProperty("m_Rect").rectValue =
            so.FindProperty("m_Rect").rectValue;
        sd.FindProperty("m_Pivot").vector2Value =
            so.FindProperty("m_Pivot").vector2Value;
        sd.FindProperty("m_Border").vector4Value =
            so.FindProperty("m_Border").vector4Value;
        sd.FindProperty("m_PixelsToUnits").floatValue =
            so.FindProperty("m_PixelsToUnits").floatValue;
        sd.FindProperty("m_RD.textureRect").rectValue =
            so.FindProperty("m_RD.textureRect").rectValue;
        sd.FindProperty("m_RD.uvTransform").vector4Value =
            so.FindProperty("m_RD.uvTransform").vector4Value;
        sd.ApplyModifiedProperties();
        EditorUtility.SetDirty(dest);
        Debug.Log($"Copy: {source.name} -> {dest.name}");
    }

    public SpriteMetaData CreateMeta(Sprite sprite)
    {
        var meta = new SpriteMetaData()
        {
            name = sprite.name,
            rect = sprite.rect,
            pivot = GetPivot(sprite.rect, sprite.pivot),
            alignment = (int)SpriteAlignment.Custom,
        };
        return meta;
    }

    public static TextureImporter GetImporter(Texture2D texture)
    {
        string path = AssetDatabase.GetAssetPath(texture);

        var importer =
            AssetImporter.GetAtPath(path) as TextureImporter;
        return importer;
    }

    public static void ApplySpriteSheet(
        Texture2D tex,
        SpriteMetaData[] newMetas)
    {
        var importer =
            GetImporter(tex);

        if (importer == null)
            return;

        importer.textureType = TextureImporterType.Sprite;
        importer.spriteImportMode = SpriteImportMode.Multiple;
        importer.isReadable = true;
        importer.spritesheet = newMetas;
        EditorUtility.SetDirty(importer);
        importer.SaveAndReimport();
    }
}
#endif