using LongDH.LongDHModules;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;
[CreateAssetMenu(menuName = "Skill/Công Pháp /" + nameof(UViemQuyet))]
public class UViemQuyet : CongPhap
{
    [SerializeField]
    private int[] buffValues = new int[]
      {
        3, 6 , 9,12, 20
      };
    public void ExecuteParams2(params object[] data)
    {
        // Sử dụng 'as' để ép kiểu an toàn, nếu sai kiểu sẽ trả về null
        CharacterCombat caster = data.Length > 0 ? data[0] as CharacterCombat : null;
        CharacterCombat target = data.Length > 1 ? data[1] as CharacterCombat : null;

        // Tìm trong danh sách trang bị của caster
        if (target  != null && IsCanUse(target))
        {
            Execute2(caster, target, data);

        }
    }

    public override void Execute(CharacterCombat caster, CharacterCombat target, params object[] data)
    {
        if (CheckIsEqualETypeSkill(caster)) 
        {
            Debug.Log("UViemQuyetExcute1");

            target.AddDamageData(new List<int> { buffValues[CurrentLevelCongPhapInCombat(caster)-1] });
        }


    }
    public void Execute2(CharacterCombat caster, CharacterCombat target, params object[] data)
    {
       if(CheckIsEqualETypeSkill(caster)) // sender là hỏa hệ thần thông thì giảm dame cho target
       {
            Debug.Log("UViemQuyetExcute2");
            target.AddDamageData(new List<int> { -buffValues[CurrentLevelCongPhapInCombat(target) - 1] });


       }


    }
    public void ExecuteParams3(params object[] data)
    {
        // Sử dụng 'as' để ép kiểu an toàn, nếu sai kiểu sẽ trả về null
        CharacterCombat caster = data.Length > 0 ? data[0] as CharacterCombat : null;
        CharacterCombat target = data.Length > 1 ? data[1] as CharacterCombat : null;

        // Tìm trong danh sách trang bị của caster
        if (caster != null && IsCanUse(caster))
        {
            Execute3(caster, target, data);

        }
    }
    public void Execute3(CharacterCombat caster, CharacterCombat target, params object[] data)
    {
        ETypeSkill[] eTypeSkills = (ETypeSkill[])data[3];
        if (eTypeSkills != null && CheckIsEqualETypeSkill(eTypeSkills)) // sender là hỏa hệ thần thông thì giảm dame cho target
        {
            Debug.Log("UViemQuyetExcute3");

            target.AddDamageData(new List<int> { buffValues[CurrentLevelCongPhapInCombat(caster) - 1] });


        }


    }
    public void ExecuteParams4(params object[] data)
    {
        // Sử dụng 'as' để ép kiểu an toàn, nếu sai kiểu sẽ trả về null
        CharacterCombat caster = data.Length > 0 ? data[0] as CharacterCombat : null;
        CharacterCombat target = data.Length > 1 ? data[1] as CharacterCombat : null;

        // Tìm trong danh sách trang bị của caster
        if (target != null && IsCanUse(target))
        {
            Execute4(caster, target, data);

        }
    }
    public void Execute4(CharacterCombat caster, CharacterCombat target, params object[] data)
    {
        ETypeSkill[] eTypeSkills = (ETypeSkill[])data[3];

        if (eTypeSkills != null && CheckIsEqualETypeSkill(eTypeSkills)) // sender là hỏa hệ thần thông thì giảm dame cho target
        {
            Debug.Log("UViemQuyetExcute4");

            target.AddDamageData(new List<int> { -buffValues[CurrentLevelCongPhapInCombat(target) - 1] });


        }


    }
    public override string GetDesc(int level)
    {
        return $"Nhận {GetTextLinhKhi(ELinhKhi.Hoa)} hệ tổn thương {GetColoredText($"-{buffValues[level - 1]}", "red")}, tăng lên {GetTextLinhKhi(ELinhKhi.Hoa)} hệ tổn thương {GetColoredText($"+{buffValues[level - 1]}", "red")}";
    }

    public override void InitPosCallExecute(CharacterCombat caster)
    {
        // 1 2 là add dame và giảm dame của play thần thông
        // 3 4 là add dame và giảm dame của static 
        Observer.AddObserver(EventList.GamePlay.CACULATE_DAME_BY_THANTHONG, ExecuteParams);
        Observer.AddObserver(EventList.GamePlay.CACULATE_DAME_BY_THANTHONG, ExecuteParams2);

        Observer.AddObserver(EventList.GamePlay.CHARACTER_TAKE_DAME_BY_STATIC, ExecuteParams3);
        Observer.AddObserver(EventList.GamePlay.CHARACTER_TAKE_DAME_BY_STATIC, ExecuteParams4);

    }
}
