using LongDH.LongDHModules;
using System.Linq;
using UnityEngine;
[CreateAssetMenu(fileName = nameof(VuAnThuat), menuName = "Skill/Thần Thông/" + nameof(VuAnThuat))]

public class VuAnThuat : ThanThong
{
    [SerializeField] private int requireHuyenVu = 4;
    public override void Execute(CharacterCombat caster, CharacterCombat target, params object[] data)
    {
        caster.AddThanThongImpactAfterPlaySkill(this);
        caster.RemoveBuffAndDebuffCount(EBuffAndDeBuff.HuyenVu, requireHuyenVu);
        Observer.AddObserver(EventList.GamePlay.NEW_TURN_COMBAT, ObserverEffect);

    }
    private void ObserverEffect(params object[] data)
    {
        CharacterCombat currentTurn = (CharacterCombat)data[0];
        if (currentTurn == null) return;
        if (currentTurn.specialThanThongImpactAfterPlaySkill.Any(x => x is VuAnThuat))
        {
            currentTurn.RemoveThanThongImpactAfterPlaySkill(this);

        }




    }
    public override string GetDesc(int level)
    {
        return $"Gỡ ra {GetColoredText($"{requireHuyenVu}", "red")} tầng {GetKeyWordBuffOrDeBuff(EBuffAndDeBuff.HuyenVu)}. Trong hiệp tiếp theo, né tránh toàn bộ sát thương. {GetColoredText($"Mỗi cuộc chiến chỉ có thể sử dụng một lần", "red")}";

    }
    public override bool IsCanUse(CharacterCombat caster, CharacterCombat target)
    {
        caster.buffAndDebuffCount.TryGetValue(EBuffAndDeBuff.HuyenVu, out int huyenVu);
        if(huyenVu >= requireHuyenVu && !caster.thanThongUseThisComBat.Any(x => x.thanThong is VuAnThuat))
        {
            return true;
        }    
        return false;
    }

   
}
