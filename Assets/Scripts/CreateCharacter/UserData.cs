using Base;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using Network;
using Network.Extensions;
using UnityEngine;

namespace CreateCharacter
{
    [Serializable]
    public class UserData : IMemorySerializable<UserData>
    {
        [field: SerializeField] public List<CharacterData> CharacterDatas { get; private set; }
        public UserData()
        {
            CharacterDatas = new List<CharacterData>();
        }

        public int ByteSize()
        {
            return CharacterDatas.ByteSize();
        }

        public void Serialize(IWriter writer)
        {
            writer.WriteArray(CharacterDatas);
        }

        public void Deserialize(IReader reader, ref UserData value)
        {
            value.CharacterDatas = reader.ReadArray<CharacterData>().ToList();
        }
    }
}
