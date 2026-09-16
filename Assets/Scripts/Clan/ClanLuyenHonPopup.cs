using System;
using System.Linq;
using System.Threading.Tasks;
using CreateCharacter;
using LongDH.LongDHModules;
using LongDHModules;
using LuyenDan;
using Network.Core;
using Network.Models.Clans;
using UnityEngine;
using UnityEngine.UI;

namespace Clan
{
    /// <summary>
    /// Popup Luyện Hồn tông môn, tách khỏi ClanPanel. Cả hai thao tác đều cần quyền
    /// <see cref="ClanPermission.ManageHon"/>.
    ///
    /// Bám theo ClanMessages/ClanHandler hiện tại:
    /// - Luyện hồn: tiêu Hồn Tinh (SoulEssence) theo tỉ lệ ClanConfig.SoulEssencePerHon để tăng Hồn,
    ///   chặn trên bởi Clan.MaxHon. Số lượng do người chơi chọn bằng SelectNum.
    /// - Nâng giới hạn: tiêu Quỹ theo ClanConfig.UpgradeHonLimitFundCostByLevel để lên
    ///   Clan.HonLimitLevel kế tiếp, kéo theo MaxHon (MaxHonByLimitLevel) và giới hạn Hồn Tinh
    ///   (MaxSoulEssenceByLimitLevel). Handler.UpgradeMaxHon nhận CẤP giới hạn, không phải số quỹ.
    /// </summary>
    public class ClanLuyenHonPopup : ClanPopup
    {
        [Header("Thông tin")]
        [SerializeField] private Text honText;
        [SerializeField] private Text soulEssenceText;
        [SerializeField] private Text fundsText;
        [SerializeField] private Text honLimitLevelText;
        [SerializeField] private Text soulEssencePerHonText;
        [SerializeField] private Text nextHonLimitText;

        [Header("Thao tác")]
        [SerializeField] private Button luyenHonButton;
        [SerializeField] private Button upgradeHonLimitButton;

        [Tooltip("Bảng chọn số lượng Hồn muốn luyện. Nút Xác Nhận của nó chính là bước xác nhận " +
                 "cuối cùng, không hỏi thêm PopupConfirmAction nữa (giống ClanInheritancePanel).")]
        [SerializeField] private SelectNum luyenHonSelectNum;

        [Header("Gỡ lỗi")]
        [Tooltip("Ép nâng lên đúng cấp giới hạn này thay vì cấp hiện tại + 1. Để 0 = tự động.")]
        [SerializeField] private int overrideNextHonLimitLevel;

        private ClanPanel panel;
        private bool isProcessing;

        private Network.Models.Clans.Clan Clan => Handler?.CurrentClan;

        protected override void Start()
        {
            base.Start();
            SetEventButton(luyenHonButton, OpenLuyenHonSelection);
            SetEventButton(upgradeHonLimitButton, ConfirmUpgradeHonLimit);
            if (luyenHonSelectNum != null) luyenHonSelectNum.confirmAction.AddListener(ConfirmLuyenHon);
            Refresh();
        }

        /// <summary>Mở popup từ ClanPanel (panel dùng để kiểm tra quyền của người chơi).</summary>
        public void Open(ClanPanel owner)
        {
            panel = owner;
            Show();
        }

        protected override void BindClanEvents(ClanHandler handler)
        {
            handler.HonChanged += OnHonChanged;
            handler.FundsChanged += OnFundsChanged;
        }

        protected override void UnbindClanEvents(ClanHandler handler)
        {
            handler.HonChanged -= OnHonChanged;
            handler.FundsChanged -= OnFundsChanged;
        }

        protected override void OnShown()
        {
            Refresh();
        }

        private void OnHonChanged(ClanHonChanged changed)
        {
            if (changed != null && changed.ClanId == Clan?.Id) Refresh();
        }

        private void OnFundsChanged(ClanFundsChanged changed)
        {
            if (changed != null && changed.ClanId == Clan?.Id) Refresh();
        }

