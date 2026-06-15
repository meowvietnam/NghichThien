using System.Linq;
using Base;
using ComboSkillResource;
using MemoryPack;

namespace Snapshot
{
    [MemoryPackable]
    public partial class ComboWithType
    {
        public string NameCombo { get; set; }
        public Combo[] ComboWithDaiCanhGioi  { get; set; }
        public BaseSkill.ETypeSkill[] ETypeSkill { get; set; }
        public ETypeCombo TypeCombo { get; set; }
        public EMonPhai MonPhai { get; set; }
        [MemoryPackConstructor]
        public ComboWithType()
        {
            
        }
        public ComboWithType(global::ComboSkillResource.ComboSkillResource.ComboWithType clone)
        {
            NameCombo = clone.nameCombo;
            ComboWithDaiCanhGioi = clone.comboWithDaiCanhGioi.Select(x => new Combo(x)).ToArray();
            ETypeSkill = clone.eTypeSkill;
            TypeCombo = clone.typeCombo;
            MonPhai = clone.monPhai;
        }
    }
}