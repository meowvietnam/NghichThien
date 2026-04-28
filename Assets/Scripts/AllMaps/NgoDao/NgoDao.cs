using UnityEngine;

namespace AllMaps.NgoDao
{
    public abstract class NgoDao : BaseData
    {
        [SerializeField] public int require;
        [SerializeField] public BaseSkill.ETypeSkill[] eTypeRequire;

        public virtual void Learn(INetworkEvent.ILearnNgoDao data)
        {
        }    
        public abstract string GetDes();
   
    }
}
