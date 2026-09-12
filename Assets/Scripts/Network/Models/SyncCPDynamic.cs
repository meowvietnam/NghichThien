
using System.Collections.Generic;
using MemoryPack;

namespace Network.Models
{
    [MemoryPackable]
    public partial class SyncCPDynamic
    {
        public int CharId { get; set; }
        public int IdCpDynamic { get; set; }
        public int NewCpDynamic { get; set; }

    }
}