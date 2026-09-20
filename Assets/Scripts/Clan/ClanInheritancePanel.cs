using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using CreateCharacter;
using LongDH.LongDHModules;
using LongDHModules;
using LuyenDan;
using LuyenKhi;
using Network;
using Network.Core;
using Network.Models.Clans;
using UnityEngine;

namespace Clan
{
    public class ClanInheritancePanel : ClanPopup
    {
        [Header("Thông tin cấp")]
        [SerializeField] private UnityEngine.UI.Text levelText;
        [SerializeField] private UnityEngine.UI.Text expText;
        [SerializeField] private UnityEngine.UI.Text fundsText;
        [SerializeField] private UnityEngine.UI.Slider expSlider;
        [SerializeField] private UnityEngine.UI.Text slotRuleText;
        [SerializeField] private UnityEngine.UI.Text permissionText;
        [SerializeField] private UnityEngine.UI.Text costHintText;

        [Header("Nâng cấp bằng quỹ")]
        [SerializeField] private GameObject upgradeRoot;
        [SerializeField] private UnityEngine.UI.Button upgradeButton;
        [SerializeField] private SelectNum upgradeFundSelectNum;
        [SerializeField] private UnityEngine.UI.Text upgradeHintText;
        [SerializeField] private UnityEngine.UI.Text upgradePreviewText;

        [Header("Hai cột truyền thừa")]
        [SerializeField] private RectTransform startTurnContainer;
        [SerializeField] private RectTransform endTurnContainer;
        [SerializeField] private ClanInheritanceSlotElement slotTemplate;

        [Header("Popup chọn hiệu ứng")]
        [SerializeField] private GameObject selectionRoot;
        [SerializeField] private UnityEngine.UI.Text selectionTitleText;
        [SerializeField] private UnityEngine.UI.Text selectionHintText;
        [SerializeField] private RectTransform optionContainer;
        [SerializeField] private ClanInheritanceOptionElement optionTemplate;
        [SerializeField] private UnityEngine.UI.Button clearButton;
        [SerializeField] private UnityEngine.UI.Button cancelSelectionButton;

        private readonly List<ClanInheritanceSlotElement> startSlots = new();
        private readonly List<ClanInheritanceSlotElement> endSlots = new();
        private readonly List<ClanInheritanceOptionElement> options = new();
        private ClanInheritanceSlotElement selectedSlot;
        private bool isSubmitting;

        protected override void Awake()
        {
            base.Awake();
            CacheSlots(startTurnContainer, startSlots);
            CacheSlots(endTurnContainer, endSlots);
            if (slotTemplate != null) slotTemplate.gameObject.SetActive(false);
            if (optionTemplate != null) optionTemplate.gameObject.SetActive(false);
            if (selectionRoot != null) selectionRoot.SetActive(false);
        }

        protected override void Start()
        {
            base.Start();
            SetEventButton(clearButton, ClearSelected);
            SetEventButton(cancelSelectionButton, CloseSelection);
            SetEventButton(upgradeButton, OpenUpgradeFundsSelection);
            if (upgradeFundSelectNum != null)
            {
                upgradeFundSelectNum.confirmAction.AddListener(ConfirmUpgradeWithFunds);
            }
        }

        protected override void BindClanEvents(ClanHandler handler)
        {
            handler.ClanChanged += OnClanChanged;
            handler.InheritanceChanged += OnInheritanceChanged;
            handler.FundsChanged += OnFundsChanged;
            handler.ExpChanged += OnExpChanged;
            handler.LevelChanged += OnLevelChanged;
        }

        protected override void UnbindClanEvents(ClanHandler handler)
        {
            handler.LevelChanged -= OnLevelChanged;
            handler.ExpChanged -= OnExpChanged;
            handler.FundsChanged -= OnFundsChanged;
            handler.InheritanceChanged -= OnInheritanceChanged;
            handler.ClanChanged -= OnClanChanged;
        }

        protected override void OnShown()
        {
            CloseSelection();
            Render();
        }

