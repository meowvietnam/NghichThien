using UnityEngine;
[CreateAssetMenu]
public class PhaThe : ThanThong , BaseSkill.TuongDong
{
    private int debuff = 5;
    private int debuffTuongDong = 2;

    public override void Execute(CharacterCombat caster, CharacterCombat target, params object[] data)
    {
        target.AddBuffAndDebuffCount(EBuffAndDeBuff.DeTonThuong, debuff);
    }

    public void ExecuteTuongDong(CharacterCombat caster, CharacterCombat target)
    {
        target.AddBuffAndDebuffCount(EBuffAndDeBuff.DeTonThuong, debuffTuongDong);
    }

    public override string GetDesc()
    {
        return $"Đối thủ {GetColoredText($"+{debuff}", "red")} tầng {GetKeyWordBuffOrDeBuff(EBuffAndDeBuff.DeTonThuong)}. Nếu {GetKeyWord(EKeyWord.TuongDong)} đối phương nhận thêm {GetColoredText($"+{debuffTuongDong}", "red")} tầng {GetKeyWordBuffOrDeBuff(EBuffAndDeBuff.DeTonThuong)}";

    }


}
