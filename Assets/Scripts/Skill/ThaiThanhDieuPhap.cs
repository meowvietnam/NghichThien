using UnityEngine;
[CreateAssetMenu(fileName = nameof(ThaiThanhDieuPhap), menuName = "Skill/Thần Thông/" + nameof(ThaiThanhDieuPhap))]

public class ThaiThanhDieuPhap : ThanThong
{
    [SerializeField] private int[] buffs;
    public override void Execute(CharacterCombat caster, CharacterCombat target, params object[] data)
    {
        HauKeVoLuc(caster);
        caster.characterStats.hp += buffs[GetIndexLevelThanThong(caster)];
        caster.Healing(0);
    }

    public override string GetDesc(int level)
    {
        return $"{GetKeyWord(EKeyWord.HauKeVoLuc)}. Tăng tạm thời {GetColoredText($"+{buffs[level-1]}", "red")} Hp tối đa trong trận chiến này.";

    }

}