        public void Render()
        {
            if (!InitializeHandler()) return;
            var clan = Handler.CurrentClan;
            var config = Handler.CurrentConfig;
            if (clan == null || config == null) return;

            var unlockedSlots = Handler.GetInheritanceSlots(clan.Level);
            var shownSlots = Math.Max(unlockedSlots, Math.Max(0, config.MaxInheritanceSlotsPerColumn));
            RenderColumn(ClanInheritanceTrigger.StartTurn, startTurnContainer, startSlots,
                clan.StartTurnEffects, shownSlots, unlockedSlots);
            RenderColumn(ClanInheritanceTrigger.EndTurn, endTurnContainer, endSlots,
                clan.EndTurnEffects, shownSlots, unlockedSlots);

            if (levelText != null) levelText.text = $"Cấp {clan.Level}";
            if (fundsText != null) fundsText.text = $"Quỹ: {clan.Funds:n0}";
            RenderExp(clan.Level, clan.Exp, config.ClanLevelRequiredTotalExp);
            if (slotRuleText != null) slotRuleText.text = $"Đã mở {unlockedSlots} ô mỗi cột";

            var canManage = CanManage();
            var canUpgrade = CanUpgradeClan();
            if (permissionText != null)
            {
                permissionText.text = canManage
                    ? "Tộc Chủ có thể thay đổi truyền thừa"
                    : "Chỉ Tộc Chủ có thể thay đổi";
            }
            if (costHintText != null)
                costHintText.text = $"Ô trống miễn phí · Đổi/xóa: {config.ChangeInheritanceContribPointsCost:n0} điểm cống hiến; thiếu điểm bù Tiên Ngọc";
            RenderUpgrade(canUpgrade, clan.Funds);
        }

        private void RenderUpgrade(bool canUpgrade, long funds)
        {
            if (upgradeRoot != null) upgradeRoot.SetActive(canUpgrade);
            if (upgradeButton != null)
            {
                upgradeButton.gameObject.SetActive(canUpgrade);
                upgradeButton.interactable = canUpgrade && funds > 0 && !isSubmitting;
            }
            if (upgradeHintText != null)
            {
                upgradeHintText.text = canUpgrade
                    ? $"Dùng quỹ tông môn để tăng EXP · Quỹ hiện có: {funds:n0}"
                    : "Chỉ Tộc Chủ có thể dùng quỹ nâng cấp";
            }
            if (upgradePreviewText != null) upgradePreviewText.text = string.Empty;
        }

        private void RenderColumn(
            ClanInheritanceTrigger trigger,
            RectTransform container,
            List<ClanInheritanceSlotElement> cache,
            IReadOnlyList<ClanInheritanceEffect> effects,
            int shownSlots,
            int unlockedSlots)
        {
            if (container == null || slotTemplate == null) return;
            EnsureSlots(container, cache, shownSlots);
            for (var index = 0; index < cache.Count; index++)
            {
                var visible = index < shownSlots;
                cache[index].gameObject.SetActive(visible);
                if (!visible) continue;
                var effect = effects?.FirstOrDefault(x => x.SlotIndex == index);
                cache[index].Init(trigger, index, effect, Handler.GetInheritanceDisplayValue(effect), index < unlockedSlots,
                    GetUnlockLevel(index));
            }
        }

        private void EnsureSlots(RectTransform container, List<ClanInheritanceSlotElement> cache, int count)
        {
            while (cache.Count < count)
            {
                var element = Instantiate(slotTemplate, container);
                element.name = $"InheritanceSlot_{cache.Count + 1}";
                element.Clicked += OpenSelection;
                element.gameObject.SetActive(true);
                cache.Add(element);
            }
        }

        private void CacheSlots(RectTransform container, List<ClanInheritanceSlotElement> cache)
        {
            if (container == null) return;
            foreach (var element in container.GetComponentsInChildren<ClanInheritanceSlotElement>(true))
            {
                if (element == null || element == slotTemplate || element.name == "SlotTemplate" || cache.Contains(element))
                    continue;
                element.Clicked += OpenSelection;
                cache.Add(element);
            }
        }

