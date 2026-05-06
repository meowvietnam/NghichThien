using LongDH.LongDHModules;
using System.Linq;
using Skill;
using UnityEngine;
[CreateAssetMenu(fileName = nameof(DaiThonDietThuat), menuName = "Skill/Thần Thông/" + nameof(DaiThonDietThuat))]

public class DaiThonDietThuat : ThanThong
{
    [SerializeField]
    private int[] buffValues = new int[]
   {
        9,15,28,50,86
   };
    [SerializeField] private float delayTimeEffect = 1f;


    public override void Execute(CharacterCombat caster, CharacterCombat target, params object[] data)
    {
        target.buffAndDebuffCount.TryGetValue(EBuffAndDeBuff.HoTrao, out SecureInt hoTraoTarget);
        caster.buffAndDebuffCount.TryGetValue(EBuffAndDeBuff.HoTrao, out SecureInt hoTraoCaster);
        caster.RemoveBuffAndDebuffCount(EBuffAndDeBuff.HoTrao, hoTraoCaster);
        target.RemoveBuffAndDebuffCount(EBuffAndDeBuff.HoTrao, hoTraoTarget);

        caster.AddThanThongImpactAfterPlaySkill(this);
        Observer.AddObserver(EventList.Combat.END_TURN_COMBAT, ObserverEffect);
        ShowSkillEffectWorld(target, delayTimeEffect);


    }
    private void ObserverEffect(params object[] data)
    {
        CharacterCombat currentTurn = (CharacterCombat)data[0];
        CharacterCombat targetTurn = (CharacterCombat)data[1];

        if (currentTurn == null) return;

        if (currentTurn.specialThanThongImpactAfterPlaySkill.Any(x => x is DaiThonDietThuat))
        {
            currentTurn.RemoveAllThanThongImpactAfterPlaySkill(this);
            currentTurn.AddBuffAndDebuffCount(EBuffAndDeBuff.HoTrao, buffValues[GetIndexLevelThanThong(currentTurn)]);
        }


    }
    public override bool IsCanUse(CharacterCombat caster, CharacterCombat target)
    {
        
        if (!caster.thanThongUseThisComBat.Any(x => x.Any(y => y.thanThong is DaiThonDietThuat)))
        {
            return true;
        }
        return false;
    }


    public override string GetDesc(int level)
    {
        return $"Gỡ ra toàn bộ số tầng {GetKeyWordBuffOrDeBuff(EBuffAndDeBuff.HoTrao)} của bản thân và đối thủ. Hiệp kết thúc, thu hoạch được {GetColoredText($"+{buffValues[level-1]}", "red")} tầng {GetKeyWordBuffOrDeBuff(EBuffAndDeBuff.HoTrao)} {GetColoredText($"Mỗi cuộc chiến chỉ có thể sử dụng một lần", "red")}";

    }
}
