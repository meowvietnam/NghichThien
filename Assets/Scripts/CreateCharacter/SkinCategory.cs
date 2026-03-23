using Sirenix.OdinInspector;
using System;
using System.Collections.Generic;
using System.Linq;
using Network;
using Network.Extensions;
using UnityEngine;

namespace CreateCharacter
{
    [Serializable]
    public class SkinCategory : IMemorySerializable<SkinCategory>
    {
        [field: SerializeField] public string Id { get; set; }
        [field: SerializeField] public string Name { get; private set; }
        [field: SerializeField] public List<SkinData> Skins { get; private set; }
        public SkinCategory()
        {
        }
        public SkinCategory(string name, List<SkinData> skins)
        {
            Name = name;
            Skins = skins;
        }
        public void Deserialize(IReader reader, ref SkinCategory value)
        {
            value.Id = reader.Read();
            value.Name = reader.Read();
            value.Skins = reader.ReadArray<SkinData>().ToList();
        }
        public int ByteSize()
        {
            return Id.ByteSize() + Name.ByteSize() + Skins.ByteSize();
        }

        public void Serialize(IWriter writer)
        {
            writer.Write(Id);
            writer.Write(Name);
            writer.WriteArray(Skins);
        }
    }
}
