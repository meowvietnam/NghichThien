using LuyenDan;
using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using LongDH.Script.ObjectPool;
using LongDH.LongDHModules;

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
    public void InitShowEventReward(RewardSnapshot rewardData, string contentText)
    {
        InitShow(rewardData, contentText);
        imgBGTilte.sprite = spriteEvent;
       
    }
    public void InitShowCombatReward(RewardSnapshot rewardData, string contentText)
    {
        InitShow(rewardData, contentText);
        if(rewardData != null)
        {
            imgBGTilte.sprite = spriteWin;
            this.InitActionClose(() =>
            {
                Pool.Get<GameManager>().BackAllMapFromCombat(GameManager.ECombatResult.Win);
            });
        }
        else
        {
            imgBGTilte.sprite = spriteLose;
            this.InitActionClose(() =>
            {
                Pool.Get<GameManager>().BackAllMapFromCombat(GameManager.ECombatResult.Lose);
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
    private void SetVisualRewardMoney(ItemInventory itemInventory , Text textCount)
    {
        itemInventory.InitIcon(spriteMoney,null);
        itemInventory.textName.text = "Linh Thạch";
        textCount.text = rewardDataCurrent.Money.ToString();

    }
    private void SetVisualRewardMoneyVip(ItemInventory itemInventory, Text textCount)
    {
        itemInventory.InitIcon(spriteMoneyVip, null);
        itemInventory.textName.text = "Tiên Ngọc";
        textCount.text = rewardDataCurrent.MoneyVip.ToString();

    }
   
    private void SetVisualRewardItem(ItemInventory itemInventory, Text textCount, ItemRef itemRef)
    {
        ItemTypeHandlerBagFactory.Create(itemRef.Type).Handle(itemInventory,itemRef,false);
        textCount.text = itemRef.Count.ToString();    
    }
    private void SetReward()
    {
        if (this.rewardDataCurrent == null) return;
        var itemReward = itemRewardPrefab.Spawn(itemRewardPrefab.transform.parent);
        itemReward.transform.localScale = Vector3.one;
        ItemInventory itemInventory = itemReward.transform.GetChild(0).GetComponent<ItemInventory>();
        Text textCountReward = itemReward.transform.GetChild(1).GetComponent<Text>();
        if(rewardDataCurrent.Money > 0)
        {
            SetVisualRewardMoney(itemInventory, textCountReward);

        }
        if (rewardDataCurrent.MoneyVip > 0)
        {
            SetVisualRewardMoneyVip(itemInventory, textCountReward);

        }
        foreach (var item in this.rewardDataCurrent.ItemRefs)
        {
            SetVisualRewardItem(itemInventory, textCountReward , item);

        }


    }

    public void Init()
    {
        Pool.Set(this);
    }
}
