using UnityEngine;
using System.Collections;
using System.Collections.Generic;
[CreateAssetMenu]
public class BasicTTDealDame : ThanThong, BaseSkill.LienKich
{

    [SerializeField] private int dame;
    [SerializeField] private int dameLienKich;
    [SerializeField] private ELinhKhi elinhKhi;
    [SerializeField]
    private float delayTimeEffect  = 0.5f;

  public override void Execute(CharacterCombat caster, CharacterCombat target)
    {

        target.TakeDameCaculate(caster, new List<int> { dame });
        ShowSkillEffect(target, delayTimeEffect);

    }

    public void ExecuteLienKich(CharacterCombat caster, CharacterCombat target)
    {
        target.TakeDameCaculate(caster, new List<int> { dameLienKich });
        Debug.Log("call lien kich basse tt dame");

    }

    public override string GetDesc()
    {
        return $"Cơ sở {GetTextLinhKhi(elinhKhi)} hệ thần thông, tạo thành {GetColoredText($"{dame}","red")} điểm sát thương. {GetKeyWord(EKeyWord.LienKich)} nếu trước đó dùng {GetTextLinhKhi(CombatManager.linhKhiLienKich[elinhKhi])} hệ kĩ năng thì tổn thương {GetColoredText($"+{dameLienKich}", "red")}";
    }
  
}
