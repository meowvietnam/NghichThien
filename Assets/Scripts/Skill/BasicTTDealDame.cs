using UnityEngine;
using System.Collections;
using System.Collections.Generic;
[CreateAssetMenu(fileName = nameof(BasicTTDealDame), menuName = "Skill/Thần Thông/" + nameof(BasicTTDealDame))]
public class BasicTTDealDame : ThanThong, BaseSkill.LienKich
{

    [SerializeField] private int[] dame;
    [SerializeField] private int[] dameLienKich;
    [SerializeField] private ELinhKhi elinhKhi;
    [SerializeField]
    private float delayTimeEffect  = 0.5f;
    public override void Execute(CharacterCombat caster, CharacterCombat target, params object[] data)
    {

        target.TakeDameByThanThong(caster, new List<int> {     dame[GetIndexLevelThanThong(caster)] });
        ShowSkillEffectBuff(target, delayTimeEffect);

    }

    public void ExecuteLienKich(CharacterCombat caster, CharacterCombat target)
    {
        target.TakeDameByThanThong(caster, new List<int> { dameLienKich[GetIndexLevelThanThong(caster)] });
        Debug.Log("call lien kich basse tt dame");

    }

    public override string GetDesc(int level)
    {
        return $"Cơ sở {GetTextLinhKhi(elinhKhi)} hệ thần thông, tạo thành {GetColoredText($"{dame[level-1]}","red")} điểm sát thương. {GetKeyWord(EKeyWord.LienKich)} nếu trước đó dùng {GetTextLinhKhi(CombatManager.linhKhiLienKich[elinhKhi])} hệ kĩ năng thì tổn thương {GetColoredText($"+{dameLienKich[level-1]}", "red")}";
    }

   
}
