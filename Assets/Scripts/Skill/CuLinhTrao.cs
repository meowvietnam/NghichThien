using System.Linq;
using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = nameof(CuLinhTrao), menuName = "Skill/Thần Thông/" + nameof(CuLinhTrao))]

public class CuLinhTrao : ThanThong
{

    [SerializeField] private int[] dames;
    [SerializeField] private int[] dameBuffs;
    public override void Execute(CharacterCombat caster, CharacterCombat target, params object[] data)
    {
        int dame = dames[GetIndexLevelThanThong(caster)];
        int countUse = GetListSkillUseCheck(caster)
            .Count(data => data.thanThong is CuLinhTrao);
        
        target.TakeDameByThanThong(caster, new List<int> { dame + (countUse * dameBuffs[GetIndexLevelThanThong(caster)])}, eTypeSkill);
    }

    protected virtual  List<CharacterCombat.PlaySkillData> GetListSkillUseCheck(CharacterCombat caster)
    {
        return caster.thanThongUseThisTurn;
    }    
    protected virtual string GetLogicText()
    {
        return "Trong hiệp này";
    }    
   
    public override string GetDesc(int level)
    {
        return $"Tạo thành {GetColoredText($"+{dames[level - 1]}", "red")} điểm sát thương,{GetColoredText($"{GetLogicText()}", "red")} mỗi lần sử dụng {GetColoredText($"[Cự Linh Trảo]", "green")} thì tổn thương {GetColoredText($"+{dameBuffs[level - 1]}", "red")} điểm sát thương";
    }
}
