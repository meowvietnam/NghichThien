using Base;
using CreateCharacter;
using LongDH.LongDHModules;
using LuyenDan;
using LuyenDan.Inventory;
using Network;
using Sirenix.OdinInspector;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
public interface ISingleton
{
    void Init();
}
public class UIManager : BaseBehaviour
{
    [SerializeField] public Sprite[] spriteQualityFulls;
    [SerializeField] public Sprite[] spriteFrames;
    [SerializeField] public Color[] colorQualitys;
    [SerializeField] private Sprite spriteTTInven;
    [SerializeField] private Sprite spriteCPInven;
    [SerializeField] private Sprite spriteRecipeInven;

    public void InitVisualItemSkill(ItemInventory item, BaseSkill data,ItemRef itemRef = null, ETypeInventoryItem eTypeInventoryItem = ETypeInventoryItem.Skill)
    {
        Sprite frame = null;
        if (data != null)
        {
            frame = spriteFrames[((((int)data.eTierSkill - 1) / 3) * 2) + 1];

            item.SetColorName(colorQualitys[((((int)data.eTierSkill - 1) / 3) * 2) + 1]);
            item.SetQualityFull(spriteQualityFulls[((((int)data.eTierSkill - 1) / 3) * 2) + 1]);

        }

        item.Init(data, frame);

        item.InitInventory(eTypeInventoryItem, itemRef, null);


    }
    public void InitVisualEquipment(ItemInventory item, BaseItemData data, ItemRef itemRef = null, ETypeInventoryItem eTypeInventoryItem = ETypeInventoryItem.Skill)
    {
        Sprite frame = null;
        Equipment equipment = null;
        var typeCurrent = itemRef.Type;
        if (typeCurrent == ItemType.VuKhi)
        {
            equipment = Client.Instance.CharacterData.InventoryManager.VuKhis.FirstOrDefault(x => x.SelfId == itemRef.Id);
           

        }
        else if (typeCurrent == ItemType.PhongCu)
        {
            equipment = Client.Instance.CharacterData.InventoryManager.PhongCus.FirstOrDefault(x => x.SelfId == itemRef.Id);
        }
        else if (typeCurrent == ItemType.TrangSuc)
        {
            equipment = Client.Instance.CharacterData.InventoryManager.TrangSucs.FirstOrDefault(x => x.SelfId == itemRef.Id);
        }
      
        if(data == null)
        {
            data = new BaseItemData
            {
                nameItem = equipment.Name,
                spriteItem = equipment.Item.Icon,
                


            };
        }    
        if (data != null)
        {
            frame = spriteFrames[(int)equipment.Item.Tier + 1];
            item.SetColorName(colorQualitys[(int)equipment.Item.Tier + 1]);
            item.SetQualityFull(spriteQualityFulls[(int)equipment.Item.Tier + 1]);

        }

        item.Init(data, frame);
        item.InitInventory(eTypeInventoryItem, itemRef, null);


    }
    public void InitVisualItemInventory(ItemInventory item, IItemData data, ItemRef itemRef , bool isIconSkillBase = false)
    {
        Sprite frame = null;
        if (data != null)
        {
            var quality = CoreConfig.Instance.FindQuality(data.Tier);
            frame = quality.IconUp;

            item.SetColorName(quality.Color);
            item.SetQualityFull(quality.Background);
            item.Init(new BaseItemData { nameItem = data.Name, spriteItem = data.Icon }, frame);
        }
        if (itemRef.Type == ItemType.Recipe)
        {
            item.InitIcon(spriteRecipeInven, frame);
        }
        if (itemRef != null && !isIconSkillBase)
        {
            item.InitCount(itemRef.Count);
            if (itemRef.Type == ItemType.ThanThong)
            {
                item.InitIcon(spriteTTInven, frame);
            }
            if (itemRef.Type == ItemType.CongPhap)
            {
                item.InitIcon(spriteCPInven, frame);
            }
           
        }


        item.InitInventory(ETypeInventoryItem.Item, itemRef, data);



    }

    public Image commonMask ;
    public GameObject panelGroup;
    private void Awake()
    {
        // Kiểm tra nếu trong Pool đã có một Manager loại này rồi
        var existing = Pool.Get<UIManager>();

        if (existing != null && existing != this)
        {
            // Nếu đã có bản cũ (từ Scene trước), xóa bản mới này đi
            Destroy(gameObject);
            return;
        }

        // Nếu chưa có, mình là "trùm", đăng ký vào Pool và giữ lại
        Pool.Set(this);
        DontDestroyOnLoad(gameObject);
    }
    private void Start()
    {
        var listPanel = transform.GetComponentsInChildren<PanelGroup>(true);
        var listSingleTon = transform.GetComponentsInChildren<ISingleton>(true);
        foreach (var item in listPanel)
        {
            item.Init();
        }
        foreach (var item in listSingleTon)
        {
            item.Init();
        }
    }
    [Button]
    private void ShowShop()
    {
    }    
}
