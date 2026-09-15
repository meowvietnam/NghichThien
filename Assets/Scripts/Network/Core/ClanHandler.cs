using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LuyenDan;
using LuyenKhi;
using Network.Extensions.MemoryPack;
using Network.Models;
using Network.Models.Clans;
using UnityEngine;
using ClanModel = Network.Models.Clans.Clan;
namespace Network.Core
{
    public class ClanHandler : IClientHandler
    {
        private readonly Client client;

        public ClanModel CurrentClan { get; private set; }
        public ClanConfig CurrentConfig { get; private set; }
        public int MaxShopQuantityPerRequest { get; private set; }
        public int MaxShopItemsAddedPerDay { get; private set; }
        public int ShopItemsAddedToday { get; private set; }
        public int ShopSlotsUsed => CurrentClan?.ShopItems?.Count ?? 0;
        public int MaxShopSlots => CurrentClan == null ? 0 : GetMaxShopSlots(CurrentClan.Level);
        public int FreeContributionTimesRemaining { get; private set; }
        private DateTime freeContributionStateDateUtc;

        public event Action<ClanModel> ClanChanged;
        public event Action<ClanMember> MemberAdded;
        public event Action<ClanRemoveMember> MemberRemoved;
        public event Action<ClanContribution> ContributionChanged;
        public event Action<IReadOnlyList<ClanShopItem>> ShopChanged;
        public event Action<ClanMemberPointsChanged> MemberPointsChanged;
        public event Action<ClanMemberRoleChanged> MemberRoleChanged;
        public event Action<ClanJoinRequest> JoinRequestAdded;
        public event Action<ClanJoinRequestRemoved> JoinRequestRemoved;
        public event Action<ClanInheritanceChanged> InheritanceChanged;
        public event Action<ClanFundsChanged> FundsChanged;
        public event Action<ClanExpChanged> ExpChanged;
        public event Action<ClanLevelChanged> LevelChanged;
        public event Action<ClanMapNode> MapNodeChanged;

        public ClanHandler()
        {
            client = Client.Instance;
        }

        public async void OnEnter()
        {
            try
            {
                await RefreshClan();
            }
            catch (Exception exception)
            {
                Debug.LogException(exception);
            }
        }

        public async Task<GetClanResult> RefreshClan()
        {
            var result = await Request<GetClanRequest, GetClanResult>(
                MessageTag.GetClanRequest,
                new GetClanRequest());
            if (result.Success)
            {
                CurrentConfig = result.Config;
                SetCurrentClan(result.Clan);
            }
            return result;
        }

        public ClanPermission GetPermissions(ClanRole role)
        {
            if (CurrentConfig?.RolePermissions != null &&
                CurrentConfig.RolePermissions.TryGetValue(role, out var permissions))
            {
                return permissions;
            }

            return ClanPermission.None;
        }

        public int GetInheritanceSlots(int clanLevel)
        {
            var config = CurrentConfig;
            if (config == null || config.MaxInheritanceSlotsPerColumn <= 0) return 0;
            var slots = (long)Math.Max(0, config.BaseInheritanceSlots) +
                        (long)Math.Max(0, clanLevel - 1) * Math.Max(0, config.InheritanceSlotsPerLevel);
            return (int)Math.Min(slots, config.MaxInheritanceSlotsPerColumn);
        }

        public int GetMaxShopSlots(int clanLevel)
        {
            return Math.Max(1, clanLevel) * 3;
        }

        public (int Min, int Max) GetInheritanceValueRange(EffectType? effectType)
        {
            var config = CurrentConfig;
            if (config == null) return (0, 0);
            var clanBuffResources = global::Clan.ClanBuffResources.Instance;
            var baseValue = effectType.HasValue && clanBuffResources != null
                ? clanBuffResources.GetBaseValueOf(effectType.Value)
                : config.InheritanceBaseValue;
            var multiplier = Math.Max(1, CurrentClan?.Level ?? 1);
            var minValue = (long)(baseValue / 2) * multiplier;
            var maxValue = (long)baseValue * 2 * multiplier;
            return (
                (int)Math.Max(int.MinValue, Math.Min(int.MaxValue, minValue)),
                (int)Math.Max(int.MinValue, Math.Min(int.MaxValue, maxValue)));
        }

