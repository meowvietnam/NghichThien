using System.Collections.Generic;
using System.Linq;
using Events;
using LuyenDan;
using MemoryPack;
using Network;
using UnityEngine;

namespace AllMaps.EventReward
{
    [System.Serializable]
    [MemoryPackable]
    public partial class RewardSnapshot
    {
        public RewardItems RewardItems;
        public List<ItemRef> ItemRefs;
        public List<ItemEvent> itemEvents;
        public int Money;
        public int MoneyVip;

        public void AddReward(RewardSnapshot reward)
        {
            Money += reward.Money;
            MoneyVip += reward.MoneyVip;
            if (ItemRefs != null && reward.ItemRefs != null)
            {
                var dict = ItemRefs.ToDictionary(x => x.Id);
                foreach (var item in reward.ItemRefs)
                {
                    if (dict.TryGetValue(item.Id, out var exist))
                    {
                        exist.Count += item.Count;
                    }
                    else
                    {
                        ItemRefs.Add(item);
                        dict[item.Id] = item;
                    }
                }
            }

            if (itemEvents != null && reward.itemEvents != null)
            {
                Debug.Log(itemEvents.Count);
                var dict = itemEvents.ToDictionary(x => x.Id);
                foreach (var item in reward.itemEvents)
                {
                    if (dict.TryGetValue(item.Id, out var exist))
                    {
                        exist.Count += item.Count;
                    }
                    else
                    {
                        itemEvents.Add(item);
                        dict[item.Id] = item;
                    }
                }
            }
        }

        public RewardSnapshot Clone()
        {
            return new RewardSnapshot()
            {
                ItemRefs = ItemRefs.Select(x => x.DeepClone()).ToList(),
                Money = Money,
                MoneyVip = MoneyVip,
            };
        }
    }
}