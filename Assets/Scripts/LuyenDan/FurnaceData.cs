using System;
using Network;
using Network.Extensions;
using UnityEngine;

namespace LuyenDan
{
    [Serializable]
    public class FurnaceData : IMemorySerializable<FurnaceData>
    {
        [field: SerializeField]
        public int SelfId { get; set; }
        [field: SerializeField]
        public ItemRef Ref { get; set; }
        public int Durability { get; set; }
        public event Action<int> OnDurabilityChanged;
        public int ByteSize()
        {
            return SelfId.ByteSize() + Ref.ByteSize()  + Durability.ByteSize();
        }
        public void Serialize(IWriter writer)
        {
            writer.Write(SelfId);
            writer.Write(Ref);
            writer.Write(Durability);
        }

        public void SetDurability(int durability)
        {
            Durability = durability;
            OnDurabilityChanged?.Invoke(durability);
        }
        public void Deserialize(IReader reader, ref FurnaceData value)
        {
            value.SelfId = reader.Read<int>();
            value.Ref = reader.Read<ItemRef>();
            value.Durability = reader.Read<int>();
        }
        public override int GetHashCode()
        {
            return HashCode.Combine(SelfId, Ref);
        }
    }
}