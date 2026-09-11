using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.UI;

namespace AllMaps.ui_rank
{
    public class TypeRankButton : BaseBehaviour
    {
        [field: SerializeField] public RankType RankType { get; private set; }
        [SerializeField] public RankPanel rankPanel;
        [SerializeField] private Button rankButton;
        [field: SerializeField] public RewardInfo RewardInfo { get; private set; }

        private void Start()
        {
            SetEventButton(rankButton, OnClick);
        }

        [Button]
        public void OnClick()
        {
            if (rankPanel.OnSelect(this))
            {
                rankButton.interactable = false;
            }
        }

        public void OnDeselect()
        {
            rankButton.interactable = true;
            RewardInfo.SetActive(false);
        }
    }
}