using LuyenDan;
using MemoryPack;
using UnityEngine;
namespace Network.Models
{
    [MemoryPackable]
    public partial class TangQuaNpc
    {
        public int IdChar { get; set; }
        public ItemRef ItemRef { get; set; }

    }
}

