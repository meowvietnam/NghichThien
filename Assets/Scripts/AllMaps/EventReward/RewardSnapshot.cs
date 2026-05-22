using LuyenDan;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class RewardSnapshot 
{
    public List<ItemRef> ItemRefs = new(); // voi count item ref la count user reward 
    public int Money = 0;
    public int MoneyVip = 0;

}
