using System;
using System.Collections.Generic;
using MemoryPack;

namespace DP
{
    [MemoryPackable]
    public partial class DongPhuInventory
    {
        public int CharId { get; set; }
        public HashSet<Seed> Seeds { get; set; } = new();
        public event Action<Seed> OnAddSeed;

        public void AddOrUpdate(Seed seed)
        {
            if (Seeds.TryGetValue(seed, out var mySeed))
            {
                var newCount = seed.Count + mySeed.Count;
                mySeed.SetCount(newCount);
            }
            else
            {
                Seeds.Add(seed);
                OnAddSeed?.Invoke(seed);
            }
        }
    }
}