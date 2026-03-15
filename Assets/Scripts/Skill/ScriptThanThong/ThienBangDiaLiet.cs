using UnityEngine;
[CreateAssetMenu(fileName = nameof(ThienBangDiaLiet), menuName = "Skill/Thần Thông/" + nameof(ThienBangDiaLiet))]

public class ThienBangDiaLiet : ThanThong
{
    [SerializeField]
    private int[] dames = new int[]
  {
        24,34,54,96,192
  };

    public override void Execute(CharacterCombat caster, CharacterCombat target, params object[] data)
    {
        int dame = dames[GetIndexLevelThanThong(caster)];
        caster.buffAndDebuffCount.TryGetValue(EBuffAndDeBuff.Tran, out SecureInt  tran);
        dame += tran;
        caster.RemoveBuffAndDebuffCount(EBuffAndDeBuff.Tran, tran);
        target.CaculateDameByThanThong(caster, new System.Collections.Generic.List<int> { dame }, eTypeSkill);

    }



    public override string GetDesc(int level)
    {
        return $"Tạo thành {GetColoredText($"+{dames[level - 1]}", "red")} điểm sát thương. Tiêu hao tất cả {GetKeyWordBuffOrDeBuff(EBuffAndDeBuff.Tran)} mỗi tiêu hao 1 tầng tăng lên 1 điểm sát thương";

    }
}
