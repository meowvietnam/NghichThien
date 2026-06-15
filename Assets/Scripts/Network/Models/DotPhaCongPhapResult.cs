using System;
using System.Collections.Generic;
using CreateCharacter;
using LuyenDan;
using MemoryPack;

namespace Network.Models
{
    [MemoryPackable]
    public partial class DotPhaCongPhapResult
    {
        public bool Success { get; set; }
        public string? Message { get; set; }
        public ushort Id { get; set; }
        public int RemainingMoney { get; set; }
        public TimeSpan NewTuoiTac { get; set; }
        public List<CamNgo> CamNgos { get; set; }
    }
}