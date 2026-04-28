using LongDH.LongDHModules;
using System.Collections.Generic;
using System.Linq;
using Skill;
using UnityEngine;
[CreateAssetMenu(menuName = "Skill/Công Pháp /" + nameof(LeKiemQuyet))]

public class LeKiemQuyet : CongPhap
{
    [SerializeField]
    private int[] buffValues = new int[]
       {
            1,4,7,10, 15
       };


    public override void Execute(CharacterCombat caster, CharacterCombat target, params object[] data)
    {
        if (CheckIsEqualETypeSkill(caster))
        {
            target.AddDamageData(new List<int> { buffValues[CurrentLevelCongPhapInCombat(caster) - 1] });

        }

    }
    public void ExecuteParams2(params object[] data)
    {
        // Sử dụng 'as' để ép kiểu an toàn, nếu sai kiểu sẽ trả về null
        CharacterCombat caster = data.Length > 0 ? data[0] as CharacterCombat : null;
        CharacterCombat target = data.Length > 1 ? data[1] as CharacterCombat : null;

        // Tìm trong danh sách trang bị của caster
        if (caster != null && IsCanUse(caster))
        {
            Execute2(caster, target, data);

        }
    }
    public void Execute2(CharacterCombat caster, CharacterCombat target, params object[] data)
    {
        ETypeSkill[] eTypeSkills = (ETypeSkill[])data[3];
        if (eTypeSkills != null && CheckIsEqualETypeSkill(eTypeSkills)) 
        {
            Debug.Log("UViemQuyetExcute3");

            target.AddDamageData(new List<int> { buffValues[CurrentLevelCongPhapInCombat(caster) - 1] });


        }


    }

    public override string GetDesc(int level)
    {
        return $"Mỗi sử dụng {GetTextTypeSkill(ETypeSkill.Kiem)} hệ thần thông. Nếu {GetKeyWord(EKeyWord.TuongSinh)} thu hoạch được {GetColoredText($"+{buffValues[level - 1]}", "red")} tầng {GetKeyWordBuffOrDeBuff(EBuffAndDeBuff.GiamTonThuong)}. Nếu {GetKeyWord(EKeyWord.TuongKhac)} thu hoạch được {GetColoredText($"+{buffValues[level - 1]}", "red")} tầng {GetKeyWordBuffOrDeBuff(EBuffAndDeBuff.KiemKhi)}";
    }

    public override void InitPosCallExecute(CharacterCombat caster)
    {
        Observer.AddObserver(EventList.Combat.CACULATE_DAME_BY_THANTHONG, ExecuteParams);
        Observer.AddObserver(EventList.Combat.CHARACTER_TAKE_DAME_BY_STATIC, ExecuteParams2);

    }
}
