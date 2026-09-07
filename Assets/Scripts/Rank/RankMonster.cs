using System;
using System.Collections.Generic;
using System.Linq;
using AllMaps.ui_rank;
using MemoryPack;
using Network;

namespace Rank
{
    [MemoryPackable]
    public partial class RankMonster : IRank
    {
        public RankType Type => RankType.RankMonster;
        public TimeSpan NextResetTime { get; private set; }
        public List<MonsterRankEntity> Tops { get; private set; }
        public int MyPoints { get; private set; }

        public RankMonster(List<MonsterRankEntity> tops, TimeSpan nextResetTime, int myPoints)
        {
            Tops = tops;
            NextResetTime = nextResetTime;
            MyPoints = myPoints;
        }

        public void Init(RankPanel panel)
        {
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
            var myRank = Tops.FindIndex(x => x.CharacterWrapper.Character.Id == charData.Id);
            if (myRank < 0)
            {
                myRank = 999;
            }

            panel.myRank.Init(charData, myRank + 1, Type, MyPoints);
        }

        private void Init(RankElementInfor instance, MonsterRankEntity top, int rank)
        {
            instance.Init(top.CharacterWrapper.Character, rank, Type, top.Point);
        }
    }
}