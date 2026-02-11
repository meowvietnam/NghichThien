using System.Linq;
using UnityEngine;
[CreateAssetMenu(fileName = nameof(VuTuongHoaThan), menuName = "Skill/Thần Thông/" + nameof(VuTuongHoaThan))]

public class VuTuongHoaThan : ThanThong
{
    [SerializeField] private int requireHuyenVu = 2;
    public override void Execute(CharacterCombat caster, CharacterCombat target, params object[] data)
    {
        caster.AddThanThongImpactAfterPlaySkill(this);
        caster.RemoveBuffAndDebuffCount(EBuffAndDeBuff.HuyenVu, requireHuyenVu);

    }
   
    public override string GetDesc(int level)
    {
        return $"Gỡ ra {GetColoredText($"{requireHuyenVu}", "red")} tầng {GetKeyWordBuffOrDeBuff(EBuffAndDeBuff.HuyenVu)}. Triệu hoán sương mù làm thế thân giúp ngươi né tránh sát thương (Có thể cộng dồn)";

    }
    public override bool IsCanUse(CharacterCombat caster, CharacterCombat target)
    {
        caster.buffAndDebuffCount.TryGetValue(EBuffAndDeBuff.HuyenVu, out int huyenVu);
        if (huyenVu >= requireHuyenVu)
        {
            return true;
        }
        return false;
    }

}
