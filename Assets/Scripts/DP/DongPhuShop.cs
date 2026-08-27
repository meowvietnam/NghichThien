using System;
using System.Collections.Generic;
using MemoryPack;

namespace DP
{
    [MemoryPackable]
    public partial class DongPhuShop
    {
        public int CharId { get; set; }
        public HashSet<SeedItemShop> Seeds { get; set; } = new();

        public void UpdateSeed(SeedItemShop updatedSeed)
        {
            if(Seeds.TryGetValue(updatedSeed, out var ownerSeed))
            {
                ownerSeed.SetCount(updatedSeed.Count);
                if(ownerSeed.Count <= 0)
                {
                    Seeds.Remove(ownerSeed);
                }
            }
        }
    }
}