using LongDH.LongDHModules;
using System.Linq;
using UnityEngine;
[CreateAssetMenu(fileName = nameof(LienNhac), menuName = "Skill/Thần Thông/" + nameof(LienNhac))]

public class LienNhac : ThanThong
{
    
    [SerializeField]
    private int require = 5;
    public override void Execute(CharacterCombat caster, CharacterCombat target, params object[] data)
    {
        HauKeVoLuc(caster);
        caster.RemoveBuffAndDebuffCount(EBuffAndDeBuff.Tran, require);
        caster.totalKhi += 999999;
        caster.AddThanThongImpactAfterPlaySkill(this);
        Observer.AddObserver(EventList.GamePlay.END_TURN_COMBAT, ObserverEffect);

    }
    public override bool IsCanUse(CharacterCombat caster, CharacterCombat target)
    {
        caster.buffAndDebuffCount.TryGetValue(EBuffAndDeBuff.Tran, out int tran);

        return tran>= require;
    }
    private void ObserverEffect(params object[] data)
    {
        CharacterCombat currentTurn = (CharacterCombat)data[0];
        CharacterCombat targetTurn = (CharacterCombat)data[1];

        if (currentTurn == null) return;

        if (currentTurn.specialThanThongImpactAfterPlaySkill.Any(x => x is LienNhac))
        {

            currentTurn.RemoveAllThanThongImpactAfterPlaySkill(this);
            currentTurn.totalKhi -= 999999;

            
        }


    }

    public override string GetDesc(int level)
    {
        return $"{GetKeyWord(EKeyWord.HauKeVoLuc)} Tiêu hao {GetColoredText($"{require}", "red")} tầng {GetKeyWordBuffOrDeBuff(EBuffAndDeBuff.Tran)} thì kết thúc hiệp này không cần tiêu tán linh khí";

    }
}
