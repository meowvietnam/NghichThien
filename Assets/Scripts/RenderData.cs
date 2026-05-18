using System.Linq;
using Sirenix.OdinInspector;
using UnityEditor;
using UnityEditor.VersionControl;
using UnityEngine;
using UnityEngine.UI;

public class RenderData : MonoBehaviour
{
    [field: SerializeField, ReadOnly] public Texture2D Texture { get; set; }
    [field: SerializeField, ReadOnly] public string SpriteName { get; set; }

    public void SetData(Sprite sprite)
    {
        Texture = sprite.texture;
        SpriteName = sprite.name;
    }

    public static Sprite[] GetSprites(Texture2D tex)
    {
        string path = AssetDatabase.GetAssetPath(tex);
        Sprite[] sprites =
            AssetDatabase.LoadAllAssetsAtPath(path)
                .OfType<Sprite>()
                .ToArray();
        return sprites;
    }
    
    public void OnValidate()
    {
        return;
        if (Texture == null) return;
        var image = GetComponent<Image>();
        try
        {
            if (image != null)
            {
                if (image.sprite != null)
                {
                    if (AssetOptimize.IsOriginalSprite(image.sprite))
                    {
                        return;
                    }
                }

                var sprites = GetSprites(Texture);
                image.sprite = sprites.First(s => s.name == SpriteName);
                EditorUtility.SetDirty(image);
            }
            else
            {
                var spriteRenderer = GetComponent<SpriteRenderer>();
                if (spriteRenderer == null)
                {
                    Debug.LogError($"{nameof(RenderData)} requires a sprite renderer");
                    return;
                }

                if (spriteRenderer.sprite != null)
                {
                    if (AssetOptimize.IsOriginalSprite(spriteRenderer.sprite))
                    {
                        return;
                    }
                }

                var sprites = GetSprites(Texture);
                spriteRenderer.sprite = sprites.First(s => s.name == SpriteName);
                EditorUtility.SetDirty(spriteRenderer);
            }
        }
        catch
        {
            
        }
    }
}
