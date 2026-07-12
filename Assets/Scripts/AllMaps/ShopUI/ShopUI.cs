using Base;
using CreateCharacter;
using LongDH.LongDHModules;
using LuyenDan;
using Network;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using AllMaps.ItemProvider;
using LongDHModules;

public enum EShopType
{
    ShopDuocLieu,
    ShopVatLieu,
    Skill,
}

public class ShopUI : Popup, IUISingleton, INetworkEvent.IBuyAndSell, INetworkEvent.ILoadShop, INetworkEvent.IReloadShop
{
    public EShopType eShopType;
    public EMonPhai eMonPhai => Pool.Get<GameManager>().CurrentEMonPhai;

    public void Init()
    {
        Pool.Set(this);
        gameObject.SetActive(false);
    }
    protected override void ActionBtnClose()
    {
        base.ActionBtnClose();
        Pool.Get<GameManager>().Cleanup();

    }
    public void ShowInitShop(EShopType eShopType)
    {
        this.eShopType = eShopType;
        Show();
        foreach (var item in listItemUserSell)
        {
            item.gameObject.SetActive(false);
        }

        foreach (var item in listItemUserBuy)
        {
            item.gameObject.SetActive(false);
        }

        var charData = Client.Instance.CharacterData;
        textNameUser.text = charData.Name;
        visualUserChar.SetGender(charData.Gender);
        visualUserChar.SetSkin(charData.Appearance.Skins);
        visualNpcChar.RandomSkin(Gender.Male, false);
        textMoneyUser.text = Client.Instance.CharacterData.Stats.LinhThach.ToString();
        SetTextMoneyCurrent();
        SetEventButton(btnConfirm, () =>
        {
            var listItemBuy = GetActiveTradeItemRefs(listItemUserBuy);
            var listItemSell = GetActiveTradeItemRefs(listItemUserSell);
            if (listItemBuy.Count > 0 || listItemSell.Count > 0)
            {
                Observer.Notify(EventList.Network.BUY_AND_SELL, this, listItemBuy, listItemSell, eShopType, eMonPhai);
            }
        });
        SetEventButton(btnWatchAds, ReloadShop);
        InitBagUser();
        Observer.Notify(EventList.Network.LOAD_SHOP, this, eShopType, eMonPhai);
    }

    private void ReloadShop()
    {
        var charData = Client.Instance.CharacterData;
        if (!charData.IsNoAds)
        {
            var popup = Pool.Get<PopupConfirmAction>();
            popup.ShowPopConfirmAction("Xác nhận xem quảng cáo để làm mới cửa hàng này!", ConfirmReloadShop);
        }
        else
        {
            ConfirmReloadShop();
        }
    }

    private void ConfirmReloadShop()
    {
        void Send()
        {
            Observer.Notify(EventList.Network.RELOAD_SHOP, this, eShopType, eMonPhai);

        }

        var charData = Client.Instance.CharacterData;
        if (charData.IsNoAds)
        {
            Send();
            return;
        }

        AdsService.Instance.ShowRewardVideoAds(
            "ReloadShop",
            "Xem quảng cáo để làm mới cửa hàng",
            Send,
            null,
            () => NotifyGroup.Instance.Show("Không thể tải quảng cáo")
        );
    }

    [SerializeField] private Button btnConfirm;
    [SerializeField] private Text textMoneyUser;
    [SerializeField] private Text textMoneyCurrent;
    [SerializeField] private Text textNameUser;
    [SerializeField] private CharacterGraphic visualUserChar;
    [SerializeField] private CharacterGraphic visualNpcChar;
    [SerializeField] private List<ItemInventory> listItemUserSell;
    [SerializeField] private List<ItemInventory> listItemUserBuy;
    [SerializeField] private Button btnWatchAds;
    [SerializeField] private Bag1ItemPrefabInventory bagUserList;
    [SerializeField] private Bag1ItemPrefabInventory bagNpcList;

