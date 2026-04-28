using Skill;
using UnityEngine;
[CreateAssetMenu(fileName = nameof(TranHonChuong), menuName = "Skill/Thần Thông/" + nameof(TranHonChuong))]

public class TranHonChuong : ThanThong
{

    [SerializeField]
    private int[] dames = new int[]
    {
        6,8,14,26,52
    };
    [SerializeField]
    private int[] buffValues = new int[]
    {
        1,2,3,4,5
    };
    public override void Execute(CharacterCombat caster, CharacterCombat target, params object[] data)
    {
        target.buffAndDebuffCount.TryGetValue(EBuffAndDeBuff.ChayMau, out SecureInt  chayMau);
        int dame = dames[GetIndexLevelThanThong(caster)] + chayMau * buffValues[GetIndexLevelThanThong(caster)];
        target.CaculateDameByThanThong(caster, new System.Collections.Generic.List<int> { dame }, eTypeSkill);
        caster.Healing(dame);
    }



    public override string GetDesc(int level)
    {
        return $"Tạo thành {GetColoredText($"+{dames[level - 1]}", "red")} điểm sát thương. Mỗi có một tầng {GetKeyWordBuffOrDeBuff(EBuffAndDeBuff.ChayMau)} tăng lên {GetColoredText($"+{buffValues[level - 1]}", "red")} điểm sát thương. Đồng thời, hồi một lượng HP tương đương sát thương gây ra";

    }
}
