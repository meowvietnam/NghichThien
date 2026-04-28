using LongDH.LongDHModules;
using System.Collections.Generic;
using System.Linq;
using Skill;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;
[CreateAssetMenu(menuName = "Skill/Công Pháp /" + nameof(PhanDuongQuyet))]
public class PhanDuongQuyet : CongPhap
{
    [SerializeField]
    private int[] buffValues = new int[]
     {
        2, 3 , 4,6, 9
     };
    public override void ExecuteParams(params object[] data)
    {
        // Sử dụng 'as' để ép kiểu an toàn, nếu sai kiểu sẽ trả về null
        CharacterCombat caster = data.Length > 0 ? data[0] as CharacterCombat : null;
        CharacterCombat target = data.Length > 1 ? data[1] as CharacterCombat : null;
        // Tìm trong danh sách trang bị của caster
        if (target!=null&&IsCanUse(target))
        {
            Execute(caster, target, data);

        }
    }
    public override void Execute(CharacterCombat caster, CharacterCombat target, params object[] data)
    {
        caster.AddBuffAndDebuffCount(EBuffAndDeBuff.ThieuDot, buffValues[CurrentLevelCongPhapInCombat(target) - 1]);


    }

    public override string GetDesc(int level)
    {
        return $"Mỗi lần nhận tổn thương bởi thần thông, đối thủ nhận phải {GetColoredText($"+{buffValues[level - 1]}", "red")} tầng {GetKeyWordBuffOrDeBuff(EBuffAndDeBuff.ThieuDot)}";
    }

    public override void InitPosCallExecute(CharacterCombat caster)
    {
        Observer.AddObserver(EventList.Combat.CACULATE_DAME_BY_THANTHONG, ExecuteParams);

    }
}