        // ============================================================
        // Hiển thị
        // ============================================================

        public void Refresh()
        {
            var clan = Clan;

            SetText(honText, clan != null ? $"Hồn: {clan.Hon:n0}/{clan.MaxHon:n0}" : "—");
            SetText(soulEssenceText,
                clan != null ? $"Hồn Tinh: {clan.SoulEssence:n0}/{GetMaxSoulEssence(clan):n0}" : "—");
            SetText(fundsText, clan != null ? $"Quỹ: {clan.Funds:n0}" : "—");
            SetText(honLimitLevelText, clan != null ? $"Giới hạn cấp {GetHonLimitLevel(clan):n0}" : "—");
            SetText(soulEssencePerHonText, $"1 Hồn = {GetSoulEssencePerHon():n0} Hồn Tinh");

            if (nextHonLimitText != null)
            {
                if (clan != null && TryGetNextHonLimitInfo(clan, out var nextLevel, out var cost, out var nextMaxHon,
                        out var nextMaxSoulEssence))
                {
                    nextHonLimitText.text =
                        $"Mở cấp {nextLevel:n0}: cần {cost:n0} Quỹ\n" +
                        $"Giới hạn Hồn {nextMaxHon:n0} · Hồn Tinh {nextMaxSoulEssence:n0}";
                }
                else
                {
                    nextHonLimitText.text = clan != null ? "Giới hạn hồn đã đạt cấp tối đa" : "—";
                }
            }

            UpdateButtons();
        }

        private void UpdateButtons()
        {
            var clan = Clan;
            var canManage = CanManage();

            var canLuyenHon = canManage && !isProcessing && GetMaxLuyenHonAmount(clan) > 0;
            var canUpgrade = canManage && !isProcessing && clan != null &&
                             TryGetNextHonLimitInfo(clan, out _, out var cost, out _, out _) &&
                             clan.Funds >= cost;

            if (luyenHonButton != null)
            {
                luyenHonButton.gameObject.SetActive(canManage);
                luyenHonButton.interactable = canLuyenHon;
            }

            if (upgradeHonLimitButton != null)
            {
                upgradeHonLimitButton.gameObject.SetActive(canManage);
                upgradeHonLimitButton.interactable = canUpgrade;
            }
        }

        /// <summary>
        /// Có quyền quản lý hồn hay không. Khi đang xem tông môn của người khác thì
        /// ClanPanel.CurrentMember là null nên HasPermission trả false, không cần kiểm tra riêng.
        /// </summary>
        private bool CanManage()
        {
            if (panel == null)
            {
                panel = FindObjectsByType<ClanPanel>(FindObjectsInactive.Include, FindObjectsSortMode.None)
                    .FirstOrDefault();
            }

            return panel != null && panel.HasPermission(ClanPermission.ManageHon);
        }

        // ============================================================
        // Luyện hồn (chọn số lượng bằng SelectNum)
        // ============================================================

        /// <summary>Số Hồn tối đa có thể luyện lúc này: chặn bởi cả MaxHon lẫn số Hồn Tinh đang có.</summary>
        private int GetMaxLuyenHonAmount(Network.Models.Clans.Clan clan)
        {
            if (clan == null) return 0;

            var remaining = Math.Max(0, clan.MaxHon - clan.Hon);
            var affordable = Math.Max(0, clan.SoulEssence / GetSoulEssencePerHon());
            return (int)Math.Min(int.MaxValue, Math.Min(remaining, affordable));
        }

