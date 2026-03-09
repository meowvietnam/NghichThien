using UnityEngine;
[CreateAssetMenu(fileName = nameof(KimHongKiemPhap), menuName = "Skill/Thần Thông/" + nameof(KimHongKiemPhap))]

public class KimHongKiemPhap : KhuVatThuat , BaseSkill.LienKich
{
    [SerializeField]
    private int[] buffValues = new int[]
      {
        1,1,1,1,1
      };
    public override void Execute(CharacterCombat caster, CharacterCombat target, params object[] data)
    {
        base.Execute(caster, target, data);
        KiemY(caster);
    }

    public void ExecuteLienKich(CharacterCombat caster, CharacterCombat target)
    {
        caster.AddBuffAndDebuffCount(EBuffAndDeBuff.KiemKhi, buffValues[GetIndexLevelThanThong(caster)]);
    }
    public override string GetDesc(int level)
    {
        return $"{GetKeyWord(EKeyWord.KiemY)} {base.GetDesc(level)}. {GetKeyWord(EKeyWord.LienKich)} Nếu trước đó dùng {GetTextLinhKhi(ELinhKhi.Tho)} hệ thần thông thì thu hoạch được {GetColoredText($"+{buffValues[level - 1]}", "red")} tầng {GetKeyWordBuffOrDeBuff(EBuffAndDeBuff.KiemKhi)}";

    }
}
