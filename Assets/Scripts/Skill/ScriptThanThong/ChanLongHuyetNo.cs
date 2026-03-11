using LongDH.LongDHModules;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
[CreateAssetMenu(fileName = nameof(ChanLongHuyetNo), menuName = "Skill/Thần Thông/" + nameof(ChanLongHuyetNo))]

public class ChanLongHuyetNo : ThanThong
{
    [SerializeField] private int require = 1;   
    
    public override void Execute(CharacterCombat caster, CharacterCombat target, params object[] data)
    {
        HauKeVoLuc(caster);
        caster.RemoveBuffAndDebuffCount(EBuffAndDeBuff.ChanLongChiHuyet,require);
        caster.AddThanThongImpactAfterPlaySkill(this);
        Observer.AddObserver(EventList.GamePlay.CACULATE_DAME_BY_THANTHONG, ObserverEffect);

    }
    private void ObserverEffect(params object[] data)
    {
        CharacterCombat currentTurn = (CharacterCombat)data[0];
        CharacterCombat targetTurn = (CharacterCombat)data[1];

        if (currentTurn == null) return;
        if (currentTurn.specialThanThongImpactAfterPlaySkill.Any(x => x is ChanLongHuyetNo))
        {
            currentTurn.RemoveOneThanThongImpactAfterPlaySkill(this);
            targetTurn.AddDamageData(new List<int> { targetTurn.allDamages[0] });
        }



    }
    public override bool IsCanUse(CharacterCombat caster, CharacterCombat target)
    {
        caster.buffAndDebuffCount.TryGetValue(EBuffAndDeBuff.ChanLongChiHuyet, out SecureInt buff);
        if(buff > require)
        {
            return true;
        }
        return false;
    }



    public override string GetDesc(int level)
    {
        return $"{GetKeyWord(EKeyWord.HauKeVoLuc)} Tiêu hao {GetColoredText($"{require}", "red")} tầng {GetKeyWordBuffOrDeBuff(EBuffAndDeBuff.ChanLongChiHuyet)}. Lần sử dụng thần thông tiếp theo tổn thương gấp đôi";

    }
}