        private void OpenLuyenHonSelection()
        {
            var clan = Clan;
            if (isProcessing || Handler == null || clan == null || luyenHonSelectNum == null) return;
            if (!CanManage())
            {
                NotifyGroup.Instance?.Show("Bạn không có quyền luyện hồn tông môn");
                return;
            }

            if (clan.Hon >= clan.MaxHon)
            {
                NotifyGroup.Instance?.Show("Hồn tông môn đã đạt giới hạn");
                return;
            }

            var max = GetMaxLuyenHonAmount(clan);
            if (max <= 0)
            {
                NotifyGroup.Instance?.Show($"Hồn Tinh không đủ, cần {GetSoulEssencePerHon():n0} Hồn Tinh cho 1 Hồn");
                return;
            }

            luyenHonSelectNum.ValueChanged -= OnLuyenHonAmountChanged;
            luyenHonSelectNum.ValueChanged += OnLuyenHonAmountChanged;
            luyenHonSelectNum.SetMin(1);
            luyenHonSelectNum.SetMax(max);
            luyenHonSelectNum.SetText(GetLuyenHonPreviewText(1));
            luyenHonSelectNum.Show(0.15f);
            luyenHonSelectNum.SetValue(1);
        }

        private void OnLuyenHonAmountChanged(int value)
        {
            luyenHonSelectNum?.SetText(GetLuyenHonPreviewText(Math.Max(1, value)));
        }

        private string GetLuyenHonPreviewText(long amount)
        {
            return $"Luyện {amount:n0} Hồn, tiêu {amount * GetSoulEssencePerHon():n0} Hồn Tinh";
        }

        /// <summary>Nút Xác Nhận của SelectNum. SelectNum tự ẩn nên không cần hỏi lại lần nữa.</summary>
        private void ConfirmLuyenHon()
        {
            if (isProcessing || Handler == null || luyenHonSelectNum == null) return;

            var amount = luyenHonSelectNum.GetValue();
            if (amount <= 0) return;

            _ = LuyenHon(amount);
        }

        private async Task LuyenHon(long amount)
        {
            if (isProcessing || Handler == null) return;
            SetProcessing(true);
            try
            {
                var result = await Handler.LuyenHon(amount);
                if (!result.Success)
                {
                    NotifyGroup.Instance?.Show(string.IsNullOrWhiteSpace(result.Message)
                        ? "Không thể luyện hồn"
                        : result.Message);
                }
                else
                {
                    NotifyGroup.Instance?.Show(
                        $"Luyện hồn thành công: +{result.AddedHon:n0} Hồn, tiêu {result.SoulEssenceSpent:n0} Hồn Tinh");
                    Refresh();
                    panel?.RefreshClanSummary();
                }
            }
            catch (Exception exception)
            {
                Debug.LogException(exception, this);
                NotifyGroup.Instance?.Show("Không thể luyện hồn, vui lòng thử lại");
            }
            finally
            {
                SetProcessing(false);
            }
        }

        // ============================================================
        // Nâng giới hạn hồn
        // ============================================================

        private void ConfirmUpgradeHonLimit()
        {
            var clan = Clan;
            if (isProcessing || Handler == null || clan == null) return;
            if (!CanManage())
            {
                NotifyGroup.Instance?.Show("Bạn không có quyền nâng giới hạn hồn tông môn");
                return;
            }

            if (!TryGetNextHonLimitInfo(clan, out var nextLevel, out var cost, out var nextMaxHon,
                    out var nextMaxSoulEssence))
            {
                NotifyGroup.Instance?.Show("Giới hạn hồn đã đạt cấp tối đa");
                return;
            }

            if (clan.Funds < cost)
            {
                NotifyGroup.Instance?.Show($"Quỹ tông môn không đủ, cần {cost:n0}");
                return;
            }

            Pool.Get<PopupConfirmAction>()?.ShowPopConfirmAction(
                $"Tiêu {cost:n0} Quỹ tông môn để mở giới hạn hồn cấp {nextLevel:n0}.\n" +
                $"Giới hạn Hồn: {nextMaxHon:n0}\nGiới hạn Hồn Tinh: {nextMaxSoulEssence:n0}",
                () => _ = UpgradeHonLimit(nextLevel));
        }