    private void SetTextMoneyCurrent()
    {
        foreach (var item in listItemUserBuy.Where(IsActiveTradeItem))
        {
            item.itemRef.SetCount(item.count);
        }

        foreach (var item in listItemUserSell.Where(IsActiveTradeItem))
        {
            item.itemRef.SetCount(item.count);
        }

        var buyItemsAmount = listItemUserBuy.Where(IsActiveTradeItem).Select(x =>
        {
            var provider = ItemProviderFactory.GetProvider(x.itemRef.Type);
            return new
            {
                x.itemRef.Id,
                x.count,
                Amount = x.count * provider.GetPriceItemBuy(x.itemRef.Id)
            };
        }).ToArray();
        var sellItemsAmount = listItemUserSell.Where(IsActiveTradeItem).Select(x =>
        {
            var provider = ItemProviderFactory.GetProvider(x.itemRef.Type);
            return new
            {
                x.itemRef.Id,
                x.count,
                Amount = x.count * provider.GetPriceItemSell(x.itemRef.Id)
            };
        }).ToArray();
        Debug.Log(
            $"SellItems: {string.Join(",\n", sellItemsAmount.Select(x => $"Id: {x.Id},Count: {x.count}, Amount: {x.Amount}"))}");
        Debug.Log(
            $"BuyItems: {string.Join(",\n", buyItemsAmount.Select(x => $"Id: {x.Id},Count: {x.count}, Amount: {x.Amount}"))}");
        var totalMoney = sellItemsAmount.Sum(x => x.Amount) - buyItemsAmount.Sum(x => x.Amount);
        if (totalMoney >= 0)
        {
            textMoneyCurrent.text = $"+{totalMoney}";
        }
        else
        {
            textMoneyCurrent.text = $"{totalMoney}";
        }
    }

    private void InitBagUser()
    {
        var characterData = Client.Instance.CharacterData;
        var gameConfig = Pool.Get<GameManager>();
        List<ItemRef> itemDatas = new List<ItemRef>();
        itemDatas.AddRange(characterData.InventoryManager.Herbs.ToList());
        itemDatas.AddRange(characterData.InventoryManager.Recipes.ToList());
        itemDatas.AddRange(characterData.InventoryManager.ThanThongs.ToList());
        itemDatas.AddRange(characterData.InventoryManager.CongPhaps.ToList());
        // itemDatas.AddRange(characterData.InventoryManager.VuKhis.ToList());
        // itemDatas.AddRange(characterData.InventoryManager.PhongCus.ToList());
        // itemDatas.AddRange(characterData.InventoryManager.TrangSucs.ToList());
        itemDatas.AddRange(characterData.InventoryManager.LinhDans.ToList());
        // Sử dụng LINQ để trích xuất Ref từ mỗi phần tử trong furnaceDatas
        var furnaceRefs = characterData.InventoryManager.Furnaces.Select(f => f.CreateRefSelfID()).ToList();

        // Thêm danh sách Ref vừa lấy được vào itemDatas
        itemDatas.AddRange(furnaceRefs);
        InitBagLogic(bagUserList, listItemUserSell, itemDatas);
    }

    private void InitBagLogic(Bag1ItemPrefabInventory bagList, List<ItemInventory> listItem, List<ItemRef> itemDatas)
    {
        bagList.InitDataItem(itemDatas, (ItemInventory) =>
        {
            ItemInventory.InitAction(null, null, () =>
            {
                foreach (var slotItemGift in listItem)
                {
                    if (!IsSameItem(slotItemGift.itemRef, ItemInventory.itemRef))
                    {
                        if (!slotItemGift.gameObject.activeSelf)
                        {
                            slotItemGift.gameObject.SetActive(true);
                            var tradeItemRef = CreateItemRef(ItemInventory.itemRef, 0);
                            if (ItemInventory.itemRef.Type == ItemType.ThanThong ||
                                ItemInventory.itemRef.Type == ItemType.CongPhap)
                            {
                                Pool.Get<UIManager>()
                                    .InitVisualItemSkill(slotItemGift, (BaseSkill)ItemInventory.itemData, tradeItemRef);
                            }

                            Pool.Get<UIManager>().InitVisualItemInventory(slotItemGift, ItemInventory.iItemData,
                                tradeItemRef);
                            AddCountItemToItem(ItemInventory, slotItemGift);
                            slotItemGift.InitAction(null, null,
                                () => { AddCountItemToItem(slotItemGift, ItemInventory); });
                            SetTextMoneyCurrent();
                            break;
                        }
                    }
                    else
                    {
                        if (slotItemGift.gameObject.activeSelf)
                        {
                            if (ItemInventory.count > 0)
                            {
                                AddCountItemToItem(ItemInventory, slotItemGift);
                            }

                            break;
                        }
                    }
                }
                //foreach (var item in listItemSlotThanThongs)
                //{
                //    if (item.itemData != null)
                //    {
                //        DisableHighlight(item);
                //    }
                //}
            });
        }, false, true);
        bagList.InitBag(false);
    }

