using System;
using System.Collections.Generic;
using System.Linq;
using AllMaps.ui_rank;
using CreateCharacter;
using MemoryPack;
using Network;
using ArenaManager = AllMaps.DauTruong.ArenaManager;

namespace Rank
{
    [MemoryPackable]
    public partial class RankArena : IRank
    {
        public RankType Type => RankType.RankArena;
        public List<CharacterWrapper> Tops { get; private set; }
        public TimeSpan NextResetTime { get; private set; }
        public Dictionary<ETypeArenaRank, int> TypeRankArenaWithValue { get; private set; }
        public Dictionary<ETypeArenaRank, int> TypeRankArenaWithReward { get; private set; }

        public void Init(RankPanel panel)
        {
            ArenaManager.TypeRankArenaWithValue = TypeRankArenaWithValue;
            ArenaManager.TypeRankArenaWithReward = TypeRankArenaWithReward;
            var container = panel.rankContainer;
            var prefab = panel.rankPrefab;
            var tops = panel.tops;
            for (var i = 0; i < Tops.Count; i++)
            {
                var top = Tops[i];
                var instance = UnityEngine.Object.Instantiate(prefab, container);
                Init(instance, top, i + 1);
                tops.Add(instance);
            }

            var top3 = Tops.Take(3).ToList();
            for (var i = 0; i < top3.Count; i++)
            {
                var top = top3[i];
                var uiTop = panel.top3[i];
                uiTop.SetActive(true);
                Init(uiTop, top, i + 1);
            }

            var charData = Client.Instance.CharacterData;
            var myRank = Tops.FindIndex(x => x.Character.Id == charData.Id);
            if (myRank < 0)
            {
                myRank = 999;
            }

            Init(panel.myRank, charData, myRank + 1);
        }

        private void Init(RankElementInfor instance, CharacterWrapper top, int rank)
        {
            instance.Init(top.Character, rank, Type, top.Character.ArenaManager.Points);
        }

        private void Init(RankElementInfor instance, CharacterData top, int rank)
        {
            instance.Init(top, rank, Type, top.ArenaManager.Points);
        }

        public RankArena(List<CharacterWrapper> tops, TimeSpan nextResetTime,
            Dictionary<ETypeArenaRank, int> typeRankArenaWithValue,
            Dictionary<ETypeArenaRank, int> typeRankArenaWithReward)
        {
            Tops = tops;
            NextResetTime = nextResetTime;
            TypeRankArenaWithValue = typeRankArenaWithValue;
            TypeRankArenaWithReward = typeRankArenaWithReward;
        }
    }
}