using System;
using Network;
using Network.Extensions;
using UnityEngine;

namespace LuyenKhi
{
    [Serializable]
    public class EquipmentTierConfig : IMemorySerializable<EquipmentTierConfig>
    {
        [field: SerializeField] public EquipmentTier Tier { get; private set; }
        [field: SerializeField] public Color Color { get; private set; }
        [field: SerializeField] public int DayCost { get; private set; }

        public void Deserialize(IReader reader, ref EquipmentTierConfig value)
        {
            value.Tier = reader.Read<EquipmentTier>();
            value.Color = reader.Read<Color>();
            value.DayCost = reader.Read<int>();
        }

        public int ByteSize()
        {
            return Tier.ByteSize() + Color.ByteSize() + DayCost.ByteSize();
        }

        public void Serialize(IWriter writer)
        {
            writer.Write(Tier);
            writer.Write(Color);
            writer.Write(DayCost);
        }
    }
}