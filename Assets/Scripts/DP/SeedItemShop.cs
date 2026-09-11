using System;
using Auction;
using LuyenDan;
using MemoryPack;

namespace DP
{
    [MemoryPackable]
    public partial class SeedItemShop : IEquatable<SeedItemShop>
    {
        public int CharId { get; set; }
        public int PricePerSeed { get; set; }
        public ushort HerbId { get; set; }
        public int Count { get; set; }
        public event Action<int> OnCountChanged;

        public override int GetHashCode()
        {
            return HerbId;
        }

        public LuyenDan.HerbData GetHerb()
        {
            return LuyenDanResources.Instance.FindHerb(HerbId);
        }

        public void SetCount(int count)
        {
            Count = count;
            OnCountChanged?.Invoke(count);
        }

        public bool Equals(SeedItemShop? other)
        {
            if (other is null) return false;
            if (ReferenceEquals(this, other)) return true;
            return HerbId == other.HerbId;
        }
    }
}