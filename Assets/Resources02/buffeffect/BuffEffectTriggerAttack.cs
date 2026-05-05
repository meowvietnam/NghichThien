using LongDH.LongDHModules;
using UnityEngine;

public class BuffEffectTriggerAttack : BuffEffect
{
    protected override void Awake()
    {
        base.Awake();
        if(eBuff == EBuffAndDeBuff.HoTrao)
        {
            Observer.AddObserver(EventList.Combat.CHARACTER_TAKE_DAME, CharacterActiveAnim);

        }
        else if (eBuff == EBuffAndDeBuff.AnhDich)
        {
            Observer.AddObserver(EventList.Combat.ANH_DICH, CharacterActiveAnim);

        }
        else if (eBuff == EBuffAndDeBuff.QuyTon)
        {
            Observer.AddObserver(EventList.Combat.QUY_TON, CharacterActiveAnim);

        }

    }
    private void CharacterActiveAnim(params object[] data)
    {
        CharacterCombat activeTurn = (CharacterCombat)data[0];

        if (activeTurn == null) return;
        activeTurn.GetComponentInChildren<BuffEffectTriggerAttack>().anim.SetTrigger("Active");
    }
}