        public int GetInheritanceDisplayValue(ClanInheritanceEffect effect)
        {
            if (effect == null) return 0;
            return effect.Value;
        }

        public long GetClanUpgradeExpFromFunds(long funds)
        {
            return funds;
        }

        public async Task<CreateClanResult> CreateClan(CreateClanRequest request)
        {
            if (request == null) throw new ArgumentNullException(nameof(request));

            var result = await Request<CreateClanRequest, CreateClanResult>(MessageTag.CreateClanRequest, request);
            if (result.Success && result.NewMoneyVip.HasValue)
            {
                client.CharacterData.MoneyVip = result.NewMoneyVip.Value;
            }

            return result;
        }

        public Task<UploadImageResult> UploadImage(byte[] imageData)
        {
            if (imageData == null || imageData.Length == 0)
                throw new ArgumentException("Dữ liệu ảnh không được để trống", nameof(imageData));

            return Request<UploadImageRequest, UploadImageResult>(
                MessageTag.UploadClanImageRequest,
                new UploadImageRequest { Data = imageData });
        }

        public Task<AddClanMemberResult> AddMember(int charId, ClanRole role = ClanRole.Member)
        {
            return AddMemberInternal(new AddClanMemberRequest { CharId = charId, Role = role });
        }

        private async Task<AddClanMemberResult> AddMemberInternal(AddClanMemberRequest request)
        {
            var result = await Request<AddClanMemberRequest, AddClanMemberResult>(
                MessageTag.AddClanMemberRequest,
                request);
            if (result.Success) ApplyMemberAdded(result.Member, false);
            return result;
        }

        public async Task<KickClanMemberResult> KickMember(int charId)
        {
            var result = await Request<KickClanMemberRequest, KickClanMemberResult>(
                MessageTag.KickClanMemberRequest,
                new KickClanMemberRequest { CharId = charId });
            if (result.Success && CurrentClan != null)
            {
                ApplyMemberRemoved(new ClanRemoveMember
                {
                    ClanId = CurrentClan.Id,
                    CharId = result.CharId,
                    Reason = ClanRemoveMemberReason.Kick
                }, false);
            }

            return result;
        }

        public async Task<SetClanMemberRoleResult> SetMemberRole(int charId, ClanRole role)
        {
            var result = await Request<SetClanMemberRoleRequest, SetClanMemberRoleResult>(
                MessageTag.SetClanMemberRoleRequest,
                new SetClanMemberRoleRequest { CharId = charId, Role = role });
            if (result.Success) ApplyMemberRoleChanged(result.Change, false);
            return result;
        }

        public async Task<LeaveClanResult> LeaveClan()
        {
            var result = await Request<LeaveClanRequest, LeaveClanResult>(
                MessageTag.LeaveClanRequest,
                new LeaveClanRequest());
            if (result.Success)
            {
                ApplyMemberRemoved(new ClanRemoveMember
                {
                    ClanId = result.ClanId,
                    CharId = client.CharacterData.Id,
                    Reason = ClanRemoveMemberReason.Leave
                }, false);
            }

            return result;
        }

        public async Task<ClanContributionResult> Contribute(int times)
        {
            var result = await Request<ClanContributionRequest, ClanContributionResult>(
                MessageTag.ClanContributionRequest,
                new ClanContributionRequest { Times = times });

            if (result.Success)
            {
                client.CharacterData.MoneyVip = result.NewMoneyVip;
                ApplyContribution(result.Contribution, false);
            }

            return result;
        }

        public Task<SearchClanResult> Search(string name, int limit = 20)
        {
            return Request<SearchClanRequest, SearchClanResult>(
                MessageTag.SearchClanRequest,
                new SearchClanRequest { Name = name ?? string.Empty, Limit = limit });
        }

