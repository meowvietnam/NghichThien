using Skill;
using UnityEngine;
[CreateAssetMenu(fileName = nameof(LyHoaTuyetViem), menuName = "Skill/Thần Thông/" + nameof(LyHoaTuyetViem))]

public class LyHoaTuyetViem : ThanThong
{
    [SerializeField] protected int thieuDot = 2;
    [SerializeField] protected int hapThu = 3;
    [SerializeField] private float delayTimeEffect = 1f;

    public override void Execute(CharacterCombat caster, CharacterCombat target, params object[] data)
    {

        HauKeVoLuc(caster);
        caster.AddBuffAndDebuffCount(EBuffAndDeBuff.ThieuDot, thieuDot);
        caster.HapThuLinhKhi(ELinhKhi.Hoa,hapThu);
        ShowSkillEffectWorld(caster, delayTimeEffect);

    }


    public override string GetDesc(int level)
    {
        return $"{GetKeyWord(EKeyWord.HauKeVoLuc)} Thu hoạch được {GetColoredText($"+{hapThu}", "red")} điểm {GetTextLinhKhi(ELinhKhi.Hoa)} hệ linh khí. Đồng thời, nhận phải {GetColoredText($"+{thieuDot}", "red")} tầng {GetKeyWordBuffOrDeBuff(EBuffAndDeBuff.ThieuDot)}";

    }
}
