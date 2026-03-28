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
        public HashSet<int> Recipes { get; set; } = new();
        [ShowInInspector]
        public HashSet<ItemRef> Herbs { get; set; } = new();
        public int ByteSize()
        {
            return Furnaces.ByteSize() + Recipes.ByteSize() + Herbs.ByteSize();
        }
        public void Serialize(IWriter writer)
        {
            writer.WriteArray(Furnaces);
            writer.WriteArray(Recipes);
            writer.WriteArray(Herbs);
        }
        public void Deserialize(IReader reader, ref InventoryManager value)
        {
            value.Furnaces = reader.ReadArray<FurnaceData>().ToHashSet();
            value.Recipes = reader.ReadArray<int>().ToHashSet();
            value.Herbs = reader.ReadArray<ItemRef>().ToHashSet();
        }

        public void Clone(InventoryManager other)
        {
            Furnaces = new HashSet<FurnaceData>(other.Furnaces);
            Recipes = new HashSet<int>(other.Recipes);
            Herbs = new HashSet<ItemRef>(other.Herbs);
        }
    }
}