        public Task<GetClanMapNodesResult> GetClanMapNodes()
        {
            return Request<GetClanMapNodesRequest, GetClanMapNodesResult>(
                MessageTag.GetClanMapNodesRequest,
                new GetClanMapNodesRequest());
        }

        // Xin gia nhập một tông môn (dùng khi đang xem tông môn của người khác, vd. từ ClanListPanel).
        public Task<RequestJoinClanResult> RequestJoinClan(int clanId)
        {
            return Request<RequestJoinClanRequest, RequestJoinClanResult>(
                MessageTag.RequestJoinClanRequest,
                new RequestJoinClanRequest { ClanId = clanId });
        }

        // Duyệt đơn xin gia nhập — chỉ thành viên có quyền ClanPermission.AddMember mới nên gọi
        // (server tự kiểm tra lại quyền, client chỉ ẩn UI cho gọn).
        public async Task<ApproveClanJoinRequestResult> ApproveJoinRequest(int charId)
        {
            var result = await Request<ApproveClanJoinRequestRequest, ApproveClanJoinRequestResult>(
                MessageTag.ApproveClanJoinRequestRequest,
                new ApproveClanJoinRequestRequest { CharId = charId });
            if (result.Success)
            {
                ApplyJoinRequestRemoved(new ClanJoinRequestRemoved
                {
                    ClanId = CurrentClan?.Id ?? 0,
                    CharId = charId
                }, false);
                if (result.Member != null) ApplyMemberAdded(result.Member, false);
            }

            return result;
        }

        public async Task<RejectClanJoinRequestResult> RejectJoinRequest(int charId)
        {
            var result = await Request<RejectClanJoinRequestRequest, RejectClanJoinRequestResult>(
                MessageTag.RejectClanJoinRequestRequest,
                new RejectClanJoinRequestRequest { CharId = charId });
            if (result.Success)
            {
                ApplyJoinRequestRemoved(new ClanJoinRequestRemoved
                {
                    ClanId = CurrentClan?.Id ?? 0,
                    CharId = result.CharId
                }, false);
            }

            return result;
        }

        public async Task<GetClanShopResult> GetShop()
        {
            var result = await Request<GetClanShopRequest, GetClanShopResult>(
                MessageTag.GetClanShopRequest,
                new GetClanShopRequest());

            if (result.Success)
            {
                MaxShopQuantityPerRequest = result.MaxQuantityPerRequest;
                MaxShopItemsAddedPerDay = result.MaxItemsAddedPerDay;
                ShopItemsAddedToday = result.ShopItemsAddedToday;
                ReplaceShop(result.Items);
            }

            return result;
        }

        public async Task<BuyClanShopResult> BuyShopItem(int shopItemId, int quantity)
        {
            var result = await Request<BuyClanShopRequest, BuyClanShopResult>(
                MessageTag.BuyClanShopRequest,
                new BuyClanShopRequest { ShopItemId = shopItemId, Quantity = quantity });

            if (result.Success)
            {
                ApplyInventoryItem(result.InventoryItem);
                ApplyInventoryFurnace(result.InventoryFurnace);
                ApplyMemberPoints(new ClanMemberPointsChanged
                {
                    ClanId = CurrentClan != null ? CurrentClan.Id : 0,
                    CharId = client.CharacterData.Id,
                    ContribPoints = result.ContribPoints
                }, false);
            }

            return result;
        }

        public async Task<SetClanInheritanceResult> SetInheritance(
            ClanInheritanceTrigger trigger,
            int slotIndex,
            EffectType? effectType)
        {
            var result = await Request<SetClanInheritanceRequest, SetClanInheritanceResult>(
                MessageTag.SetClanInheritanceRequest,
                new SetClanInheritanceRequest
                {
                    Trigger = trigger,
                    SlotIndex = slotIndex,
                    EffectType = effectType
                });

            if (result.Success)
            {
                if (CurrentClan != null) CurrentClan.Funds = result.ClanFunds;
                ApplyInheritanceChanged(result.Change, false);
            }

            return result;
        }

