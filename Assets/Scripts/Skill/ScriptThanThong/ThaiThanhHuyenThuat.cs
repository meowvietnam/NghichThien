using LongDH.LongDHModules;
using System.Linq;
using UnityEngine;
[CreateAssetMenu(fileName = nameof(ThaiThanhHuyenThuat), menuName = "Skill/Thần Thông/" + nameof(ThaiThanhHuyenThuat))]

public class ThaiThanhHuyenThuat : NeTranhTT
{
    public override void Execute(CharacterCombat caster, CharacterCombat target, params object[] data)
    {
        base.Execute(caster, target, data);
        HauKeVoLuc(caster);
        Observer.AddObserver(EventList.GamePlay.NEW_TURN_COMBAT, ObserverEffect);

    }
    protected void ObserverEffect(params object[] data)
    {
        CharacterCombat currentTurn = (CharacterCombat)data[0];
        if (currentTurn == null) return;
        currentTurn.RemoveAllThanThongImpactAfterPlaySkill(this);


    }
    public override string GetDesc(int level)
    {
        return $"{GetKeyWord(EKeyWord.HauKeVoLuc)}. Trong hiệp đối thủ lần tới, bản thân có {GetColoredText($"{percentNeTranh[level - 1]}%", "red")} né tránh sát thương";

    }

    protected override void NeTranhSuccess(CharacterCombat takeDameChar)
    {
    }

  
}
