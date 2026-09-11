using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LuyenDan;
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

        public event Action<ClanModel> ClanChanged;
        public event Action<ClanMember> MemberAdded;
        public event Action<ClanRemoveMember> MemberRemoved;
        public event Action<ClanContribution> ContributionChanged;
        public event Action<IReadOnlyList<ClanShopItem>> ShopChanged;
        public event Action<ClanMemberPointsChanged> MemberPointsChanged;

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
                ApplyMemberPoints(new ClanMemberPointsChanged
                {
                    ClanId = CurrentClan != null ? CurrentClan.Id : 0,
                    CharId = client.CharacterData.Id,
                    ContribPoints = result.ContribPoints
                }, false);
            }

            return result;
        }

        public bool IsShopItemOwner(ClanShopItem shopItem)
        {
            return shopItem != null && shopItem.OwnerCharId == client.CharacterData.Id;
        }

        public async Task<AddClanShopItemResult> AddShopItem(ItemRef item, long price)
        {
            if (item == null) throw new ArgumentNullException(nameof(item));

            var result = await Request<AddClanShopItemRequest, AddClanShopItemResult>(
                MessageTag.AddClanShopItemRequest,
                new AddClanShopItemRequest { Item = item, Price = price });

            if (result.Success)
            {
                MaxShopItemsAddedPerDay = result.MaxItemsAddedPerDay;
                ShopItemsAddedToday = result.ShopItemsAddedToday;
                ApplyInventoryItem(result.InventoryItem, item);
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
            }

            CurrentClan = clan;
            NotifyMainThread(() => ClanChanged?.Invoke(clan));
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
            if (member != null) member.ContribPoints = contribution.MemberContribPoints;
            if (notify) NotifyMainThread(() => ContributionChanged?.Invoke(contribution));
        }

        private void ApplyMemberPoints(ClanMemberPointsChanged changed, bool notify)
        {
            if (changed == null || CurrentClan == null || changed.ClanId != CurrentClan.Id) return;

            var member = CurrentClan.Members?.FirstOrDefault(x => x.CharId == changed.CharId);
            if (member != null) member.ContribPoints = changed.ContribPoints;
            if (notify) NotifyMainThread(() => MemberPointsChanged?.Invoke(changed));
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
                if (removedItem != null) inventory.Remove(removedItem);
            }
            else if (!inventory.Update(inventoryItem))
            {
                inventory.Add(inventoryItem);
            }

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
            MessageTag.ClanShopItemChanged
        };

        public MessageTag[] HandleTag => Handlers;
    }
}