        public async Task<UpgradeClanWithFundsResult> UpgradeWithFunds(long funds)
        {
            var result = await Request<UpgradeClanWithFundsRequest, UpgradeClanWithFundsResult>(
                MessageTag.UpgradeClanWithFundsRequest,
                new UpgradeClanWithFundsRequest { Funds = funds });

            if (result.Success)
            {
                ApplyUpgradeWithFunds(result, false);
            }

            return result;
        }

        public bool IsShopItemOwner(ClanShopItem shopItem)
        {
            return shopItem != null && shopItem.OwnerCharId == client.CharacterData.Id;
        }

        public async Task<AddClanShopItemResult> AddShopItem(ItemRef item)
        {
            if (item == null) throw new ArgumentNullException(nameof(item));

            var result = await Request<AddClanShopItemRequest, AddClanShopItemResult>(
                MessageTag.AddClanShopItemRequest,
                new AddClanShopItemRequest { Item = item });

            if (result.Success)
            {
                MaxShopItemsAddedPerDay = result.MaxItemsAddedPerDay;
                ShopItemsAddedToday = result.ShopItemsAddedToday;
                ApplyInventoryItem(result.InventoryItem, item);
                ApplyInventoryFurnace(result.InventoryFurnace);
                ApplyShopItemAdded(result.ShopItem, false);
            }

            return result;
        }

        public async Task<RemoveClanShopItemResult> RemoveShopItem(int shopItemId)
        {
            var result = await Request<RemoveClanShopItemRequest, RemoveClanShopItemResult>(
                MessageTag.RemoveClanShopItemRequest,
                new RemoveClanShopItemRequest { ShopItemId = shopItemId });

            if (result.Success)
            {
                ApplyInventoryItem(result.InventoryItem);
                ApplyInventoryFurnace(result.InventoryFurnace);
                if (CurrentClan != null)
                {
                    ApplyShopItemRemoved(new ClanShopItemRemoved
                    {
                        ClanId = CurrentClan.Id,
                        ShopItemId = result.ShopItemId
                    }, false);
                }
            }

            return result;
        }

        public void Handle(Message<MessageTag> message)
        {
            switch (message.Tag)
            {
                case MessageTag.NewClan:
                    SetCurrentClan(message.Reader.MemRead<ClanModel>());
                    break;
                case MessageTag.ClanAddMember:
                    ApplyMemberAdded(message.Reader.MemRead<ClanAddMember>().Member, true);
                    break;
                case MessageTag.ClanRemoveMember:
                    ApplyMemberRemoved(message.Reader.MemRead<ClanRemoveMember>(), true);
                    break;
                case MessageTag.ClanContribution:
                    ApplyContribution(message.Reader.MemRead<ClanContribution>(), true);
                    break;
                case MessageTag.ClanMemberPointsChanged:
                    ApplyMemberPoints(message.Reader.MemRead<ClanMemberPointsChanged>(), true);
                    break;
                case MessageTag.ClanShopItemAdded:
                    ApplyShopItemAdded(message.Reader.MemRead<ClanShopItemAdded>().ShopItem);
                    break;
                case MessageTag.ClanShopItemRemoved:
                    ApplyShopItemRemoved(message.Reader.MemRead<ClanShopItemRemoved>());
                    break;
                case MessageTag.ClanShopItemChanged:
                    ApplyShopItemChanged(message.Reader.MemRead<ClanShopItemChanged>().ShopItem);
                    break;
                case MessageTag.ClanMemberRoleChanged:
                    ApplyMemberRoleChanged(message.Reader.MemRead<ClanMemberRoleChanged>(), true);
                    break;
                case MessageTag.ClanJoinRequestAdded:
                    ApplyJoinRequestAdded(message.Reader.MemRead<ClanJoinRequestAdded>().Request, true);
                    break;
                case MessageTag.ClanJoinRequestRemoved:
                    ApplyJoinRequestRemoved(message.Reader.MemRead<ClanJoinRequestRemoved>(), true);
                    break;
                case MessageTag.ClanInheritanceChanged:
                    ApplyInheritanceChanged(message.Reader.MemRead<ClanInheritanceChanged>(), true);
                    break;
                case MessageTag.ClanFundsChanged:
                    ApplyFundsChanged(message.Reader.MemRead<ClanFundsChanged>(), true);
                    break;
                case MessageTag.ClanExpChanged:
                    ApplyExpChanged(message.Reader.MemRead<ClanExpChanged>(), true);
                    break;
                case MessageTag.ClanLevelChanged:
                    ApplyLevelChanged(message.Reader.MemRead<ClanLevelChanged>(), true);
                    break;
                case MessageTag.ClanMapNodeChanged:
                    var changed = message.Reader.MemRead<ClanMapNodeChanged>();
                    NotifyMainThread(() => MapNodeChanged?.Invoke(changed.Node));
                    break;
            }
        }

