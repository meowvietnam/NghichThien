using LongDH.LongDHModules;
using LongDH.Script.ObjectPool;
using System.Linq;
using UnityEngine;
[CreateAssetMenu(fileName = nameof(ThaiThanhHuyenThuat), menuName = "Skill/Thần Thông/" + nameof(ThaiThanhHuyenThuat))]

public class ThaiThanhHuyenThuat : NeTranhTT
{
    [SerializeField] private float cameraSize = 5;
    public override void Execute(CharacterCombat caster, CharacterCombat target, params object[] data)
    {
        base.Execute(caster, target, data);
        HauKeVoLuc(caster);
        Observer.AddObserver(EventList.Combat.NEW_TURN_COMBAT, ObserverEffect);
        caster.effectSkillNoDestroy.Add(this,ShowSkillEffectParticleBase(caster, 0, cameraSize , false));
    }
    protected void ObserverEffect(params object[] data)
    {
        CharacterCombat currentTurn = (CharacterCombat)data[0];
        if (currentTurn == null) return;
        currentTurn.RemoveAllThanThongImpactAfterPlaySkill(this);
        currentTurn.RemoveEffectSkillNoDestroy(this);
            

    }
    public override string GetDesc(int level)
    {
        return $"{GetKeyWord(EKeyWord.HauKeVoLuc)}. Trong hiệp đối thủ lần tới, bản thân có {GetColoredText($"{percentNeTranh[level - 1]}%", "red")} né tránh sát thương";

    }

    protected override void NeTranhSuccess(CharacterCombat takeDameChar)
    {
    }

  
}
