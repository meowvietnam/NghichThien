using UnityEngine;
[CreateAssetMenu(fileName = nameof(TrienMocQuyet), menuName = "Skill/Thần Thông/" + nameof(TrienMocQuyet))]

public class TrienMocQuyet : ThanThong
{
    [SerializeField] private int[] trienNhieu;
    public override void Execute(CharacterCombat caster, CharacterCombat target, params object[] data)
    {

        target.AddBuffAndDebuffCount(EBuffAndDeBuff.TrienNhieu, trienNhieu[GetIndexLevelThanThong(caster)]);


    }

    public override string GetDesc(int level)
    {
        return $"Đối phương nhận phải {GetColoredText($"{trienNhieu[level - 1]}", "red")} tầng {GetKeyWordBuffOrDeBuff(EBuffAndDeBuff.TrienNhieu)}";
    }
}

