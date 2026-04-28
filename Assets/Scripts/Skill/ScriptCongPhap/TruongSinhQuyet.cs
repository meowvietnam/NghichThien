using LongDH.LongDHModules;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;
using Skill;
using UnityEngine;
[CreateAssetMenu(menuName = "Skill/Công Pháp /" + nameof(TruongSinhQuyet))]
public class TruongSinhQuyet : CongPhap
{
    [SerializeField]
    private int[] buffValues = new int[]
      {
            5, 10 , 20,40, 80
      };
    [SerializeField] private int require = 25;

    public override void Execute(CharacterCombat caster, CharacterCombat target, params object[] data)
    {
        if (caster.characterStatsTemp.hp <= caster.characterStatsTemp.hpMax * (require / 100f))
        {
            caster.Healing(buffValues[CurrentLevelCongPhapInCombat(caster)-1]);

        }

    }

    public override string GetDesc(int level)
    {
        return $"Kết thúc hiệp, nếu HP thấp hơn {GetColoredText($"{require}%", "red")} thì khôi phục {GetColoredText($"+{buffValues[level - 1]}", "red")} điểm HP";

    }

    public override void InitPosCallExecute(CharacterCombat caster)
    {
        Observer.AddObserver(EventList.Combat.END_TURN_COMBAT, ExecuteParams);

    }
}
