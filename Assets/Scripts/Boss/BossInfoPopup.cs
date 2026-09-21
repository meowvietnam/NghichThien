using System;
using AllMaps;
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
            status = value;
            joining = false;
            nextRefresh = Time.unscaledTime + 2f;
            transform.SetAsLastSibling();
            Show();
            btnClose.interactable = true;
            SetEventButton(joinButton, Join);
            var stats = CharacterStatsSecureBuilder.FromCharacterSnapshot(value.Boss);
            bossVisual.InitBoss(value.SkinId, stats);
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
                    messageText.text = "Phiên boss đã kết thúc.";
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
            joinButton.interactable = value.IsAlive && value.RemainingAttempts > 0;
            joinLabel.text = "Tham gia";
            messageText.text = !value.IsAlive ? "Boss đã bị tiêu diệt hoặc hết giờ."
                : value.RemainingAttempts <= 0 ? "Bạn đã dùng hết lượt đánh boss này." : "";
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
                    joinLabel.text = "Tham gia";
                    messageText.text = result.Message;
                    nextRefresh = 0;
                    return;
                }
                Hide();
                Pool.Get<GameManager>().GoToBossCombat(result);
            });
        }
    }
}
