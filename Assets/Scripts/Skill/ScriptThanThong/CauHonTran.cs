using LongDH.LongDHModules;
using UnityEngine;
[CreateAssetMenu(fileName = nameof(CauHonTran), menuName = "Skill/Thần Thông/" + nameof(CauHonTran))]
public class CauHonTran : TranPhapTT
{
    [SerializeField]
    private int[] buffValues = new int[]
     {
        3,4,5,7,10
     };
    [SerializeField]
    private int[] requires = new int[]
    {
        2,5,7,10,13
    };
    public override void Execute(CharacterCombat caster, CharacterCombat target, params object[] data)
    {
        base.Execute(caster, target, data);
        Observer.AddObserver(EventList.Combat.END_TURN_COMBAT, ObserverEffect);


    }
    public override void ExecuteTranPhap(CharacterCombat caster, CharacterCombat target)
    {
        caster.TakeDameByStatic(null, new System.Collections.Generic.List<int> { requires[GetIndexLevelThanThong(caster)] },false,null,false);
        caster.AddBuffAndDebuffCount(EBuffAndDeBuff.Hon, buffValues[GetIndexLevelThanThong(caster)]);
    }

    public override string GetDescTranPhapTT(int level)
    {
        return $"Kết thúc hiệp, Nhận {GetColoredText($"{requires[level - 1]}", "red")} điểm {GetKeyWord(EKeyWord.ChanThucTonThuong)} thu hoạch được {GetColoredText($"+{buffValues[level - 1]}", "red")} tầng {GetKeyWordBuffOrDeBuff(EBuffAndDeBuff.Hon)}";
    }
}
