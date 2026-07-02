using Base;
using LongDH.LongDHModules;
using LuyenDan;
using LuyenDan.Inventory;
using Network;
using Sirenix.OdinInspector;
using System.Collections.Generic;
using System.Linq;
using AllMaps;
using UnityEngine;
using UnityEngine.UI;
using AllMaps.ItemProvider;
using LongDHModules;

public class TaskDaily : BaseBehaviour
{
    [SerializeField] private ItemInventory itemRequire;
    [SerializeField] private Image imgGap;
    [SerializeField] private Text linhThachText;
    [SerializeField] private Text tienNgocText;
    [SerializeField] private Button btnConfirm;
    [SerializeField] private Image btnCantConfirm;
    [SerializeField] private Image imgDone;
    public DailyQuest DailyQuest { get; private set; }
    public void InitTask(DailyQuest quest)
    {
        DailyQuest = quest;
        var itemRef = quest.QuestReward.Target;
        var isDone = quest.IsRewardClaimed;
        var money = 0;
        var moneyVip = 0;
        var isGap = false;
        if (quest.QuestReward is QuestMoneyVipReward moneyVipReward)
        {
            money = moneyVipReward.Money;
            moneyVip = moneyVipReward.MoneyVip;
            isGap = true;
        }
        else if (quest.QuestReward is QuestMoneyReward moneyReward)
        {
            money = moneyReward.Money;
            moneyVip = 0;
            isGap = false;
        }
        imgDone.gameObject.SetActive(false);
        imgGap.gameObject.SetActive(isGap);
        var provider = ItemProviderFactory.GetProvider<IItemData>(itemRef.Type);
        var iItemData = provider.GetDataItemByIdRef(itemRef.Id);
        linhThachText.text = $"{money}";
        tienNgocText.text = $"{moneyVip}";
        Pool.Get<UIManager>().InitVisualItemInventory(itemRequire,iItemData,itemRef);
        
        var characterData = Client.Instance.CharacterData;
        var collection = characterData.InventoryManager.GetCollection(itemRef.Type) as HashSet<ItemRef>;
        if(!collection.TryGetValue(itemRef, out var itemUserHave))
        {
            itemRequire.InitCount(0);
        }
        else
        {
            itemRequire.InitCount(itemUserHave.Count);
        }
        itemRequire.InitCountRequire(itemRef.Count);
        bool isConfirm = itemRequire.count >= itemRequire.countRequire;
        btnConfirm.gameObject.SetActive(isConfirm);
        btnCantConfirm.gameObject.SetActive(!isConfirm);
        SetEventButton(btnConfirm, () =>
        {
            if(itemRequire.count >= itemRequire.countRequire)
            {
                DoneTask();
                Pool.Get<TaskDailyPanel>().DoneTask(this);
            }
        });
        if(isDone)
        {
            DoneTask();
        }
    }
    public void DoneTask()
    {
        imgDone.gameObject.SetActive(true);
        btnConfirm.gameObject.SetActive(false);
        btnCantConfirm.gameObject.SetActive(false);
    }
    public static Dictionary<Tier, int> PriceTienNgocVatLieuAndHerbTask = new Dictionary<Tier, int>
    {
        { Tier.NhatPham, 20 },
        { Tier.NhiPham, 30 },
        { Tier.TamPham, 40 },
        { Tier.TuPham, 50 },
        { Tier.NguPham, 60 },
        { Tier.LucPham, 70 },
    };

    private void OnDestroy()
    {
        
    }
}

