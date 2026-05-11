using UnityEngine;
[CreateAssetMenu(fileName = nameof(BachDeThanhKiem), menuName = "Skill/Thần Thông/" + nameof(BachDeThanhKiem))]

public class BachDeThanhKiem : NguKiemThuat
{
    [SerializeField]
    private int[] buffTuongSinh = new int[]
    {
        1,2,4,9,18
    };

    public override void Execute(CharacterCombat caster, CharacterCombat target, params object[] data)
    {
        base.Execute(caster, target, data);
        caster.AddBuffAndDebuffCount(EBuffAndDeBuff.NhanKiem, buffValues[GetIndexLevelThanThong(caster)]);
        caster.AddBuffAndDebuffCount(EBuffAndDeBuff.DiaKiem, buffValues[GetIndexLevelThanThong(caster)]);
        caster.AddBuffAndDebuffCount(EBuffAndDeBuff.ThienKiem, buffValues[GetIndexLevelThanThong(caster)]);


    }
    protected override void ShowEffect(CharacterCombat caster, CharacterCombat target)
    {
        ShowSkillEffectWorld(target, delayTimeEffect);
    }
    public override void ExecuteTuongSinh(CharacterCombat caster, CharacterCombat target)
    {
        caster.AddBuffAndDebuffCount(EBuffAndDeBuff.KiemKhi, buffTuongSinh[GetIndexLevelThanThong(caster)]);
    }


    public override string GetDesc(int level)
    {
        return $"{GetKeyWord(EKeyWord.KiemY)} Tạo thành {GetColoredText($"+{dames[level - 1]}", "red")} điểm sát thương, thu hoạch được {GetColoredText($"+{buffValues[level - 1]}", "red")} tầng {GetKeyWordBuffOrDeBuff(EBuffAndDeBuff.NhanKiem)}, {GetKeyWordBuffOrDeBuff(EBuffAndDeBuff.DiaKiem)} và {GetKeyWordBuffOrDeBuff(EBuffAndDeBuff.ThienKiem)}. {GetKeyWord(EKeyWord.TuongSinh)} thu hoạch được {GetColoredText($"+{buffTuongSinh[level - 1]}", "red")} tầng {GetKeyWordBuffOrDeBuff(EBuffAndDeBuff.KiemKhi)}";

    }
}