        private async Task<TResult> Request<TRequest, TResult>(MessageTag tag, TRequest request)
        {
            var reader = await client.MemRequestWait(tag, request);
            return reader.MemRead<TResult>();
        }

        private void SetCurrentClan(ClanModel clan)
        {
            if (clan != null)
            {
                clan.Members ??= new List<ClanMember>();
                clan.JoinRequests ??= new List<ClanJoinRequest>();
                clan.ShopItems ??= new List<ClanShopItem>();
                clan.StartTurnEffects ??= new List<ClanInheritanceEffect>();
                clan.EndTurnEffects ??= new List<ClanInheritanceEffect>();
            }

            CurrentClan = clan;
            if (clan != null)
            {
                MaxShopItemsAddedPerDay = CurrentConfig?.MaxShopItemsAddedPerDay ?? MaxShopItemsAddedPerDay;
                RefreshShopQuotaStateFromClan();
            }
            else
            {
                MaxShopItemsAddedPerDay = 0;
                ShopItemsAddedToday = 0;
            }
            RefreshFreeContributionStateFromClan();
            NotifyMainThread(() => ClanChanged?.Invoke(clan));
        }

        public bool HasFreeContributionTurn()
        {
            var todayUtc = DateTime.UtcNow.Date;
            if (freeContributionStateDateUtc != todayUtc)
            {
                RefreshFreeContributionStateFromClan();
            }

            return FreeContributionTimesRemaining > 0;
        }

        private void RefreshFreeContributionStateFromClan()
        {
            freeContributionStateDateUtc = DateTime.UtcNow.Date;
            var charId = client.CharacterData?.Id ?? 0;
            var member = CurrentClan?.Members?.FirstOrDefault(x => x.CharId == charId);
            FreeContributionTimesRemaining = member != null &&
                                             member.LastFreeContributionAt.Date != freeContributionStateDateUtc
                ? 1
                : 0;
        }

        private void RefreshShopQuotaStateFromClan()
        {
            var todayUtc = DateTime.UtcNow.Date;
            var charId = client.CharacterData?.Id ?? 0;
            var member = CurrentClan?.Members?.FirstOrDefault(x => x.CharId == charId);
            ShopItemsAddedToday = member != null && member.LastShopItemAddedAt.Date == todayUtc
                ? member.ShopItemsAddedToday
                : 0;
        }

        private void ApplyMemberAdded(ClanMember member, bool notify)
        {
            if (member == null || CurrentClan == null || member.ClanId != CurrentClan.Id) return;

            CurrentClan.Members ??= new List<ClanMember>();
            var index = CurrentClan.Members.FindIndex(x => x.CharId == member.CharId);
            if (index >= 0) CurrentClan.Members[index] = member;
            else CurrentClan.Members.Add(member);
            CurrentClan.MemberCount = CurrentClan.Members.Count;

            if (notify) NotifyMainThread(() => MemberAdded?.Invoke(member));
        }

        private void ApplyMemberRemoved(ClanRemoveMember removed, bool notify)
        {
            if (removed == null || CurrentClan == null || removed.ClanId != CurrentClan.Id) return;

            CurrentClan.Members?.RemoveAll(x => x.CharId == removed.CharId);
            CurrentClan.MemberCount = CurrentClan.Members?.Count ?? 0;
            if (removed.CharId == client.CharacterData.Id)
            {
                CurrentClan = null;
            }

            if (notify) NotifyMainThread(() => MemberRemoved?.Invoke(removed));
            if (CurrentClan == null) NotifyMainThread(() => ClanChanged?.Invoke(null));
        }

