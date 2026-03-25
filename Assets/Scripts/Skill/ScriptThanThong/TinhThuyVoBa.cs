using UnityEngine;
[CreateAssetMenu(fileName = nameof(TinhThuyVoBa), menuName = "Skill/Thần Thông/" + nameof(TinhThuyVoBa))]

public class TinhThuyVoBa : ThanThong
{
    [SerializeField] private int[] kinhDaoReset;
    [SerializeField] private int[] healValues;
    [SerializeField] private int[] healPerChiThuyValues;
    [SerializeField] private float delayTimeEffect = 1f;


    public override void Execute(CharacterCombat caster, CharacterCombat target, params object[] data)
    {
        caster.AddBuffAndDebuffCount(EBuffAndDeBuff.TriLieu,healValues[GetIndexLevelThanThong(caster)]);

        caster.buffAndDebuffCount.TryGetValue(EBuffAndDeBuff.KinhDao, out SecureInt  kinhDao);
        caster.buffAndDebuffCount.TryGetValue(EBuffAndDeBuff.ChiThuy, out SecureInt  chiThuy);

        caster.AddBuffAndDebuffCount(EBuffAndDeBuff.TriLieu,healPerChiThuyValues[GetIndexLevelThanThong(caster)] * chiThuy);

        caster.RemoveBuffAndDebuffCount(EBuffAndDeBuff.KinhDao, kinhDao);
        caster.RemoveBuffAndDebuffCount(EBuffAndDeBuff.ChiThuy, chiThuy);
        caster.AddBuffAndDebuffCount(EBuffAndDeBuff.KinhDao, kinhDaoReset[GetIndexLevelThanThong(caster)]);
        ShowSkillEffectWorld(caster, delayTimeEffect);
    }

    public override string GetDesc(int level)
    {
        return $"Thu hoạch được {GetColoredText($"+{healValues[level - 1]}", "red")} tầng {GetKeyWordBuffOrDeBuff(EBuffAndDeBuff.TriLieu)}. Tiêu hao tất cả {GetKeyWordBuffOrDeBuff(EBuffAndDeBuff.ChiThuy)}. Mỗi có một tầng {GetKeyWordBuffOrDeBuff(EBuffAndDeBuff.ChiThuy)} thu hoạch được {GetColoredText($"+{healPerChiThuyValues[level - 1]}", "red")} tầng {GetKeyWordBuffOrDeBuff(EBuffAndDeBuff.TriLieu)}, Làm mới {GetKeyWordBuffOrDeBuff(EBuffAndDeBuff.KinhDao)} thành {GetColoredText($"{kinhDaoReset[level - 1]}", "red")} tầng";
    }
}
