using LongDH.LongDHModules;
using System.Linq;
using UnityEngine;
[CreateAssetMenu(fileName = nameof(BangVuThuat), menuName = "Skill/Thần Thông/" + nameof(BangVuThuat))]

public class BangVuThuat : ThanThong
{
    [SerializeField] private int[] suongGias;
    public override void Execute(CharacterCombat caster, CharacterCombat target, params object[] data)
    {
        caster.AddThanThongImpactAfterPlaySkill(this);
        Observer.AddObserver(EventList.GamePlay.END_TURN_COMBAT, ObserverEffect);

    }
    private void ObserverEffect(params object[] data)
    {
        CharacterCombat currentTurn = (CharacterCombat)data[0];
        CharacterCombat targetTurn = (CharacterCombat)data[1];

        if (currentTurn == null) return;

        if (currentTurn.specialThanThongImpactAfterPlaySkill.Any(x => x is BangVuThuat))
        {
            targetTurn.buffAndDebuffCount.TryGetValue(EBuffAndDeBuff.HoTrao, out int hotrao);
            if(hotrao<=0)
            {
                targetTurn.AddBuffAndDebuffCount(EBuffAndDeBuff.SuongGia, suongGias[GetIndexLevelThanThong(currentTurn)]);
            }    

        }


    }
    public override string GetDesc(int level)
    {
        return $"Kết thúc hiệp, Nếu đối thủ không có {GetKeyWordBuffOrDeBuff(EBuffAndDeBuff.HoTrao)} thì nhận phải {GetColoredText($"+{suongGias[level-1]}", "red")} tầng {GetKeyWordBuffOrDeBuff(EBuffAndDeBuff.SuongGia)} {GetColoredText($"Mỗi cuộc chiến chỉ có thể sử dụng một lần", "red")}";

    }
    public override bool IsCanUse(CharacterCombat caster, CharacterCombat target)
    {
        if ( !caster.thanThongUseThisComBat.Any(x => x.Any(y => y.thanThong is BangVuThuat)))
        {
            return true;
        }
        return false;
    }
}