        private void ApplyContribution(ClanContribution contribution, bool notify)
        {
            if (contribution == null || CurrentClan == null || contribution.ClanId != CurrentClan.Id) return;

            CurrentClan.Funds = contribution.ClanFunds;
            var member = CurrentClan.Members?.FirstOrDefault(x => x.CharId == contribution.CharId);
            if (member != null)
            {
                member.ContribPoints = contribution.MemberContribPoints;
                // FreeTimesRemaining là trạng thái authoritative của response/event.
                // Fallback UtcNow giúp tương thích với server cũ chưa gửi timestamp mới.
                member.LastFreeContributionAt = contribution.LastFreeContributionAt == default &&
                                                contribution.FreeContributionTimesRemaining == 0
                    ? DateTime.UtcNow
                    : contribution.LastFreeContributionAt;
            }

            if (contribution.CharId == client.CharacterData.Id)
            {
                freeContributionStateDateUtc = DateTime.UtcNow.Date;
                FreeContributionTimesRemaining = Math.Max(0, contribution.FreeContributionTimesRemaining);
            }
            if (notify) NotifyMainThread(() => ContributionChanged?.Invoke(contribution));
        }

        private void ApplyMemberPoints(ClanMemberPointsChanged changed, bool notify)
        {
            if (changed == null || CurrentClan == null || changed.ClanId != CurrentClan.Id) return;

            var member = CurrentClan.Members?.FirstOrDefault(x => x.CharId == changed.CharId);
            if (member != null) member.ContribPoints = changed.ContribPoints;
            if (notify) NotifyMainThread(() => MemberPointsChanged?.Invoke(changed));
        }

        private void ApplyMemberRoleChanged(ClanMemberRoleChanged changed, bool notify)
        {
            if (changed == null || CurrentClan == null || changed.ClanId != CurrentClan.Id) return;

            var member = CurrentClan.Members?.FirstOrDefault(x => x.CharId == changed.CharId);
            if (member == null) return;
            member.Role = changed.Role;
            if (notify) NotifyMainThread(() => MemberRoleChanged?.Invoke(changed));
        }

        private void ApplyJoinRequestAdded(ClanJoinRequest request, bool notify)
        {
            if (request == null || CurrentClan == null || request.ClanId != CurrentClan.Id) return;

            CurrentClan.JoinRequests ??= new List<ClanJoinRequest>();
            var index = CurrentClan.JoinRequests.FindIndex(x => x.CharId == request.CharId);
            if (index >= 0) CurrentClan.JoinRequests[index] = request;
            else CurrentClan.JoinRequests.Add(request);

            if (notify) NotifyMainThread(() => JoinRequestAdded?.Invoke(request));
        }

        private void ApplyJoinRequestRemoved(ClanJoinRequestRemoved removed, bool notify)
        {
            if (removed == null || CurrentClan == null || removed.ClanId != CurrentClan.Id) return;
            CurrentClan.JoinRequests?.RemoveAll(x => x.CharId == removed.CharId);
            if (notify) NotifyMainThread(() => JoinRequestRemoved?.Invoke(removed));
        }

        private void ApplyInheritanceChanged(ClanInheritanceChanged changed, bool notify)
        {
            if (changed == null || CurrentClan == null || changed.ClanId != CurrentClan.Id) return;

            var effects = changed.Trigger == ClanInheritanceTrigger.StartTurn
                ? CurrentClan.StartTurnEffects
                : CurrentClan.EndTurnEffects;
            effects ??= new List<ClanInheritanceEffect>();
            if (changed.Trigger == ClanInheritanceTrigger.StartTurn)
                CurrentClan.StartTurnEffects = effects;
            else
                CurrentClan.EndTurnEffects = effects;

            effects.RemoveAll(x => x.SlotIndex == changed.SlotIndex);
            if (changed.Effect != null) effects.Add(changed.Effect);
            effects.Sort((left, right) => left.SlotIndex.CompareTo(right.SlotIndex));

            if (notify) NotifyMainThread(() => InheritanceChanged?.Invoke(changed));
        }

