using Base;
using MemoryPack;
using System.Collections.Generic;

namespace Network.Models
{
    [MemoryPackable]
    public partial class CompleteSideQuest
    {
        public List<int> ListIdNpc { get; set; }
    }
}