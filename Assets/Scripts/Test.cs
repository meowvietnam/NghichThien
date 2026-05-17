using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinInspector;
using Unity.EditorCoroutines.Editor;
using UnityEditor;
using UnityEngine;

public class Test : MonoBehaviour
{
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
        // var target = 2048;
        // string path = AssetDatabase.GetAssetPath(texture);
        // var importer = AssetImporter.GetAtPath(path) as TextureImporter;
        // if (importer == null) return;
        // int maxSize = GetMaxSize(importer);
        var metas = sprites.Select(x => CreateMeta(x)).ToArray();
        ApplySpriteSheet(texture, metas);
        //SetMaxSize(importer, target);
        // var originalSprites = GetOriginalSprites(texture);
        // foreach (var sprite in sprites)
        // {
        //     var originalSprite = originalSprites.FirstOrDefault(x => x.name == sprite.name);
        //     Copy(originalSprite, sprite);
        // }
        // importer.SaveAndReimport();
        // if (maxSize >= 1024 * 4)
        // {
        //
        // }
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

    public static void ApplySpriteSheet(Texture2D tex, SpriteMetaData[] metas)
    {
        string path = AssetDatabase.GetAssetPath(tex);
        TextureImporter importer = AssetImporter.GetAtPath(path) as TextureImporter;
        if (importer == null) return;
        importer.textureType = TextureImporterType.Sprite;
        importer.spriteImportMode = SpriteImportMode.Multiple;
        importer.isReadable = true;
        importer.spritesheet = metas;
        importer.SaveAndReimport();
        AssetDatabase.ImportAsset(path, ImportAssetOptions.ForceUpdate);
    }
}