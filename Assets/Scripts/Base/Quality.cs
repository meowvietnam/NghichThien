using System;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Base
{
    [Serializable]
    public class Quality
    {
        [field: SerializeField] public string Name { get; private set; }
        [field: SerializeField] public Tier Tier { get; private set; }
        [field: SerializeField] public Color Color { get; private set; }
        [field: SerializeField, PreviewField(75)] public Sprite Background { get; set; }
        [field: SerializeField, PreviewField(75)] public Sprite IconUp { get; set; }
    }
}