using System;
using LuyenKhi;
using Network;
using Network.Extensions;
using UnityEngine;

namespace CreateCharacter
{
    [Serializable]
    public class Effect : IMemorySerializable<Effect>
    {
        [field: SerializeField] public EffectType Type { get; set; }
        [field: SerializeField] public byte Value { get; set; }

        public void Deserialize(IReader reader, ref Effect value)
        {
            value.Type = reader.Read<EffectType>();
            value.Value = reader.Read<byte>();
        }

        public int ByteSize()
        {
            return Type.ByteSize() + Value.ByteSize();
        }

        public void Serialize(IWriter writer)
        {
            writer.Write(Type);
            writer.Write(Value);
        }
    }
}