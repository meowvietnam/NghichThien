using LongDH.LongDHModules;
using System.Linq;
using UnityEngine;
[CreateAssetMenu(fileName = nameof(VuTuongHoaThan), menuName = "Skill/Thần Thông/" + nameof(VuTuongHoaThan))]

public class VuTuongHoaThan : NeTranhTT
{
    [SerializeField] private int requireHuyenVu = 2;
    public override void Execute(CharacterCombat caster, CharacterCombat target, params object[] data)
    {
        caster.AddThanThongImpactAfterPlaySkill(this);

        Observer.AddObserver(EventList.Combat.CHECK_NE_TRANH, ObserverEffectNeTranh, 0);
        caster.RemoveBuffAndDebuffCount(EBuffAndDeBuff.HuyenVu, requireHuyenVu);

    }
   
    public override string GetDesc(int level)
    {
        return $"Gỡ ra {GetColoredText($"{requireHuyenVu}", "red")} tầng {GetKeyWordBuffOrDeBuff(EBuffAndDeBuff.HuyenVu)}. Triệu hoán sương mù làm thế thân giúp ngươi né tránh sát thương (Có thể cộng dồn)";

    }
    public override bool IsCanUse(CharacterCombat caster, CharacterCombat target)
    {
        caster.buffAndDebuffCount.TryGetValue(EBuffAndDeBuff.HuyenVu, out SecureInt  huyenVu);
        if (huyenVu >= requireHuyenVu && base.IsCanUse(caster, target))
        {
            return true;
        }
        return false;
    }

    protected override void NeTranhSuccess(CharacterCombat takeDameChar)
    {
        takeDameChar.RemoveOneThanThongImpactAfterPlaySkill(this);
    }
}
