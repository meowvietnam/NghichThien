using System;
using System.Collections.Generic;
using LuyenDan;
using MemoryPack;
using Sirenix.OdinInspector;

namespace AllMaps
{
    [MemoryPackable]
    [MemoryPackUnion(0, typeof(HerbsShop))]
    [MemoryPackUnion(1, typeof(MaterialShop))]
    [MemoryPackUnion(2, typeof(SkillShop))]
    public abstract partial class Shop
    {
        [ShowInInspector] public HashSet<ItemRef> Items { get; protected set; }
        [ShowInInspector] public DateTime LastReloadTime { get; protected set; } = DateTime.UtcNow;
        public abstract EShopType GetTypeShop();
    }
}