using System.Linq;
using Skill;
using UnityEngine;
[CreateAssetMenu(fileName = nameof(ThaiNhienVoBa), menuName = "Skill/Thần Thông/" + nameof(ThaiNhienVoBa))]
public class ThaiNhienVoBa : ThanThong
{
    
    [SerializeField] private int[] chanKiemPerKiemKhiValues;
    [SerializeField] private float delayTimeEffect = 1f;


    public override void Execute(CharacterCombat caster, CharacterCombat target, params object[] data)
    {
        HauKeVoLuc(caster);

        caster.buffAndDebuffCount.TryGetValue(EBuffAndDeBuff.KiemKhi, out SecureInt  kiemKhi);

        caster.AddBuffAndDebuffCount(EBuffAndDeBuff.ChanKiem, chanKiemPerKiemKhiValues[GetIndexLevelThanThong(caster)] * kiemKhi);

        caster.RemoveBuffAndDebuffCount(EBuffAndDeBuff.KiemKhi, kiemKhi);
        ShowSkillEffectWorld(caster, delayTimeEffect);

    }

    public override string GetDesc(int level)
    {
        return $"{GetKeyWord(EKeyWord.HauKeVoLuc)} Tiêu hao tất cả {GetKeyWordBuffOrDeBuff(EBuffAndDeBuff.KiemKhi)}. Mỗi có một tầng {GetKeyWordBuffOrDeBuff(EBuffAndDeBuff.KiemKhi)} thu hoạch được {GetColoredText($"+{chanKiemPerKiemKhiValues[level - 1]}", "red")} tầng {GetKeyWordBuffOrDeBuff(EBuffAndDeBuff.ChanKiem)}";
    }
}