        private void OpenSelection(ClanInheritanceSlotElement slot)
        {
            if (isSubmitting || slot == null) return;
            if (!CanManage())
            {
                Notify("Chỉ Tộc Chủ được quản lý truyền thừa");
                return;
            }

            selectedSlot = slot;
            var columnName = slot.Trigger == ClanInheritanceTrigger.StartTurn ? "Bắt đầu hiệp" : "Kết thúc hiệp";
            if (selectionTitleText != null) selectionTitleText.text = $"{columnName} · Ô {slot.SlotIndex + 1}";
            if (selectionHintText != null)
            {
                selectionHintText.text = slot.Effect == null
                    ? "Lần chọn đầu tiên cho ô này được miễn phí"
                    : $"Đổi/xóa: {Handler.CurrentConfig.ChangeInheritanceContribPointsCost:n0} điểm cống hiến; thiếu điểm bù Tiên Ngọc cá nhân";
            }
            if (clearButton != null) clearButton.gameObject.SetActive(slot.Effect != null);
            RenderOptions(slot.Effect?.EffectType);
            if (selectionRoot != null) selectionRoot.SetActive(true);
        }

        private void RenderOptions(EffectType? selected)
        {
            var whitelist = Handler.CurrentConfig?.InheritanceBuffWhitelist ?? Array.Empty<EffectType>();
            while (options.Count < whitelist.Length)
            {
                var option = Instantiate(optionTemplate, optionContainer);
                option.name = $"InheritanceOption_{options.Count + 1}";
                option.Clicked += OnOptionClicked;
                options.Add(option);
            }

            for (var index = 0; index < options.Count; index++)
            {
                var visible = index < whitelist.Length;
                options[index].gameObject.SetActive(visible);
                if (visible)
                {
                    var effectType = whitelist[index];
                    var range = Handler.GetInheritanceValueRange(effectType);
                    options[index].Init(effectType, range.Min, range.Max, effectType == selected);
                }
            }
        }

        private void OnOptionClicked(ClanInheritanceOptionElement option)
        {
            if (option != null) ConfirmChange(option.EffectType);
        }

        private void ClearSelected()
        {
            if (selectedSlot?.Effect != null) ConfirmChange(null);
        }

        private void ConfirmChange(EffectType? effectType)
        {
            if (selectedSlot == null || isSubmitting) return;
            if (selectedSlot.Effect?.EffectType == effectType) return;

            var totalCost = selectedSlot.Effect == null ? 0 : Handler.CurrentConfig.ChangeInheritanceContribPointsCost;
            if (totalCost < 0)
            {
                Notify("Cấu hình phí thay đổi truyền thừa không hợp lệ");
                return;
            }
            var charId = Client.Instance?.CharacterData?.Id ?? 0;
            var member = Handler.CurrentClan.Members?.FirstOrDefault(x => x.CharId == charId);
            if (member == null) return;
            var pointsCost = (int)Math.Min(Math.Max(0, member.ContribPoints), totalCost);
            var shortfall = totalCost - pointsCost;
            ShowInheritanceConfirmation(selectedSlot.Trigger, selectedSlot.SlotIndex, effectType,
                pointsCost, shortfall / 2 + shortfall % 2);
        }

        private void ShowInheritanceConfirmation(ClanInheritanceTrigger trigger, int slotIndex,
            EffectType? effectType, int pointsCost, int moneyVipCost, bool priceChanged = false)
        {
            var name = effectType == null ? "xóa truyền thừa" : $"chọn {GetEffectName(effectType)}";
            var column = trigger == ClanInheritanceTrigger.StartTurn ? "Bắt đầu hiệp" : "Kết thúc hiệp";
            var content = (priceChanged ? "<b>Chi phí đã thay đổi. Vui lòng xác nhận lại.</b>\n" : "")
                + $"Xác nhận <b>{name}</b>?\n{column} · Ô {slotIndex + 1}\n";
            if (pointsCost == 0 && moneyVipCost == 0)
                content += "<color=#67DCA2><b>Miễn phí</b></color>";
            else
            {
                content += $"Cống hiến cá nhân: <color=#FFD166><b>{pointsCost:n0} điểm</b></color>"
                    + $"\nTiên Ngọc cá nhân: <color=#69D9EF><b>{moneyVipCost:n0}</b></color>";
                if (moneyVipCost > 0)
                    content += "\n1 Tiên Ngọc bù 2 điểm thiếu, làm tròn lên.";
            }
            var popup = Pool.Get<PopupConfirmAction>();
            if (popup == null)
            {
                Notify("Không thể mở xác nhận truyền thừa");
                return;
            }
            var clanId = Handler.CurrentClan.Id;
            popup.ShowPopConfirmAction(content, () =>
            {
                if (Handler?.CurrentClan?.Id != clanId) return;
                _ = Submit(trigger, slotIndex, effectType, pointsCost, moneyVipCost);
            });
        }

