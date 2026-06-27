using LuyenDan;
using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using AllMaps.EventReward;
using LongDH.Script.ObjectPool;
using LongDH.LongDHModules;
using LongDHModules;
using Sirenix.OdinInspector;

public class EventRewardPanel : Popup , IUISingleton 
{
    
    [SerializeField] private Text txtDesc;
    [SerializeField] private Sprite spriteEvent;
    [SerializeField] private Sprite spriteWin;
    [SerializeField] private Sprite spriteLose;
    [SerializeField] private Image imgBGTilte;
    [SerializeField] private GameObject itemRewardPrefab;
    [SerializeField] private Sprite spriteMoney;
    [SerializeField] private Sprite spriteMoneyVip;
    [SerializeField] private RewardSnapshot rewardDataCurrent;
    [Button]
    public void InitShowEventReward(RewardSnapshot rewardData, string contentText)
    {
        InitShow(rewardData, contentText);
        imgBGTilte.sprite = spriteEvent;
        this.InitActionClose(null);
    }
    public void InitShowCombatReward(RewardSnapshot rewardData, string contentText,ETypeResultCombat resultCombat)
    {
        InitShow(rewardData, contentText);
        if(resultCombat == ETypeResultCombat.Win)
        {
            imgBGTilte.sprite = spriteWin;
            this.InitActionClose(() =>
            {
                Pool.Get<GameManager>().BackAllMapFromCombat(resultCombat);
            });
        }
        else
        {
            imgBGTilte.sprite = spriteLose;
            this.InitActionClose(() =>
            {
                Pool.Get<GameManager>().BackAllMapFromCombat(resultCombat);
            });

        }
       
    }
    public void InitShow(RewardSnapshot rewardData,string contentText)
    {
        Show();
        Transform child = this.transform.GetChild(1);
        this.rewardDataCurrent = rewardData;
        child.transform.GetChild(0).transform.GetChild(2).gameObject.SetActive(this.rewardDataCurrent != null);
        child.transform.GetChild(0).transform.GetChild(3).gameObject.SetActive(this.rewardDataCurrent != null);
        child.transform.GetChild(2).gameObject.SetActive(this.rewardDataCurrent != null);
        SetReward();
        txtDesc.text = contentText;
    }
    private void SetVisualRewardMoney()
    {
        var itemReward = GetComponentItemReward();

        itemReward.itemInventory.InitIcon(spriteMoney,null);
        itemReward.itemInventory.textName.text = "Linh Thạch";
        itemReward.textCount.text = rewardDataCurrent.Money.ToString();

    }
    private void SetVisualRewardMoneyVip()
    {
        var itemReward = GetComponentItemReward();

        itemReward.itemInventory.InitIcon(spriteMoneyVip, null);
        itemReward.itemInventory.textName.text = "Tiên Ngọc";
        itemReward.textCount.text = rewardDataCurrent.MoneyVip.ToString();

    }
   
    private void SetVisualRewardItem( ItemRef itemRef)
    {
        var itemReward = GetComponentItemReward();
        ItemTypeHandlerBagFactory.Create(itemRef.Type).Handle(itemReward.itemInventory, itemRef,false);
        itemReward.textCount.text = itemRef.Count.ToString();    
    }
    (ItemInventory itemInventory , Text textCount) GetComponentItemReward()
    {
        var itemReward = GameManager.GetItemPoolWithPrefabInParent(itemRewardPrefab.transform);
        itemReward.transform.localScale = Vector3.one;
        ItemInventory itemInventory = itemReward.transform.GetChild(0).GetComponent<ItemInventory>();
        Text textCountReward = itemReward.transform.GetChild(1).GetComponent<Text>();
        return (itemInventory, textCountReward);
    }
    private void SetReward()
    {
        for (int i = 0; i < itemRewardPrefab.transform.parent.childCount; i++)
        {
            itemRewardPrefab.transform.parent.GetChild(i).gameObject.SetActive(false);
        }

        if (this.rewardDataCurrent == null) return;
        if(rewardDataCurrent.Money > 0)
        {
            SetVisualRewardMoney();

        }
        if (rewardDataCurrent.MoneyVip > 0)
        {
            SetVisualRewardMoneyVip();

        }
       
        if (this.rewardDataCurrent?.ItemRefs != null) // Check nhanh cả Reward và ItemRefs
        {
            foreach (var item in this.rewardDataCurrent.ItemRefs)
            {
                SetVisualRewardItem(item);

            }
            
        }

    }

    public void Init()
    {
        Pool.Set(this);
    }
}
