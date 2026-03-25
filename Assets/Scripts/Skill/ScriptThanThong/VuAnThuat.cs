using LongDH.LongDHModules;
using System.Linq;
using UnityEngine;
[CreateAssetMenu(fileName = nameof(VuAnThuat), menuName = "Skill/Thần Thông/" + nameof(VuAnThuat))]

public class VuAnThuat : NeTranhTT
{
    [SerializeField] private int requireHuyenVu = 4;
    [SerializeField] private float delayTimeEffect = 1f;
    public override void Execute(CharacterCombat caster, CharacterCombat target, params object[] data)
    {
        base.Execute(caster, target, data);
        caster.RemoveBuffAndDebuffCount(EBuffAndDeBuff.HuyenVu, requireHuyenVu);
        Observer.AddObserver(EventList.Combat.NEW_TURN_COMBAT, ObserverEffect);
        ShowSkillEffectWorld(caster, delayTimeEffect);

    }
    private void ObserverEffect(params object[] data)
    {
        CharacterCombat currentTurn = (CharacterCombat)data[0];
        if (currentTurn == null) return;
        if (currentTurn.specialThanThongImpactAfterPlaySkill.Any(x => x is VuAnThuat))
        {
            currentTurn.RemoveAllThanThongImpactAfterPlaySkill(this);

        }




    }
    public override string GetDesc(int level)
    {
        return $"Gỡ ra {GetColoredText($"{requireHuyenVu}", "red")} tầng {GetKeyWordBuffOrDeBuff(EBuffAndDeBuff.HuyenVu)}. Trong hiệp tiếp theo, né tránh toàn bộ sát thương. {GetColoredText($"Mỗi cuộc chiến chỉ có thể sử dụng một lần", "red")}";

    }
    public override bool IsCanUse(CharacterCombat caster, CharacterCombat target)
    {
        caster.buffAndDebuffCount.TryGetValue(EBuffAndDeBuff.HuyenVu, out SecureInt  huyenVu);
        if(huyenVu >= requireHuyenVu && !caster.thanThongUseThisComBat.Any(x => x.Any(y => y.thanThong is VuAnThuat)) && base.IsCanUse(caster, target))
        {
            return true;
        }    
        return false;
    }

    protected override void NeTranhSuccess(CharacterCombat takeDameChar)
    {
        return;
    }
}
