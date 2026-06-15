using System.Linq;
using MemoryPack;

namespace Snapshot
{
    [MemoryPackable]
    public partial class Combo
    {
        public ushort[] ThanThongIds { get; set; }
        public ushort[] CongPhapIds { get; set; }
        public ushort[] NgoDaoIds { get; set; }
        
        [MemoryPackConstructor]
        public Combo()
        {
            
        }
        public Combo(global::ComboSkillResource.ComboSkillResource.Combo combo)
        {
            ThanThongIds = combo.thanThongs.Select(x => (ushort)x.Id).ToArray();
            CongPhapIds = combo.congPhaps.Select(x => (ushort)x.Id).ToArray();
            NgoDaoIds = combo.ngoDaos.Select(x => (ushort)x.Id).ToArray();
        }
        
    }
}