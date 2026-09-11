using System;
using System.Collections.Generic;
using CreateCharacter;
using LongDH.LongDHModules;
using Network.Callback;
using Rank;
using UnityEngine;
using UnityEngine.UI;

namespace AllMaps.ui_rank
{
    public class RankPanel : PanelGroup, IGetRank
    {
        [SerializeField] public PopupInfor popupInfor;
        [SerializeField] public Popup mainPopup;
        [SerializeField] private Button infoRewardBtn;
        [SerializeField] public Transform rankContainer;
        [SerializeField] public RankElementInfor rankPrefab;
        [SerializeField] public RankElementInfor myRank;
        [SerializeField] public RankElementInfor[] top3;
        [SerializeField] public TypeRankButton[] rankButtons;
        private TypeRankButton currentSelect;
        public List<RankElementInfor> tops { get; private set; } = new();
        private TimeSpan resetTime = TimeSpan.Zero;
        private bool isLock;

        private void Start()
        {
            Observer.AddObserver(EventList.GamePlay.SHOW_RANK_INFO_POPUP, ShowRankInfoPopup);
            SetEventButton(infoRewardBtn, ShowHideRewardInfo);
            ClearTops();
        }

        private void ShowRankInfoPopup(object[] data)
        {
            var charData = data[0] as CharacterStatsSecure;
            popupInfor.InitPopDoXet(charData);
        }


        private void FixedUpdate()
        {
            if (currentSelect == null)
            {
                return;
            }

            var time = TimeSpan.FromSeconds(Time.fixedUnscaledDeltaTime);
            resetTime -= time;
            currentSelect.RewardInfo.SetEndTime(resetTime);
        }

        private void ShowHideRewardInfo()
        {
            if (currentSelect == null)
            {
                return;
            }

            var rewardInfo = currentSelect.RewardInfo;
            rewardInfo.SetActive(!rewardInfo.GetActive());
        }

        public void DoShow()
        {
            mainPopup.Show();
            if (currentSelect == null)
            {
                rankButtons[0].OnClick();
            }
            else
            {
                LoadRank();
            }
        }

        private void LoadRank()
        {
            var type = currentSelect.RankType;
            isLock = true;
            Observer.Notify(EventList.Network.GET_RANK, this, type);
        }

        public bool OnSelect(TypeRankButton button)
        {
            if (isLock) return false;
            if (currentSelect != null)
            {
                currentSelect.OnDeselect();
            }

            currentSelect = button;
            LoadRank();
            return true;
        }

        public void GetRankSuccess(IRank rank)
        {
            isLock = false;
            resetTime = rank.NextResetTime;
            ClearTops();
            rank.Init(this);
        }

        public void ClearTops()
        {
            foreach (var rankElementInfor in tops)
            {
                rankElementInfor.DestroySelf();
            }

            tops.Clear();
            foreach (var rankElementInfor in top3)
            {
                rankElementInfor.SetActive(false);
            }
        }

        public void GetRankFail(string message)
        {
            isLock = false;
            NotifyGroup.Instance.Show(message);
        }

        private void OnDestroy()
        {
            Observer.RemoveObserver(EventList.GamePlay.SHOW_RANK_INFO_POPUP, ShowRankInfoPopup);
            infoRewardBtn.onClick.RemoveAllListeners();
        }
    }
}