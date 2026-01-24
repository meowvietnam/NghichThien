using LongDH.LongDHModules;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(menuName = "Skill/Công Pháp /" + nameof(BasicCPBuffDame2))]
public class BasicCPBuffDame2 : CongPhap
{
    [SerializeField] private int[] dameBuff;
    [SerializeField] private ELinhKhi eLinhKhi;





    public override void Execute(CharacterCombat caster, CharacterCombat target, params object[] data)
    {
        // Kiểm tra hệ của Skill đang dùng có khớp với Công Pháp (SO) này không

        if (CheckIsEqualETypeSkill(caster) && CombatManager.IsLienKich(caster.thanThongCurrentUse.thanThong,caster.thanThongOldUse.thanThong))
        {
            target.AddDamageData(new List<int>{ dameBuff[CurrentLevelCongPhapInCombat(caster) - 1] });
        }
    }
    public override string GetDesc(int level)
    {
        return $"Khi sử dụng {GetTextLinhKhi(eLinhKhi)} hệ thần thông sát thương, nếu trước đó dùng {GetTextLinhKhi(CombatManager.linhKhiLienKich[eLinhKhi])} thì sát thương gây ra {GetColoredText($"+{dameBuff[level - 1]}", "red")}";

    }


    public override void Init()
    {
        InitPosCallExecute();

    }

    public override void InitPosCallExecute()
    {
        Observer.AddObserver(EventList.GamePlay.CHARACTER_TAKE_DAME_BY_THANTHONG, ExecuteParams);
    }

}
