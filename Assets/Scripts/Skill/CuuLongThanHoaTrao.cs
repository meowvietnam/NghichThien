using LongDH.LongDHModules;
using UnityEngine;
[CreateAssetMenu(fileName = nameof(CuuLongThanHoaTrao), menuName = "Skill/Thần Thông/" + nameof(CuuLongThanHoaTrao))]

public class CuuLongThanHoaTrao : TranPhapTT
{
    [SerializeField]
    private int[] buffValues = new int[]
      {
        2,3,4,6,9
      };

    public override void Execute(CharacterCombat caster, CharacterCombat target, params object[] data)
    {
        base.Execute(caster, target, data);
        Observer.AddObserver(EventList.GamePlay.END_TURN_COMBAT, ObserverEffect);


    }
    public override void ExecuteTranPhap(CharacterCombat caster, CharacterCombat target)
    {
        caster.AddBuffAndDebuffCount(EBuffAndDeBuff.HoaDiem, buffValues[GetIndexLevelThanThong(caster)]);
    }

    public override string GetDescTranPhapTT(int level)
    {
        return $"Kết thúc hiệp, thu hoạch được {GetColoredText($"+{buffValues[level - 1]}", "red")} tầng {GetKeyWordBuffOrDeBuff(EBuffAndDeBuff.HoaDiem)}";
    }
}
