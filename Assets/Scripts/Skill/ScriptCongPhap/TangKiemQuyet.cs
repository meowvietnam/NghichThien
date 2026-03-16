using LongDH.LongDHModules;
using UnityEngine;
[CreateAssetMenu(menuName = "Skill/Công Pháp /" + nameof(TangKiemQuyet))]
public class TangKiemQuyet : CongPhap
{
    [SerializeField] private int[] kiemKhiBuff;
    public override void Execute(CharacterCombat caster, CharacterCombat target, params object[] data)
    {
        caster.AddBuffAndDebuffCount(EBuffAndDeBuff.KiemKhi, kiemKhiBuff[CurrentLevelCongPhapInCombat(caster) - 1]);
    }

    public override string GetDesc(int level)
    {
        return $"Lúc bắt đầu hiệp, thu hoạch được {GetColoredText($"+{kiemKhiBuff[level - 1]}", "red")} tầng {GetKeyWordBuffOrDeBuff(EBuffAndDeBuff.KiemKhi)}";
    }

   

    public override void InitPosCallExecute(CharacterCombat caster)
    {
        Observer.AddObserver(EventList.Combat.NEW_TURN_COMBAT, ExecuteParams);
    }

}