        private async Task Submit(ClanInheritanceTrigger trigger, int slotIndex, EffectType? effectType,
            int pointsCost, int moneyVipCost)
        {
            if (isSubmitting) return;
            isSubmitting = true;
            SetSelectionInteractable(false);
            SetClanInheritanceResult result = null;
            try
            {
                result = await Handler.SetInheritance(trigger, slotIndex, effectType, pointsCost, moneyVipCost);
                if (!result.RequiresConfirmation) Notify(result.Message);
                if (result.Success)
                {
                    CloseSelection();
                    Render();
                }
            }
            catch (Exception exception)
            {
                Debug.LogException(exception, this);
                Notify("Không thể cập nhật truyền thừa");
            }
            finally
            {
                isSubmitting = false;
                SetSelectionInteractable(true);
            }
            if (result != null && result.RequiresConfirmation && gameObject.activeInHierarchy)
            {
                Render();
                ShowInheritanceConfirmation(trigger, slotIndex, effectType, result.Cost, result.MoneyVipCost, true);
            }
        }

        private void OpenUpgradeFundsSelection()
        {
            if (isSubmitting || upgradeFundSelectNum == null) return;
            if (!CanUpgradeClan())
            {
                Notify("Chỉ Tộc Chủ được nâng cấp tông môn");
                return;
            }

            var funds = Handler?.CurrentClan?.Funds ?? 0;
            var max = (int)Math.Min(int.MaxValue, Math.Max(0, funds));
            if (max <= 0)
            {
                Notify("Quỹ tông môn không đủ");
                return;
            }

            upgradeFundSelectNum.ValueChanged -= OnUpgradeFundsChanged;
            upgradeFundSelectNum.ValueChanged += OnUpgradeFundsChanged;
            upgradeFundSelectNum.SetMin(1);
            upgradeFundSelectNum.SetMax(max);
            upgradeFundSelectNum.SetText(GetUpgradePreviewText(1));
            upgradeFundSelectNum.Show(0.15f);
            upgradeFundSelectNum.SetValue(1);
        }

        private void OnUpgradeFundsChanged(int value)
        {
            var funds = Math.Max(1, value);
            var text = GetUpgradePreviewText(funds);
            upgradeFundSelectNum?.SetText(text);
            if (upgradePreviewText != null) upgradePreviewText.text = text;
        }

        private string GetUpgradePreviewText(long funds)
        {
            var exp = Handler?.GetClanUpgradeExpFromFunds(funds) ?? funds;
            return $"Dùng {funds:n0} Quỹ → +{exp:n0} EXP";
        }

        private async void ConfirmUpgradeWithFunds()
        {
            if (isSubmitting || upgradeFundSelectNum == null) return;

            var funds = upgradeFundSelectNum.GetValue();
            if (funds <= 0) return;

            isSubmitting = true;
            if (upgradeButton != null) upgradeButton.interactable = false;
            try
            {
                var result = await Handler.UpgradeWithFunds(funds);
                Notify(result.Success
                    ? $"Đã dùng {result.FundsSpent:n0} Quỹ, tăng {result.AddedExp:n0} EXP tông môn"
                    : result.Message);
                if (result.Success) Render();
            }
            catch (Exception exception)
            {
                Debug.LogException(exception, this);
                Notify("Không thể nâng cấp tông môn");
            }
            finally
            {
                isSubmitting = false;
                Render();
            }
        }

        private bool CanManage()
        {
            var charId = Client.Instance?.CharacterData?.Id ?? 0;
            var member = Handler?.CurrentClan?.Members?.FirstOrDefault(x => x.CharId == charId);
            return member != null &&
                   (Handler.GetPermissions(member.Role) & ClanPermission.ManageInheritance) != 0;
        }

