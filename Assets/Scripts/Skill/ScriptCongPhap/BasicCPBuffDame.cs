using LongDH.LongDHModules;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[CreateAssetMenu(menuName = "Skill/Công Pháp /" + nameof(BasicCPBuffDame))]
public class BasicCPBuffDame : CongPhap
{
    [SerializeField] private int[] dameBuff;



    public override void ExecuteParams(params object[] data)
    {
        // BỎ DÒNG NÀY Ở ĐÂY: Observer.RemoveObserver(..., ExecuteParams);

        // Chỉ gọi base để thực thi logic Execute
        base.ExecuteParams(data);
    }

    public override void Execute(CharacterCombat caster, CharacterCombat target, params object[] data)
    {
        // Kiểm tra hệ của Skill đang dùng có khớp với Công Pháp (SO) này không

        if (CheckIsEqualETypeSkill(caster))
        {
            Debug.Log($"<color=cyan>{this.name}</color>: Đúng hệ! Buff dame và tự gỡ bỏ.");

            // CHỈ REMOVE KHI ĐÃ THỰC THI BUFF THÀNH CÔNG (Lần thứ 1 của mỗi hiệp) 
            RemoveCongPhap(caster);

            int dame = dameBuff[CurrentLevelCongPhapInCombat(caster) - 1];
            target.AddDamageData(new List<int> { dame });
        }
        else
        {
            // Không khớp hệ thì im lặng, không Remove để đợi Skill sau trong cùng Turn
            Debug.Log($"<color=orange>{this.name}</color>: Không đúng hệ, tiếp tục chờ...");
        }
    }
    public virtual void ExecuteParams2(params object[] data)
    {
        CharacterCombat caster = (CharacterCombat)data[0];
        CharacterCombat target = (CharacterCombat)data[1];
        // Tìm trong danh sách trang bị của caster
        if (GetCongPhapMono(caster.congPhapEquip) != null)
        {
            Execute2(caster, target, data);

        }
    }
    public void Execute2(CharacterCombat caster, CharacterCombat target, params object[] data)
    {
        var congPhapMono = GetCongPhapMono(caster.congPhapEquip);
        if (GetCongPhapMono(caster.congPhapDynamic) != null) return;
        caster.AddCPDynamic(congPhapMono);

    }
    public override string GetDesc(int level)
    {
        return $"Mỗi hiệp lần thứ nhất tạo thành {GetTextTypeSkill()} hệ thần thông sát thương {GetColoredText($"+{dameBuff[level - 1]}", "red")}";

    }
    


    public override void InitPosCallExecute(CharacterCombat caster)
    {
        Observer.AddObserver(EventList.Combat.NEW_TURN_COMBAT, ExecuteParams2);
        Observer.AddObserver(EventList.Combat.CACULATE_DAME_BY_THANTHONG, ExecuteParams);

    }


}
