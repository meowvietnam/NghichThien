using System;
using UnityEngine;

namespace LuyenDan
{
    [Serializable]
    public class Quality
    {
        [field: SerializeField] public Tier Tier { get; private set; }
        [field: SerializeField] public Color Color { get; private set; }
        [field: SerializeField] public Sprite Background { get; private set; }
        [field: SerializeField] public Sprite IconUp { get; private set; }
    }
}