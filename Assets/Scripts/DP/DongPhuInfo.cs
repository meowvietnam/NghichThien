using MemoryPack;

namespace DP
{
    [MemoryPackable]
    public partial class DongPhuInfo
    {
        public DongPhuConfig Config { get; set; }
        public DongPhu DongPhu { get; set; }
        public DongPhuLevel MyLevel { get; set; }
        public int CurrentWateringHelpCount { get; set; }
        public bool CanLike { get; set; }
    }
}
