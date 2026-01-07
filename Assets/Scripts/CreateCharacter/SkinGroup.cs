using Base;
using Sirenix.Serialization;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace CreateCharacter
{
    [Serializable]
    public class SkinGroup
    {
        [field: SerializeField] public string Id { get; private set; }
        [field: SerializeField] public string Name { get; private set; }
        [field: SerializeField] public bool HasColor { get; private set; }
        [field: SerializeField] public List<SkinCategory> Categories { get; private set; }
        [field: SerializeField] public List<ColorGroup> ColorGroups { get; private set; }
    }
}
