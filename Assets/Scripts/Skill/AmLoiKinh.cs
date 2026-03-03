using UnityEngine;
[CreateAssetMenu(fileName = nameof(AmLoiKinh), menuName = "Skill/Thần Thông/" + nameof(AmLoiKinh))]

public class AmLoiKinh : AmKinhTT , BaseSkill.TuongSinh
{
    [SerializeField]
    private int[] dames = new int[]
    {
        10,15,24,44,88
    };
    [SerializeField]
    private int[] buffValues = new int[]
    {
        1,2,3,5,10
    };
    public override void ExecuteAmKinh(CharacterCombat caster, CharacterCombat target)
    {
        int dame = dames[GetIndexLevelThanThong(caster)];
        target.AddDamageData(new System.Collections.Generic.List<int> { dame } , false);
        target.TakeDame(caster);
    }

    public void ExecuteTuongSinh(CharacterCombat caster, CharacterCombat target)
    {
        caster.AddBuffAndDebuffCount(EBuffAndDeBuff.HoTrao, buffValues[GetIndexLevelThanThong(caster)]);
    }

    public override string GetDescAmKinhTT(int level)
    {
        return $"{GetKeyWord(EKeyWord.TuongSinh)} Thu hoạch được {GetColoredText($"+{buffValues[level - 1]}", "red")} tầng {GetKeyWordBuffOrDeBuff(EBuffAndDeBuff.HoTrao)}";
    }
}
