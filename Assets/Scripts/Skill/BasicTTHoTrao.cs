using UnityEngine;
[CreateAssetMenu(fileName = nameof(BasicTTHoTrao), menuName = "Skill/Thần Thông/" + nameof(BasicTTHoTrao))]
public class BasicTTHoTrao : ThanThong, BaseSkill.LienKich
{

    [SerializeField] private int[] buff;
    [SerializeField] private ELinhKhi elinhKhi;

    public override void Execute(CharacterCombat caster, CharacterCombat target, params object[] data)
    {
        caster.AddBuffAndDebuffCount(EBuffAndDeBuff.HoTrao, buff[GetIndexLevelThanThong(caster)-1]);

    }

    public void ExecuteLienKich(CharacterCombat caster, CharacterCombat target)
    {
        caster.AddBuffAndDebuffCount(EBuffAndDeBuff.HoTrao, buff[GetIndexLevelThanThong(caster)-1] *2);

    }

    public override string GetDesc(int level)
    {
        return $"Cơ sở {GetTextLinhKhi(elinhKhi)} hệ thần thông, thu hoạch được {GetColoredText($"+{buff[level-1]}", "red")} tầng {GetKeyWordBuffOrDeBuff(EBuffAndDeBuff.HoTrao)}. {GetKeyWord(EKeyWord.LienKich)} nếu trước đó dùng {GetTextLinhKhi(CombatManager.linhKhiLienKich[elinhKhi])} hệ kĩ năng thì {GetColoredText($"hiệu quả gấp đôi", "red")}";
    }
}
