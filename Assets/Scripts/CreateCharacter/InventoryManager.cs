using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
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
        [ShowInInspector]
        public HashSet<FurnaceData> Furnaces { get; set; } = new();
        [ShowInInspector]
        public HashSet<ItemRef> Recipes { get; set; } = new();
        [ShowInInspector]
        public HashSet<ItemRef> Herbs { get; set; } = new();

        [ShowInInspector]
        public HashSet<ItemRef> ThanThongs { get; set; } = new();
        [ShowInInspector]
        public HashSet<ItemRef> CongPhaps { get; set; } = new();
        [ShowInInspector]
        public HashSet<ItemRef> VuKhis { get; set; } = new();
        [ShowInInspector]
        public HashSet<ItemRef> PhongCus { get; set; } = new();
        [ShowInInspector]
        public HashSet<ItemRef> TrangSucs { get; set; } = new();

        public int ByteSize()
        {
            return Furnaces.ByteSize() + Recipes.ByteSize() + Herbs.ByteSize() + ThanThongs.ByteSize() + CongPhaps.ByteSize() + VuKhis.ByteSize() + PhongCus.ByteSize() + TrangSucs.ByteSize();
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

        }
    }
}