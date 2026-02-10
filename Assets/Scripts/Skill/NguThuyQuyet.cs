using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(fileName = nameof(NguThuyQuyet), menuName = "Skill/Thần Thông/" + nameof(NguThuyQuyet))]

public class NguThuyQuyet : ThanThong
{
    [SerializeField] private int hapThuLinhKhi = 2;
    [SerializeField] private int[] buffValues;
    [SerializeField]
    private float delayTimeEffect = 0.5f;
    public override void Execute(CharacterCombat caster, CharacterCombat target, params object[] data)
    {
        HauKeVoLuc(caster);
        caster.HapThuLinhKhi(ELinhKhi.Thuy, hapThuLinhKhi);
        caster.characterStatsCurrent.percentThuy += buffValues[GetIndexLevelThanThong(caster)]; 
        //ShowSkillEffect(target, delayTimeEffect);

    }

   
    public override string GetDesc(int level)
    {
        return $"{GetKeyWord(EKeyWord.HauKeVoLuc)}. Thu hoạch được {GetColoredText($"+{hapThuLinhKhi}", "red")} điểm {GetTextTypeSkill()} hệ linh khí, {GetTextTypeSkill()} linh căn thuộc tính {GetColoredText($"+{buffValues[level - 1]}", "red")}";
    }

}