        private void ApplyFundsChanged(ClanFundsChanged changed, bool notify)
        {
            if (changed == null || CurrentClan == null || changed.ClanId != CurrentClan.Id) return;
            CurrentClan.Funds = changed.Funds;
            if (notify) NotifyMainThread(() => FundsChanged?.Invoke(changed));
        }

        private void ApplyUpgradeWithFunds(UpgradeClanWithFundsResult result, bool notify)
        {
            if (result == null || CurrentClan == null) return;

            CurrentClan.Funds = result.ClanFunds;
            CurrentClan.Level = result.Level;
            CurrentClan.Exp = result.Exp;
            CurrentClan.MaxMembers = result.MaxMembers;
            var effects = result.Effects ?? Array.Empty<ClanInheritanceEffect>();
            CurrentClan.StartTurnEffects = effects
                .Where(x => x.Trigger == ClanInheritanceTrigger.StartTurn)
                .OrderBy(x => x.SlotIndex)
                .ToList();
            CurrentClan.EndTurnEffects = effects
                .Where(x => x.Trigger == ClanInheritanceTrigger.EndTurn)
                .OrderBy(x => x.SlotIndex)
                .ToList();

            if (notify)
            {
                NotifyMainThread(() =>
                {
                    FundsChanged?.Invoke(new ClanFundsChanged { ClanId = CurrentClan.Id, Funds = result.ClanFunds });
                    ExpChanged?.Invoke(new ClanExpChanged { ClanId = CurrentClan.Id, Exp = result.Exp });
                    LevelChanged?.Invoke(new ClanLevelChanged
                    {
                        ClanId = CurrentClan.Id,
                        Level = result.Level,
                        Exp = result.Exp,
                        MaxMembers = result.MaxMembers,
                        InheritanceSlotsPerColumn = result.InheritanceSlotsPerColumn,
                        Effects = effects
                    });
                });
            }
        }

        private void ApplyExpChanged(ClanExpChanged changed, bool notify)
        {
            if (changed == null || CurrentClan == null || changed.ClanId != CurrentClan.Id) return;
            CurrentClan.Exp = changed.Exp;
            if (notify) NotifyMainThread(() => ExpChanged?.Invoke(changed));
        }

        private void ApplyLevelChanged(ClanLevelChanged changed, bool notify)
        {
            if (changed == null || CurrentClan == null || changed.ClanId != CurrentClan.Id) return;

            CurrentClan.Level = changed.Level;
            CurrentClan.Exp = changed.Exp;
            CurrentClan.MaxMembers = changed.MaxMembers;
            var effects = changed.Effects ?? Array.Empty<ClanInheritanceEffect>();
            CurrentClan.StartTurnEffects = effects
                .Where(x => x.Trigger == ClanInheritanceTrigger.StartTurn)
                .OrderBy(x => x.SlotIndex)
                .ToList();
            CurrentClan.EndTurnEffects = effects
                .Where(x => x.Trigger == ClanInheritanceTrigger.EndTurn)
                .OrderBy(x => x.SlotIndex)
                .ToList();

            if (notify) NotifyMainThread(() => LevelChanged?.Invoke(changed));
        }

        private void ReplaceShop(IEnumerable<ClanShopItem> items)
        {
            if (CurrentClan == null) return;
            CurrentClan.ShopItems = items?.ToList() ?? new List<ClanShopItem>();
            NotifyShopChanged();
        }

        private void ApplyShopItemAdded(ClanShopItem shopItem, bool notify = true)
        {
            if (shopItem == null || CurrentClan == null || shopItem.ClanId != CurrentClan.Id) return;

            CurrentClan.ShopItems ??= new List<ClanShopItem>();
            var index = CurrentClan.ShopItems.FindIndex(x => x.Id == shopItem.Id);
            if (index >= 0) CurrentClan.ShopItems[index] = shopItem;
            else CurrentClan.ShopItems.Add(shopItem);
            if (notify) NotifyShopChanged();
        }

