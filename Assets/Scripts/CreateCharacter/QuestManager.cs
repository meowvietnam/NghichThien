using System;
using System.Collections.Generic;
using AllMaps;
using LuyenDan;
using MemoryPack;
using Network;
using Network.Extensions;
using Newtonsoft.Json;
using Sirenix.OdinInspector;
using UnityEngine;

namespace CreateCharacter
{
    [MemoryPackable]
    [Serializable]
    public partial class QuestManager
    {
        [field: SerializeField] public int CurrentMainQuest { get; set; } = 0;
        [field: SerializeField] public int CurrentCountSideQuest { get; set; } = 0;
        [ShowInInspector] public Dictionary<EDailyQuest, List<DailyQuest>> DailyQuests { get; set; } = new();
    }
}