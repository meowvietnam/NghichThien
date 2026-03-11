using LongDH.LongDHModules;
using UnityEngine;
[CreateAssetMenu(fileName = nameof(SuongThienHanNguyetTran), menuName = "Skill/Thần Thông/" + nameof(SuongThienHanNguyetTran))]
public class SuongThienHanNguyetTran : TranPhapTT
{
    [SerializeField]
    private int[] buffValues = new int[]
    {
        4,5,6,8,15
    };
    public override void Execute(CharacterCombat caster, CharacterCombat target, params object[] data)
    {
        base.Execute(caster, target, data);
        Observer.AddObserver(EventList.GamePlay.NEW_TURN_COMBAT, ObserverEffect);


    }
    public override void ExecuteTranPhap(CharacterCombat caster, CharacterCombat target)
    {
        target.AddBuffAndDebuffCount(EBuffAndDeBuff.SuongGia, buffValues[GetIndexLevelThanThong(caster)]);
    }

    public override string GetDescTranPhapTT(int level)
    {
        return $"Bắt đầu hiệp, đối thủ nhận phải {GetColoredText($"+{buffValues[level - 1]}", "red")} tầng {GetKeyWordBuffOrDeBuff(EBuffAndDeBuff.SuongGia)}";
    }
}
