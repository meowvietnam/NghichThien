using Base;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace CreateCharacter
{
    [Serializable]
    public class CharacterData
    {
        [field: SerializeField] public Gender Gender { get; set; }
        [field: SerializeField] public string Name { get; set; }
        public Dictionary<string, string> Skins { get; set; }
        public Dictionary<string, Color> Colors { get; set; }
        public CharacterData(Gender gender,string name, Dictionary<string, string> skins)
        {
            Gender = gender;
            Name = name;
            Skins = skins;
        }
    }
}
