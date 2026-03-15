using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[CreateAssetMenu(fileName = nameof(ThoiTamTrao), menuName = "Skill/Thần Thông/" + nameof(ThoiTamTrao))]
public class ThoiTamTrao : CuLinhTrao
{

    protected override List<CharacterCombat.PlaySkillData>
GetListSkillUseCheck(CharacterCombat caster)
    {
        return caster.thanThongUseThisComBat
                     .SelectMany(t => t)
                     .ToList();
    }

    protected override string GetLogicText()
    {
        return "Trong trận đấu này";

    }
}
