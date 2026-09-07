using UnityEngine;
using UnityEditor;

public class TextureOptimizer : EditorWindow
{
    [MenuItem("Tools/Optimize Textures")]
    public static void Optimize()
    {
        string[] guids = AssetDatabase.FindAssets("t:Texture2D");
        foreach (string guid in guids)
        {
            string path = AssetDatabase.GUIDToAssetPath(guid);
            TextureImporter importer = AssetImporter.GetAtPath(path) as TextureImporter;
            if (importer != null)
            {
                // Cấu hình Android
                TextureImporterPlatformSettings androidSettings = importer.GetPlatformTextureSettings("Android");
                androidSettings.overridden = true;
                androidSettings.maxTextureSize = 1024;
                androidSettings.format = TextureImporterFormat.ASTC_6x6;
                androidSettings.compressionQuality = (int)TextureCompressionQuality.Normal;
                importer.SetPlatformTextureSettings(androidSettings);

                // Cấu hình iOS
                TextureImporterPlatformSettings iosSettings = importer.GetPlatformTextureSettings("iPhone");
                iosSettings.overridden = true;
                iosSettings.maxTextureSize = 1024;
                iosSettings.format = TextureImporterFormat.ASTC_6x6;
                iosSettings.compressionQuality = (int)TextureCompressionQuality.Normal;
                importer.SetPlatformTextureSettings(iosSettings);

                importer.SaveAndReimport();
            }
        }

        Debug.Log("Đã tối ưu xong toàn bộ Texture!");
    }
}