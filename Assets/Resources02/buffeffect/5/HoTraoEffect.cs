using LongDH.LongDHModules;
using System.Linq;
using UnityEngine;

public class HoTraoEffect : BuffEffect
{

    protected override void Awake()
    {
        base.Awake();
        Observer.AddObserver(EventList.Combat.CHARACTER_TAKE_DAME , CharacterTakeDame);

    }
    private void CharacterTakeDame(params object[] data)
    {
        CharacterCombat targetTurn = (CharacterCombat)data[1];

        if (targetTurn == null) return;
        targetTurn.GetComponentInChildren<HoTraoEffect>().anim.SetTrigger("TakeDame");
    }    

}
