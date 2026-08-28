using System;
using MemoryPack;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Base
{
    [Serializable]
    [MemoryPackable]
    public partial class Quality
    {
        [field: SerializeField] public string Name { get; private set; }
        [field: SerializeField] public Tier Tier { get; private set; }
        [field: SerializeField] public Color Color { get; private set; }
        [field: SerializeField, PreviewField(75)]
        [MemoryPackIgnore]
        public Sprite Background { get; set; }
        [field: SerializeField, PreviewField(75)]
        [MemoryPackIgnore]
        public Sprite IconUp { get; set; }
    }
}