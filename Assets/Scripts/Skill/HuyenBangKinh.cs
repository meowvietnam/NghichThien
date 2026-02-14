using UnityEngine;
[CreateAssetMenu(fileName = nameof(HuyenBangKinh), menuName = "Skill/Thần Thông/" + nameof(HuyenBangKinh))]

public class HuyenBangKinh : AmKinhTT
{
    [SerializeField] private int[] suongGias;
    public override void ExecuteAmKinh(CharacterCombat caster, CharacterCombat target)
    {
        target.AddBuffAndDebuffCount(EBuffAndDeBuff.SuongGia, suongGias[GetIndexLevelThanThong(caster)]);
    }

    public override string GetDescAmKinhTT(int level)
    {
        return $"Đối phương nhận phải {GetColoredText($"+{suongGias[level - 1]}", "red")} tầng {GetKeyWordBuffOrDeBuff(EBuffAndDeBuff.SuongGia)}";
    }


}
