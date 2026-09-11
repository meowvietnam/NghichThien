using System;
using MemoryPack;

namespace DP
{
    [MemoryPackable]
    public partial class Seed : IEquatable<Seed>
    {
        public int CharId { get; set; }
        public ushort HerbId { get; set; }
        public int Count { get; set; }
        public event Action<int> OnCountUpdated;

        public void SetCount(int count)
        {
            Count = count;
            OnCountUpdated?.Invoke(count);
        }

        public override int GetHashCode()
        {
            return HerbId;
        }

        public bool Equals(Seed? other)
        {
            if (other is null) return false;
            if (ReferenceEquals(this, other)) return true;
            return HerbId == other.HerbId;
        }

        public Seed Clone()
        {
            return new Seed()
            {
                CharId = CharId,
                HerbId = HerbId,
                Count = Count,
            };
        }
    }
}