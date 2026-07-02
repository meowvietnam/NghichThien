using System;
using MemoryPack;
using Network;
using Sirenix.OdinInspector;
using UnityEngine;

namespace AllMaps
{
    [MemoryPackable]
    [Serializable]
    public partial class DailyQuest
    {
        [field: SerializeField] public long Id { get; set; }
        [field: SerializeField] public EDailyQuest EDailyQuest { get; set; }
        [ShowInInspector] public IQuestReward QuestReward { get; set; }
        [field: SerializeField] public bool IsRewardClaimed { get; set; }
        [ShowInInspector] public DateTime LastReload { get; set; }
        public void Claim()
        {
            if (IsRewardClaimed)
            {
                return;
            }
            var charData = Client.Instance.CharacterData;
            QuestReward.Claim(charData);
            IsRewardClaimed = true;
        }
    }
}