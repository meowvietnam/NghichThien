using System;
using System.Collections.Generic;
using Base;
using Network;
using Network.Extensions;
using Sirenix.OdinInspector;
using UnityEngine;

namespace LuyenDan
{
    [Serializable]
    public class ItemRef : IMemorySerializable<ItemRef>,IEquatable<ItemRef>, IDeepClone<ItemRef>
    {
        [field: SerializeField] public int Id { get; set; }
        [field: SerializeField] public ItemType Type { get; set; }
        [field: SerializeField] public int Count { get; set; }
        public Action<int> OnCountChanged { get; set; }
        public ItemRef() {}
        public ItemRef(int id, ItemType type, int count)
        {
            Id = id;
            Type = type;
            Count = count;
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
        public int ByteSize()
        {
            return Id.ByteSize() + Type.ByteSize() + Count.ByteSize() ;
        }
        public void Serialize(IWriter writer)
        {
            writer.Write(Id);
            writer.Write(Type);
            writer.Write(Count);
        }
        public void Deserialize(IReader reader, ref ItemRef value)
        {
            value.Id = reader.Read<int>();
            value.Type = reader.Read<ItemType>();
            value.Count = reader.Read<int>();
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Id, (int)Type);
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