        private async Task UpgradeHonLimit(int limitLevel)
        {
            if (isProcessing || Handler == null) return;
            SetProcessing(true);
            try
            {
                var result = await Handler.UpgradeMaxHon(limitLevel);
                if (!result.Success)
                {
                    NotifyGroup.Instance?.Show(string.IsNullOrWhiteSpace(result.Message)
                        ? "Không thể nâng giới hạn hồn"
                        : result.Message);
                }
                else
                {
                    NotifyGroup.Instance?.Show(
                        $"Nâng giới hạn hồn thành công: +{result.AddedMaxHon:n0} giới hạn Hồn, tiêu {result.FundsSpent:n0} Quỹ");
                    Refresh();
                    panel?.RefreshClanSummary();
                }
            }
            catch (Exception exception)
            {
                Debug.LogException(exception, this);
                NotifyGroup.Instance?.Show("Không thể nâng giới hạn hồn, vui lòng thử lại");
            }
            finally
            {
                SetProcessing(false);
            }
        }

        private void SetProcessing(bool value)
        {
            isProcessing = value;
            UpdateButtons();
        }

        // ============================================================
        // Đọc config
        // ============================================================

        private int GetHonLimitLevel(Network.Models.Clans.Clan clan)
        {
            return Math.Max(1, clan?.HonLimitLevel ?? Handler?.CurrentConfig?.CreateClanHonLimitLevel ?? 1);
        }

        private long GetSoulEssencePerHon()
        {
            return Math.Max(1, Handler?.CurrentConfig?.SoulEssencePerHon ?? 1);
        }

        private long GetMaxSoulEssence(Network.Models.Clans.Clan clan)
        {
            var config = Handler?.CurrentConfig;
            var index = GetHonLimitLevel(clan) - 1;
            if (config?.MaxSoulEssenceByLimitLevel != null &&
                index >= 0 &&
                index < config.MaxSoulEssenceByLimitLevel.Length)
            {
                return Math.Max(0, config.MaxSoulEssenceByLimitLevel[index]);
            }

            return Math.Max(0, clan?.SoulEssence ?? 0);
        }

        private bool TryGetNextHonLimitInfo(
            Network.Models.Clans.Clan clan,
            out int nextLevel,
            out long cost,
            out long maxHon,
            out long maxSoulEssence)
        {
            nextLevel = overrideNextHonLimitLevel > 0 ? overrideNextHonLimitLevel : GetHonLimitLevel(clan) + 1;
            cost = 0;
            maxHon = 0;
            maxSoulEssence = 0;

            var config = Handler?.CurrentConfig;
            var index = nextLevel - 1;
            if (config?.MaxHonByLimitLevel == null ||
                config.MaxSoulEssenceByLimitLevel == null ||
                config.UpgradeHonLimitFundCostByLevel == null ||
                index <= 0 ||
                index >= config.MaxHonByLimitLevel.Length ||
                index >= config.MaxSoulEssenceByLimitLevel.Length ||
                index >= config.UpgradeHonLimitFundCostByLevel.Length)
            {
                return false;
            }

            if (nextLevel != GetHonLimitLevel(clan) + 1)
            {
                return false;
            }

            cost = Math.Max(0, config.UpgradeHonLimitFundCostByLevel[index]);
            maxHon = Math.Max(0, config.MaxHonByLimitLevel[index]);
            maxSoulEssence = Math.Max(0, config.MaxSoulEssenceByLimitLevel[index]);
            return maxHon > clan.MaxHon;
        }

        private static void SetText(Text target, string value)
        {
            if (target != null) target.text = value ?? string.Empty;
        }

        protected override void OnDestroy()
        {
            if (luyenHonSelectNum != null)
            {
                luyenHonSelectNum.ValueChanged -= OnLuyenHonAmountChanged;
                luyenHonSelectNum.confirmAction.RemoveListener(ConfirmLuyenHon);
            }

            base.OnDestroy();
        }
    }
}
