using System;
using System.Collections.Generic;
using Network;
using Network.Extensions;
using Sirenix.OdinInspector;
using UnityEngine;

namespace CreateCharacter
{
    [Serializable]
    public class Appearance : IMemorySerializable<Appearance>
    {
        [ShowInInspector]
        public Dictionary<string, string> Skins { get; set; } = new();
        [ShowInInspector]
        public Dictionary<string, Color> Colors { get; set; } = new();
        public void Deserialize(IReader reader, ref Appearance value)
        {
            value.Skins = DictionaryUtils.CreateDictionary(reader.ReadArray(), reader.ReadArray());
            value.Colors = DictionaryUtils.CreateDictionary(reader.ReadArray(), reader.ReadArray<Color>());
        }

        public int ByteSize()
        {
            return Skins.Keys.ByteSize() + Skins.Values.ByteSize() + Colors.Keys.ByteSize() + Colors.Values.ByteSize();
        }
        public void Serialize(IWriter writer)
        {
            writer.WriteArray(Skins.Keys);
            writer.WriteArray(Skins.Values);
            writer.WriteArray(Colors.Keys);
            writer.WriteArray(Colors.Values);
        }
    }
}