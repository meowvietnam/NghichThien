using System;
using System.Collections.Generic;
using System.IO;
using Sirenix.OdinInspector;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class CreateImage : MonoBehaviour
{
    [SerializeField] private Texture2D texture;
    [SerializeField] private TextAsset textAsset;
    [SerializeField] private string folder = "ConceptDesign/WebUISpriteSheet/Sliced";

    [Button]
    public void Create()
    {
        if (texture == null)
        {
            Debug.LogError("CreateImage: Texture is missing.");
            return;
        }

        if (textAsset == null)
        {
            Debug.LogError("CreateImage: Atlas JSON TextAsset is missing.");
            return;
        }

        var atlas = JsonUtility.FromJson<SpriteSheetAtlas>(textAsset.text);
        if (atlas == null || atlas.sprites == null || atlas.sprites.Length == 0)
        {
            Debug.LogError("CreateImage: Atlas JSON has no sprites.");
            return;
        }

        var readableTexture = LoadReadableTexture(texture);
        if (readableTexture == null)
        {
            Debug.LogError("CreateImage: Could not create a readable copy of the texture.");
            return;
        }

        if (atlas.size != null && (atlas.size.w != readableTexture.width || atlas.size.h != readableTexture.height))
        {
            Debug.LogWarning(
                $"CreateImage: Atlas size is {atlas.size.w}x{atlas.size.h}, but texture size is {readableTexture.width}x{readableTexture.height}."
            );
        }

        var outputFolder = GetOutputFolder();
        Directory.CreateDirectory(outputFolder);

        var exportedCount = 0;
        var usedNames = new HashSet<string>();
        for (var i = 0; i < atlas.sprites.Length; i++)
        {
            var sprite = atlas.sprites[i];
            if (!IsValidSpriteRect(sprite, readableTexture.width, readableTexture.height))
            {
                Debug.LogWarning($"CreateImage: Skip invalid sprite rect '{sprite.id}' at index {i}.");
                continue;
            }

            var unityY = readableTexture.height - sprite.y - sprite.h;
            var croppedTexture = new Texture2D(sprite.w, sprite.h, TextureFormat.RGBA32, false);
            croppedTexture.SetPixels(readableTexture.GetPixels(sprite.x, unityY, sprite.w, sprite.h));
            croppedTexture.Apply(false, false);

            var fileName = GetUniqueFileName(sprite.id, i, usedNames);
            var outputPath = Path.Combine(outputFolder, fileName);
            File.WriteAllBytes(outputPath, croppedTexture.EncodeToPNG());
            exportedCount++;

            DestroyTexture(croppedTexture);
        }

        DestroyTexture(readableTexture);

#if UNITY_EDITOR
        AssetDatabase.Refresh();
        SetImportedSprites(outputFolder);
#endif

        Debug.Log($"CreateImage: Exported {exportedCount}/{atlas.sprites.Length} sprites to {outputFolder}");
    }

    private static Texture2D LoadReadableTexture(Texture2D source)
    {
#if UNITY_EDITOR
        var assetPath = AssetDatabase.GetAssetPath(source);
        if (!string.IsNullOrEmpty(assetPath))
        {
            var fullPath = Path.GetFullPath(Path.Combine(Application.dataPath, "..", assetPath));
            var bytes = File.ReadAllBytes(fullPath);
            var fromFile = new Texture2D(2, 2, TextureFormat.RGBA32, false);
            if (ImageConversion.LoadImage(fromFile, bytes))
            {
                fromFile.name = $"{source.name}_Readable";
                return fromFile;
            }

            DestroyTexture(fromFile);
        }
#endif

        var previous = RenderTexture.active;
        var renderTexture = RenderTexture.GetTemporary(
            source.width,
            source.height,
            0,
            RenderTextureFormat.ARGB32,
            RenderTextureReadWrite.Default
        );

        Graphics.Blit(source, renderTexture);
        RenderTexture.active = renderTexture;

        var readable = new Texture2D(source.width, source.height, TextureFormat.RGBA32, false);
        readable.ReadPixels(new Rect(0, 0, source.width, source.height), 0, 0);
        readable.Apply(false, false);

        RenderTexture.active = previous;
        RenderTexture.ReleaseTemporary(renderTexture);

        return readable;
    }

    private string GetOutputFolder()
    {
        var relativeFolder = string.IsNullOrWhiteSpace(folder)
            ? "ConceptDesign/WebUISpriteSheet/Sliced"
            : folder.Trim().Replace("\\", "/");

        if (relativeFolder.StartsWith("Assets/", StringComparison.Ordinal))
        {
            relativeFolder = relativeFolder.Substring("Assets/".Length);
        }

        var assetsRoot = Path.GetFullPath(Application.dataPath);
        var outputFolder = Path.GetFullPath(Path.Combine(assetsRoot, relativeFolder));
        var assetsRootWithSeparator = assetsRoot.EndsWith(Path.DirectorySeparatorChar.ToString(), StringComparison.Ordinal)
            ? assetsRoot
            : assetsRoot + Path.DirectorySeparatorChar;

        if (outputFolder != assetsRoot && !outputFolder.StartsWith(assetsRootWithSeparator, StringComparison.Ordinal))
        {
            throw new InvalidOperationException("CreateImage: Output folder must stay inside Assets.");
        }

        return outputFolder;
    }

    private static bool IsValidSpriteRect(SpriteRect sprite, int textureWidth, int textureHeight)
    {
        if (sprite == null)
        {
            return false;
        }

        return sprite.x >= 0
               && sprite.y >= 0
               && sprite.w > 0
               && sprite.h > 0
               && sprite.x + sprite.w <= textureWidth
               && sprite.y + sprite.h <= textureHeight;
    }

    private static string GetUniqueFileName(string id, int index, HashSet<string> usedNames)
    {
        var baseName = SanitizeFileName(string.IsNullOrWhiteSpace(id) ? $"sprite_{index + 1:00}" : id);
        var fileName = $"{baseName}.png";
        var suffix = 2;

        while (usedNames.Contains(fileName))
        {
            fileName = $"{baseName}_{suffix}.png";
            suffix++;
        }

        usedNames.Add(fileName);
        return fileName;
    }

    private static string SanitizeFileName(string value)
    {
        foreach (var invalidChar in Path.GetInvalidFileNameChars())
        {
            value = value.Replace(invalidChar, '_');
        }

        return value;
    }

    private static void DestroyTexture(Texture2D target)
    {
        if (target == null)
        {
            return;
        }

        if (Application.isPlaying)
        {
            Destroy(target);
        }
        else
        {
            DestroyImmediate(target);
        }
    }

#if UNITY_EDITOR
    private static void SetImportedSprites(string outputFolder)
    {
        var assetsRoot = Path.GetFullPath(Application.dataPath).Replace("\\", "/");
        var normalizedOutput = Path.GetFullPath(outputFolder).Replace("\\", "/");
        var assetsRootWithSeparator = assetsRoot.EndsWith("/", StringComparison.Ordinal) ? assetsRoot : assetsRoot + "/";
        if (normalizedOutput != assetsRoot && !normalizedOutput.StartsWith(assetsRootWithSeparator, StringComparison.Ordinal))
        {
            return;
        }

        foreach (var filePath in Directory.GetFiles(outputFolder, "*.png"))
        {
            var normalizedFilePath = Path.GetFullPath(filePath).Replace("\\", "/");
            var assetPath = "Assets/" + normalizedFilePath.Substring(assetsRootWithSeparator.Length);
            var importer = AssetImporter.GetAtPath(assetPath) as TextureImporter;
            if (importer == null)
            {
                continue;
            }

            importer.textureType = TextureImporterType.Sprite;
            importer.spriteImportMode = SpriteImportMode.Single;
            importer.alphaIsTransparency = true;
            importer.mipmapEnabled = false;
            importer.SaveAndReimport();
        }
    }
#endif

    [Serializable]
    private class SpriteSheetAtlas
    {
        public string image;
        public AtlasSize size;
        public SpriteRect[] sprites;
    }

    [Serializable]
    private class AtlasSize
    {
        public int w;
        public int h;
    }

    [Serializable]
    private class SpriteRect
    {
        public string id;
        public int x;
        public int y;
        public int w;
        public int h;
        public int area;
    }
}
