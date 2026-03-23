using Base;
using Sirenix.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;
using Network;
using Network.Extensions;
using UnityEngine;

namespace CreateCharacter
{
    [Serializable]
    public class SkinGroup : IMemorySerializable<SkinGroup>
    {
        [field: SerializeField] public string Id { get; private set; }
        [field: SerializeField] public string Name { get; private set; }
        [field: SerializeField] public List<SkinCategory> Categories { get; private set; }
        [field: SerializeField] public List<ColorGroup> ColorGroups { get; private set; }

        public int ByteSize()
        {
            return Id.ByteSize() + Name.ByteSize() + Categories.ByteSize() + ColorGroups.ByteSize();
        }
        public void Serialize(IWriter writer)
        {
            writer.Write(Id);
            writer.Write(Name);
            writer.WriteArray(Categories);
            writer.WriteArray(ColorGroups);
        }
        public void Deserialize(IReader reader, ref SkinGroup value)
        {
            value.Id = reader.Read();
            value.Name = reader.Read();
            value.Categories = reader.ReadArray<SkinCategory>().ToList();
            value.ColorGroups = reader.ReadArray<ColorGroup>().ToList();
        }
    }
}
