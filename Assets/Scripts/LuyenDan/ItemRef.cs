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
        [field: SerializeField] public ushort Id { get; set; }
        [field: SerializeField] public ItemType Type { get; set; }
        [field: SerializeField] public ushort Count { get; set; }
        public Action<int> OnCountChanged { get; set; }
        public ItemRef() {}
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
            value.Id = reader.Read<ushort>();
            value.Type = reader.Read<ItemType>();
            value.Count = reader.Read<ushort>();
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