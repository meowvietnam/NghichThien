using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Base;
using LuyenDan;
using LuyenKhi;
using Network;
using Network.Extensions;
using Network.Extensions.MemoryPack;
using Network.Models;
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
   
        [ShowInInspector] public HashSet<ItemRef> LinhDans { get; set; } = new();

        [ShowInInspector] public HashSet<NgoDao> NgoDaos { get; set; } = new();
        [ShowInInspector] public HashSet<CamNgo> CamNgos { get; set; } = new();
        [ShowInInspector] public HashSet<CongPhap> CongPhapOwners { get; set; } = new();
        [ShowInInspector] public HashSet<ThanThong> ThanThongOwners { get; set; } = new();
        [ShowInInspector] public HashSet<ItemRef> Materials { get; set; } = new();
        [ShowInInspector] public HashSet<Equipment> VuKhis { get; set; } = new();
        [ShowInInspector] public HashSet<Equipment> PhongCus { get; set; } = new();
        [ShowInInspector] public HashSet<Equipment> TrangSucs { get; set; } = new();
        public int ByteSize()
        {
            return Furnaces.ByteSize() + Recipes.ByteSize() + Herbs.ByteSize() + ThanThongs.ByteSize() +
                   CongPhaps.ByteSize() + VuKhis.ByteSize() + PhongCus.ByteSize() + TrangSucs.ByteSize() +
                   LinhDans.ByteSize() + NgoDaos.ByteSize() + CamNgos.ByteSize() + CongPhapOwners.ByteSize()
                   + ThanThongOwners.ByteSize() + Materials.ByteSize();
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
            writer.WriteArray(NgoDaos);
            writer.WriteArray(CamNgos);
            writer.WriteArray(CongPhapOwners);
            writer.WriteArray(ThanThongOwners);
            writer.WriteArray(Materials);
        }

        public void Deserialize(IReader reader, ref InventoryManager value)
        {
            value.Furnaces = reader.ReadArray<FurnaceData>().ToHashSet();
            value.Recipes = reader.ReadArray<ItemRef>().ToHashSet();
            value.Herbs = reader.ReadArray<ItemRef>().ToHashSet();
            value.ThanThongs = reader.ReadArray<ItemRef>().ToHashSet();
            value.CongPhaps = reader.ReadArray<ItemRef>().ToHashSet();
            value.VuKhis = reader.ReadArray<Equipment>().ToHashSet();
            value.PhongCus = reader.ReadArray<Equipment>().ToHashSet();
            value.TrangSucs = reader.ReadArray<Equipment>().ToHashSet();
            value.LinhDans = reader.ReadArray<ItemRef>().ToHashSet();
            value.NgoDaos = reader.ReadArray<NgoDao>().ToHashSet();
            value.CamNgos = reader.ReadArray<CamNgo>().ToHashSet();
            value.CongPhapOwners = reader.ReadArray<CongPhap>().ToHashSet();
            value.ThanThongOwners = reader.ReadArray<ThanThong>().ToHashSet();
            value.Materials = reader.ReadArray<ItemRef>().ToHashSet();
        }

        public void Clone(InventoryManager other)
        {
            Furnaces = new HashSet<FurnaceData>(other.Furnaces);
            Recipes = new HashSet<ItemRef>(other.Recipes);
            Herbs = new HashSet<ItemRef>(other.Herbs);
            ThanThongs = new HashSet<ItemRef>(other.ThanThongs);
            CongPhaps = new HashSet<ItemRef>(other.CongPhaps);
            VuKhis = new HashSet<Equipment>(other.VuKhis);
            PhongCus = new HashSet<Equipment>(other.PhongCus);
            TrangSucs = new HashSet<Equipment>(other.TrangSucs);
            LinhDans = new HashSet<ItemRef>(other.LinhDans);
            NgoDaos = new HashSet<NgoDao>(other.NgoDaos);
            CamNgos = new HashSet<CamNgo>(other.CamNgos);
            CongPhapOwners = new HashSet<CongPhap>(other.CongPhapOwners);
            ThanThongOwners = new HashSet<ThanThong>(other.ThanThongOwners);
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
            other.NgoDaos = DeepClone(NgoDaos);
            other.CamNgos = DeepClone(CamNgos);
            other.CongPhapOwners = DeepClone(CongPhapOwners);
            other.ThanThongOwners = DeepClone(ThanThongOwners);
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
            else if (type == ItemType.Recipe)
            {
                Add(Recipes, itemRef);
            }
            else if (type == ItemType.Material)
            {
                Add(Materials, itemRef);
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
        public bool Update(ItemRef itemRef)
        {
            var array = new[]
                { Herbs, LinhDans, ThanThongs, CongPhaps, Recipes, Materials };
            return array.Any(x =>
            {
                if (x.TryGetValue(itemRef, out var item))
                {
                    item.Count = itemRef.Count;
                    return true;
                }

                return false;
            });
        }
        public bool Remove(ItemRef itemRef)
        {
            var array = new[]
                { Herbs, LinhDans, ThanThongs, CongPhaps, Recipes, Materials };
            return array.Any(x =>
            {
                if (x.Remove(itemRef))
                {
                    return true;
                }

                return false;
            });
        }
        private void Add(HashSet<Equipment> inventory, Equipment equipment)
        {
            inventory.Add(equipment);
        }
        public bool Update(Equipment equipment)
        {
            var array = new[] { VuKhis,PhongCus,TrangSucs };
            return array.Any(x =>
            {
                if (x.TryGetValue(equipment, out var e))
                {
                    e.Name = equipment.Name;
                    return true;
                }
                return false;
            });
        }
        public bool Remove(Equipment equipment)
        {
            var array = new[] { VuKhis,PhongCus,TrangSucs };
            return array.Any(x =>
            {
                if (x.Remove(equipment))
                {
                    return true;
                }
                return false;
            });
        }
        public void Add(Equipment equipment)
        {
            var type = equipment.GetEquipmentType();
            switch (type)
            {
                case EquipmentType.Weapon:
                    Add(VuKhis, equipment);
                    break;
                case EquipmentType.Armor:
                    Add(PhongCus, equipment);
                    break;
                case EquipmentType.Jewelry:
                    Add(TrangSucs, equipment);
                    break;
            }
        }
        public void Add(NgoDao ngoDao)
        {
            NgoDaos.Add(ngoDao);
        }
        public void AddOrUpdate(CamNgo camNgo)
        {
            if (CamNgos.TryGetValue(camNgo, out var cam))
            {
                Update(cam, camNgo);
            }
            else
            {
                Add(camNgo);
            }
        }
        public void Update(CamNgo oldValue, CamNgo newValue)
        {
            oldValue.Value = newValue.Value;
        }
        public void Add(CamNgo camNgo)
        {
            CamNgos.Add(camNgo);
        }
        public void AddOrUpdate(CongPhap congPhap)
        {
            if (CongPhapOwners.TryGetValue(congPhap, out var congPhapOwner))
            {
                Update(congPhapOwner, congPhap);
            }
            else
            {
                Add(congPhap);
            }
        }
        public void Update(CongPhap oldValue, CongPhap newValue)
        {
            oldValue.Level = newValue.Level;
        }
        public void Add(CongPhap congPhap)
        {
            CongPhapOwners.Add(congPhap);
        }

        public void Add(ThanThong thanThong)
        {
            ThanThongOwners.Add(thanThong);
        }
        public void ApplyModify(InventoryModify m)
        {
            m.Apply(this);
        }

        public void EquipThanThong(ushort id)
        {
            var thanThong = new ThanThong()
            {
                Id = id,
            };
            ThanThongOwners.Add(thanThong);
        }

        public void UnEquipThanThong(ushort id)
        {
            var thanThong = new ThanThong()
            {
                Id = id,
            };
            ThanThongOwners.Remove(thanThong);
        }
    }
}