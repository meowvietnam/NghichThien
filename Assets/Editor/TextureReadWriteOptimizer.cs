using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

public class TextureReadWriteOptimizer
{
    [MenuItem("Tools/Tối ưu hóa Texture (Tắt Read-Write)")]
    public static void DisableReadWriteForAllTextures()
    {
        // Tìm tất cả các file có đuôi là .png, .jpg, .tga, .psd...
        string[] guids = AssetDatabase.FindAssets("t:Texture");
        int count = 0;

        foreach (string guid in guids)
        {
            string path = AssetDatabase.GUIDToAssetPath(guid);
            TextureImporter importer = AssetImporter.GetAtPath(path) as TextureImporter;

            if (importer != null && importer.isReadable)
            {
                // Tắt Read/Write
                importer.isReadable = false;
                importer.SaveAndReimport();
                count++;
            }
        }

        Debug.Log($"Đã hoàn tất! Đã tắt Read/Write cho {count} file Texture.");
        EditorUtility.DisplayDialog("Thông báo", $"Đã xử lý xong {count} texture.", "OK");
    }
}