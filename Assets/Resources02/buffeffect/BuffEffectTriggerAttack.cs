using LongDH.LongDHModules;
using UnityEngine;

public class BuffEffectTriggerAttack : BuffEffect
{
    public void ActiveAnim()
    {
        anim.SetTrigger("Active");
    }
}