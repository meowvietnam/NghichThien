using Base;
using CreateCharacter;
using LongDH.LongDHModules;
using LuyenDan;
using Network;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
public enum EShopType
{
    ShopDuocLieu,
    ShopVatLieu,
    Skill,
}
public class ShopUI : Popup, ISingleton , INetworkEvent.IBuyAndSell , INetworkEvent.ILoadShop, INetworkEvent.IReLoadShop
{
    public EShopType eShopType;
    public EMonPhai eMonPhai = EMonPhai.None;

    public void Init()
    {
        Pool.Set(this);
        gameObject.SetActive(false);
        
    }
    public override void Show(float delay = 0.5F, Action actionComplete = null)
    {
        base.Show(delay, actionComplete);
        foreach (var item in listItemUserSell)
        {
            item.gameObject.SetActive(false);
        }
        foreach (var item in listItemUserBuy)
        {
            item.gameObject.SetActive(false);

        }
        textNameUser.text = Client.Instance.CharacterData.Name;
        visualUserChar.SetGender(Client.Instance.CharacterData.Gender);
        visualUserChar.SetSkin(Client.Instance.CharacterData.Skins);
        visualNpcChar.RandomSkin(Base.Gender.Male,false);
        textMoneyUser.text = Client.Instance.CharacterData.Stats.LinhThach.ToString();
        SetTextMoneyCurrent();
        SetEventButton(btnConfirm, () =>
        {
            if(listItemUserBuy.Any(x=>x.gameObject.activeSelf) || listItemUserSell.Any(x => x.gameObject.activeSelf))
            {
                // Lấy ra danh sách các itemRef (có thể chứa các phần tử trùng nhau)
                foreach (var item in listItemUserBuy)
                {
                    if (!item.gameObject.activeSelf) break;
                    item.itemRef.SetCount(item.count);
                }
                foreach (var item in listItemUserSell)
                {
                    if (!item.gameObject.activeSelf) break;
                    item.itemRef.SetCount(item.count);

                }
                List<ItemRef> listItemBuy = listItemUserBuy.Select(x => x.itemRef).ToList();
                List<ItemRef> listItemSell = listItemUserSell.Select(x => x.itemRef).ToList();
                Observer.Notify(EventList.Network.BUY_AND_SELL,this, listItemBuy, listItemSell , eShopType , eMonPhai);
            }
           
        });
        SetEventButton(btnWatchAds, () =>
        {
            Pool.Get<PopupConfirmAction>().ShowPopConfirmAction("Xác nhận xem quảng cáo để làm mới cửa hàng này!", () =>
            {
                Observer.Notify(EventList.Network.RE_LOAD_SHOP, this, eShopType, eMonPhai);

            });
        });
        InitBagUser();
        Observer.Notify(EventList.Network.LOAD_SHOP,this,eShopType, eMonPhai);

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
    [SerializeField]
    private Bag1ItemPrefabInventory bagUserList;
    [SerializeField]
    private Bag1ItemPrefabInventory bagNpcList;
   private void SetTextMoneyCurrent()
    {
        int totalMoney = 0;
        foreach (ItemInventory item in listItemUserSell)
        {
            if(!item.gameObject.activeSelf)
            {
                break;
            }
            totalMoney += GameConfig.GetPriceItemSell(item.itemRef,item.iItemData) * item.count;
        }
        foreach (ItemInventory item in listItemUserBuy)
        {
            if (!item.gameObject.activeSelf)
            {
                break;
            }
            totalMoney -= GameConfig.GetPriceItemBuy(item.itemRef, item.iItemData) * item.count;
        }
        if(totalMoney >= 0)
        {
            textMoneyCurrent.text = $"+{totalMoney}";
        }    
        else
        {
            textMoneyCurrent.text = $"-{totalMoney}";

        }
    }   
    private void InitBagUser()
    {
        var characterData = Client.Instance.CharacterData;
        var gameConfig = Pool.Get<GameConfig>();
        List<ItemRef> itemDatas = new List<ItemRef>();
        itemDatas.AddRange(characterData.InventoryManager.Herbs.ToList());
        itemDatas.AddRange(characterData.InventoryManager.Recipes.ToList());
        itemDatas.AddRange(characterData.InventoryManager.ThanThongs.ToList());
        itemDatas.AddRange(characterData.InventoryManager.CongPhaps.ToList());
        itemDatas.AddRange(characterData.InventoryManager.VuKhis.ToList());
        itemDatas.AddRange(characterData.InventoryManager.PhongCus.ToList());
        itemDatas.AddRange(characterData.InventoryManager.TrangSucs.ToList());
        itemDatas.AddRange(characterData.InventoryManager.LinhDans.ToList());
        // Sử dụng LINQ để trích xuất Ref từ mỗi phần tử trong furnaceDatas
        var furnaceRefs = characterData.InventoryManager.Furnaces.Select(f => f.Ref).ToList();

        // Thêm danh sách Ref vừa lấy được vào itemDatas
        itemDatas.AddRange(furnaceRefs);
        InitBagLogic(bagUserList,listItemUserSell,itemDatas);


    }
    private void InitBagLogic(Bag1ItemPrefabInventory bagList , List<ItemInventory> listItem , List<ItemRef> itemDatas)
    {
        bagList.InitDataItem(itemDatas, (ItemInventory) =>
        {
            ItemInventory.InitAction(null, null, () =>
            {


                foreach (var slotItemGift in listItem)
                {
                    if (slotItemGift.itemRef.Id != ItemInventory.itemRef.Id)
                    {
                        if (!slotItemGift.gameObject.activeSelf)
                        {
                            slotItemGift.gameObject.SetActive(true);
                            if (ItemInventory.itemRef.Type == ItemType.ThanThong || ItemInventory.itemRef.Type == ItemType.CongPhap)
                            {
                                Pool.Get<UIManager>().InitVisualItemSkill(slotItemGift, (BaseSkill)ItemInventory.itemData);


                            }
                            Pool.Get<UIManager>().InitVisualItemInventory(slotItemGift, ItemInventory.iItemData, ItemInventory.itemRef);
                            AddCountItemToItem(ItemInventory, slotItemGift);
                            slotItemGift.InitCount(1);
                            slotItemGift.InitAction(null, null, () =>
                            {
                                AddCountItemToItem(slotItemGift, ItemInventory);


                            });
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
        });
        bagUserList.InitBag(false);
    }
    private void AddCountItemToItem(ItemInventory itemStart,ItemInventory itemEnd , int count = 1)
    {

        if (itemStart.count > 0)
        {
            itemEnd.AddCount(count);
            if (itemEnd.count > 0 && !itemEnd.gameObject.activeSelf)
            {
                itemEnd.gameObject.SetActive(true);
            }
            itemStart.AddCount(-count);
            if (itemStart.count <= 0)
            {
                itemStart.gameObject.SetActive(false);
            }
            SetTextMoneyCurrent();

        }
    }
    private void InitBagNPC(List<ItemRef> itemDatas)
    {
        var characterData = Client.Instance.CharacterData;
        var gameConfig = Pool.Get<GameConfig>();

        InitBagLogic(bagNpcList, listItemUserBuy, itemDatas);

    }

    public void SuccessBuyAndSell()
    {
        throw new NotImplementedException();
    }

    public void FailBuyAndSell(string message)
    {
        throw new NotImplementedException();
    }
    public void SuccessLoadShop(List<ItemRef> itemRefs)
    {
        InitBagNPC(itemRefs);

    }

    public void FailLoadShop(string message)
    {
        Hide();
    }
    public void SuccessReLoadShop(List<ItemRef> itemRefs)
    {
        InitBagNPC(itemRefs);

    }

    public void FailReLoadShop(string message)
    {
        Hide();
    }
}
