using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

namespace Base
{
    public static class ImageLoader
    {
        private static readonly Dictionary<string, Sprite> SpriteCache = new();
        public static async Awaitable<Sprite> GetSpriteFromUrl(string url)
        {
            if (string.IsNullOrWhiteSpace(url))
                return null;

            if (SpriteCache.TryGetValue(url, out var cachedSprite) && cachedSprite != null)
                return cachedSprite;
            using var request = UnityWebRequestTexture.GetTexture(url);
            await request.SendWebRequest();
            if (request.result != UnityWebRequest.Result.Success)
            {
                Debug.LogError($"Load image failed: {url}\n{request.error}");
                return null;
            }
            var texture = DownloadHandlerTexture.GetContent(request);
            var sprite = Sprite.Create(
                texture,
                new Rect(0, 0, texture.width, texture.height),
                new Vector2(0.5f, 0.5f),
                100f
            );
            SpriteCache[url] = sprite;
            return sprite;
        }
        public static bool TryGetCached(string url, out Sprite sprite)
        {
            return SpriteCache.TryGetValue(url, out sprite) && sprite != null;
        }
        public static void RemoveCache(string url)
        {
            if (!SpriteCache.TryGetValue(url, out var sprite))
                return;

            if (sprite != null)
            {
                var texture = sprite.texture;
                Object.Destroy(sprite);
                if (texture != null)
                    Object.Destroy(texture);
            }
            SpriteCache.Remove(url);
        }
        public static void ClearCache()
        {
            foreach (var sprite in SpriteCache.Values)
            {
                if (sprite == null)
                    continue;
                var texture = sprite.texture;
                Object.Destroy(sprite);
                if (texture != null)
                    Object.Destroy(texture);
            }
            SpriteCache.Clear();
        }
    }
}