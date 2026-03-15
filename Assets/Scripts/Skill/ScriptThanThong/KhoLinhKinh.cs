using System.Linq;
using UnityEngine;
[CreateAssetMenu(fileName = nameof(KhoLinhKinh), menuName = "Skill/Thần Thông/" + nameof(KhoLinhKinh))]

public class KhoLinhKinh : AmKinhTT
{
    public override string GetDescAmKinhTT(int level)
    {
        return $"đối thủ thu hoạch được {GetColoredText($"+1", "red")} tầng {GetKeyWordBuffOrDeBuff(EBuffAndDeBuff.KietSuc)} {GetColoredText($"Mỗi cuộc chiến chỉ có thể sử dụng một lần", "red")}";
    }

    public override void ExecuteAmKinh(CharacterCombat caster, CharacterCombat target)
    {
        target.AddBuffAndDebuffCount(EBuffAndDeBuff.KietSuc,1);
    }
    public override bool IsCanUse(CharacterCombat caster, CharacterCombat target)
    {
        if (!caster.thanThongUseThisComBat.Any(x => x.Any(y => y.thanThong is KhoLinhKinh)) && base.IsCanUse(caster, target))
        {
            return true;
        }
        return false;
    }
}
