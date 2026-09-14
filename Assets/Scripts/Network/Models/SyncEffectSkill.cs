
using System.Collections.Generic;
using MemoryPack;

namespace Network.Models
{
    [MemoryPackable]
    public partial class SyncEffectSkill
    {
        public int CharId { get; set; } // charcast effect
        public int IdSkill { get; set; }
        public bool IsThanThong { get; set; }
    }
}