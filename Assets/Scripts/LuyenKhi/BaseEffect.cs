using System;
using System.Linq;
using Network;
using Network.Extensions;
using Sirenix.OdinInspector;
using UnityEngine;

namespace LuyenKhi
{
    [Serializable]
    public class BaseEffect : IMemorySerializable<BaseEffect>
    {
        [ShowInInspector] public string Name => LuyenKhiResources.Instance.EffectTypeNameConfigs.First(x => x.Type == Type).Name;
        [field: SerializeField] public int Value { get; set; }
        [field: SerializeField] public EffectType Type { get; set; }

        public void Deserialize(IReader reader, ref BaseEffect value)
        {
            value.Value = reader.Read<int>();
            value.Type = reader.Read<EffectType>();
        }

        public int ByteSize()
        {
            return Value.ByteSize() + Type.ByteSize();
        }

        public void Serialize(IWriter writer)
        {
            writer.Write(Value);
            writer.Write(Type);
        }
    }
}