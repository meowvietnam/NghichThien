using LongDH.LongDHModules;
using UnityEngine;
[CreateAssetMenu(fileName = nameof(DuLinhTran), menuName = "Skill/Thần Thông/" + nameof(DuLinhTran))]

public class DuLinhTran : TranPhapTT
{
    [SerializeField]
    private int[] buffValues = new int[]
     {
        3,4,5,6,11
     };
    public override void Execute(CharacterCombat caster, CharacterCombat target, params object[] data)
    {
        base.Execute(caster, target, data);
        Observer.AddObserver(EventList.Combat.END_TURN_COMBAT, ObserverEffect);


    }
    public override void ExecuteTranPhap(CharacterCombat caster, CharacterCombat target)
    {
        caster.AddBuffAndDebuffCount(EBuffAndDeBuff.TriLieu, buffValues[GetIndexLevelThanThong(caster)]);
    }

    public override string GetDescTranPhapTT(int level)
    {
        return $"Kết thúc hiệp, thu hoạch được {GetColoredText($"+{buffValues[level - 1]}", "red")} tầng {GetKeyWordBuffOrDeBuff(EBuffAndDeBuff.TriLieu)}";
    }
}
