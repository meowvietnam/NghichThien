using MemoryPack;

namespace Network.Models
{
    [MemoryPackable]
    public partial class SyncPlaySkill
    {
        public int CharId { get; set; }
        public int SkillId { get; set; }
    }
}