        private void ApplyShopItemRemoved(ClanShopItemRemoved removed, bool notify = true)
        {
            if (removed == null || CurrentClan == null || removed.ClanId != CurrentClan.Id) return;
            CurrentClan.ShopItems?.RemoveAll(x => x.Id == removed.ShopItemId);
            if (notify) NotifyShopChanged();
        }

        private void ApplyShopItemChanged(ClanShopItem shopItem)
        {
            if (shopItem == null || CurrentClan == null || shopItem.ClanId != CurrentClan.Id) return;

            CurrentClan.ShopItems ??= new List<ClanShopItem>();
            var index = CurrentClan.ShopItems.FindIndex(x => x.Id == shopItem.Id);
            if (index >= 0) CurrentClan.ShopItems[index] = shopItem;
            else CurrentClan.ShopItems.Add(shopItem);
            NotifyShopChanged();
        }

        private void ApplyInventoryItem(ItemRef inventoryItem, ItemRef removedItem = null)
        {
            var inventory = client.CharacterData.InventoryManager;
            if (inventoryItem == null)
            {
                if (removedItem != null)
                {
                    if (removedItem.Type == ItemType.Furnace) inventory.RemoveFurnace(removedItem.Id);
                    else inventory.Remove(removedItem);
                }
            }
            else if (!inventory.Update(inventoryItem))
            {
                inventory.Add(inventoryItem);
            }

            client.CharacterData.InventoryChanged();
        }

        private void ApplyInventoryFurnace(FurnaceData furnace)
        {
            if (furnace == null) return;

            client.CharacterData.InventoryManager.Furnaces.Add(furnace);
            client.CharacterData.InventoryChanged();
        }

        private void NotifyShopChanged()
        {
            var items = (IReadOnlyList<ClanShopItem>)(CurrentClan?.ShopItems ?? new List<ClanShopItem>());
            NotifyMainThread(() => ShopChanged?.Invoke(items));
        }

        private static void NotifyMainThread(Action action)
        {
            var networkManager = NetworkManager.Instance;
            if (networkManager != null) networkManager.Invoke(action);
            else action();
        }

        public async Awaitable<ClanModel> ViewClan(int id)
        {
            var reader = await client.MemRequestWait(MessageTag.ViewClanRequest, body: id);
            return reader.MemRead<ClanModel>();
        }
        public void OnLeave()
        {
            CurrentClan = null;
            CurrentConfig = null;
            ClanChanged = null;
            MemberAdded = null;
            MemberRemoved = null;
            ContributionChanged = null;
            ShopChanged = null;
            MemberPointsChanged = null;
            MemberRoleChanged = null;
            JoinRequestAdded = null;
            JoinRequestRemoved = null;
            InheritanceChanged = null;
            FundsChanged = null;
            ExpChanged = null;
            LevelChanged = null;
            MapNodeChanged = null;
        }

        public void OnDisconnect(IProtocol<MessageTag> protocol)
        {
        }

        private static readonly MessageTag[] Handlers =
        {
            MessageTag.NewClan,
            MessageTag.ClanAddMember,
            MessageTag.ClanRemoveMember,
            MessageTag.ClanContribution,
            MessageTag.ClanMemberPointsChanged,
            MessageTag.ClanShopItemAdded,
            MessageTag.ClanShopItemRemoved,
            MessageTag.ClanShopItemChanged,
            MessageTag.ClanMemberRoleChanged,
            MessageTag.ClanJoinRequestAdded,
            MessageTag.ClanJoinRequestRemoved,
            MessageTag.ClanInheritanceChanged,
            MessageTag.ClanFundsChanged,
            MessageTag.ClanExpChanged,
            MessageTag.ClanLevelChanged,
            MessageTag.ClanMapNodeChanged
        };

        public MessageTag[] HandleTag => Handlers;

    }
}
