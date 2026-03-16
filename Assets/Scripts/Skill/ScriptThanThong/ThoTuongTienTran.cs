using LongDH.LongDHModules;
using UnityEngine;
[CreateAssetMenu(fileName = nameof(ThoTuongTienTran), menuName = "Skill/Thần Thông/" + nameof(ThoTuongTienTran))]
public class ThoTuongTienTran : TranPhapTT
{
    [SerializeField]
    private int[] buffValues = new int[]
       {
        4,6,8,15,30
       };
    public override void Execute(CharacterCombat caster, CharacterCombat target, params object[] data)
    {
        base.Execute(caster, target, data);
        Observer.AddObserver(EventList.Combat.NEW_TURN_COMBAT, ObserverEffect);


    }
    public override void ExecuteTranPhap(CharacterCombat caster, CharacterCombat target)
    {
        caster.AddBuffAndDebuffCount(EBuffAndDeBuff.HoTrao, buffValues[GetIndexLevelThanThong(caster)]);
    }

    public override string GetDescTranPhapTT(int level)
    {
        return $"Bắt đầu hiệp, thu hoạch được {GetColoredText($"+{buffValues[level - 1]}", "red")} tầng {GetKeyWordBuffOrDeBuff(EBuffAndDeBuff.HoTrao)}";
    }
}
