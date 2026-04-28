using LongDH.LongDHModules;
using Skill;
using UnityEngine;
[CreateAssetMenu(menuName = "Skill/Công Pháp /" + nameof(DungThuanQuyet))]

public class DungThuanQuyet : CongPhap
{
    [SerializeField]
    private int[] buffValues = new int[]
      {
        5, 10 , 20 ,40, 80
      };
   
    
    public override void Execute(CharacterCombat caster, CharacterCombat target, params object[] data)
    {
        caster.buffAndDebuffCount.TryGetValue(EBuffAndDeBuff.HoaDiem, out SecureInt hoaDiem);
        if (hoaDiem > 0)
        {
            caster.AddBuffAndDebuffCount(EBuffAndDeBuff.HoTrao, buffValues[CurrentLevelCongPhapInCombat(caster) - 1]);

        }
       


    }
    public override string GetDesc(int level)
    {
        return $"Kết thúc hiệp, nếu bản thân đang có trạng thái {GetKeyWordBuffOrDeBuff(EBuffAndDeBuff.HoaDiem)} thì thu hoạch được {GetColoredText($"+{buffValues[level - 1]}", "red")} tầng {GetKeyWordBuffOrDeBuff(EBuffAndDeBuff.HoTrao)}";
    }

    public override void InitPosCallExecute(CharacterCombat caster)
    {
        Observer.AddObserver(EventList.Combat.END_TURN_COMBAT, ExecuteParams);

    }
}
