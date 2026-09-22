using System;
using System.Linq;
using AllMaps;
using Base;
using LuyenDan;
using LongDH.Script.ObjectPool;
using LongDHModules;
using Network;
using Network.Core;
using Network.Models;
using UnityEngine.UI;
using UnityEngine;

namespace Boss
{
    public class BossInfoPopup : Popup
    {
        [SerializeField] private Text titleText;
        [SerializeField] private Text levelText;
        [SerializeField] private Text hpText;
        [SerializeField] private Text playerCountText;
        [SerializeField] private Text attemptsText;
        [SerializeField] private Text messageText;
        [SerializeField] private UnityEngine.UI.Image hpFill;
        [SerializeField] private UnityEngine.UI.Button joinButton;
        [SerializeField] private Text joinLabel;
        [SerializeField] private CharacterVisualControl bossVisual;
        private static BossInfoPopup instance;
        private BossStatus status;
        private bool joining;
        private float nextRefresh;
        private Text damageText;
        private Text scheduleText;
        private Text[] chanceTexts;
        private Text firstJoinRewardText;
        private UnityEngine.UI.Image firstJoinRewardIcon;
        private BossHandler Handler => Client.Instance.CoreModule.GetHandler<BossHandler>();

        public static void Open(BossStatus status)
        {
            if (instance == null)
            {
                var prefab = Resources.Load<GameObject>("BossInfoPopup");
                if (prefab == null)
                {
                    Debug.LogError("Missing Resources/BossInfoPopup.prefab");
                    return;
                }
                instance = prefab.Spawn(Pool.Get<UIManager>().transform).GetComponent<BossInfoPopup>();
            }
            instance.OpenStatus(status);
        }

        private void OpenStatus(BossStatus value)
        {
            EnsureRewardUi();
            status = value;
            joining = false;
            nextRefresh = Time.unscaledTime + 2f;
            transform.SetAsLastSibling();
            Show();
            btnClose.interactable = true;
            SetEventButton(joinButton, Join);
            var stats = CharacterStatsSecureBuilder.FromCharacterSnapshot(value.Boss);
            bossVisual.Init(value.SkinId, stats);
            RefreshContent(value);
        }

        private void Update()
        {
            if (joining || status == null || Time.unscaledTime < nextRefresh) return;
            nextRefresh = Time.unscaledTime + 2f;
            Handler.GetStatus(status.NodeId, value =>
            {
                if (this == null || !gameObject.activeInHierarchy || joining) return;
                if (value.SpawnId != status.SpawnId)
                {
                    joinButton.interactable = false;
                    SetFooter("Phiên boss đã kết thúc.");
                    return;
                }
                RefreshContent(value);
            });
        }

        private void RefreshContent(BossStatus value)
        {
            status = value;
            titleText.text = value.Boss.Name;
            levelText.text = GameManager.levelWithText.TryGetValue(value.Boss.Level, out var level)
                ? level : "Cấp " + value.Boss.Level;
            hpText.text = $"{value.Boss.Hp:N0} / {value.Boss.SinhMenh:N0}";
            hpFill.fillAmount = value.Boss.SinhMenh > 0 ? (float)value.Boss.Hp / value.Boss.SinhMenh : 0;
            playerCountText.text = $"Đang chiến đấu: {value.FightingCount} người";
            attemptsText.text = $"Lượt còn lại: {value.RemainingAttempts}/3";
            damageText.text = $"Sát thương của bạn: {value.DamageDone:N0} / {value.Boss.SinhMenh:N0} HP";
            chanceTexts[0].text = $"Tam phẩm  {value.TamPhamChance:0.##}%";
            chanceTexts[1].text = $"Tứ phẩm  {value.TuPhamChance:0.##}%";
            chanceTexts[2].text = $"Ngũ phẩm  {value.NguPhamChance:0.##}%";
            chanceTexts[3].text = $"Lục phẩm  {value.LucPhamChance:0.##}%";
            scheduleText.text = value.ScheduleText ?? "";
            joinButton.interactable = value.IsAlive && value.RemainingAttempts > 0;
            SetJoinLabel();
            SetFooter(!value.IsAlive ? "Boss đã bị tiêu diệt hoặc hết giờ."
                : value.RemainingAttempts <= 0 ? "Bạn đã dùng hết lượt đánh boss này." : "");
        }

