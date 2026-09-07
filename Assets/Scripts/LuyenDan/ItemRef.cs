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
        [field: SerializeField]
        public ushort
            Id
        {
            get;
            set;
        } // id này gửi lên server có thể là id bình thường hoặc nó cũng là self id đối với equipment và đan lô

        [field: SerializeField] public ItemType Type { get; set; }
        [field: SerializeField] public int Count { get; set; }
        [MemoryPackIgnore] public Action<int> OnCountChanged { get; set; }

        [MemoryPackIgnore]
        [field: SerializeField]
        public ushort TempId { get; set; } // cái này là id của furnace (đan lô) để lấy ref icon và name, tier ....


        [MemoryPackConstructor]
        public ItemRef()
        {
        }

        public ItemRef(ushort id, ItemType type, int count, ushort tempId = 0)
        {
            Id = id;
            Type = type;
            Count = count;
            TempId = tempId;
        }

        public void SetCount(int count)
        {
            Count = count;
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

        public override string ToString()
        {
            return $"{Id} {Type} {Count}";
        }
    }
}