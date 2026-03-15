using LongDH.LongDHModules;
using UnityEngine;
[CreateAssetMenu(fileName = nameof(DiaSatKiemTran), menuName = "Skill/Thần Thông/" + nameof(DiaSatKiemTran))]

public class DiaSatKiemTran : TranPhapTT
{
    [SerializeField]
    private int[] buffValues = new int[]
      {
        3,5,6,8,10
      };
    public override void Execute(CharacterCombat caster, CharacterCombat target, params object[] data)
    {
        base.Execute(caster, target, data);
        Observer.AddObserver(EventList.GamePlay.NEW_TURN_COMBAT, ObserverEffect);


    }
    public override void ExecuteTranPhap(CharacterCombat caster, CharacterCombat target)
    {
        caster.AddBuffAndDebuffCount(EBuffAndDeBuff.NguKiem, buffValues[GetIndexLevelThanThong(caster)]);
    }

    public override string GetDescTranPhapTT(int level)
    {
        return $"Bắt đầu hiệp, thu hoạch được {GetColoredText($"+{buffValues[level - 1]}", "red")} tầng {GetKeyWordBuffOrDeBuff(EBuffAndDeBuff.NguKiem)}";
    }

}
