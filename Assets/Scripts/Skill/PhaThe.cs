using UnityEngine;
[CreateAssetMenu(fileName = nameof(PhaThe), menuName = "Skill/Thần Thông/" + nameof(PhaThe))]
public class PhaThe : ThanThong , BaseSkill.TuongDong
{
    [SerializeField]private int[] debuffValues;
    [SerializeField]private int[] debuffTuongDong;

    public override void Execute(CharacterCombat caster, CharacterCombat target, params object[] data)
    {
        target.AddBuffAndDebuffCount(EBuffAndDeBuff.DeTonThuong, debuffValues[GetIndexLevelThanThong(caster)-1]);
    }

    public void ExecuteTuongDong(CharacterCombat caster, CharacterCombat target)
    {
        target.AddBuffAndDebuffCount(EBuffAndDeBuff.DeTonThuong, debuffTuongDong[GetIndexLevelThanThong(caster)-1]);
    }

    public override string GetDesc(int level)
    {
        return $"Đối thủ {GetColoredText($"+{debuffValues[level-1]}", "red")} tầng {GetKeyWordBuffOrDeBuff(EBuffAndDeBuff.DeTonThuong)}. Nếu {GetKeyWord(EKeyWord.TuongDong)} đối phương nhận thêm {GetColoredText($"+{debuffTuongDong[level-1]}", "red")} tầng {GetKeyWordBuffOrDeBuff(EBuffAndDeBuff.DeTonThuong)}";

    }


}