        private bool CanUpgradeClan()
        {
            var charId = Client.Instance?.CharacterData?.Id ?? 0;
            var clan = Handler?.CurrentClan;
            var member = clan?.Members?.FirstOrDefault(x => x.CharId == charId);
            return clan != null && member != null &&
                   (member.Role == ClanRole.Leader || clan.LeaderCharId == charId);
        }

        private int GetUnlockLevel(int slotIndex)
        {
            var config = Handler.CurrentConfig;
            if (slotIndex < config.BaseInheritanceSlots) return 1;
            if (config.InheritanceSlotsPerLevel <= 0) return int.MaxValue;
            var remaining = slotIndex + 1 - config.BaseInheritanceSlots;
            return 1 + Mathf.CeilToInt(remaining / (float)config.InheritanceSlotsPerLevel);
        }

        private void RenderExp(int level, long exp, long[] milestones)
        {
            milestones ??= Array.Empty<long>();
            var current = level - 1 >= 0 && level - 1 < milestones.Length ? milestones[level - 1] : 0;
            var hasNext = level < milestones.Length;
            var next = hasNext ? milestones[level] : current;
            if (expText != null)
                expText.text = hasNext ? $"EXP {exp:n0} / {next:n0}" : $"EXP {exp:n0} · Cấp tối đa";
            if (expSlider != null)
            {
                expSlider.minValue = 0;
                expSlider.maxValue = 1;
                expSlider.value = hasNext && next > current
                    ? Mathf.Clamp01((float)(exp - current) / (next - current))
                    : 1;
            }
        }

        private void SetSelectionInteractable(bool value)
        {
            if (clearButton != null) clearButton.interactable = value;
            if (cancelSelectionButton != null) cancelSelectionButton.interactable = value;
            foreach (var option in options)
            {
                var button = option != null ? option.GetComponent<UnityEngine.UI.Button>() : null;
                if (button != null) button.interactable = value;
            }
        }

        private void CloseSelection()
        {
            selectedSlot = null;
            if (selectionRoot != null) selectionRoot.SetActive(false);
        }

        private void OnClanChanged(Network.Models.Clans.Clan value)
        {
            if (value == null) Hide(0f);
            else Render();
        }

        private void OnInheritanceChanged(ClanInheritanceChanged value)
        {
            if (value?.ClanId == Handler?.CurrentClan?.Id) Render();
        }

        private void OnFundsChanged(ClanFundsChanged value)
        {
            if (value?.ClanId == Handler?.CurrentClan?.Id) Render();
        }

        private void OnExpChanged(ClanExpChanged value)
        {
            if (value?.ClanId == Handler?.CurrentClan?.Id) Render();
        }

        private void OnLevelChanged(ClanLevelChanged value)
        {
            if (value?.ClanId == Handler?.CurrentClan?.Id) Render();
        }

        private static void Notify(string message)
        {
            if (!string.IsNullOrWhiteSpace(message)) NotifyGroup.Instance?.Show(message);
        }

        public static string GetEffectName(EffectType? effectType)
        {
            if (effectType == null) return "Không chọn";
            var config = ClanBuffResources.Instance?.EffectTypeConfigs?
                .FirstOrDefault(x => x.Type == effectType.Value);
            if (config != null && !string.IsNullOrWhiteSpace(config.Name)) return config.Name;
            return Regex.Replace(effectType.Value.ToString(), "(?<!^)([A-Z])", " $1");
        }

        protected override void OnDestroy()
        {
            foreach (var slot in startSlots.Concat(endSlots))
                if (slot != null) slot.Clicked -= OpenSelection;
            foreach (var option in options)
                if (option != null) option.Clicked -= OnOptionClicked;
            if (upgradeFundSelectNum != null)
            {
                upgradeFundSelectNum.ValueChanged -= OnUpgradeFundsChanged;
                upgradeFundSelectNum.confirmAction.RemoveListener(ConfirmUpgradeWithFunds);
            }
            base.OnDestroy();
        }
    }
}
