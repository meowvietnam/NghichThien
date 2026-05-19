using System.Collections.Generic;
using System.Linq;
using Sirenix.Utilities;
using UnityEngine;
using UnityEngine.Profiling;

public class Size
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
public class AssetSize : MonoBehaviour
{
    public static Size CalculateSize(HashSet<Sprite> sprites)
    {
        var textures = sprites.Select(x => x.texture).ToHashSet();
        var spriteSize = sprites.Sum(x => Profiler.GetRuntimeMemorySizeLong(x));
        var textureSize = textures.Sum(x => Profiler.GetRuntimeMemorySizeLong(x));
        return new Size { spriteSize = spriteSize, textureSize = textureSize };
    }
}