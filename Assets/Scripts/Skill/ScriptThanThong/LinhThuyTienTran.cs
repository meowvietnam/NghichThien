using LongDH.LongDHModules;
using UnityEngine;
[CreateAssetMenu(fileName = nameof(LinhThuyTienTran), menuName = "Skill/Thần Thông/" + nameof(LinhThuyTienTran))]
public class LinhThuyTienTran : TranPhapTT
{
    [SerializeField]
    private int[] buffValues = new int[]
    {
        7,9,13,19,35
    };
    public override void Execute(CharacterCombat caster, CharacterCombat target, params object[] data)
    {
        base.Execute(caster, target, data);
        Observer.AddObserver(EventList.GamePlay.NEW_TURN_COMBAT, ObserverEffect);


    }
    public override void ExecuteTranPhap(CharacterCombat caster, CharacterCombat target)
    {
        caster.Healing(buffValues[GetIndexLevelThanThong(caster)]);
    }

    public override string GetDescTranPhapTT(int level)
    {
        return $"Bắt đầu hiệp, khôi phục {GetColoredText($"+{buffValues[level - 1]}", "red")} điểm HP";
    }
}
