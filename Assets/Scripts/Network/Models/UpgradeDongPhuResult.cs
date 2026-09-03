using MemoryPack;
using DP;
namespace Network.Models
{
    [MemoryPackable]
    public partial class UpgradeDongPhuNotify
    {
        public DongPhuLevel NewLevel { get; set; }
    }
    [MemoryPackable]
    public partial class UpgradeDongPhuResult : Result
    {
        public DongPhuLevel NewLevel { get; set; }
        public int NewMoneyVip { get; set; }
    }
}
