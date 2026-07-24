using System.Collections.Generic;
using LuyenDan;
using MemoryPack;

namespace Network.Models
{
    [MemoryPackable]
    public partial class ConvertElementalSkillResult : Result
    {
        public HashSet<ItemRef> ItemConverts { get; set; }
        public ItemRef ItemResult { get; set; }
    }
}