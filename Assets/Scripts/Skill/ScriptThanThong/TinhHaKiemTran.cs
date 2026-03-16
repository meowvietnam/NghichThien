using LongDH.LongDHModules;
using UnityEngine;
[CreateAssetMenu(fileName = nameof(TinhHaKiemTran), menuName = "Skill/Thần Thông/" + nameof(TinhHaKiemTran))]

public class TinhHaKiemTran : TranPhapTT
{
    [SerializeField]
    private int[] buffValues = new int[]
     {
        14,20,30,55,110
     };
    public override void Execute(CharacterCombat caster, CharacterCombat target, params object[] data)
    {
        base.Execute(caster, target, data);
        Observer.AddObserver(EventList.Combat.END_TURN_COMBAT, ObserverEffect);


    }
    public override void ExecuteTranPhap(CharacterCombat caster, CharacterCombat target)
    {
        caster.AddBuffAndDebuffCount(EBuffAndDeBuff.ChanKiem, buffValues[GetIndexLevelThanThong(caster)]);
    }

    public override string GetDescTranPhapTT(int level)
    {
        return $"Kết thúc hiệp, thu hoạch được {GetColoredText($"+{buffValues[level - 1]}", "red")} tầng {GetKeyWordBuffOrDeBuff(EBuffAndDeBuff.ChanKiem)}";
    }

}
