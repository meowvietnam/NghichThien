using System;
using System.Collections.Generic;
using System.Linq;
using CreateCharacter;
using Database;
using LuyenDan;
using MemoryPack;
using Network;
using Network.Extensions;
using Network.Extensions.MemoryPack;
using Sirenix.OdinInspector;
using SQLite;
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

        [field: SerializeField]
        [ValueConverter(typeof(MemoryPackValueConverter<List<IMailReward>>))]
        public List<IMailReward> Rewards { get; set; } = new();
        [field: ShowInInspector] public DateTime CreatedAt { get; set; }
    }
}