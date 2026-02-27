using LongDH.LongDHModules;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(menuName = "Skill/Công Pháp /" + nameof(LiemTucThuat))]

public class LiemTucThuat : CongPhap
{
    [SerializeField]
    private int[] buffValues = new int[]
          {
            1, 3 , 5,8, 13
          };

    public override void Execute(CharacterCombat caster, CharacterCombat target, params object[] data)
    {
        caster.buffAndDebuffCount.TryGetValue(EBuffAndDeBuff.AnNac, out int anNac);
        if(anNac > 0)
        {
            target.AddDamageData(new List<int> { buffValues[CurrentLevelCongPhapInCombat(caster) - 1] });

        }


    }

    public override string GetDesc(int level)
    {
        return $"Bản thân có trạng thái {GetKeyWordBuffOrDeBuff(EBuffAndDeBuff.AnNac)} thì tạo thành {GetColoredText($"+{buffValues[level - 1]}", "red")} điểm sát thương (Chỉ có tác dụng với các thần thông không giải trừ {GetKeyWordBuffOrDeBuff(EBuffAndDeBuff.AnNac)})";
    }

    public override void InitPosCallExecute(CharacterCombat caster)
    {
        Observer.AddObserver(EventList.GamePlay.CHARACTER_TAKE_DAME_BY_THANTHONG, ExecuteParams);

    }
}
