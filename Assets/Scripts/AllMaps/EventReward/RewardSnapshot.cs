using LuyenDan;
using System.Collections.Generic;
using UnityEngine;

public class RewardSnapshot 
{
    public List<ItemRef> ItemRefs { get; set; } = new(); // voi count item ref la count user reward 
    public int Money { get; set; } = 0;
    public int MoneyVip { get; set; } = 0;

}
