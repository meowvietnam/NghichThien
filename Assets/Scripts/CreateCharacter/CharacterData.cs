using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace CreateCharacter
{
    [Serializable]
    public class CharacterData
    {
        [field: SerializeField] public CharacterSkinData SkinData { get; private set; }
    }
}
