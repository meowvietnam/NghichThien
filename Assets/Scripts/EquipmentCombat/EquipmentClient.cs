using CreateCharacter;
using UnityEngine;
using System.Collections.Generic;
using LuyenKhi;
using System.Linq;
[System.Serializable]
public class EquipmentClient 
{
    [System.Serializable]
    public class EffectClient : IEffect
    {
        public EffectType Type { get; set; }
        public SecureInt Value = new SecureInt(0);

        public int GetValue() => Value.Value;
    
    }
    public SecureInt id = new SecureInt(ushort.MaxValue);
    public string nameEquip;
    public List<EffectClient> listEffectEquip= new List<EffectClient>();
    public LuyenKhiRewardItem Item => LuyenKhiResources.Instance.RewardItems.FirstOrDefault(x => x.Id == id);

}
