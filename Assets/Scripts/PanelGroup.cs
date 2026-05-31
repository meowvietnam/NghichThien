using LongDH.LongDHModules;
using LongDHModules;
using UnityEngine;

public class PanelGroup : BaseBehaviour
{
    public virtual void Init()
    {
        Pool.Set(this.GetType(), this);
    }    
}
