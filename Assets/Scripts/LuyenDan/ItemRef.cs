using System;
using Network;
using Network.Extensions;
using Sirenix.OdinInspector;
using UnityEngine;

namespace LuyenDan
{
    [Serializable]
    public class ItemRef : IMemorySerializable<ItemRef>
    {
        [field: SerializeField] public int Id { get; private set; }
        [field: SerializeField] public ItemType Type { get; private set; }
        [field: SerializeField] public int Count { get; private set; }
        
        [field: SerializeField, ShowIf(nameof(IsFurnace))] public int Durability { get; private set; }
        public Action<int> OnCountChanged { get; set; }
        public ItemRef() {}
        public ItemRef(int id, ItemType type, int count)
        {
            Id = id;
            Type = type;
            Count = count;
        }
        public ItemRef(int id, ItemType type, int count, int durability)
        {
            Id = id;
            Type = type;
            Count = count;
            Durability = durability;
        }
        public void SetCount(int count)
        {
            Count = count;
            OnCountChanged?.Invoke(count);
        }
        public void SetDurability(int durability)
        {
            Durability = durability;
        }
        public bool IsFurnace()
        {
            return Type == ItemType.Furnace;
        }
        public int ByteSize()
        {
            return Id.ByteSize() + Type.ByteSize() + Count.ByteSize() + Durability.ByteSize();
        }
        public void Serialize(IWriter writer)
        {
            writer.Write(Id);
            writer.Write(Type);
            writer.Write(Count);
            writer.Write(Durability);
        }
        public void Deserialize(IReader reader, ref ItemRef value)
        {
            value.Id = reader.Read<int>();
            value.Type = reader.Read<ItemType>();
            value.Count = reader.Read<int>();
            value.Durability = reader.Read<int>();
        }
    }
}