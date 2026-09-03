using System;
using MemoryPack;

namespace Network.Models
{
    [MemoryPackable]
    public partial class RandomLuyenKhiResult : Result
    {
        public TimeSpan TuoiTac { get; set; }
    }
}