using System.Collections.Generic;
using LuyenDan;
using MemoryPack;

namespace Network.Models
{
    [MemoryPackable]
    public partial class ConvertElementalSkill
    {
        public BaseSkill.ETypeSkill TypeSkill { get; set; }
        public HashSet<ItemRef> ItemConverts { get; set; }
    }
}