    private bool IsActiveTradeItem(ItemInventory item)
    {
        return item.gameObject.activeSelf
               && item.itemRef != null
               && item.itemRef.Id != 0
               && item.count > 0;
    }

    private List<ItemRef> GetActiveTradeItemRefs(List<ItemInventory> tradeItems)
    {
        return tradeItems
            .Where(IsActiveTradeItem)
            .Select(x => CreateItemRef(x.itemRef, x.count))
            .ToList();
    }

    private ItemRef CreateItemRef(ItemRef source, int count)
    {
        return new ItemRef
        {
            Id = source.Id,
            Type = source.Type,
            Count = (ushort)count,
            TempId = source.TempId
        };
    }

    private void AddCountItemToItem(ItemInventory itemStart, ItemInventory itemEnd, int count = 1)
    {
        if (itemStart.count > 0)
        {
            itemEnd.AddCount(count);
            itemEnd.InitCount(itemEnd.count);
            if (itemEnd.count > 0 && !itemEnd.gameObject.activeSelf)
            {
                itemEnd.gameObject.SetActive(true);
            }

            itemStart.AddCount(-count);
            itemStart.InitCount(itemStart.count);
            if (itemStart.count <= 0)
            {
                itemStart.gameObject.SetActive(false);
            }

            SetTextMoneyCurrent();
        }
    }

    private void InitBagNPC(List<ItemRef> itemDatas)
    {
        InitBagLogic(bagNpcList, listItemUserBuy, itemDatas);
    }

    private void ReturnItemsToBag(List<ItemInventory> tradeItems, Bag1ItemPrefabInventory bag)
    {
        foreach (var tradeItem in tradeItems)
        {
            if (!tradeItem.gameObject.activeSelf || tradeItem.itemRef == null || tradeItem.count <= 0)
            {
                continue;
            }

            var bagItem = bag.InventoryUIFacade.listItemUI.FirstOrDefault(x => IsSameItem(x.itemRef, tradeItem.itemRef));
            if (bagItem != null)
            {
                AddCountItemToItem(tradeItem, bagItem, tradeItem.count);
            }

            tradeItem.gameObject.SetActive(false);
        }
    }

    private bool IsSameItem(ItemRef a, ItemRef b)
    {
        return a != null
               && b != null
               && a.Type == b.Type
               && a.Id == b.Id
               && a.TempId == b.TempId;
    }

    private void ResetTradeItems()
    {
        ReturnItemsToBag(listItemUserSell, bagUserList);
        ReturnItemsToBag(listItemUserBuy, bagNpcList);
        textMoneyCurrent.text = "0";
    }

    private void HideTradeItems()
    {
        foreach (var item in listItemUserSell)
        {
            item.gameObject.SetActive(false);
        }

        foreach (var item in listItemUserBuy)
        {
            item.gameObject.SetActive(false);
        }
    }

    public void SuccessBuyAndSell(string message)
    {
        Pool.Get<GameManager>().Cleanup();
        HideTradeItems();
        InitBagUser();
        InitBagNPC(Client.Instance.CharacterData.ShopManager.GetShop(eShopType).Items.ToList());
        NotifyGroup.Instance.Show(message);
        textMoneyCurrent.text = "0";
        textMoneyUser.text = Client.Instance.CharacterData.Stats.LinhThach.ToString();
    }

    public void FailBuyAndSell(string message)
    {
        NotifyGroup.Instance.Show(message);
    }

    public void SuccessLoadShop(List<ItemRef> itemRefs)
    {
        InitBagNPC(itemRefs);
    }

    public void FailLoadShop(string message)
    {
        Hide();
    }

    public void SuccessReloadShop(List<ItemRef> itemRefs)
    {
        Pool.Get<GameManager>().Cleanup();
        ResetTradeItems();
        InitBagNPC(itemRefs);
    }

    public void FailReLoadShop(string message)
    {
        Hide();
    }
}
