using UnityEngine;
[CreateAssetMenu(fileName = nameof(DiemLinhChieuLai), menuName = "Skill/Thần Thông/" + nameof(DiemLinhChieuLai))]

public class DiemLinhChieuLai : ThanThong
{
    [SerializeField] private int require = 5;
    [SerializeField]
    private int[] dameValues = new int[]
    {
        2,3,4,7,12
    };
  
    public override void Execute(CharacterCombat caster, CharacterCombat target, params object[] data)
    {
        caster.RemoveBuffAndDebuffCount(EBuffAndDeBuff.Diem, require);
        int dame = dameValues[GetIndexLevelThanThong(caster)];
        target.TakeDameByThanThong(caster,new System.Collections.Generic.List<int> { dame });



    }
  
    public override bool IsCanUse(CharacterCombat caster, CharacterCombat target)
    {
        caster.buffAndDebuffCount.TryGetValue(EBuffAndDeBuff.Diem, out int diem);
        if (diem >= require)
        {
            return true;
        }
        return false;
    }



    public override string GetDesc(int level)
    {
        return $"Tiêu hao {GetColoredText($"{require}", "red")} tầng {GetKeyWordBuffOrDeBuff(EBuffAndDeBuff.Diem)}, Tạo thành {GetColoredText($"+{dameValues[level - 1]}", "red")} điểm sát thương";

    }
}
