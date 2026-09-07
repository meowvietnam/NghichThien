#if UNITY_EDITOR
using System;
using UnityEditor;
using UnityEngine;

public static class CopyAndroidTextureOverrideToIOS
{
    private const string AndroidPlatform = "Android";
    private const string IOSPlatform = "iPhone";

    [MenuItem("Tools/Textures/Copy Android Override To iOS")]
    public static void CopyAll()
    {
        string[] textureGuids = AssetDatabase.FindAssets("t:Texture2D");

        int updatedCount = 0;
        int skippedCount = 0;
        int errorCount = 0;

        try
        {
            AssetDatabase.StartAssetEditing();

            for (int i = 0; i < textureGuids.Length; i++)
            {
                string assetPath =
                    AssetDatabase.GUIDToAssetPath(textureGuids[i]);

                EditorUtility.DisplayProgressBar(
                    "Copy Android Texture Override To iOS",
                    $"{i + 1}/{textureGuids.Length}\n{assetPath}",
                    (float)(i + 1) / textureGuids.Length);

                try
                {
                    var importer =
                        AssetImporter.GetAtPath(assetPath) as TextureImporter;

                    if (importer == null)
                    {
                        skippedCount++;
                        continue;
                    }

                    TextureImporterPlatformSettings androidSettings =
                        importer.GetPlatformTextureSettings(AndroidPlatform);

                    // Chỉ xử lý texture có bật Override for Android.
                    if (!androidSettings.overridden)
                    {
                        skippedCount++;
                        continue;
                    }

                    TextureImporterPlatformSettings iosSettings =
                        importer.GetPlatformTextureSettings(IOSPlatform);

                    CopySettings(androidSettings, iosSettings);

                    importer.SetPlatformTextureSettings(iosSettings);

                    // Chưa reimport ngay vì đang dùng StartAssetEditing.
                    AssetDatabase.WriteImportSettingsIfDirty(assetPath);

                    updatedCount++;
                }
                catch (Exception exception)
                {
                    errorCount++;

                    Debug.LogError(
                        $"Không thể xử lý texture: {assetPath}\n{exception}");
                }
            }
        }
        finally
        {
            AssetDatabase.StopAssetEditing();
            EditorUtility.ClearProgressBar();

            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }

        Debug.Log(
            $"Copy Android Override → iOS hoàn tất. " +
            $"Updated={updatedCount}, Skipped={skippedCount}, Errors={errorCount}");
    }

    private static void CopySettings(
        TextureImporterPlatformSettings android,
        TextureImporterPlatformSettings ios)
    {
        ios.name = IOSPlatform;
        ios.overridden = true;

        ios.maxTextureSize = android.maxTextureSize;
        ios.resizeAlgorithm = android.resizeAlgorithm;

        ios.textureCompression = android.textureCompression;
        ios.compressionQuality = android.compressionQuality;
        ios.crunchedCompression = android.crunchedCompression;

        ios.format = ConvertAndroidFormatToIOS(android.format);
    }

    private static TextureImporterFormat ConvertAndroidFormatToIOS(
        TextureImporterFormat androidFormat)
    {
        return androidFormat switch
        {
            // Auto để Unity tự chọn định dạng phù hợp cho iOS.
            TextureImporterFormat.Automatic =>
                TextureImporterFormat.Automatic,

            // ASTC dùng được cho cả Android và iOS.
            TextureImporterFormat.ASTC_4x4 =>
                TextureImporterFormat.ASTC_4x4,

            TextureImporterFormat.ASTC_5x5 =>
                TextureImporterFormat.ASTC_5x5,

            TextureImporterFormat.ASTC_6x6 =>
                TextureImporterFormat.ASTC_6x6,

            TextureImporterFormat.ASTC_8x8 =>
                TextureImporterFormat.ASTC_8x8,

            TextureImporterFormat.ASTC_10x10 =>
                TextureImporterFormat.ASTC_10x10,

            TextureImporterFormat.ASTC_12x12 =>
                TextureImporterFormat.ASTC_12x12,

            // ETC2 RGBA8 gần chất lượng ASTC 6x6.
            TextureImporterFormat.ETC2_RGBA8 =>
                TextureImporterFormat.ASTC_6x6,

            // ETC2 RGB giữ chất lượng khá tốt nhưng dung lượng vừa phải.
            TextureImporterFormat.ETC2_RGB4 =>
                TextureImporterFormat.ASTC_6x6,

            // ETC/ETC2 1-bit alpha có thể chuyển sang ASTC 8x8.
            TextureImporterFormat.ETC2_RGB4_PUNCHTHROUGH_ALPHA =>
                TextureImporterFormat.ASTC_8x8,

            TextureImporterFormat.ETC_RGB4 =>
                TextureImporterFormat.ASTC_8x8,

            // Định dạng không nén.
            TextureImporterFormat.RGBA32 =>
                TextureImporterFormat.RGBA32,

            TextureImporterFormat.RGB24 =>
                TextureImporterFormat.RGB24,

            TextureImporterFormat.Alpha8 =>
                TextureImporterFormat.Alpha8,

            TextureImporterFormat.R8 =>
                TextureImporterFormat.R8,

            TextureImporterFormat.R16 =>
                TextureImporterFormat.R16,

            TextureImporterFormat.RG16 =>
                TextureImporterFormat.RG16,

            TextureImporterFormat.RGBAHalf =>
                TextureImporterFormat.RGBAHalf,

            TextureImporterFormat.RGBAFloat =>
                TextureImporterFormat.RGBAFloat,

            // DXT/BC không phù hợp cho iOS.
            // Chuyển về ASTC có chất lượng tương ứng.
            TextureImporterFormat.DXT1 =>
                TextureImporterFormat.ASTC_6x6,

            TextureImporterFormat.DXT5 =>
                TextureImporterFormat.ASTC_6x6,

            TextureImporterFormat.BC4 =>
                TextureImporterFormat.ASTC_6x6,

            TextureImporterFormat.BC5 =>
                TextureImporterFormat.ASTC_6x6,

            TextureImporterFormat.BC6H =>
                TextureImporterFormat.ASTC_4x4,

            TextureImporterFormat.BC7 =>
                TextureImporterFormat.ASTC_4x4,

            // Không nhận diện được thì để Unity tự chọn.
            _ => TextureImporterFormat.Automatic
        };
    }
}
#endif