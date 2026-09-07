using System;
using Network;
using Network.Extensions;
using UnityEngine;

namespace LuyenKhi
{
    [Serializable]
    public class EquipmentGradeConfig : IMemorySerializable<EquipmentGradeConfig>
    {
        [field: SerializeField] public int MinGrade { get; private set; }
        [field: SerializeField] public EquipmentTier Tier { get; private set; }
        [field: SerializeField] public EquipmentGrade Grade { get; private set; }
        [field: SerializeField] public string Name { get; private set; }

        public void Deserialize(IReader reader, ref EquipmentGradeConfig value)
        {
            value.MinGrade = reader.Read<int>();
            value.Tier = reader.Read<EquipmentTier>();
            value.Grade = reader.Read<EquipmentGrade>();
        }

        public int ByteSize()
        {
            return MinGrade.ByteSize() + Tier.ByteSize() + Grade.ByteSize();
        }

        public void Serialize(IWriter writer)
        {
            writer.Write(MinGrade);
            writer.Write(Tier);
            writer.Write(Grade);
        }
    }
}