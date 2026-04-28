using Skill;
using UnityEngine;
[CreateAssetMenu(fileName = nameof(HuyenLinhThuat), menuName = "Skill/Thần Thông/" + nameof(HuyenLinhThuat))]

public class HuyenLinhThuat : ThanThong
{
    
    [SerializeField]
    private int[] buffValues = new int[]
    {
        5,5,5,5,5
    };
    public override void Execute(CharacterCombat caster, CharacterCombat target, params object[] data)
    {
        HauKeVoLuc(caster);
        caster.HapThuLinhKhi(buffValues[GetIndexLevelThanThong(caster)]);
        caster.AddBuffAndDebuffCount(EBuffAndDeBuff.TreKhi, buffValues[GetIndexLevelThanThong(caster)]);

    }



    public override string GetDesc(int level)
    {
        return $"{GetKeyWord(EKeyWord.HauKeVoLuc)} Hấp thụ {GetColoredText($"+{buffValues[level - 1]}", "red")} điểm linh khí. Thu hoạch được {GetColoredText($"+{buffValues[level - 1]}", "red")} tầng {GetKeyWordBuffOrDeBuff(EBuffAndDeBuff.TreKhi)}";

    }
}
