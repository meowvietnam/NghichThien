using LongDH.LongDHModules;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
[CreateAssetMenu(menuName = "Skill/Công Pháp /" + nameof(ThongBaoHopNguyenQuyet))]
public class ThongBaoHopNguyenQuyet : CongPhap
{
    [SerializeField]
    private int[] buffValues = new int[]
        {
            3, 5 , 8,14, 25
        };

    public override void Execute(CharacterCombat caster, CharacterCombat target, params object[] data)
    {
        if (caster.thanThongOldUse == null || caster.thanThongCurrentUse == null) return;
        if(caster.thanThongCurrentUse.thanThong == caster.thanThongOldUse.thanThong)
        {
            target.AddDamageData(new List<int> { (int)(buffValues[CurrentLevelCongPhapInCombat(caster) - 1]) });
        }


    }

    public override string GetDesc(int level)
    {
        return $"Sử dụng cùng một loại thần thông gây tổn thương liên tục thì tăng lên {GetColoredText($"+{buffValues[level - 1]}", "red")} điểm sát thương";
    }

    public override void InitPosCallExecute(CharacterCombat caster)
    {
        Observer.AddObserver(EventList.GamePlay.CACULATE_DAME_BY_THANTHONG, ExecuteParams);

    }
}
