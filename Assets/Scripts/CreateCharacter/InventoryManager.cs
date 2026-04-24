using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Base;
using LuyenDan;
using Network;
using Network.Extensions;
using Newtonsoft.Json;
using Sirenix.OdinInspector;

namespace CreateCharacter
{
    [Serializable]
    public class InventoryManager : IMemorySerializable<InventoryManager>
    {
        [ShowInInspector] public HashSet<FurnaceData> Furnaces { get; set; } = new();
        [ShowInInspector] public HashSet<ItemRef> Recipes { get; set; } = new();
        [ShowInInspector] public HashSet<ItemRef> Herbs { get; set; } = new();

        [ShowInInspector] public HashSet<ItemRef> ThanThongs { get; set; } = new();
        [ShowInInspector] public HashSet<ItemRef> CongPhaps { get; set; } = new();
        [ShowInInspector] public HashSet<ItemRef> VuKhis { get; set; } = new();
        [ShowInInspector] public HashSet<ItemRef> PhongCus { get; set; } = new();
        [ShowInInspector] public HashSet<ItemRef> TrangSucs { get; set; } = new();
        [ShowInInspector] public HashSet<ItemRef> LinhDans { get; set; } = new();


        public int ByteSize()
        {
            return Furnaces.ByteSize() + Recipes.ByteSize() + Herbs.ByteSize() + ThanThongs.ByteSize() +
                   CongPhaps.ByteSize() + VuKhis.ByteSize() + PhongCus.ByteSize() + TrangSucs.ByteSize() +
                   LinhDans.ByteSize();
        }

        public void Serialize(IWriter writer)
        {
            writer.WriteArray(Furnaces);
            writer.WriteArray(Recipes);
            writer.WriteArray(Herbs);
            writer.WriteArray(ThanThongs);
            writer.WriteArray(CongPhaps);
            writer.WriteArray(VuKhis);
            writer.WriteArray(PhongCus);
            writer.WriteArray(TrangSucs);
            writer.WriteArray(LinhDans);
        }

        public void Deserialize(IReader reader, ref InventoryManager value)
        {
            value.Furnaces = reader.ReadArray<FurnaceData>().ToHashSet();
            value.Recipes = reader.ReadArray<ItemRef>().ToHashSet();
            value.Herbs = reader.ReadArray<ItemRef>().ToHashSet();
            value.ThanThongs = reader.ReadArray<ItemRef>().ToHashSet();
            value.CongPhaps = reader.ReadArray<ItemRef>().ToHashSet();
            value.VuKhis = reader.ReadArray<ItemRef>().ToHashSet();
            value.PhongCus = reader.ReadArray<ItemRef>().ToHashSet();
            value.TrangSucs = reader.ReadArray<ItemRef>().ToHashSet();
            value.LinhDans = reader.ReadArray<ItemRef>().ToHashSet();
        }

        public void Clone(InventoryManager other)
        {
            Furnaces = new HashSet<FurnaceData>(other.Furnaces);
            Recipes = new HashSet<ItemRef>(other.Recipes);
            Herbs = new HashSet<ItemRef>(other.Herbs);
            ThanThongs = new HashSet<ItemRef>(other.ThanThongs);
            CongPhaps = new HashSet<ItemRef>(other.CongPhaps);
            VuKhis = new HashSet<ItemRef>(other.VuKhis);
            PhongCus = new HashSet<ItemRef>(other.PhongCus);
            TrangSucs = new HashSet<ItemRef>(other.TrangSucs);
            LinhDans = new HashSet<ItemRef>(other.LinhDans);
        }
        public InventoryManager DeepClone()
        {
            var other = new InventoryManager();
            other.Furnaces = DeepClone(Furnaces);
            other.Recipes = DeepClone(Recipes);
            other.Herbs = DeepClone(Herbs);
            other.ThanThongs = DeepClone(ThanThongs);
            other.CongPhaps = DeepClone(CongPhaps);
            other.VuKhis = DeepClone(VuKhis);
            other.PhongCus = DeepClone(PhongCus);
            other.TrangSucs = DeepClone(TrangSucs);
            other.LinhDans = DeepClone(LinhDans);
            return other;
        }
        private HashSet<T> DeepClone<T>(IEnumerable<T> collection) where T : IDeepClone<T>
        {
            return new HashSet<T>(collection.Select(x => x.DeepClone()));
        }

        public void AddToInventory(RewardItems rewardItems)
        {
            var items = rewardItems.OtherItems;
            var furnaces = rewardItems.Furnaces;
            foreach (var furnaceData in furnaces)
            {
                Furnaces.Add(furnaceData);
            }

            foreach (var itemRef in items)
            {
                Add(itemRef);
            }
        }

        public void Add(ItemRef itemRef)
        {
            var type = itemRef.Type;
            if (type == ItemType.Herb)
            {
                Add(Herbs, itemRef);
            }
            else if (type == ItemType.LinhDan)
            {
                Add(LinhDans, itemRef);
            }
            else if (type == ItemType.ThanThong)
            {
                Add(ThanThongs, itemRef);
            }
            else if (type == ItemType.CongPhap)
            {
                Add(CongPhaps, itemRef);
            }
            else if (type == ItemType.VuKhi)
            {
                Add(VuKhis, itemRef);
            }
            else if (type == ItemType.PhongCu)
            {
                Add(PhongCus, itemRef);
            }
            else if (type == ItemType.TrangSuc)
            {
                Add(TrangSucs, itemRef);
            }
            else if (type == ItemType.Recipe)
            {
                Add(Recipes, itemRef);
            }
        }

        private void Add(HashSet<ItemRef> inventory, ItemRef itemRef)
        {
            if (inventory.TryGetValue(itemRef, out var item))
            {
                item.Count += itemRef.Count;
            }
            else
            {
                inventory.Add(itemRef);
            }
        }
    }
}