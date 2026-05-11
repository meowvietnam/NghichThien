using System;
using Base;
using Network;
using Network.Extensions;
using UnityEngine;

namespace LuyenDan
{
    [Serializable]
    public class FurnaceData : IMemorySerializable<FurnaceData>, IEquatable<FurnaceData>, IDeepClone<FurnaceData>
    {
        [field: SerializeField] public ushort SelfId { get; set; }
        [field: SerializeField] public ushort Id { get; set; }
        [field: SerializeField] public sbyte Durability { get; set; }

        public event Action<int> OnDurabilityChanged;

        public int ByteSize()
        {
            return SelfId.ByteSize() + Id.ByteSize() + Durability.ByteSize();
        }

        public ItemRef CreateRefSelfID()
        {
            return new ItemRef()
            {
                Id = SelfId,
                Type = ItemType.Furnace,
                Count = 1
            };
        }
        public ItemRef CreateRef()
        {
            return new ItemRef()
            {
                Id = Id,
                Type = ItemType.Furnace,
                Count = 1
            };
        }
        public void Serialize(IWriter writer)
        {
            writer.Write(SelfId);
            writer.Write(Id);
            writer.Write(Durability);
        }

        public void SetDurability(int durability)
        {
            Durability = (sbyte)durability;
            OnDurabilityChanged?.Invoke(durability);
        }

        public void Deserialize(IReader reader, ref FurnaceData value)
        {
            value.SelfId = reader.Read<ushort>();
            value.Id = reader.Read<ushort>();
            value.Durability = reader.Read<sbyte>();
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(SelfId, Id, Durability);
        }

        public bool Equals(FurnaceData other)
        {
            if (other is null) return false;
            if (ReferenceEquals(this, other)) return true;
            return SelfId == other.SelfId && Id == other.Id && Durability == other.Durability;
        }

        public override bool Equals(object obj)
        {
            if (obj is null) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != GetType()) return false;
            return Equals((FurnaceData)obj);
        }

        public FurnaceData DeepClone()
        {
            return new FurnaceData()
            {
                SelfId = SelfId,
                Durability = Durability,
                Id = Id,
                OnDurabilityChanged = null,
            };
        }
    }
}