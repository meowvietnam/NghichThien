using UnityEngine;
[CreateAssetMenu(fileName = nameof(HoatHuyetThuat), menuName = "Skill/Thần Thông/" + nameof(HoatHuyetThuat))]

public class HoatHuyetThuat : AmKinhTT
{
    [SerializeField] private int[] healValues;
    public override string GetDescAmKinhTT(int level)
    {
        return $"Khôi phục {GetColoredText($"+{healValues[level-1]}", "red")} Hp";
    }

    public override void ExecuteAmKinh(CharacterCombat caster, CharacterCombat target)
    {
        caster.Healing(healValues[GetIndexLevelThanThong(caster)]);
    }

    
}
