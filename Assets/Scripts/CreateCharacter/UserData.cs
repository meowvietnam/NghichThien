using Base;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace CreateCharacter
{
    [Serializable]
    public class UserData
    {
        [field: SerializeField] public List<CharacterData> CharacterDatas { get; private set; }
    }
}
