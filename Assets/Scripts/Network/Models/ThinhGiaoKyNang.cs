using LuyenDan;
using MemoryPack;
using UnityEngine;

namespace Network.Models
{
    [MemoryPackable]
    public partial class ThinhGiaoKyNang
    {
        public int IdChar { get; set; }
        public int IdSkill { get; set; }
        public ItemType TypeSkill { get; set; }
    }
}