using LongDH.LongDHModules;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class LuuQuangChanQuyet : CongPhap
{
    [SerializeField] private int[] dameBuff;
    [SerializeField] private int noiThuong = 1;





    public override void Execute(CharacterCombat caster, CharacterCombat target, params object[] data)
    {
        // Ki?m tra h? c?a Skill ?ang dùng có kh?p v?i Công Pháp (SO) này không

        if (CheckIsEqualETypeSkill(caster) && CombatManager.IsLienKich(caster.thanThongCurrentUse.thanThong, caster.thanThongOldUse.thanThong))
        {
            Observer.RemoveObserver(EventList.GamePlay.CHARACTER_TAKE_DAME_BY_THANTHONG, ExecuteParams);
            int dame = dameBuff[CurrentLevelCongPhapInCombat(caster) - 1];
            target.AddDamageData(new List<int> { dame });
        }
    }
    public virtual void ExecuteParams2(params object[] data)
    {
        CharacterCombat caster = (CharacterCombat)data[0];
        CharacterCombat target = (CharacterCombat)data[1];
        // Tìm trong danh sách trang bị của caster
        if (GetCongPhapMono(caster) != null)
        {
            Execute2(caster, target, data);

        }
    }
    public void Execute2(CharacterCombat caster, CharacterCombat target, params object[] data)
    {
        caster.AddBuffAndDebuffCount(EBuffAndDeBuff.NoiThuong,noiThuong);
        
    }
    public override string GetDesc(int level)
    {
        return $"Bắt Đầu hiệp, nhận phải {GetColoredText($"{noiThuong}", "red")} tầng {GetKeyWordBuffOrDeBuff(EBuffAndDeBuff.NoiThuong)}, sử dụng thần thông {GetTextTypeSkill()} hệ gây tổn thương đồng thời phát động {GetKeyWord(EKeyWord.LienKich)} thì tăng lên {GetColoredText($"+{dameBuff[level - 1]}", "red")} sát thương";

    }


    public override void Init()
    {
        Observer.AddObserver(EventList.GamePlay.NEW_TURN_COMBAT, ExecuteParams2);
        Observer.AddObserver(EventList.GamePlay.NEW_TURN_COMBAT, InitPosCallExecute);

    }

    public override void InitPosCallExecute()
    {
        Observer.AddObserver(EventList.GamePlay.CHARACTER_TAKE_DAME_BY_THANTHONG, ExecuteParams);
    }
}
