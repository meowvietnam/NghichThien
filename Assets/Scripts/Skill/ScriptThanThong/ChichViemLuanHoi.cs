using UnityEngine;
[CreateAssetMenu(fileName = nameof(ChichViemLuanHoi), menuName = "Skill/Thần Thông/" + nameof(ChichViemLuanHoi))]

public class ChichViemLuanHoi : ThanThong
{
    [SerializeField] private int hapThu = 4;
    [SerializeField] private float delayTimeEffect = 1f;
    public override void Execute(CharacterCombat caster, CharacterCombat target, params object[] data)
    {
        HauKeVoLuc(caster);
        caster.TieuTanAllLinhKhi();
        caster.HapThuLinhKhi(hapThu);
        ShowSkillEffectWorld(caster, delayTimeEffect);
    }

    public override string GetDesc(int level)
    {
        return $"{GetKeyWord(EKeyWord.HauKeVoLuc)} Tiêu tán toàn bộ linh khí. Hấp thụ {GetColoredText($"+{hapThu}", "red")} điểm linh khí";
    }
}
