using LongDH.LongDHModules;
using System.Linq;
using UnityEngine;
[CreateAssetMenu(fileName = nameof(NguyetBocHanHuy), menuName = "Skill/Thần Thông/" + nameof(NguyetBocHanHuy))]

public class NguyetBocHanHuy : ThanThong
{

    [SerializeField]
    private int percent = 25;
    [SerializeField]
    private int[] buffs = new int[]
 {
        1,1,2,2,3
 };
    [SerializeField] private float delayTimeEffect = 1f;
    [SerializeField] private float cameraSize = 5;
    public override void Execute(CharacterCombat caster, CharacterCombat target, params object[] data)
    {
        HauKeVoLuc(caster);
       
        caster.AddThanThongImpactAfterPlaySkill(this);
        Observer.AddObserver(EventList.Combat.PLAY_SKILL, ObserverEffect);
        Observer.AddObserver(EventList.Combat.NEW_TURN_COMBAT, ObserverEffect3);
        ShowSkillEffectParticleBase(target, delayTimeEffect, cameraSize);

    }
    private void ObserverEffect(params object[] data)
    {
        // mình playskill
        CharacterCombat currentTurn = (CharacterCombat)data[0];
        CharacterCombat targetTurn = (CharacterCombat)data[1];

        if (targetTurn == null) return;

        if (targetTurn.specialThanThongImpactAfterPlaySkill.Any(x => x is NguyetBocHanHuy))
        {
            if(CombatStatic.IsActiveWithPercent(percent))
            {
                int sum = currentTurn.thanThongCurrentUse.linhKhiTieuTan.Sum(x => x.count);
                currentTurn.AddBuffAndDebuffCount(EBuffAndDeBuff.ThanThuong, sum * buffs[GetIndexLevelThanThong(targetTurn)]);
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
        return $"{GetKeyWord(EKeyWord.HauKeVoLuc)} Đối thủ sử dụng thần thông có {GetColoredText($"{percent}%", "red")} nhận phải {GetColoredText($"+{buffs[level - 1]} * X", "red")} tầng {GetKeyWordBuffOrDeBuff(EBuffAndDeBuff.ThanThuong)} (X là tổng số linh khí tiêu hao của thần thông đó)";

    }
}
