using System;
using System.Collections.Generic;
using Network;
using Network.Extensions;
using Network.Extensions.MemoryPack;
using UnityEngine;

namespace Rank
{
    public class MonsterKillPanel : Popup
    {
        [SerializeField] private ElementReward elementPrefab;
        [SerializeField] private RectTransform content;
        private List<ElementReward> elements = new();

        public override void Show(float delay = 0.5f, Action actionComplete = null)
        {
            foreach (var element in elements)
            {
                Destroy(element.gameObject);
            }

            elements.Clear();
            GetData();
        }

        private async void GetData()
        {
            var reader = await Client.Instance.RequestWait(MessageTag.GetMonsterKillRequest);
            var monsterKill = reader.MemRead<MonsterKill>();
            foreach (var reward in monsterKill.Rewards)
            {
                var element = Instantiate(elementPrefab, content);
                element.Init(monsterKill.Points, reward);
                elements.Add(element);
            }

            base.Show();
        }
    }
}