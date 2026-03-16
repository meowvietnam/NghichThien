using LongDH.LongDHModules;
using UnityEngine;
[CreateAssetMenu(menuName = "Skill/Công Pháp /" + nameof(DiepDiemQuyet))]

public class DiepDiemQuyet : CongPhap
{
    [SerializeField]
    private int[] buffValues = new int[]
      {
        3, 4 , 5 ,6, 7
      };


    public override void Execute(CharacterCombat caster, CharacterCombat target, params object[] data)
    {
        if (caster.thanThongOldUse == null || caster.thanThongCurrentUse == null) return;
        if (CheckIsEqualETypeSkill(caster)&&CombatManager.IsLienKich(caster.thanThongCurrentUse.thanThong,caster.thanThongOldUse.thanThong))
        {
            caster.AddBuffAndDebuffCount(EBuffAndDeBuff.Diem, buffValues[CurrentLevelCongPhapInCombat(caster) - 1]);

        }



    }
    public override string GetDesc(int level)
    {
        return $"Khi sử dụng {GetTextLinhKhi(ELinhKhi.Hoa)} hệ thần thông, nếu trước đó sử dụng {GetTextLinhKhi(ELinhKhi.Moc)} hệ thần thông thì thu hoạch được {GetColoredText($"+{buffValues[level - 1]}", "red")} tầng {GetKeyWordBuffOrDeBuff(EBuffAndDeBuff.Diem)}";
    }

    public override void InitPosCallExecute(CharacterCombat caster)
    {
        Observer.AddObserver(EventList.Combat.PLAY_SKILL, ExecuteParams);

    }
}
