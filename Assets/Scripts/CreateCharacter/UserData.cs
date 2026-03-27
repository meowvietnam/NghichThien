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
        [field: SerializeField] public int Id { get; set; }
        [field: SerializeField] public List<CharacterData> CharacterDatas { get; set; }
        [field: SerializeField] public bool IsInfiniteEnergy { get; set; }

        public UserData()
        {
            CharacterDatas = new List<CharacterData>();
        }

        public int ByteSize()
        {
            return Id.ByteSize() + CharacterDatas.ByteSize() + IsInfiniteEnergy.ByteSize();
        }

        public void Serialize(IWriter writer)
        {
            writer.Write(Id);
            writer.WriteArray(CharacterDatas);
            writer.Write(IsInfiniteEnergy);
        }

        public void Deserialize(IReader reader, ref UserData value)
        {
            value.Id = reader.Read<int>();
            value.CharacterDatas = reader.ReadArray<CharacterData>().ToList();
            value.IsInfiniteEnergy = reader.Read<bool>();
        }
    }
}