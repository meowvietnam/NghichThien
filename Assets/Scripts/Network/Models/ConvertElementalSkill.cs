using System.Collections.Generic;
using LuyenDan;
using MemoryPack;

namespace Network.Models
{
    [MemoryPackable]
    public partial class ConvertElementalSkill
    {
        public HashSet<ItemRef> ItemConverts { get; set; }
    }
}