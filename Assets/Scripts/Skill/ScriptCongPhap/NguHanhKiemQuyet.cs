using LongDH.LongDHModules;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
[CreateAssetMenu(menuName = "Skill/Công Pháp /" + nameof(NguHanhKiemQuyet))]

public class NguHanhKiemQuyet : CongPhap
{
    [SerializeField]
    private int[] buffValues = new int[]
          {
            1, 2 , 4,6, 10
          };
    

    public override void Execute(CharacterCombat caster, CharacterCombat target, params object[] data)
    {
        if(CheckIsEqualETypeSkill(caster))
        {
            int indexLevel = CurrentLevelCongPhapInCombat(caster) - 1;
            int count = caster.congPhapDynamic.Count(x => x.congPhap is NguHanhKiemQuyet);
            caster.AddBuffAndDebuffCount(EBuffAndDeBuff.KiemKhi, count * buffValues[indexLevel]);
            caster.AddBuffAndDebuffCount(EBuffAndDeBuff.NguKiem, count * buffValues[indexLevel]);
            caster.AddBuffAndDebuffCount(EBuffAndDeBuff.ChanKiem, count* buffValues[indexLevel]);
            caster.AddCPDynamic(GetCongPhapMono(caster.congPhapEquip));

        }
    }
    public virtual void ExecuteParams2(params object[] data)
    {
        // Sử dụng 'as' để ép kiểu an toàn, nếu sai kiểu sẽ trả về null
        CharacterCombat caster = data.Length > 0 ? data[0] as CharacterCombat : null;
        CharacterCombat target = data.Length > 1 ? data[1] as CharacterCombat : null;

        // Kiểm tra caster không null và thỏa mãn điều kiện sử dụng
        if (caster != null && IsCanUse(caster))
        {
            Execute2(caster, target, data);
        }
    }
    public void Execute2(CharacterCombat caster, CharacterCombat target, params object[] data)
    {
        if (caster.thanThongCurrentUse == null || caster.thanThongOldUse == null) return;
        ThanThong thanThong = (ThanThong)data[2];
        bool isEqualType = CheckIsEqualETypeSkill(caster);
        // 1. Extract the complex check into a descriptive variable
        bool isLienKichTrigger = thanThong is LienKich &&
                                 CombatManager.IsLienKich(caster.thanThongCurrentUse.thanThong, caster.thanThongOldUse.thanThong);

        // 2. The logic simplified: 
        // Reset if types don't match, OR it's not a combo skill, OR it's a valid combo sequence.
        if (!isEqualType || (isEqualType&&!isLienKichTrigger))
        {
            RemoveCongPhap(caster);
            caster.AddCPDynamic(GetCongPhapMono(caster.congPhapEquip));
        }



    }
    public override string GetDesc(int level)
    {
        return $"Sử dụng {GetTextTypeSkill(ETypeSkill.Kiem)} hệ thần thông và kích hoạt ngũ hành {GetKeyWord(EKeyWord.LienKich)} thì thu hoạch được {GetColoredText($"+{buffValues[level - 1]}*X", "red")} tầng {GetKeyWordBuffOrDeBuff(EBuffAndDeBuff.KiemKhi)}, {GetKeyWordBuffOrDeBuff(EBuffAndDeBuff.NguKiem)} và {GetKeyWordBuffOrDeBuff(EBuffAndDeBuff.ChanKiem)} (X là số lần kích hoạt công pháp này liên tục nếu không làm mới về 1)";
    }

    public override void InitPosCallExecute(CharacterCombat caster)
    {
        Observer.AddObserver(EventList.GamePlay.LIEN_KICH, ExecuteParams);
        Observer.AddObserver(EventList.GamePlay.PLAY_SKILL, ExecuteParams2);

    }
}
