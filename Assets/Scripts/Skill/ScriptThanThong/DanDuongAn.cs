using UnityEngine;
[CreateAssetMenu(fileName = nameof(DanDuongAn), menuName = "Skill/Thần Thông/" + nameof(DanDuongAn))]

public class DanDuongAn : ThanThong
{
    
    [SerializeField]
    private int[] valueBuffs = new int[]
    {
        2,3,4,5,6
    };

    public override void Execute(CharacterCombat caster, CharacterCombat target, params object[] data)
    {
        HauKeVoLuc(caster);
        caster.AddBuffAndDebuffCount(EBuffAndDeBuff.Diem, valueBuffs[GetIndexLevelThanThong(caster)]);
       


    }



    public override string GetDesc(int level)
    {
        return $"{GetKeyWord(EKeyWord.HauKeVoLuc)} thu hoạch được {GetColoredText($"+{valueBuffs[level - 1]}", "red")} tầng {GetKeyWordBuffOrDeBuff(EBuffAndDeBuff.Diem)}";

    }
}
