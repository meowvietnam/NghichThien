using LongDH.LongDHModules;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[CreateAssetMenu(fileName = nameof(TuLienKimAnhTrao), menuName = "Skill/Thần Thông/" + nameof(TuLienKimAnhTrao))]
public class TuLienKimAnhTrao : ThanThong
{
    [SerializeField] private int[] hoTrao;
    [SerializeField] private int[] hoTraoBonus;

    public override void Execute(CharacterCombat caster, CharacterCombat target, params object[] data)
    {
        caster.AddBuffAndDebuffCount(EBuffAndDeBuff.HoTrao, hoTrao[GetIndexLevelThanThong(caster)]);
        caster.AddThanThongImpactAfterPlaySkill(this);
        Observer.AddObserver(EventList.GamePlay.END_TURN_COMBAT, ObserverEffect);

    }
    private void ObserverEffect(params object[] data)
    {
        Debug.Log("có gọi vào LinhQuangANRemoveCheck1");
        CharacterCombat currentTurn = (CharacterCombat)data[0];
        if (currentTurn == null) return;
        if (currentTurn.specialThanThongImpactAfterPlaySkill.Any(x => x is TuLienKimAnhTrao))
        {
            List<TuLienKimAnhTrao> listThisSkill = currentTurn.specialThanThongImpactAfterPlaySkill
                                 .OfType<TuLienKimAnhTrao>()
                                 .ToList();
            currentTurn.RemoveAllThanThongImpactAfterPlaySkill(this);
            if (currentTurn.CurrentLinhKhi <= 0)
            {
                foreach (var item in listThisSkill)
                {
                    currentTurn.AddBuffAndDebuffCount(EBuffAndDeBuff.HoTrao, hoTraoBonus[GetIndexLevelThanThong(currentTurn)]);

                }

            }
        }
     



    }
 


    public override string GetDesc(int level)
    {
        return $" Thu hoạch được  {GetColoredText($"+{hoTrao[level - 1]}", "red")} tầng {GetKeyWordBuffOrDeBuff(EBuffAndDeBuff.HoTrao)}. Kết thúc hiệp, nếu không còn bất kì linh khí nào thì thu hoạch được {GetColoredText($"+{hoTraoBonus[level - 1]}", "red")} tầng {GetKeyWordBuffOrDeBuff(EBuffAndDeBuff.HoTrao)}";
    }
}
