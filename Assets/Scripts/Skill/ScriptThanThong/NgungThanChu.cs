using Skill;
using UnityEngine;
[CreateAssetMenu(fileName = nameof(NgungThanChu), menuName = "Skill/Thần Thông/" + nameof(NgungThanChu))]

public class NgungThanChu : ThanThong
{
   
    [SerializeField]
    private int[] buffValues = new int[]
    {
        1,2,3,4,5
    };
    public override void Execute(CharacterCombat caster, CharacterCombat target, params object[] data)
    {
        caster.AddBuffAndDebuffCount(EBuffAndDeBuff.NgungThan, buffValues[GetIndexLevelThanThong(caster)]);


    }



    public override string GetDesc(int level)
    {
        return $"Thu hoạch được {GetColoredText($"+{buffValues[level - 1]}", "red")} tầng {GetKeyWordBuffOrDeBuff(EBuffAndDeBuff.NgungThan)}";

    }
}
