using UnityEngine;
[CreateAssetMenu(fileName = nameof(DaiThonDiemThuat), menuName = "Skill/Thần Thông/" + nameof(DaiThonDiemThuat))]

public class DaiThonDiemThuat : ThanThong
{
    
    [SerializeField]
    private int[] healValues = new int[]
     {
        2,3,4,5,6
     };
    public override void Execute(CharacterCombat caster, CharacterCombat target, params object[] data)
    {


        caster.buffAndDebuffCount.TryGetValue(EBuffAndDeBuff.Diem, out int diem);
        int heal = diem * healValues[GetIndexLevelThanThong(caster)];
        caster.Healing(heal);
        caster.RemoveBuffAndDebuffCount(EBuffAndDeBuff.Diem, diem);
       

    }

    public override bool IsCanUse(CharacterCombat caster, CharacterCombat target)
    {
        caster.buffAndDebuffCount.TryGetValue(EBuffAndDeBuff.Diem, out int diem);
        if (diem > 0)
        {
            return true;
        }
        return false;
    }
    public override string GetDesc(int level)
    {
        return $"Bản thân gỡ ra trạng thái {GetKeyWordBuffOrDeBuff(EBuffAndDeBuff.Diem)} mỗi tầng tiêu hao thì khôi phục {GetColoredText($"+{healValues[level - 1]}", "red")} điểm Hp";

    }
}
