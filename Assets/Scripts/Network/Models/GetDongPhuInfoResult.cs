using DP;
using MemoryPack;

namespace Network.Models
{
    [MemoryPackable]
    public partial class GetDongPhuInfoResult : Result
    {
        public DongPhuInfo DongPhuInfo { get; set; }
    }
}
