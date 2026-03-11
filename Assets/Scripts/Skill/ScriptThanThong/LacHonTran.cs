using LongDH.LongDHModules;
using UnityEngine;
[CreateAssetMenu(fileName = nameof(LacHonTran), menuName = "Skill/Thần Thông/" + nameof(LacHonTran))]

public class LacHonTran : TranPhapTT
{
    [SerializeField]
    private int[] buffValues = new int[]
     {
        1,1,1,1,1
     };
    public override void Execute(CharacterCombat caster, CharacterCombat target, params object[] data)
    {
        base.Execute(caster, target, data);
        Observer.AddObserver(EventList.GamePlay.END_TURN_COMBAT, ObserverEffect);


    }
    public override void ExecuteTranPhap(CharacterCombat caster, CharacterCombat target)
    {
        caster.buffAndDebuffCount.TryGetValue(EBuffAndDeBuff.Hon, out SecureInt hon);
        caster.AddBuffAndDebuffCount(EBuffAndDeBuff.ChanThan, hon * buffValues[GetIndexLevelThanThong(caster)]);
    }

    public override string GetDescTranPhapTT(int level)
    {
        return $"Bắt đầu hiệp, mỗi có một tầng {GetKeyWordBuffOrDeBuff(EBuffAndDeBuff.Hon)} thì thu hoạch được {GetColoredText($"+{buffValues[level - 1]}", "red")} tầng {GetKeyWordBuffOrDeBuff(EBuffAndDeBuff.ChanThan)}";
    }

}
