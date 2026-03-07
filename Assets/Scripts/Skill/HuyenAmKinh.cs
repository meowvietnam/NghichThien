using UnityEngine;
[CreateAssetMenu(fileName = nameof(HuyenAmKinh), menuName = "Skill/Thần Thông/" + nameof(HuyenAmKinh))]
public class HuyenAmKinh : AmKinhTT
{
    [SerializeField]
    private int[] buffValues = new int[]
    {
        9,9,9,10,10
    };
    public override void ExecuteAmKinh(CharacterCombat caster, CharacterCombat target)
    {
        target.AddBuffAndDebuffCount(EBuffAndDeBuff.NoiThuong, buffValues[GetIndexLevelThanThong(caster)]);
    }

    public override string GetDescAmKinhTT(int level)
    {
        return $"Đối thủ nhận phải {GetColoredText($"+{buffValues[level - 1]}", "red")} tầng {GetKeyWordBuffOrDeBuff(EBuffAndDeBuff.NoiThuong)}";
    }

}
