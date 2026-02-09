using LongDH.LongDHModules;
using UnityEngine;
[CreateAssetMenu(menuName = "Skill/Công Pháp /" + nameof(CuuTrungDoTap))]
public class CuuTrungDoTap : CongPhap
{
    [SerializeField] private int[] countDebuff;
    public override void Execute(CharacterCombat caster, CharacterCombat target, params object[] data)
    {
        if (caster.thanThongCurrentUse == null || caster.thanThongOldUse == null) return;
        if (CheckIsEqualETypeSkill(caster) && CombatManager.IsLienKich(caster.thanThongCurrentUse.thanThong,caster.thanThongOldUse.thanThong))
        {
            int count = countDebuff[CurrentLevelCongPhapInCombat(caster) - 1];
            for (int i = 0; i < count; i++)
            {
                var debuff = debuffs[CombatStatic.IndexRandom(0,debuffs.Count)];
                target.AddBuffAndDebuffCount(debuff,1);
            }
            Debug.Log("co voi vao CuuTrungDoTap");
        }



    }
    public override string GetDesc(int level)
    {
        return $"Sử dụng {GetTextTypeSkill()} thần thông, nếu trước đó dùng Thủy hệ thần thông thì đối phương nhận phải {GetColoredText($"+{countDebuff[level - 1]}", "red")} lần ngẫu nhiên trạng thái tiêu cực";

    }



    public override void InitPosCallExecute()
    {
        Observer.AddObserver(EventList.GamePlay.PLAY_SKILL, ExecuteParams);
    }
}
