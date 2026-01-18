using LongDH.LongDHModules;
using UnityEngine;
[CreateAssetMenu]
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

    public override void Init()
    {
        InitPosCallExecute();
    }

    public override void InitPosCallExecute()
    {
        Observer.AddObserver(EventList.GamePlay.NEW_TURN_COMBAT, ExecuteParams);
    }

}
