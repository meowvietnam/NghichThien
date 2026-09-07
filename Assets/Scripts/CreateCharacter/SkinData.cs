using System;
using System.Collections.Generic;
using System.Text;
using Network;
using Network.Extensions;
using UnityEngine;

namespace CreateCharacter
{
    [Serializable]
    public class SkinData : IMemorySerializable<SkinData>
    {
        [field: SerializeField] public string Name { get; private set; }
        [field: SerializeField] public Sprite Icon { get; set; }

        public SkinData(string name, Sprite icon)
        {
            Name = name;
            Icon = icon;
        }

        public SkinData()
        {
        }

        public void Deserialize(IReader reader, ref SkinData value)
        {
            value.Name = reader.Read();
        }

        public int ByteSize()
        {
            return Name.ByteSize();
        }

        public void Serialize(IWriter writer)
        {
            writer.Write(Name);
        }
    }
}