using Skill;
using UnityEngine;
[CreateAssetMenu(fileName = nameof(VanThoQuyTran), menuName = "Skill/Thần Thông/" + nameof(VanThoQuyTran))]

public class VanThoQuyTran : ThanThong
{
    [SerializeField]
    private int[] buffValues = new int[]
    {
        1,3,7,20,40
    };
    public override void Execute(CharacterCombat caster, CharacterCombat target, params object[] data)
    {
        HauKeVoLuc(caster);
        caster.AddBuffAndDebuffCount(EBuffAndDeBuff.Tran, caster.khiTho * buffValues[GetIndexLevelThanThong(caster)]);
        caster.TieuTanLinhKhi(ELinhKhi.Tho, caster.khiTho);
    }



    public override string GetDesc(int level)
    {
        return $"{GetKeyWord(EKeyWord.HauKeVoLuc)} Tiêu tán tất cả {GetTextLinhKhi(ELinhKhi.Tho)} hệ linh khí, mỗi tiêu tán một điểm thu hoạch được {GetColoredText($"+{buffValues[level - 1]}", "red")} tầng {GetKeyWordBuffOrDeBuff(EBuffAndDeBuff.Tran)}";

    }
}
