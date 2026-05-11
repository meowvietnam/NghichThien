using Base;
using LongDH.LongDHModules;
using LuyenDan;
using LuyenDan.Inventory;
using Network;
using Sirenix.OdinInspector;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using CreateCharacter;

public class TaskDaily : BaseBehaviour
{
    [SerializeField] private ItemInventory itemRequire;
    [SerializeField] private Image imgGap;
    [SerializeField] private Text linhThachText;
    [SerializeField] private Text tienNgocText;
    [SerializeField] private Button btnConfirm;
    [SerializeField] private Image btnCantConfirm;
    [SerializeField] private Image imgDone;
    [SerializeField] public TaskDailySnapshot taskDailySnapshot;

    [Button]
    public void InitTask(TaskDailySnapshot taskDailySnapshot)
    {
        this.taskDailySnapshot = taskDailySnapshot;
        var itemRef = taskDailySnapshot.ItemRef;
        var iItemData = taskDailySnapshot.itemSnapShot.IItemData;
        var isGap = taskDailySnapshot.IsGap;
        imgDone.gameObject.SetActive(false);
        imgGap.gameObject.SetActive(isGap);
        linhThachText.text = ItemProviderFactory.GetDisplay(itemRef.Type).GetPriceItemBuy(taskDailySnapshot.itemSnapShot).ToString();
        if (isGap )
        {
            tienNgocText.text = PriceTienNgocVatLieuAndHerbTask.GetValueOrDefault(iItemData.Tier).ToString();

        }
        else
        {
            tienNgocText.text = "0";
        }
        Pool.Get<UIManager>().InitVisualItemInventory(itemRequire,iItemData,itemRef);
        var characterData = Client.Instance.CharacterData;
        List<ItemRef> itemDatas = new List<ItemRef>();
        itemDatas.AddRange(characterData.InventoryManager.Herbs.ToList());
        itemDatas.AddRange(characterData.InventoryManager.Recipes.ToList());
        itemDatas.AddRange(characterData.InventoryManager.ThanThongs.ToList());
        itemDatas.AddRange(characterData.InventoryManager.CongPhaps.ToList());
        // itemDatas.AddRange(characterData.InventoryManager.VuKhis.ToList());
        // itemDatas.AddRange(characterData.InventoryManager.PhongCus.ToList());
        // itemDatas.AddRange(characterData.InventoryManager.TrangSucs.ToList());
        itemDatas.AddRange(characterData.InventoryManager.LinhDans.ToList());

        var itemUserHave = itemDatas.FirstOrDefault(x => x == itemRef);

        if(itemUserHave == null)
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
        if(taskDailySnapshot.IsDone)
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

}

