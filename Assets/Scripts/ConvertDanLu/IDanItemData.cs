using Base;
using UnityEngine;

namespace ConvertDanLu
{
    public class DanVisualQuality
    {
        public Sprite Background { get; }
        public Sprite IconUp { get; }
        public Color Color { get; }

        public DanVisualQuality(Sprite background, Sprite iconUp, Color color)
        {
            Background = background;
            IconUp = iconUp;
            Color = color;
        }
    }

    public interface IDanItemData
    {
        int Id { get; }
        Sprite Icon { get; }
        string Name { get; }
        Tier Tier { get; }
        string Description { get; }
        DanVisualQuality VisualQuality { get; }
    }
}
