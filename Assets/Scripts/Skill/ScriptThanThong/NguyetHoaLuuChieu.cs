using LongDH.LongDHModules;
using System.Collections.Generic;
using System.Linq;
using Skill;
using UnityEngine;
[CreateAssetMenu(fileName = nameof(NguyetHoaLuuChieu), menuName = "Skill/Thần Thông/" + nameof(NguyetHoaLuuChieu))]
public class NguyetHoaLuuChieu : ThanThong
{
    [SerializeField]
    private int percent = 20;
    [SerializeField]
    private int[] buffs = new int[]
 {
        4,4,6,6,8
 };
    [SerializeField] private float delayTimeEffect = 1f;
    [SerializeField] private float cameraSize = 5;
    public override void Execute(CharacterCombat caster, CharacterCombat target, params object[] data)
    {
        HauKeVoLuc(caster);
        
        caster.AddThanThongImpactAfterPlaySkill(this);
        target.HapThuLinhKhi(buffs[GetIndexLevelThanThong(caster)]);
        Observer.AddObserver(EventList.Combat.CACULATE_DAME_BY_THANTHONG, ObserverEffect);
        Observer.AddObserver(EventList.Combat.NEW_TURN_COMBAT, ObserverEffect3);
        ShowSkillEffectParticleBase(caster, delayTimeEffect, cameraSize);
    }
    private void ObserverEffect(params object[] data)
    {
        CharacterCombat currentTurn = (CharacterCombat)data[0]; // ng gây dame
        CharacterCombat targetTurn = (CharacterCombat)data[1];// người nhận dame

        // hiệu ứng giảm bớt dame 
        if (targetTurn == null) return;

        if (targetTurn.specialThanThongImpactAfterPlaySkill.Any(x => x is NguyetHoaLuuChieu))
        {
         
            for (int i = 0; i < targetTurn.allDamages.Count; i++)
            {
                int item = targetTurn.allDamages[i];
                int dame = (int)(item * percent / 100f);
                targetTurn.allDamages[i] -= dame;
            }

        }


    }
    private void ObserverEffect3(params object[] data)
    {
        CharacterCombat currentTurn = (CharacterCombat)data[0];
        CharacterCombat targetTurn = (CharacterCombat)data[1];

        if (currentTurn == null) return;

        currentTurn.RemoveAllThanThongImpactAfterPlaySkill(this);



    }



    public override string GetDesc(int level)
    {
        return $"{GetKeyWord(EKeyWord.HauKeVoLuc)} Đối thủ hấp thụ {GetColoredText($"+{buffs[level - 1]} * X", "red")} điểm linh khí. Bản thân nhận thần thông tổn thương giảm {GetColoredText($"{percent}%", "red")}";

    }
}
