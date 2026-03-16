using LongDH.LongDHModules;
using UnityEngine;
[CreateAssetMenu(menuName = "Skill/Công Pháp /" + nameof(TamTaiHoaGiapKiep))]
public class TamTaiHoaGiapKiep : CongPhap
{
    [SerializeField]
    private int[] buffValues = new int[]
      {
        1, 1 , 2,2, 3
      };
    [SerializeField]
    private int require = 6;
    public override void Execute(CharacterCombat caster, CharacterCombat target, params object[] data)
    {
        int p = caster.CurrentLinhKhi / require;
        caster.AddBuffAndDebuffCount(EBuffAndDeBuff.HoTrao, p * buffValues[CurrentLevelCongPhapInCombat(caster) - 1]);

    }

    public override string GetDesc(int level)
    {
        return $"Khi kết thúc hiệp, mỗi có {GetColoredText($"{require}", "red")} điểm linh khí thì thu hoạch được {GetColoredText($"+{buffValues[level - 1]}", "red")} tầng {GetKeyWordBuffOrDeBuff(EBuffAndDeBuff.HoTrao)}";
    }

    public override void InitPosCallExecute(CharacterCombat caster)
    {
        Observer.AddObserver(EventList.Combat.END_TURN_COMBAT, ExecuteParams);

    }

}
