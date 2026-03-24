using UnityEngine;
[CreateAssetMenu(fileName = nameof(TamTuongSinhLinhQuyet), menuName = "Skill/Thần Thông/" + nameof(TamTuongSinhLinhQuyet))]

public class TamTuongSinhLinhQuyet : ThanThong
{
    [SerializeField] private int[] buffs;
    [SerializeField] private int tuKhi = 2;
    [SerializeField] private float delayTimeEffect = 1f;
    public override void Execute(CharacterCombat caster, CharacterCombat target, params object[] data)
    {
        HauKeVoLuc(caster);
        caster.AddBuffAndDebuffCount(EBuffAndDeBuff.TuKhi,tuKhi);
        float percent = (float)buffs[GetIndexLevelThanThong(caster)]/100f;
        caster.Healing((int)(percent*(caster.characterStatsMax.hp - caster.characterStatsCurrent.hp)));
        ShowSkillEffectWorld(caster, delayTimeEffect);
    }

    public override string GetDesc(int level)
    {
        return $"{GetKeyWord(EKeyWord.HauKeVoLuc)}. Hồi phục {GetColoredText($"{buffs[level-1]}%", "red")} lượng Hp đã mất. Đồng thời thu hoạch được {GetColoredText($"{tuKhi}", "red")} tầng {GetKeyWordBuffOrDeBuff(EBuffAndDeBuff.TuKhi)}";

    }
}
