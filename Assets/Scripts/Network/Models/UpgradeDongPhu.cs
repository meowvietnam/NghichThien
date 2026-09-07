using MemoryPack;

namespace Network.Models
{
    public enum UpgradeDongPhuBy
    {
        MoneyVip,
        Iap,
    }

    [MemoryPackable]
    public partial class UpgradeDongPhu
    {
        public UpgradeDongPhuBy By { get; set; }
    }
}