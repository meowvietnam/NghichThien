using Skill;
using UnityEngine;
[CreateAssetMenu(fileName = nameof(PhucTheAn), menuName = "Skill/Thần Thông/" + nameof(PhucTheAn))]

public class PhucTheAn : ThanThong
{
    [SerializeField] private int[] buff;
    [SerializeField] private float delayTimeEffect = 1f;
    public override void Execute(CharacterCombat caster, CharacterCombat target, params object[] data)
    {
        int khiKim = caster.khiKim;
        if (khiKim <= 0) return;
        caster.AddBuffAndDebuffCount(EBuffAndDeBuff.GiamTonThuong, khiKim * buff[GetIndexLevelThanThong(caster)]);
        caster.TieuTanLinhKhi(ELinhKhi.Kim, khiKim);
        ShowSkillEffectWorld(caster, delayTimeEffect);
    }



    public override string GetDesc(int level)
    {
        return $"{GetKeyWord(EKeyWord.HauKeVoLuc)}. Tiêu tán toàn bộ {GetTextLinhKhi(ELinhKhi.Kim)} hệ linh khí, mỗi tiêu tán một điểm thu hoạch được {GetColoredText($"{buff[level - 1]}", "red")} tầng {GetKeyWordBuffOrDeBuff(EBuffAndDeBuff.GiamTonThuong)}";
    }
}
