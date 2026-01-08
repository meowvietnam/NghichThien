using Base;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace CreateCharacter
{
    [Serializable]
    public class CharacterSkinData
    {
        [field: SerializeField] public Gender Gender { get; private set; }
        public Dictionary<string, string> Skins { get; private set; }
        public CharacterSkinData(Gender gender, Dictionary<string, string> skins)
        {
            Gender = gender;
            Skins = skins;
        }
    }
}
