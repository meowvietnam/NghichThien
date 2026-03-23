using UnityEngine;
public abstract class NgoDao : BaseData
{
    [SerializeField] public int require;
    [SerializeField] public BaseSkill.ETypeSkill[] eTypeRequire;

    public virtual void Learn(CharacterStats charStats)
    {

    }    
    public abstract string GetDes();
   
}
