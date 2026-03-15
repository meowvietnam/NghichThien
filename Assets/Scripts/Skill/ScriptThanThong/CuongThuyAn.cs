using LongDH.LongDHModules;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
[CreateAssetMenu(fileName = nameof(CuongThuyAn), menuName = "Skill/Thần Thông/" + nameof(CuongThuyAn))]

public class CuongThuyAn : ThanThong
{
    [SerializeField] private int[] dames;
 

    public override void Execute(CharacterCombat caster, CharacterCombat target, params object[] data)
    {
        HauKeVoLuc(caster);
        Observer.AddObserver(EventList.GamePlay.NEW_TURN_COMBAT, ObserverEffect2);
        Observer.AddObserver(EventList.GamePlay.CACULATE_DAME_BY_THANTHONG, ObserverEffect);


    }
    private void ObserverEffect(params object[] data)
    {
        CharacterCombat currentTurn = (CharacterCombat)data[0];
        CharacterCombat targetTurn = (CharacterCombat)data[1];

        if (currentTurn == null) return;
        if (currentTurn.specialThanThongImpactAfterPlaySkill.Any(x => x is CuongThuyAn) && CheckIsEqualETypeSkill(currentTurn))
        {
            int dame = dames[GetIndexLevelThanThong(currentTurn)];
            targetTurn.AddDamageData(new List<int> { dame });


            


        }



    }
    private void ObserverEffect2(params object[] data)
    {
        CharacterCombat currentTurn = (CharacterCombat)data[0];
        CharacterCombat targetTurn = (CharacterCombat)data[1];

        if (currentTurn == null) return;
        if (currentTurn.thanThongUseLastTurn == null) return;
        if(currentTurn.thanThongUseLastTurn.Any(x=>x.thanThong is CuongThuyAn))
        {
            if(!currentTurn.specialThanThongImpactAfterPlaySkill.Any(x => x is CuongThuyAn))
            {
                currentTurn.AddThanThongImpactAfterPlaySkill(this);
            }    
        }    
        else
        {
            if (currentTurn.specialThanThongImpactAfterPlaySkill.Any(x => x is CuongThuyAn))
            {
                currentTurn.RemoveAllThanThongImpactAfterPlaySkill(this);
            }
        }
        


    }
    public override string GetDesc(int level)
    {
        return $"{GetKeyWord(EKeyWord.HauKeVoLuc)} Sử dụng {GetTextTypeSkill()} hệ thần thông thì tổn thương {GetColoredText($"+{dames[level - 1]}", "red")} (tồn tại ở hiệp sau)";
    }
}
