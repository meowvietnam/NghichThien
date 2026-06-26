using System;
using Base;
using MemoryPack;
using UnityEngine;

namespace LuyenDan
{
    [Serializable]
    [MemoryPackable]
    public partial class ItemRef : IEquatable<ItemRef>, IDeepClone<ItemRef>
    {
        [field: SerializeField] public ushort Id { get; set; }
        [field: SerializeField] public ItemType Type { get; set; }
        [field: SerializeField] public ushort Count { get; set; }
        [MemoryPackIgnore] public Action<int> OnCountChanged { get; set; }
        [MemoryPackConstructor]
        public ItemRef()
        {
        }

        public ItemRef(ushort id, ItemType type, int count)
        {
            Id = id;
            Type = type;
            Count = (ushort)count;
        }

        public void SetCount(int count)
        {
            Count = (ushort)count;
            OnCountChanged?.Invoke(count);
        }

        public bool IsFurnace()
        {
            return Type == ItemType.Furnace;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Id, (byte)Type);
        }

        public bool Equals(ItemRef other)
        {
            if (other is null) return false;
            if (ReferenceEquals(this, other)) return true;
            return Id == other.Id && Type == other.Type;
        }

        public ItemRef DeepClone()
        {
            return new ItemRef(Id, Type, Count);
        }
    }
}