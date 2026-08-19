using System;
using LuyenDan;
using MemoryPack;
using System;
using UnityEngine;

namespace Network.Models
{
    [Serializable]
    [MemoryPackable]
    public partial class CreateNewAuctionSession
    {
        [field: SerializeField] public int Price { get; set; }
        [field: SerializeField] public ItemRef Item { get; set; }
    }
}