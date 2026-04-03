using System;
using LuyenDan.Inventory;
using Network;
using Network.Extensions;
using Sirenix.OdinInspector;
using UnityEngine;

namespace LuyenDan
{
    [Serializable]
    public class TempFurnaceData : IItemData, IMemorySerializable<TempFurnaceData>
    {
        [field: SerializeField] public int Id { get; private set; }

        [field: SerializeField, PreviewField(75)]
        public Sprite Icon { get; private set; }

        [field: SerializeField, PreviewField(75)]
        public Sprite BigIcon { get; private set; }

        [field: SerializeField] public int PricePerDurability { get; private set; }
        [field: SerializeField] public string Name { get; private set; }
        [field: SerializeField] public Tier Tier { get; private set; }
        [field: SerializeField] public string Description { get; private set; }

        public void Deserialize(IReader reader, ref TempFurnaceData value)
        {
            value.Id = reader.Read<int>();
            value.PricePerDurability = reader.Read<int>();
            value.Name = reader.Read();
            value.Tier = reader.Read<Tier>();
            value.Description = reader.Read();
        }

        public int ByteSize()
        {
            return Id.ByteSize() + PricePerDurability.ByteSize() 
                                 + Name.ByteSize() + Tier.ByteSize() + Description.ByteSize();
        }

        public void Serialize(IWriter writer)
        {
            writer.Write(Id);
            writer.Write(PricePerDurability);
            writer.Write(Name);
            writer.Write(Tier);
            writer.Write(Description);
        }
    }
}