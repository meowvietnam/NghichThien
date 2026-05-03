using Skill;
using UnityEngine;
[CreateAssetMenu(fileName = nameof(KhienLinhChu), menuName = "Skill/Thần Thông/" + nameof(KhienLinhChu))]

public class KhienLinhChu : ThanThong , BaseSkill.TuongSinh
{
    [SerializeField]
    private int decreaseHiep = 1;
    [SerializeField]
    private int[] buffValues = new int[]
    {
        2,3,4,6,9
    };
    public override void Execute(CharacterCombat caster, CharacterCombat target, params object[] data)
    {
        CombatStatic.AmKinhTTNewTurn(target, caster);
            


    }

    public void ExecuteTuongSinh(CharacterCombat caster, CharacterCombat target)
    {
        caster.Healing(buffValues[GetIndexLevelThanThong(caster)]);
    }

    public override string GetDesc(int level)
    {
        return $"Tất cả {GetKeyWord(EKeyWord.AmKinh)} của đối thủ số hiệp {GetColoredText($"-{decreaseHiep}", "red")}. {GetKeyWord(EKeyWord.TuongSinh)} Khôi phục {GetColoredText($"+{buffValues[level - 1]}", "red")} điểm HP";

    }
}
