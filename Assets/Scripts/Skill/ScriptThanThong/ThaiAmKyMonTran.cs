using LongDH.LongDHModules;
using UnityEngine;
[CreateAssetMenu(fileName = nameof(ThaiAmKyMonTran), menuName = "Skill/Thần Thông/" + nameof(ThaiAmKyMonTran))]

public class ThaiAmKyMonTran : TranPhapTT
{
    [SerializeField]
    private int[] buffValues = new int[]
    {
        2,3,4,5,6
    };
    public override void Execute(CharacterCombat caster, CharacterCombat target, params object[] data)
    {
        base.Execute(caster, target, data);
        Observer.AddObserver(EventList.Combat.NEW_TURN_COMBAT, ObserverEffect);


    }
    public override void ExecuteTranPhap(CharacterCombat caster, CharacterCombat target)
    {
        target.AddBuffAndDebuffCount(EBuffAndDeBuff.DeTonThuong, buffValues[GetIndexLevelThanThong(caster)]);
    }

    public override string GetDescTranPhapTT(int level)
    {
        return $"Bắt đầu hiệp, đối thủ nhận phải {GetColoredText($"+{buffValues[level - 1]}", "red")} tầng {GetKeyWordBuffOrDeBuff(EBuffAndDeBuff.DeTonThuong)}";
    }

}
