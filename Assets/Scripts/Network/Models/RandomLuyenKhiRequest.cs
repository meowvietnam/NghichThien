using System.Collections.Generic;
using Base;
using LuyenKhi;
using MemoryPack;

namespace Network.Models
{
    [MemoryPackable]
    public partial class RandomLuyenKhiRequest
    {
        public ArtifactType SelectedType { get; set; }
        public HashSet<Tier> Tiers { get; set; }
        public int Count { get; set; }
    }
}