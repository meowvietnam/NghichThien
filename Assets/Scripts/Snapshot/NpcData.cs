using ComboSkillResource;
using ComboSkillResource.NPCData;
using MemoryPack;

namespace Snapshot
{
    [MemoryPackable]
    public partial class NpcData
    {
        public CharacterStatsSecureSnapshot CharStats { get; set; }
        public ETypeCombo ComboType { get; set; }
        [MemoryPackConstructor]
        public NpcData()
        {
            
        }
        public NpcData(NPCData npcData)
        {
            CharStats = new CharacterStatsSecureSnapshot(npcData.charStats);
            ComboType = npcData.comboChar.typeCombo;
        }
    }
}