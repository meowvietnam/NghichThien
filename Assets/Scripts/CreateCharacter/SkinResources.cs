using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace CreateCharacter
{
    [CreateAssetMenu]
    public class SkinResources : ScriptableObject
    {
        [field: SerializeField] public List<Color> Colors { get; private set; }
        [field: SerializeField] public List<GenderSkinGroup> GenderSkinGroups { get; private set; }
    }
}
