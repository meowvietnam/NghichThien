using System;
using System.Collections.Generic;
using CreateCharacter;
using MemoryPack;
using Sirenix.OdinInspector;
using UnityEngine;

namespace AllMaps
{
    [Serializable]
    [MemoryPackable]
    public partial class Mail
    {
        [field: SerializeField] public long Id { get; set; }
        [field: SerializeField] public string Title { get; set; } = string.Empty;
        [field: SerializeField] public string Content { get; set; } = string.Empty;
        [field: SerializeField] public bool IsRead { get; set; }
        [field: SerializeField] public bool IsClaim { get; set; }

        [field: SerializeField] public List<IMailReward> Rewards { get; set; } = new();
        [field: ShowInInspector] public DateTime CreatedAt { get; set; }

        public bool HasReward()
        {
            return Rewards.Count > 0;
        }
    }
}