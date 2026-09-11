using System.Collections.Generic;
using System.Linq;
using MemoryPack;

namespace Snapshot
{
    [MemoryPackable]
    public partial class ComboSkillResource
    {
        public ComboWithType[] ComboNpc { get; set; }

        [MemoryPackConstructor]
        public ComboSkillResource()
        {
        }

        public ComboSkillResource(global::ComboSkillResource.ComboSkillResource clone)
        {
            ComboNpc = clone.comboNPC.Select(x => new ComboWithType(x)).ToArray();
        }
    }
}