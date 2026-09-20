using System;
using System.Linq;
using System.Threading.Tasks;
using CreateCharacter;
using LongDH.LongDHModules;
using LongDHModules;
using LuyenDan;
using Network;
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
    /// - Luyện hồn: tiêu Tinh Hồn (SoulEssence) để tăng Hồn, chặn trên bởi Clan.MaxHon. Giá theo BẬC
    ///   THANG dựa trên điểm Hồn đang luyện (xem GetHonPointCost), nên một lượt đi qua nhiều mốc
    ///   phải cộng dồn từng điểm chứ không nhân đơn giá chung. Số lượng do người chơi chọn bằng SelectNum.
    /// - Nâng giới hạn: tiêu cống hiến cá nhân, bù phần thiếu bằng Tiên Ngọc theo bảng phí để lên
    ///   Clan.HonLimitLevel kế tiếp, kéo theo MaxHon (MaxHonByLimitLevel) và giới hạn Tinh Hồn
    ///   (MaxSoulEssenceByLimitLevel). Handler.UpgradeMaxHon nhận CẤP giới hạn, không phải số quỹ.
    /// </summary>
    public class ClanLuyenHonPopup : ClanPopup
    {
        [Header("Thông tin")]
        [SerializeField] private Text honText;
        [SerializeField] private Text soulEssenceText;
        [SerializeField] private Text fundsText;
        [SerializeField] private Text honLimitLevelText;
        [Tooltip("Hiển thị đơn giá Tinh Hồn của mốc hiện tại (giá tăng dần theo mốc), " +
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

        // ============================================================
        // Bảng màu rich text. Nhãn 【...】 dùng #ffb143 theo đúng quy ước sẵn có
        // của dự án (SkillProvider, HerbProvider, NgoDaoToolTip...).
        // ============================================================

        private const string ColorLabel = "#ffb143";  // nhãn 【...】
        private const string ColorHon = "#C084FC";    // Hồn
        private const string ColorSoul = "#22D3EE";   // Tinh Hồn
        private const string ColorFunds = "#FFD166";  // Quỹ
        private const string ColorLevel = "#5BE37D";  // cấp giới hạn
        private const string ColorGood = "#22C55E";   // thành công / đủ điều kiện
        private const string ColorWarn = "#EF4444";   // thiếu tài nguyên / lỗi
        private const string ColorMuted = "#AAAAAA";  // phần phụ, ít quan trọng

        private static string Label(string value) => $"<color={ColorLabel}>【{value}】</color>";
        private static string Tint(string value, string color) => $"<color={color}>{value}</color>";
        private static string Num(long value, string color) => $"<b><color={color}>{value:n0}</color></b>";
        private static string Muted(string value) => $"<color={ColorMuted}>{value}</color>";
        private static readonly string NoData = Muted("—");

        protected override void Start()
        {
            base.Start();
            SetEventButton(luyenHonButton, OpenLuyenHonSelection);
            SetEventButton(upgradeHonLimitButton, ConfirmUpgradeHonLimit);
            if (luyenHonSelectNum != null) luyenHonSelectNum.confirmAction.AddListener(ConfirmLuyenHon);
            EnableRichText();
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
            handler.MemberPointsChanged += OnMemberPointsChanged;
        }

        protected override void UnbindClanEvents(ClanHandler handler)
        {
            handler.HonChanged -= OnHonChanged;
            handler.MemberPointsChanged -= OnMemberPointsChanged;
        }

        protected override void OnShown()
        {
            Refresh();
        }

        private void OnHonChanged(ClanHonChanged changed)
        {
            if (changed != null && changed.ClanId == Clan?.Id) Refresh();
        }

        private void OnMemberPointsChanged(ClanMemberPointsChanged changed)
        {
            if (changed != null && changed.ClanId == Clan?.Id) Refresh();
        }

        // ============================================================
        // Hiển thị
        // ============================================================

        public void Refresh()
        {
            var clan = Clan;

            SetText(honText, clan != null
                ? $"{Label("Hồn")} {Num(clan.Hon, ColorHon)}{Muted($"/{clan.MaxHon:n0}")}" +
                  (clan.Hon >= clan.MaxHon ? $" {Tint("(đã đạt giới hạn)", ColorWarn)}" : string.Empty)
                : NoData);

            SetText(soulEssenceText, clan != null
                ? $"{Label("Tinh Hồn")} {Num(clan.SoulEssence, ColorSoul)}{Muted($"/{GetMaxSoulEssence(clan):n0}")}"
                : NoData);

            SetText(fundsText, clan != null
                ? $"{Label("Cống hiến cá nhân")} {Num(GetCurrentContribPoints(), ColorFunds)}"
                : NoData);

            SetText(honLimitLevelText, clan != null
                ? $"{Label("Giới hạn")} cấp {Num(GetHonLimitLevel(clan), ColorLevel)}"
                : NoData);

            SetText(honCostRateText, GetHonCostRateText(clan));

            if (nextHonLimitText != null)
            {
                if (clan != null && TryGetNextHonLimitInfo(clan, out var nextLevel, out var cost, out var nextMaxHon,
                        out var nextMaxSoulEssence))
                {
                    var costColor = GetCurrentContribPoints() >= cost ? ColorGood : ColorFunds;
                    nextHonLimitText.text =
                        $"Mở cấp {Num(nextLevel, ColorLevel)}: {Num(cost, costColor)} {Tint("điểm cống hiến", ColorFunds)}; thiếu điểm bù Tiên Ngọc\n" +
                        $"{Muted("Giới hạn")} {Tint("Hồn", ColorHon)} {Num(nextMaxHon, ColorHon)} " +
                        $"{Muted("·")} {Tint("Tinh Hồn", ColorSoul)} {Num(nextMaxSoulEssence, ColorSoul)}";
                }
                else
                {
                    nextHonLimitText.text = clan != null
                        ? Tint("Giới hạn hồn đã đạt cấp tối đa", ColorGood)
                        : NoData;
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
                             TryGetNextHonLimitInfo(clan, out _, out _, out _, out _);

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

        /// <summary>Số Hồn tối đa có thể luyện lúc này: chặn bởi cả MaxHon lẫn số Tinh Hồn đang có.</summary>
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
                NotifyGroup.Instance?.Show(Tint("Bạn không có quyền luyện hồn tông môn", ColorWarn));
                return;
            }

            if (clan.Hon >= clan.MaxHon)
            {
                NotifyGroup.Instance?.Show($"{Tint("Hồn", ColorHon)} tông môn đã đạt giới hạn");
                return;
            }

            var max = GetMaxLuyenHonAmount(clan);
            if (max <= 0)
            {
                NotifyGroup.Instance?.Show(
                    $"{Tint("Tinh Hồn", ColorSoul)} không đủ, cần {Num(GetHonPointCost(clan.Hon + 1), ColorWarn)} " +
                    $"{Tint("Tinh Hồn", ColorSoul)} cho điểm {Tint("Hồn", ColorHon)} tiếp theo");
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
            var enough = (Clan?.SoulEssence ?? 0) >= cost;
            return $"Luyện {Num(amount, ColorHon)} {Tint("Hồn", ColorHon)} " +
                   $"{Muted($"(điểm {fromHon + 1:n0}-{fromHon + amount:n0})")}, " +
                   $"tiêu {Num(cost, enough ? ColorSoul : ColorWarn)} {Tint("Tinh Hồn", ColorSoul)}";
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
                        ? Tint("Không thể luyện hồn", ColorWarn)
                        : result.Message);
                }
                else
                {
                    // Server mua tối đa trong khả năng rồi dừng, nên AddedHon có thể nhỏ hơn số đã yêu cầu.
                    var message = result.AddedHon < requestedHon
                        ? $"Chỉ luyện được {Num(result.AddedHon, ColorWarn)}{Muted($"/{requestedHon:n0}")} " +
                          $"{Tint("Hồn", ColorHon)} {Muted("(không đủ Tinh Hồn)")}, " +
                          $"tiêu {Num(result.SoulEssenceSpent, ColorSoul)} {Tint("Tinh Hồn", ColorSoul)}"
                        : $"Luyện hồn thành công: {Tint("+", ColorGood)}{Num(result.AddedHon, ColorGood)} " +
                          $"{Tint("Hồn", ColorHon)}, tiêu {Num(result.SoulEssenceSpent, ColorSoul)} " +
                          $"{Tint("Tinh Hồn", ColorSoul)}";
                    NotifyGroup.Instance?.Show(message);
                    Refresh();
                    panel?.RefreshClanSummary();
                }
            }
            catch (Exception exception)
            {
                Debug.LogException(exception, this);
                NotifyGroup.Instance?.Show(Tint("Không thể luyện hồn, vui lòng thử lại", ColorWarn));
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
                NotifyGroup.Instance?.Show(Tint("Bạn không có quyền nâng giới hạn hồn tông môn", ColorWarn));
                return;
            }

            if (!TryGetNextHonLimitInfo(clan, out var nextLevel, out var cost, out var nextMaxHon,
                    out var nextMaxSoulEssence))
            {
                NotifyGroup.Instance?.Show(Tint("Giới hạn hồn đã đạt cấp tối đa", ColorGood));
                return;
            }

            var pointsCost = Math.Min(Math.Max(0, GetCurrentContribPoints()), cost);
            var shortfall = cost - pointsCost;
            ShowUpgradeConfirmation(nextLevel, pointsCost, shortfall / 2 + shortfall % 2,
                nextMaxHon, nextMaxSoulEssence);
        }

        private long GetCurrentContribPoints()
        {
            var charId = Client.Instance?.CharacterData?.Id ?? 0;
            return Clan?.Members?.FirstOrDefault(x => x.CharId == charId)?.ContribPoints ?? 0;
        }

        private void ShowUpgradeConfirmation(int nextLevel, long pointsCost, long moneyVipCost,
            long nextMaxHon, long nextMaxSoulEssence, bool priceChanged = false)
        {
            var clanId = Clan?.Id;
            Pool.Get<PopupConfirmAction>()?.ShowPopConfirmAction(
                (priceChanged ? "<b>Chi phí đã thay đổi. Xác nhận lại?</b>\n" : "") +
                $"Mở giới hạn hồn " +
                $"cấp {Num(nextLevel, ColorLevel)}.\n" +
                $"Cống hiến cá nhân: {Num(pointsCost, ColorFunds)} điểm\n" +
                $"Tiên Ngọc cá nhân: {Num(moneyVipCost, ColorSoul)}\n" +
                (moneyVipCost > 0 ? "1 Tiên Ngọc bù 2 điểm thiếu, làm tròn lên.\n" : "") +
                $"{Muted("Giới hạn Hồn:")} {Num(nextMaxHon, ColorHon)}\n" +
                $"{Muted("Giới hạn Tinh Hồn:")} {Num(nextMaxSoulEssence, ColorSoul)}",
                () =>
                {
                    if (Clan?.Id == clanId)
                        _ = UpgradeHonLimit(nextLevel, pointsCost, moneyVipCost);
                });
        }

        private async Task UpgradeHonLimit(int limitLevel, long pointsCost, long moneyVipCost)
        {
            if (isProcessing || Handler == null) return;
            SetProcessing(true);
            UpgradeClanMaxHonResult result = null;
            try
            {
                result = await Handler.UpgradeMaxHon(limitLevel, pointsCost, moneyVipCost);
                if (!result.Success && !result.RequiresConfirmation)
                {
                    NotifyGroup.Instance?.Show(string.IsNullOrWhiteSpace(result.Message)
                        ? Tint("Không thể nâng giới hạn hồn", ColorWarn)
                        : result.Message);
                }
                else if (result.Success)
                {
                    NotifyGroup.Instance?.Show(
                        $"Nâng giới hạn hồn thành công: {Tint("+", ColorGood)}{Num(result.AddedMaxHon, ColorGood)} " +
                        $"giới hạn {Tint("Hồn", ColorHon)}, tiêu {Num(result.ContribPointsCost, ColorFunds)} điểm cống hiến " +
                        $"và {Num(result.MoneyVipCost, ColorSoul)} Tiên Ngọc");
                    Refresh();
                    panel?.RefreshClanSummary();
                }
            }
            catch (Exception exception)
            {
                Debug.LogException(exception, this);
                NotifyGroup.Instance?.Show(Tint("Không thể nâng giới hạn hồn, vui lòng thử lại", ColorWarn));
            }
            finally
            {
                SetProcessing(false);
            }
            if (result?.RequiresConfirmation == true && gameObject.activeInHierarchy &&
                TryGetNextHonLimitInfo(Clan, out var nextLevel, out _, out var nextMaxHon, out var nextMaxSoulEssence) &&
                nextLevel == limitLevel)
                ShowUpgradeConfirmation(limitLevel, result.ContribPointsCost, result.MoneyVipCost,
                    nextMaxHon, nextMaxSoulEssence, true);
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
        // Chi phí Tinh Hồn theo bậc thang (mô phỏng đúng công thức server)
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

        /// <summary>Chi phí Tinh Hồn của riêng điểm Hồn thứ <paramref name="honPoint"/> (đếm từ 1).</summary>
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
        /// Số điểm Hồn nhiều nhất có thể luyện với <paramref name="budget"/> Tinh Hồn,
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
            if (clan == null) return NoData;

            var hon = clan.Hon;
            var tierSize = GetHonCostTierSize();
            var costNow = GetHonPointCost(hon + 1);
            var tierStart = hon / tierSize * tierSize + 1;
            var tierEnd = (hon / tierSize + 1) * tierSize;
            var growth = GetHonCostGrowthPerTier();

            var text = $"{Label("Mốc hiện tại")} {Muted($"điểm {tierStart:n0}-{tierEnd:n0}")} " +
                       $"{Num(costNow, ColorSoul)} {Tint("Tinh Hồn", ColorSoul)}{Muted("/điểm")}";
            if (growth > 0)
            {
                // Mốc kế tiếp tô đỏ cho thấy giá sẽ đắt lên, nhắc người chơi luyện sớm.
                text += $"\n{Muted($"Từ điểm {tierEnd + 1:n0}:")} {Num(costNow + growth, ColorWarn)} " +
                        $"{Tint("Tinh Hồn", ColorSoul)}{Muted("/điểm")}";
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

        /// <summary>
        /// Bật Rich Text cho toàn bộ Text của popup. Prefab được gán tay nên nếu quên tick
        /// Rich Text trong Inspector thì các thẻ màu sẽ hiện ra dạng chữ thô; bật ở đây cho chắc.
        /// </summary>
        private void EnableRichText()
        {
            EnableRichText(honText);
            EnableRichText(soulEssenceText);
            EnableRichText(fundsText);
            EnableRichText(honLimitLevelText);
            EnableRichText(honCostRateText);
            EnableRichText(nextHonLimitText);
        }

        private static void EnableRichText(Text target)
        {
            if (target != null) target.supportRichText = true;
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
