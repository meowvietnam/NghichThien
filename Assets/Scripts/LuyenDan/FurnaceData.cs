using System;
using Base;
using MemoryPack;
using Network;
using Network.Extensions;
using UnityEngine;

namespace LuyenDan
{
    [MemoryPackable]
    [Serializable]
    public partial class FurnaceData : IEquatable<FurnaceData>, IDeepClone<FurnaceData>
    {
        [field: SerializeField] public ushort SelfId { get; set; }
        [field: SerializeField] public ushort Id { get; set; }
        [field: SerializeField] public sbyte Durability { get; set; }

        public event Action<int> OnDurabilityChanged;

        public ItemRef CreateRefSelfID()
        {
            return new ItemRef()
            {
                Id = SelfId,
                Type = ItemType.Furnace,
                Count = 1,
                TempId = Id,
            };
        }

        public ItemRef CreateRef()
        {
            return new ItemRef()
            {
                Id = Id,
                Type = ItemType.Furnace,
                Count = 1,
                TempId = Id,
            };
        }

        public void SetDurability(int durability)
        {
            Durability = (sbyte)durability;
            OnDurabilityChanged?.Invoke(durability);
        }

        public override int GetHashCode()
        {
            return SelfId;
        }

        public bool Equals(FurnaceData other)
        {
            if (other is null) return false;
            if (ReferenceEquals(this, other)) return true;
            return SelfId == other.SelfId;
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