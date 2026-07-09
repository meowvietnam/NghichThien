#if UNITY_EDITOR
using System.Linq;
using Sirenix.OdinInspector;
using UnityEditor;
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

    private void OnValidate()
    {
        return;
        var image = GetComponent<Image>();
        if (image != null)
        {
            if (image.sprite != null)
            {
                Texture = image.sprite.texture;
                if (AssetOptimize.IsOriginalSprite(image.sprite))
                {
                    return;
                }
            }

            var sprites = GetSprites(Texture);
            var s = sprites.FirstOrDefault(s => s.name == SpriteName);
            if (s != null)
            {
                image.sprite = s;
            }
            else
            {
                Debug.Log("Not found sprite: " + SpriteName);
            }
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
                Texture = spriteRenderer.sprite.texture;
                if (AssetOptimize.IsOriginalSprite(spriteRenderer.sprite))
                {
                    return;
                }
            }
        
            var sprites = GetSprites(Texture);
            var s =  sprites.FirstOrDefault(s => s.name == SpriteName);
            if (s != null)
            {
                spriteRenderer.sprite = s;
            }
            else
            {
                Debug.Log("Not found sprite: " + SpriteName);
            }
            EditorUtility.SetDirty(gameObject);
        }
    }
}
#endif