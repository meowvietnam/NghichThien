using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Network;
using Network.Extensions;
using UnityEngine;

namespace CreateCharacter
{
    [Serializable]
    public class ColorGroup : IMemorySerializable<ColorGroup>
    {
        [field: SerializeField] public string Id { get; private set; }
        [field: SerializeField] public List<string> CategoryIds { get; private set; }
        [field: SerializeField] public Color DefaultColor { get; private set; }
        public Action<Color> OnChanged;

        public int ByteSize()
        {
            return Id.ByteSize() + CategoryIds.ByteSize() + DefaultColor.ByteSize();
        }

        public void Serialize(IWriter writer)
        {
            writer.Write(Id);
            writer.WriteArray(CategoryIds);
            writer.Write(DefaultColor);
        }

        public void Deserialize(IReader reader, ref ColorGroup value)
        {
            value.Id = reader.Read();
            value.CategoryIds = reader.ReadArray().ToList();
            value.DefaultColor = reader.Read<Color>();
        }
    }
}