using Sirenix.OdinInspector;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace CreateCharacter
{
    [Serializable]
    public class SkinCategory
    {
        [field: SerializeField] public string Id { get; set; }
        [field: SerializeField] public string Name { get; private set; }
        [field: SerializeField] public List<SkinData> Skins { get; private set; }
        public SkinCategory(string name, List<SkinData> skins)
        {
            Name = name;
            Skins = skins;
        }
    }
}
