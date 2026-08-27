using System;
using System.Collections.Generic;
using System.Linq;
using CreateCharacter;
using LuyenDan;
using MemoryPack;

namespace DP
{
    [MemoryPackable]
    public partial class PlantSeed : IEquatable<PlantSeed>
    {
        public int CharId { get; set; }
        public ushort SlotIndex { get; set; }
        public ushort HerbId { get; set; }
        public DateTime PlantAt { get; set; }
        public DateTime EstimateHarvestAt { get; set; }
        public bool IsHavest { get; set; }
        public event Action OnHarvest;
        public bool Equals(PlantSeed? other)
        {
            if (other is null) return false;
            if (ReferenceEquals(this, other)) return true;
            return SlotIndex == other.SlotIndex;
        }

        public override int GetHashCode()
        {
            return SlotIndex;
        }
    }
}