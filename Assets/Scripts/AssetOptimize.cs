using UnityEditor;
using UnityEngine;

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
}