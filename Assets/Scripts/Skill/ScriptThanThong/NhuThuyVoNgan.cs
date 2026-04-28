using Skill;
using UnityEngine;
[CreateAssetMenu(fileName = nameof(NhuThuyVoNgan), menuName = "Skill/Thần Thông/" + nameof(NhuThuyVoNgan))]

public class NhuThuyVoNgan : ThanThong
{
    [SerializeField] private int[] kinhDaoReset;
    [SerializeField] private int[] chiThuyReset;
    [SerializeField] private int[] healValues;
    [SerializeField] private int[] healPerChiThuyValues;
    [SerializeField] private float delayTimeEffect = 1f;

    public override void Execute(CharacterCombat caster, CharacterCombat target, params object[] data)
    {
        caster.Healing(healValues[GetIndexLevelThanThong(caster)]);

        caster.buffAndDebuffCount.TryGetValue(EBuffAndDeBuff.KinhDao, out SecureInt  kinhDao);
        caster.buffAndDebuffCount.TryGetValue(EBuffAndDeBuff.ChiThuy, out SecureInt  chiThuy);

        caster.Healing(healPerChiThuyValues[GetIndexLevelThanThong(caster)] * chiThuy);

        caster.RemoveBuffAndDebuffCount(EBuffAndDeBuff.KinhDao, kinhDao);
        caster.RemoveBuffAndDebuffCount(EBuffAndDeBuff.ChiThuy, chiThuy);
        caster.AddBuffAndDebuffCount(EBuffAndDeBuff.KinhDao, kinhDaoReset[GetIndexLevelThanThong(caster)]);
        caster.AddBuffAndDebuffCount(EBuffAndDeBuff.ChiThuy, chiThuyReset[GetIndexLevelThanThong(caster)]);
        ShowSkillEffectWorld(caster, delayTimeEffect);

    }

    public override string GetDesc(int level)
    {
        return $"Khôi phục {GetColoredText($"+{healValues[level-1]}", "red")} điểm Hp. Tiêu hao tất cả {GetKeyWordBuffOrDeBuff(EBuffAndDeBuff.ChiThuy)}. Mỗi có một tầng {GetKeyWordBuffOrDeBuff(EBuffAndDeBuff.ChiThuy)} Khôi phục {GetColoredText($"+{healPerChiThuyValues[level - 1]}", "red")} điểm Hp, Làm mới {GetKeyWordBuffOrDeBuff(EBuffAndDeBuff.KinhDao)} thành {GetColoredText($"{kinhDaoReset[level - 1]}", "red")} tầng, Làm mới {GetKeyWordBuffOrDeBuff(EBuffAndDeBuff.ChiThuy)} thành {GetColoredText($"{chiThuyReset[level - 1]}", "red")} tầng";
    }
}
