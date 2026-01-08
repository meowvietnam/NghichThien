using UnityEngine;
[CreateAssetMenu]
public class BasicTTDealDame : ThanThong, BaseSkill.LienKich
{

    [SerializeField] private int dame;
    [SerializeField] private int dameLienKich;
    [SerializeField] private ELinhKhi elinhKhi;

    public override void Execute(CharacterCombat caster, CharacterCombat target)
    {
        target.TakeDame(caster,dame);

    }

    public void ExecuteLienKich(CharacterCombat caster, CharacterCombat target)
    {
        target.TakeDame(caster, dameLienKich);

    }

    public override string GetDesc()
    {
        return $"Cơ sở {GetTextLinhKhi(elinhKhi)} hệ thần thông, tạo thành {GetColoredText($"{dame}","red")} điểm sát thương. {GetKeyWord(EKeyWord.LienKich)} nếu trước đó dùng {GetTextLinhKhi(CombatManager.linhKhiLienKich[elinhKhi])} hệ kĩ năng thì tổn thương {GetColoredText($"+{dameLienKich}", "red")}";
    }
  
}