        private void EnsureRewardUi()
        {
            if (chanceTexts != null) return;
            var window = titleText.transform.parent;
            window.GetComponent<RectTransform>().sizeDelta = new Vector2(1240, 980);
            Move(window.Find("Heading"), 0, 390);
            Move(bossVisual.transform, -330, 170);
            Move(titleText.transform, 220, 300);
            titleText.fontSize = 42;
            Move(levelText.transform, 220, 245);
            levelText.fontSize = 32;
            Move(hpFill.transform.parent, 205, 185);
            Move(hpText.transform, 205, 185);
            hpText.fontSize = 26;
            Move(playerCountText.transform, 220, 120);
            playerCountText.fontSize = 31;
            Move(attemptsText.transform, 220, 70);
            attemptsText.fontSize = 31;
            Move(messageText.transform, 0, -386);
            messageText.rectTransform.sizeDelta = new Vector2(950, 42);
            messageText.fontSize = 32;
            var heading = window.Find("Heading")?.GetComponent<Text>();
            if (heading != null) heading.fontSize = 40;
            Move(btnClose.transform, -200, -450);
            Move(joinButton.transform, 200, -450);
            joinButton.GetComponent<RectTransform>().sizeDelta = new Vector2(360, 70);
            joinLabel.rectTransform.sizeDelta = new Vector2(340, 64);
            joinLabel.fontSize = 36;
            var closeLabel = btnClose.GetComponentInChildren<Text>(true);
            if (closeLabel != null) closeLabel.fontSize = 34;

            AddIcon(window, "ParticipantMoneyIcon", MoneyResources.Instance != null ? MoneyResources.Instance.MoneyVipIcon : null, -495, -5, 38);
            AddText(window, "RewardHeading", "Thưởng chung khi boss bị tiêu diệt: 1.000 Tiên Ngọc + 1 đan lô", 65, -5, 1040, 50, 36, TextAnchor.MiddleLeft);
            AddText(window, "RewardRule", "Mỗi 0,1% HP boss gây ra: +0,1% cho Tứ, Ngũ, Lục; Tam phẩm giảm tương ứng.", 0, -54, 1120, 48, 34);
            damageText = AddText(window, "PersonalDamage", "", 0, -104, 900, 48, 34);

            var tiers = new[] { Tier.TamPham, Tier.TuPham, Tier.NguPham, Tier.LucPham };
            var resources = LuyenDanResources.Instance;
            chanceTexts = new Text[tiers.Length];
            for (var i = 0; i < tiers.Length; i++)
            {
                var x = -360 + i * 240;
                var furnace = resources != null ? resources.TempFurnaceDatas.FirstOrDefault(item => item.Tier == tiers[i]) : null;
                AddIcon(window, "FurnaceIcon" + i, furnace != null ? furnace.Icon : null, x, -167, 72);
                chanceTexts[i] = AddText(window, "FurnaceChance" + i, "", x, -225, 220, 50, 36);
            }

            var lucPham = resources != null ? resources.TempFurnaceDatas.FirstOrDefault(item => item.Tier == Tier.LucPham) : null;
            AddText(window, "KillerHeading", "Kết liễu:", -325, -278, 150, 50, 36, TextAnchor.MiddleLeft);
            AddIcon(window, "KillerFurnaceIcon", lucPham != null ? lucPham.Icon : null, -225, -278, 50);
            AddText(window, "KillerFurnaceReward", "Lò Lục phẩm", -105, -278, 180, 50, 36, TextAnchor.MiddleLeft);
            AddIcon(window, "KillerMoneyIcon", MoneyResources.Instance != null ? MoneyResources.Instance.MoneyVipIcon : null, 25, -278, 50);
            AddText(window, "KillerMoneyReward", "+ 5.000 Tiên Ngọc", 200, -278, 280, 50, 36, TextAnchor.MiddleLeft);
            scheduleText = AddText(window, "BossSchedule", "", 0, -327, 1100, 48, 34);
            firstJoinRewardIcon = AddIcon(window, "FirstJoinRewardIcon", MoneyResources.Instance != null ? MoneyResources.Instance.MoneyVipIcon : null, -70, -385, 52);
            firstJoinRewardText = AddText(window, "FirstJoinReward", "Lần đầu mỗi ngày: +100 Tiên Ngọc", 220, -385, 520, 56, 40, TextAnchor.MiddleLeft);
            firstJoinRewardText.fontStyle = FontStyle.Bold;
            firstJoinRewardText.color = new Color32(255, 230, 130, 255);
            var rewardOutline = firstJoinRewardText.gameObject.AddComponent<Outline>();
            rewardOutline.effectColor = new Color32(63, 35, 16, 255);
            rewardOutline.effectDistance = new Vector2(1, -1);
        }

        private void SetFooter(string error)
        {
            messageText.text = error;
            var showReward = string.IsNullOrEmpty(error);
            firstJoinRewardText.gameObject.SetActive(showReward);
            if (firstJoinRewardIcon != null) firstJoinRewardIcon.gameObject.SetActive(showReward);
        }

        private void SetJoinLabel()
        {
            joinLabel.text = "Tham gia";
        }

        private static void Move(Transform target, float x, float y)
        {
            if (target != null) target.GetComponent<RectTransform>().anchoredPosition = new Vector2(x, y);
        }

        private Text AddText(Transform parent, string name, string value, float x, float y, float width, float height, int size,
            TextAnchor alignment = TextAnchor.MiddleCenter)
        {
            var rect = new GameObject(name, typeof(RectTransform)).GetComponent<RectTransform>();
            rect.SetParent(parent, false);
            rect.anchoredPosition = new Vector2(x, y);
            rect.sizeDelta = new Vector2(width, height);
            var label = rect.gameObject.AddComponent<Text>();
            label.font = titleText.font;
            label.fontSize = size;
            label.color = titleText.color;
            label.alignment = alignment;
            label.raycastTarget = false;
            label.text = value;
            return label;
        }

        private static UnityEngine.UI.Image AddIcon(Transform parent, string name, Sprite sprite, float x, float y, float size)
        {
            if (sprite == null) return null;
            var rect = new GameObject(name, typeof(RectTransform)).GetComponent<RectTransform>();
            rect.SetParent(parent, false);
            rect.anchoredPosition = new Vector2(x, y);
            rect.sizeDelta = new Vector2(size, size);
            var icon = rect.gameObject.AddComponent<UnityEngine.UI.Image>();
            icon.sprite = sprite;
            icon.preserveAspect = true;
            icon.raycastTarget = false;
            return icon;
        }

        private void Join()
        {
            if (joining || status == null || !joinButton.interactable) return;
            joining = true;
            joinButton.interactable = false;
            btnClose.interactable = false;
            joinLabel.text = "Đang tham gia…";
            Handler.Join(status, result =>
            {
                if (this == null) return;
                joining = false;
                btnClose.interactable = true;
                if (!result.Success)
                {
                    SetJoinLabel();
                    SetFooter(result.Message);
                    nextRefresh = 0;
                    return;
                }
                Hide();
                Pool.Get<GameManager>().GoToBossCombat(result);
            });
        }
    }
}
