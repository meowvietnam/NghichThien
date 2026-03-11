using UnityEngine;
[CreateAssetMenu(fileName = nameof(ThuNguyetHoThien), menuName = "Skill/Thần Thông/" + nameof(ThuNguyetHoThien))]

public class ThuNguyetHoThien : ThanThong
{
    [SerializeField] private int[] dames;
    [SerializeField] private int[] dameBuffs;
    public override void Execute(CharacterCombat caster, CharacterCombat target, params object[] data)
    {
        int dameFinal = dames[GetIndexLevelThanThong(caster)];
        int dameBuff = dameBuffs[GetIndexLevelThanThong(caster)];
        dameFinal += (dameBuff * caster.CurrentLinhKhi);
        target.CaculateDameByThanThong(caster,new System.Collections.Generic.List<int> {dameFinal }, eTypeSkill);
       
    }

    public override string GetDesc(int level)
    {
        return $"Tạo thành {GetColoredText($"+{dames[level - 1]}", "red")} điểm sát thương. Mỗi còn thừa một điểm linh khí thì tăng lên {GetColoredText($"+{dameBuffs[level - 1]}", "red")} điểm sát thương";
    }

}
