using System.Linq;
using UnityEngine;
[CreateAssetMenu(fileName = nameof(HuyetTePheHonThuat), menuName = "Skill/Thần Thông/" + nameof(HuyetTePheHonThuat))]

public class HuyetTePheHonThuat : ThanThong
{
   
    [SerializeField]
    private int[] buffValues = new int[]
    {
        4,5,6,7,8
    };
    public override void Execute(CharacterCombat caster, CharacterCombat target, params object[] data)
    {
        caster.buffAndDebuffCount.TryGetValue(EBuffAndDeBuff.Hon, out int hon);
        int heal = hon * buffValues[GetIndexLevelThanThong(caster)];

        caster.RemoveBuffAndDebuffCount(EBuffAndDeBuff.Hon, hon);

        caster.Healing(heal);

    }


    public override bool IsCanUse(CharacterCombat caster, CharacterCombat target)
    {
        if (!caster.thanThongUseThisComBat.Any(x => x.Any(y => y.thanThong is HuyetTePheHonThuat)))
        {
            return true;
        }
        return false;
    }
    public override string GetDesc(int level)
    {
        return $"Tiêu hao tất cả {GetKeyWordBuffOrDeBuff(EBuffAndDeBuff.Hon)} mỗi một tầng khôi phục {GetColoredText($"+{buffValues[level - 1]}", "red")} điểm HP {GetColoredText($"Mỗi cuộc chiến chỉ có thể sử dụng một lần", "red")}";

    }
}
