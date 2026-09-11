using LuyenDan;
using MemoryPack;
using UnityEngine;

namespace Network.Models
{
    [MemoryPackable]
    public partial class ThinhGiaoKyNangResult
    {
        public bool Success { get; set; }
        public string? Message { get; set; }
        public int IdChar { get; set; }
        public int NewValueRelationship { get; set; }
        public ItemRef ItemRefSkill { get; set; }
    }
}