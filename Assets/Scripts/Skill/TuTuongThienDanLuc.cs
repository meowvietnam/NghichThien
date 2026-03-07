using LongDH.LongDHModules;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
[CreateAssetMenu(menuName = "Skill/Công Pháp /" + nameof(TuTuongThienDanLuc))]
public class TuTuongThienDanLuc : CongPhap
{
    [SerializeField]
    private int[] buffValues = new int[]
          {
            8, 14 , 26,50, 86
          };

    public override void Execute(CharacterCombat caster, CharacterCombat target, params object[] data)
    {
        caster.buffAndDebuffCount.TryGetValue(EBuffAndDeBuff.DanLuc, out SecureInt  danLuc);
        caster.AddBuffAndDebuffCount(EBuffAndDeBuff.Tran, danLuc * buffValues[CurrentLevelCongPhapInCombat(caster) - 1]);


    }

    public override string GetDesc(int level)
    {
        return $"Mỗi có một tầng {GetKeyWordBuffOrDeBuff(EBuffAndDeBuff.DanLuc)} thì thu hoạch được {GetColoredText($"+{buffValues[level - 1]}", "red")} tầng {GetKeyWordBuffOrDeBuff(EBuffAndDeBuff.Tran)}";
    }

    public override void InitPosCallExecute(CharacterCombat caster)
    {
        Observer.AddObserver(EventList.GamePlay.NEW_TURN_COMBAT, ExecuteParams);

    }
}
