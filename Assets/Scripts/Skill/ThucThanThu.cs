using UnityEngine;
[CreateAssetMenu(fileName = nameof(ThucThanThu), menuName = "Skill/Thần Thông/" + nameof(ThucThanThu))]
public class ThucThanThu : ThanThong, BaseSkill.ThanThuc
{
    [SerializeField]
    private int[] dames = new int[]
     {
        3,6,10,22,50
     };
    [SerializeField]
    private int[] buffValues = new int[]
    {
        1,1,2,2,3
    };
    public override void Execute(CharacterCombat caster, CharacterCombat target, params object[] data)
    {
        if (caster.buffAndDebuffCount.TryGetValue(EBuffAndDeBuff.AnNac, out SecureInt anNac))
        {
            caster.AddBuffAndDebuffCount(EBuffAndDeBuff.AnNac, 1);
        }
        int dame = dames[GetIndexLevelThanThong(caster)];
        target.TakeDameByThanThong(caster, new System.Collections.Generic.List<int> { dame }, eTypeSkill);
    }

    public void ExecuteThanThuc(CharacterCombat caster, CharacterCombat target)
    {
  
        target.AddBuffAndDebuffCount(EBuffAndDeBuff.ThanThuong, buffValues[GetIndexLevelThanThong(caster)]);

    }

    public override string GetDesc(int level)
    {
        return $"Tạo thành {GetColoredText($"+{dames[level - 1]}", "red")} điểm sát thương. {GetKeyWord(EKeyWord.ThanThuc)} Đối thủ nhận phải {GetColoredText($"+{buffValues[level - 1]}", "red")} tầng {GetKeyWordBuffOrDeBuff(EBuffAndDeBuff.ThanThuong)}. Sử dụng thần thông này sẽ không giải trừ {GetKeyWordBuffOrDeBuff(EBuffAndDeBuff.AnNac)}";

    }
}
