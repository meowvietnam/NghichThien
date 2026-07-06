using System;
using System.Collections.Generic;
using System.Linq;
using Base;
using CreateCharacter;
using DG.Tweening;
using LongDH.LongDHModules;
using LongDHModules;
using Network;
using Network.Callback;
using Network.Models;
using UnityEngine;
using UnityEngine.UI;

namespace AllMaps.DauTruong
{
    public class ArenaManager : BaseBehaviour,
        INetworkEvent.ILoadCharCombatArena, ILoadRank,
        IClaimRankReward
    {
        [SerializeField] private Image iconRank;
        [SerializeField] private Image iconRankName;
        [SerializeField] private Button btnStart;
        [SerializeField] public Sprite[] spriteRefRank;
        [SerializeField] public Sprite[] spriteRefRankName;
        [SerializeField] private GameObject parent;
        [SerializeField] private Transform group;
        [SerializeField] private RewardRankDauTruong rewardRankDauTruongPrefab;
        [SerializeField] private Text resetAfterText;

        [SerializeField] private GameObject adsObject;
        [SerializeField] private GameObject costObject;
        [SerializeField] private Text costText;
        private MatchMakingState state;
        private int cost;
        private List<RewardRankDauTruong> rewardRankDauTruongs = new();

        private int pointUser =>
            Client.Instance.CharacterData.ArenaManager.Points; // sau này chuyển cái này thành của player

        public ETypeArenaRank CurrentTypeRank => GetArenaRankWithPoint(pointUser);
        public static ArenaManager Instance;
        [SerializeField] private PrepareElementArenaCombat userUI;
        [SerializeField] private PrepareElementArenaCombat enemyUI;
        private Transform parentPrepareCombat => userUI.transform.parent;
        [SerializeField] private Button btnBackScene;
        [SerializeField] private Sprite spriteCombat;
        [SerializeField] private Image progressRank;
        [SerializeField] private Image progressBar;
        [SerializeField] private Text txtPoint;
        private TimeSpan endTime = TimeSpan.Zero;

        private void SetProgress()
        {
            if (CurrentTypeRank == ETypeArenaRank.ThanhGiai)
            {
                progressRank.gameObject.SetActive(false);
                txtPoint.text = pointUser.ToString();
                progressBar.fillAmount = 1f;
            }
            else
            {
                ETypeArenaRank nextRank = (ETypeArenaRank)((int)CurrentTypeRank + 1);
                int valueNextRank = TypeRankArenaWithValue[nextRank];
                progressRank.gameObject.SetActive(true);
                progressRank.sprite = spriteRefRank[(int)nextRank - 1];
                txtPoint.text = $"{pointUser}/{valueNextRank}";
                progressBar.fillAmount = (float)(pointUser / valueNextRank);
            }
        }

        private void Awake()
        {
            Instance = this;
        }

        private void Start()
        {
            parent.SetActive(false);
            Observer.Notify(EventList.Network.LOAD_RANK, this);
        }

        public static ETypeArenaRank GetArenaRankWithPoint(int pointUser)
        {
            foreach (var rank in TypeRankArenaWithValue)
            {
                if (pointUser >= rank.Value)
                {
                    return rank.Key;
                }
            }

            return ETypeArenaRank.None;
        }

        public static Dictionary<ETypeArenaRank, int> TypeRankArenaWithValue = new();
        public static Dictionary<ETypeArenaRank, int> TypeRankArenaWithReward = new();

        public void SuccessLoadCharCombatArena(CharacterSnapshot user, CharacterSnapshot enemy)
        {
            var gameManager = Pool.Get<GameManager>();
            gameManager.GoToPrepareCombatArena(user, enemy);
            userUI.InitVisual(gameManager.user.charStats);
            enemyUI.InitVisual(gameManager.enemyCombat.charStats);
            parentPrepareCombat.gameObject.SetActive(true);
            gameManager.SetBGCombat(spriteCombat);
            DOVirtual.DelayedCall(2f, () => { gameManager.GoToCombatArena(); });
        }

        public void FailLoadCharCombatArena(string message)
        {
            NotifyGroup.Instance.Show(message);
        }

        public void Reward(long id)
        {
            Observer.Notify(EventList.Network.CLAIM_RANK_REWARD, this, id);
        }

        private void FixedUpdate()
        {
            if (endTime > TimeSpan.Zero)
            {
                endTime -= TimeSpan.FromSeconds(Time.fixedDeltaTime);
            }

            UpdateEndTime();
        }

        private void UpdateEndTime()
        {
            var text = $"Làm mới sau: {GetTime(endTime)}";
            resetAfterText.text = text;
        }

        private string GetTime(TimeSpan timeSpan)
        {
            if (timeSpan <= TimeSpan.Zero)
            {
                return "0 giây";
            }

            if (timeSpan.Days > 0) return $"{timeSpan.Days} ngày {timeSpan.Hours} giờ";
            if (timeSpan.Hours > 0) return $"{timeSpan.Hours} giờ {timeSpan.Minutes} phút";
            if (timeSpan.Minutes > 0) return $"{timeSpan.Minutes} phút {timeSpan.Seconds} giây";
            return $"{timeSpan.Seconds} giây";
        }

        private void Ready()
        {
            if (state == MatchMakingState.NotFree)
            {
                var popup = Pool.Get<PopupConfirmAction>();
                popup.ShowPopConfirmAction($"Sử dụng {cost} tiên ngọc để tham gia đấu trường", SendReady);
            }
            else
            {
                SendReady();
            }
        }

        private void SendReady()
        {
            Observer.Notify(EventList.Network.LOAD_CHAR_COMBAT_ARENA, this);
        }

        private void Init()
        {
            SetEventButton(btnBackScene, () => { Observer.Notify(EventList.GamePlay.BACK_SCENE); });
            SetEventButton(btnStart, Ready);
            var currentRank = CurrentTypeRank;
            if (currentRank == ETypeArenaRank.None)
            {
                iconRank.gameObject.SetActive(false);
                iconRankName.gameObject.SetActive(false);
            }
            else
            {
                iconRank.gameObject.SetActive(true);
                iconRankName.gameObject.SetActive(true);
                iconRank.sprite = spriteRefRank[(int)currentRank - 1];
                iconRank.SetNativeSize();
                iconRankName.sprite = spriteRefRankName[(int)currentRank - 1];
                iconRankName.SetNativeSize();
            }

            var ui = Pool.Get<UIManager>();
            ui.panelGroup.SetActive(false);
            ui.commonMask.gameObject.SetActive(false);
            ui.ListIcon.gameObject.SetActive(false);
            InitRewards();
            SetProgress();
            parent.SetActive(true);
        }

        private void InitRewards()
        {
            foreach (var rewardRankDauTruong in rewardRankDauTruongs)
            {
                Destroy(rewardRankDauTruong.gameObject);
            }

            rewardRankDauTruongs.Clear();
            var rewards = Client.Instance.CharacterData.ArenaManager.Rewards;
            rewards = rewards
                .OrderByDescending(x => TypeRankArenaWithValue[x.Type])
                .ToList();
            foreach (var reward in rewards)
            {
                var instance = Instantiate(rewardRankDauTruongPrefab, group);
                instance.Init(reward);
                rewardRankDauTruongs.Add(instance);
            }
        }

        public void LoadRankSuccess(LoadRankResult result)
        {
            TypeRankArenaWithReward = result.TypeRankArenaWithReward;
            TypeRankArenaWithValue = result.TypeRankArenaWithValue;
            endTime = result.NextReload;
            state = result.MatchMakingState;
            cost = result.Cost;
            switch (state)
            {
                case MatchMakingState.Free:
                    adsObject.SetActive(false);
                    costObject.SetActive(false);
                    break;
                case MatchMakingState.Ads:
                    adsObject.SetActive(true);
                    costObject.SetActive(false);
                    break;
                case MatchMakingState.NotFree:
                    adsObject.SetActive(false);
                    costText.text = $"{cost}";
                    costObject.SetActive(true);
                    break;
            }

            Init();
        }

        public void LoadRankFailure(string message)
        {
            NotifyGroup.Instance.Show(message);
        }

        public void RewardSuccess(ClaimRankRewardResult result)
        {
            var icon = MoneyResources.Instance.MoneyVipIcon;
            var item = new MoneyVipItem(icon);
            var core = CoreConfig.Instance;
            var text =
                $"Bạn nhận được: <color=#{ColorUtility.ToHtmlStringRGBA(core.FindQuality(item.Tier).Color)}>{item.Name} x{result.MoneyVip}</color>";
            NotifyGroup.Instance.Show(text);
            var ui = rewardRankDauTruongs.First(x => x.Reward.Id == result.Id);
            ui.UpdateClaim();
        }

        public void FailClaimReward(string message)
        {
            NotifyGroup.Instance.Show(message);
        }
    }
}