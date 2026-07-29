using System;
using Base;
using CreateCharacter;
using LuyenDan;
using Network;
using Network.Callback;
using Network.ClientState;
using Network.Models;
using UnityEngine;
using UnityEngine.UI;

namespace Auction
{
    public class AuctionPanel : BaseBehaviour, IAuctionCallback
    {
        [field: SerializeField] public Host Host { get; private set; }
        [field: SerializeField] public CurrentAuctionItem CurrentAuctionItem { get; private set; }
        [field: SerializeField] public PlayerList[] PlayerLists { get; private set; }
        [field: SerializeField] public InfoPanel InfoPanel { get; private set; }
        [field: SerializeField] public ResultPanel ResultPanel { get; private set; }
        [field: SerializeField] public SelectNum SelectBidAmount { get; private set; }
        [SerializeField] private GameObject root;
        [SerializeField] private Button infoButton;
        [SerializeField] private Button selectBidAmountButton;
        [SerializeField] private ProgressBar progressBar;
        [SerializeField] private Text myBidAmountText;
        private CharacterData CharData => Client.Instance.CharacterData;
        public PlayerAuctionModel Me { get; private set; }
        private AuctionState auctionState;
        public static AuctionPanel Instance { get; private set; }

        private void Awake()
        {
            Instance = this;
            SetEventButton(infoButton, ShowInfo);
            SetEventButton(selectBidAmountButton, ShowSelectBidAmount);
            root.SetActive(false);
        }

        private void Start()
        {
            auctionState = new AuctionState(this);
            Client.Instance.ChangeState(auctionState);
            JoinAuction();
        }

        private void JoinAuction()
        {
            progressBar.SetText("Đang tham gia đấu giá...");
            progressBar.DoShow(0.5f, 0.25f, null);
            auctionState.JoinAuction();
        }

        public void OnJoinAuctionResult(JoinAuctionResult result)
        {
            void Action()
            {
                if (!result.Success)
                {
                    NotifyGroup.Instance.Show(result.Message);
                    return;
                }
                Init(result.AuctionData);
            }
            if (progressBar.IsCompleted())
            {
                Action();
            }
            else
            {
                progressBar.DoKill(Action);
            }
        }

        public void OnGetAuctionDataResult(GetAuctionDataResult result)
        {
            void Action()
            {
                if (!result.Success)
                {
                    NotifyGroup.Instance.Show(result.Message);
                    return;
                }
                Init(result.AuctionData);
            }

            if (progressBar.IsCompleted())
            {
                Action();
            }
            else
            {
                progressBar.DoKill(Action);
            }
        }

        private void SetCurrentAuctionItem(Network.Models.CurrentAuctionItem currentItem)
        {
            CurrentAuctionItem.SetItem(currentItem.Item);
            CurrentAuctionItem.SetCurrentPlayerBid(currentItem.PlayerBid);
            CurrentAuctionItem.SetCurrentPrice(currentItem.CurrentPrice);
            CurrentAuctionItem.SetMinPrice(currentItem.MinPrice);
            CurrentAuctionItem.SetMaxPrice(currentItem.MaxPrice);
        }

        public void UpdateMyBidAmount()
        {
            myBidAmountText.text = Utils.FormatCount(Me.Amount);
        }

        public void UpdateHostTime(TimeSpan totalTime, TimeSpan remainingTime)
        {
            Host.SetAuctionDuration(totalTime);
            Host.SetEndAfter(remainingTime);
        }
        public void NewAuctionSession(Network.Models.CurrentAuctionItem currentItem)
        {
            UpdateHostTime(currentItem.TotalTime, currentItem.RemainingTime);
            SetCurrentAuctionItem(currentItem);
            foreach (var playerList in PlayerLists)
            {
                playerList.ResetNewSession();
            }
            Me.ResetNewSession();
            UpdateMyBidAmount();
        }
        private void Init(AuctionData data)
        {
            Init(data.CurrentItem, data.Players, data.MyPlayer);
            root.SetActive(true);
        }
        private void Init
            (Network.Models.CurrentAuctionItem currentItem, PlayerAuctionModel[] players, PlayerAuctionModel me)
        {
            Me = me;
            UpdateHostTime(currentItem.TotalTime, currentItem.RemainingTime);
            SetCurrentAuctionItem(currentItem);
            InitPlayers(players);
            UpdateMyBidAmount();
        }

        private int GetTotalAmount()
        {
            var remainingMoneyVip = CharData.MoneyVip;
            var currentBidAmount = Me.Amount;
            var total = currentBidAmount + remainingMoneyVip;
            return total;
        }

