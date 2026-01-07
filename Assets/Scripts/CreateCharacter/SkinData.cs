using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace CreateCharacter
{
    [Serializable]
    public class SkinData
    {
        [field: SerializeField] public string Name { get; private set; }
        [field: SerializeField] public Sprite Icon { get; private set; }
        public SkinData(string name, Sprite icon)
        {
            Name = name;
            Icon = icon;
        }
    }
}
