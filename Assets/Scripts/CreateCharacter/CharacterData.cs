using Base;
using System;
using System.Collections.Generic;
using Network;
using Network.Extensions;
using Newtonsoft.Json;
using UnityEngine;

namespace CreateCharacter
{
    [Serializable]
    public class CharacterData : IMemorySerializable<CharacterData>
    {
        [field: SerializeField] public int Id { get; set; }
        [field: SerializeField] public Gender Gender { get; set; }
        [field: SerializeField] public string Name { get; set; }
        [field: SerializeField] public int Level { get; set; } = 1;
        [field: SerializeField] public CharStats.CharacterStats Stats { get; set; } = new CharStats.CharacterStats();
        public Dictionary<string, string> Skins { get; set; }
        public Dictionary<string, Color> Colors { get; set; }
        public int ByteSize()
        {
            return Id.ByteSize() + Gender.ByteSize() + Name.ByteSize() + Level.ByteSize() 
                   + Stats.ByteSize() + Skins.Keys.ByteSize() + Skins.Values.ByteSize() 
                   + Colors.Keys.ByteSize() + Colors.Values.ByteSize();
        }

        public void Serialize(IWriter writer)
        {
            writer.Write(Id);
            writer.Write(Gender);
            writer.Write(Name);
            writer.Write(Level);
            writer.Write(Stats);
            writer.WriteArray(Skins.Keys);
            writer.WriteArray(Skins.Values);
            writer.WriteArray(Colors.Keys);
            writer.WriteArray(Colors.Values);
        }

        public void Deserialize(IReader reader, ref CharacterData value)
        {
            value.Id = reader.Read<int>();
            value.Gender = reader.Read<Gender>();
            value.Name = reader.Read();
            value.Level = reader.Read<int>();
            value.Stats = reader.Read<CharStats.CharacterStats>();
            value.Skins = DictionaryUtils.CreateDictionary(reader.ReadArray(), reader.ReadArray());
            value.Colors = DictionaryUtils.CreateDictionary(reader.ReadArray(), reader.ReadArray<Color>());
        }
    }
}