        private void ShowSelectBidAmount()
        {
            var totalAmount = GetTotalAmount();

            void UpdateText(int value)
            {
                var text = "Ra giá vật phẩm này" + "\n"
                                                 + $"Số tiên ngọc còn lại: {Utils.FormatCount(totalAmount)}"
                                                 +"\n"
                                                 + $"Giá bạn đặt: {Utils.FormatCount(value)}";
                SelectBidAmount.SetText(text);
            }
            SelectBidAmount.SetMin(CurrentAuctionItem.MinPrice);
            SelectBidAmount.SetMax(CurrentAuctionItem.MaxPrice);
            SelectBidAmount.SetValue(CurrentAuctionItem.MinPrice);
            SelectBidAmount.ValueChanged += UpdateText;
            UpdateText(SelectBidAmount.GetValue());
            SelectBidAmount.Show(0.15f);
        }

        public void OnSelectBidAmountConfirm()
        {
            var value = SelectBidAmount.GetValue();
            var totalAmount = GetTotalAmount();
            if (value > totalAmount)
            {
                NotifyGroup.Instance.Show("Không đủ tiên ngọc!");
                return;
            }

            auctionState.AuctionBid(value);
        }
        public void OnAuctionBidResult(AuctionBidResult result)
        {
            if (result.Success)
            {
                Me.Amount = result.Amount;
                UpdateMyBidAmount();
            }
            else
            {
                NotifyGroup.Instance.Show(result.Message);
            }
        }

        public void OnGetAuctionInfoResult(GetAuctionInfoResult result)
        {
            if (result.Success)
            {
                InfoPanel.Show();
                InfoPanel.Init(result.AuctionInfo);
            }
            else
            {
                NotifyGroup.Instance.Show(result.Message);
            }
        }

        public void HighestBidChanged(HighestBidChanged highestBid)
        {
            var playerId = highestBid.PlayerId;
            Player oldPlayerHasBid = null;
            Player newPlayerHasBid = null;
            Player leadingPlayer = null;
            foreach (var playerList in PlayerLists)
            {
                if (playerList.TryGetByStatus(PlayerAuctionStatus.HasBid,out var player))
                {
                    oldPlayerHasBid = player;
                    break;
                }
            }
            foreach (var playerList in PlayerLists)
            {
                if (playerList.TryGetByStatus(PlayerAuctionStatus.Leading,out var player))
                {
                    newPlayerHasBid = player;
                    break;
                }
            }
            foreach (var playerList in PlayerLists)
            {
                if (playerList.TryGetPlayer(playerId, out var player))
                {
                    leadingPlayer = player;
                    break;
                }
            }
            if (newPlayerHasBid != null)
            {
                if (newPlayerHasBid != leadingPlayer)
                {
                    oldPlayerHasBid?.SetStatus(PlayerAuctionStatus.Guest);
                    newPlayerHasBid.SetStatus(PlayerAuctionStatus.HasBid);
                    newPlayerHasBid.SetAmount(0);
                }
            }
            if (leadingPlayer != null)
            {
                leadingPlayer.SetStatus(PlayerAuctionStatus.Leading);
                leadingPlayer.SetAmount(highestBid.Amount);
                CurrentAuctionItem.SetCurrentPlayerBid(leadingPlayer.Model.Name);
            }
            CurrentAuctionItem.SetCurrentPrice(highestBid.Amount);
            CurrentAuctionItem.SetMinPrice(highestBid.MinAmount);
            CurrentAuctionItem.SetMaxPrice(highestBid.MaxAmount);
        }
        
        public void RefundAuctionBid(RefundAuctionBid refundBid)
        {
            Me.Amount = 0;
            UpdateMyBidAmount();
        }

        public void AuctionReward(AuctionReward reward)
        {
            ResultPanel.SetData(reward);
            ResultPanel.Show();
        }

        public void HostChat(string msg)
        {
            Host.Chat(msg, 5);
        }


        private void OnDestroy()
        {
            Instance = null;
            infoButton.onClick.RemoveAllListeners();
            selectBidAmountButton.onClick.RemoveAllListeners();
        }


        private void ShowInfo()
        {
            auctionState.GetInfo();
        }

        public void InitPlayers(PlayerAuctionModel[] players)
        {
            var splitPlayers = Utils.SplitArray(players, PlayerLists.Length);
            for (int i = 0; i < splitPlayers.Count; i++)
            {
                var models = splitPlayers[i];
                var playerList = PlayerLists[i];
                playerList.Init(models);
            }
        }

        public void ReplacePlayer(ReplaceAuctionPlayer replacePlayer)
        {
            var playerId = replacePlayer.PlayerId;
            var player = replacePlayer.Player;
            foreach (var playerList in PlayerLists)
            {
                if (playerList.TryReplacePlayer(playerId,player))
                {
                    return;
                }
            }
        }

    }
}