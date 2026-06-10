using LuyenDan;
using Network;
using Network.Extensions;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[System.Serializable]
public class RewardSnapshot : IMemorySerializable<RewardSnapshot>
{
    public List<ItemRef> ItemRefs = new(); // voi count item ref la count user reward 
    public int Money = 0;
    public int MoneyVip = 0;

    public int ByteSize()
    {
        return Money.ByteSize() + MoneyVip.ByteSize() + ItemRefs.ByteSize();
    }

    public void Deserialize(IReader reader, ref RewardSnapshot value)
    {
        value.Money = reader.Read<int>();
        value.MoneyVip = reader.Read<int>();
        value.ItemRefs = reader.ReadArray<ItemRef>().ToList();
    }

    public void Serialize(IWriter writer)
    {
        writer.Write(Money);
        writer.Write(MoneyVip);
        writer.WriteArray(ItemRefs);
    }
}
