using System;
using Base;
using Network;
using Network.Extensions;
using UnityEngine;

namespace LuyenKhi
{
    [Serializable]
    public class MaterialGradeConfig : IMemorySerializable<MaterialGradeConfig>
    {
        [field: SerializeField] public Tier Tier { get; private set; }
        [field: SerializeField] public int Grade { get; private set; }
        public void Deserialize(IReader reader, ref MaterialGradeConfig value)
        {
            value.Tier = reader.Read<Tier>();
            value.Grade = reader.Read<int>();
        }
        public int ByteSize()
        {
            return Tier.ByteSize() + Grade.ByteSize();
        }

        public void Serialize(IWriter writer)
        {
            writer.Write(Tier);
            writer.Write(Grade);
        }
    }
}