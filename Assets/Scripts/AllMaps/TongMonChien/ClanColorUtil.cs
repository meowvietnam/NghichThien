using System.Collections.Generic;
using UnityEngine;

namespace AllMaps.TongMonChien
{
    /// <summary>
    /// Màu đặc trưng của tông trong Tông môn chiến, trích từ icon tông. Thuật toán tất định nên cùng một ảnh
    /// cho cùng màu trên mọi client: trung bình các pixel alpha &gt; 0.5 có trọng số theo độ bão hoà, rồi chuẩn hoá
    /// S ≥ 0.45 và V ∈ [0.55, 0.9]. Không có ảnh / ảnh gần xám → palette 12 màu theo ClanId.
    /// </summary>
    public static class ClanColorUtil
    {
        private const int MaxSamples = 4096;
        private const float MinAverageSaturation = 0.08f;

        private static readonly Color32[] Palette =
        {
            new Color32(229, 57, 53, 255), new Color32(30, 136, 229, 255), new Color32(67, 160, 71, 255),
            new Color32(251, 140, 0, 255), new Color32(142, 36, 170, 255), new Color32(0, 172, 193, 255),
            new Color32(253, 216, 53, 255), new Color32(216, 27, 96, 255), new Color32(124, 179, 66, 255),
            new Color32(94, 53, 177, 255), new Color32(0, 137, 123, 255), new Color32(109, 76, 65, 255)
        };

        private static readonly Dictionary<string, Color> Cache = new Dictionary<string, Color>();

        public static Color FallbackColor(int clanId)
        {
            var index = ((clanId % Palette.Length) + Palette.Length) % Palette.Length;
            return Palette[index];
        }

        public static bool TryGetCached(string url, out Color color)
        {
            color = default;
            return !string.IsNullOrEmpty(url) && Cache.TryGetValue(url, out color);
        }

        public static Color FromSprite(int clanId, string url, Sprite sprite)
        {
            if (!string.IsNullOrEmpty(url) && Cache.TryGetValue(url, out var cached)) return cached;
            var color = sprite != null && sprite.texture != null ? FromTexture(clanId, sprite.texture) : FallbackColor(clanId);
            if (!string.IsNullOrEmpty(url)) Cache[url] = color;
            return color;
        }

        public static Color FromTexture(int clanId, Texture2D texture)
        {
            if (texture == null || !texture.isReadable) return FallbackColor(clanId);
            Color32[] pixels;
            try
            {
                pixels = texture.GetPixels32();
            }
            catch (UnityException)
            {
                return FallbackColor(clanId);
            }

            var step = Mathf.Max(1, pixels.Length / MaxSamples);
            double r = 0, g = 0, b = 0, weight = 0, saturationSum = 0;
            var count = 0;
            for (var i = 0; i < pixels.Length; i += step)
            {
                var p = pixels[i];
                if (p.a <= 127) continue;
                Color.RGBToHSV(p, out _, out var s, out _);
                count++;
                saturationSum += s;
                var w = s + 0.0001;
                r += p.r * w;
                g += p.g * w;
                b += p.b * w;
                weight += w;
            }

            if (count == 0 || saturationSum / count < MinAverageSaturation || weight <= 0)
                return FallbackColor(clanId);
            var mean = new Color((float)(r / weight / 255.0), (float)(g / weight / 255.0), (float)(b / weight / 255.0));
            Color.RGBToHSV(mean, out var h, out var sat, out var val);
            sat = Mathf.Max(sat, 0.45f);
            val = Mathf.Clamp(val, 0.55f, 0.9f);
            return Color.HSVToRGB(h, sat, val);
        }
    }
}
