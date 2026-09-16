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
    /// - Luyện hồn: tiêu Hồn Tinh (SoulEssence) để tăng Hồn, chặn trên bởi Clan.MaxHon. Giá theo BẬC
    ///   THANG dựa trên điểm Hồn đang luyện (xem GetHonPointCost), nên một lượt đi qua nhiều mốc
    ///   phải cộng dồn từng điểm chứ không nhân đơn giá chung. Số lượng do người chơi chọn bằng SelectNum.
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
        [Tooltip("Hiển thị đơn giá Hồn Tinh của mốc hiện tại (giá tăng dần theo mốc), " +
                 "thay cho tỉ lệ quy đổi cố định cũ.")]
        [SerializeField] private Text honCostRateText;
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
        private long requestedHon;

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
            SetText(honCostRateText, GetHonCostRateText(clan));

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
            if (remaining <= 0) return 0;

            var affordable = GetAffordableHonAmount(clan.Hon, clan.SoulEssence, remaining);
            return (int)Math.Min(int.MaxValue, affordable);
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
                NotifyGroup.Instance?.Show(
                    $"Hồn Tinh không đủ, cần {GetHonPointCost(clan.Hon + 1):n0} Hồn Tinh cho điểm Hồn tiếp theo");
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
            var fromHon = Clan?.Hon ?? 0;
            var cost = GetLuyenHonCost(fromHon, amount);
            return $"Luyện {amount:n0} Hồn (điểm {fromHon + 1:n0}-{fromHon + amount:n0}), tiêu {cost:n0} Hồn Tinh";
        }

        /// <summary>Nút Xác Nhận của SelectNum. SelectNum tự ẩn nên không cần hỏi lại lần nữa.</summary>
        private void ConfirmLuyenHon()
        {
            if (isProcessing || Handler == null || luyenHonSelectNum == null) return;

            var amount = luyenHonSelectNum.GetValue();
            if (amount <= 0) return;

            requestedHon = amount;
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
                    // Server mua tối đa trong khả năng rồi dừng, nên AddedHon có thể nhỏ hơn số đã yêu cầu.
                    var message = result.AddedHon < requestedHon
                        ? $"Chỉ luyện được {result.AddedHon:n0}/{requestedHon:n0} Hồn (không đủ Hồn Tinh), tiêu {result.SoulEssenceSpent:n0} Hồn Tinh"
                        : $"Luyện hồn thành công: +{result.AddedHon:n0} Hồn, tiêu {result.SoulEssenceSpent:n0} Hồn Tinh";
                    NotifyGroup.Instance?.Show(message);
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

        // ============================================================
        // Chi phí Hồn Tinh theo bậc thang (mô phỏng đúng công thức server)
        //
        //     tier    = (N - 1) / SoulEssenceHonCostTierSize
        //     cost(N) = SoulEssenceHonCostBase + tier * SoulEssenceHonCostGrowthPerTier
        //
        // với N là điểm Hồn thứ mấy đang được luyện (N = Hồn hiện tại + 1).
        // ============================================================

        private long GetHonCostBase()
        {
            return Math.Max(0, Handler?.CurrentConfig?.SoulEssenceHonCostBase ?? 0);
        }

        private long GetHonCostTierSize()
        {
            return Math.Max(1, Handler?.CurrentConfig?.SoulEssenceHonCostTierSize ?? 1);
        }

        private long GetHonCostGrowthPerTier()
        {
            return Math.Max(0, Handler?.CurrentConfig?.SoulEssenceHonCostGrowthPerTier ?? 0);
        }

        /// <summary>Chi phí Hồn Tinh của riêng điểm Hồn thứ <paramref name="honPoint"/> (đếm từ 1).</summary>
        private long GetHonPointCost(long honPoint)
        {
            if (honPoint < 1) honPoint = 1;
            var tier = (honPoint - 1) / GetHonCostTierSize();
            return GetHonCostBase() + tier * GetHonCostGrowthPerTier();
        }

        /// <summary>
        /// Tổng chi phí để luyện <paramref name="amount"/> điểm Hồn kế tiếp khi đang có
        /// <paramref name="fromHon"/> điểm. Cộng dồn theo từng điểm, chỉ gom các điểm CÙNG một mốc
        /// lại để tính một lần cho nhanh - kết quả giống hệt vòng lặp từng điểm.
        /// </summary>
        private long GetLuyenHonCost(long fromHon, long amount)
        {
            if (amount <= 0) return 0;
            if (fromHon < 0) fromHon = 0;

            var tierSize = GetHonCostTierSize();
            long total = 0;
            var current = fromHon;
            var left = amount;

            while (left > 0)
            {
                var costPerPoint = GetHonPointCost(current + 1);
                var pointsLeftInTier = tierSize - current % tierSize;
                var take = Math.Min(left, pointsLeftInTier);

                total += take * costPerPoint;
                current += take;
                left -= take;
            }

            return total;
        }

        /// <summary>
        /// Số điểm Hồn nhiều nhất có thể luyện với <paramref name="budget"/> Hồn Tinh,
        /// không vượt quá <paramref name="cap"/> (phần còn trống tới MaxHon).
        /// </summary>
        private long GetAffordableHonAmount(long fromHon, long budget, long cap)
        {
            if (cap <= 0 || budget <= 0) return 0;
            if (fromHon < 0) fromHon = 0;

            var tierSize = GetHonCostTierSize();
            long count = 0;
            var current = fromHon;
            var remaining = budget;

            while (count < cap)
            {
                var costPerPoint = GetHonPointCost(current + 1);
                if (costPerPoint <= 0)
                {
                    // Cấu hình giá 0 -> mua được hết phần còn trống, thoát luôn kẻo lặp vô hạn.
                    count = cap;
                    break;
                }

                var affordableHere = remaining / costPerPoint;
                if (affordableHere <= 0) break;

                var pointsLeftInTier = tierSize - current % tierSize;
                var take = Math.Min(Math.Min(cap - count, pointsLeftInTier), affordableHere);
                if (take <= 0) break;

                remaining -= take * costPerPoint;
                count += take;
                current += take;
            }

            return count;
        }

        /// <summary>Dòng mô tả đơn giá: giá mốc hiện tại + mốc kế tiếp giá bao nhiêu.</summary>
        private string GetHonCostRateText(Network.Models.Clans.Clan clan)
        {
            var hon = clan?.Hon ?? 0;
            var tierSize = GetHonCostTierSize();
            var costNow = GetHonPointCost(hon + 1);
            var tierEnd = (hon / tierSize + 1) * tierSize;
            var growth = GetHonCostGrowthPerTier();

            var text = $"Mốc hiện tại (điểm {hon / tierSize * tierSize + 1:n0}-{tierEnd:n0}): {costNow:n0} Hồn Tinh/điểm";
            if (growth > 0)
            {
                text += $"\nTừ điểm {tierEnd + 1:n0}: {costNow + growth:n0} Hồn Tinh/điểm";
            }

            return text;
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
