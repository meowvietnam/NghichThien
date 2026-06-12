using AllMaps.ItemProvider;
using Base;
using CreateCharacter;
using LongDH.LongDHModules;
using LuyenDan;
using LuyenDan.Inventory;
using LuyenKhi;
using Network;
using Network.Core;
using Sirenix.OdinInspector;
using System.Linq;
using LongDHModules;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public interface IUISingleton
{
    void Init();
}
public class UIManager : BaseBehaviour
{
    [SerializeField] public Sprite[] spriteQualityFulls ;
    [SerializeField] public Sprite[] spriteFrames;
    [SerializeField] public Color[] colorQualitys;
    [SerializeField] private Sprite spriteTTInven;
    [SerializeField] private Sprite spriteCPInven;
    [SerializeField] private Sprite spriteRecipeInven;
    [SerializeField] public GameObject loadScene;

    /// <summary>
    /// Abstract Factory để lấy equipment từ các collection khác nhau
    /// </summary>
    [Button]
    public void GetQualities()
    {
        var qualities = CoreConfig.Instance.Qualities;
        spriteQualityFulls = qualities.Select(x => x.Background).ToArray();
        colorQualitys = qualities.Select(x => x.Color).ToArray();
        spriteFrames = qualities.Select(x => x.IconUp).ToArray();
        EditorUtility.SetDirty(this);
    }
    public (Sprite,Sprite,Color) GetFrameAndQualityFullAndColorNameSkill(BaseSkill data)
    {
        if (data == null) return (default, default, default);
        int index = ((((int)data.eTierSkill - 1) / 3) * 2) + 1;
        return (spriteFrames[index], spriteQualityFulls[index], colorQualitys[index]);
    }
    public (Sprite, Sprite, Color) GetFrameAndQualityFullAndColorNameEquipment(EquipmentTier tierEquipment)
    {
        int index = (int)tierEquipment + 1;
        return (spriteFrames[index], spriteQualityFulls[index], colorQualitys[index]);
    }
    public void InitVisualItemSkill(ItemInventory item, BaseSkill data,ItemRef itemRef = null)
    {
        Sprite frame = null;
        if (data != null)
        {
            var inforVisualSkill = GetFrameAndQualityFullAndColorNameSkill(data);
            frame = inforVisualSkill.Item1;

            item.SetColorName(inforVisualSkill.Item3);
            item.SetQualityFull(inforVisualSkill.Item2);

        }

        item.Init(data, frame);
        item.InitInventory(itemRef, null);
        item.InitIcon(item.icon.sprite, item.frame.sprite);



    }
    public void InitVisualEquipment(ItemInventory item, BaseItemData data, ItemRef itemRef = null)
    {
        Sprite frame = null;
        CreateCharacter.Equipment equipment = null;
        if(itemRef != null)
        {
            var typeCurrent = itemRef.Type;

            // code cũ 
            //if (typeCurrent == ItemType.VuKhi)
            //    equipment = Client.Instance.CharacterData.InventoryManager.VuKhis.FirstOrDefault(...);
            //else if (typeCurrent == ItemType.PhongCu)
            //    equipment = Client.Instance.CharacterData.InventoryManager.PhongCus.FirstOrDefault(...);
            //else if (typeCurrent == ItemType.TrangSuc)
            //    equipment = Client.Instance.CharacterData.InventoryManager.TrangSucs.FirstOrDefault(...);
            //else if (typeCurrent == ItemType.BiKhi)  // ← PHẢI SỬA UIManager nếu muốn update
            //    equipment = Client.Instance.CharacterData.InventoryManager.BiKhis.FirstOrDefault(...);
            // ...
            // Sử dụng Abstract Factory thay vì if-else chain
            equipment = ItemProviderFactory.GetProvider<CreateCharacter.Equipment>(typeCurrent).GetDataItemById(itemRef.Id);

        }    

        if(equipment == null)
        {
            Debug.Log("equipment null");
        }    
        if(data == null && equipment != null)
        {
            data = new BaseItemData
            {
                nameItem = equipment.Name,
                spriteItem = equipment.Item.Icon,
            };
        }    
        
        if (data != null && equipment != null)
        {
            var inforVisualEquipment = GetFrameAndQualityFullAndColorNameEquipment(equipment.Item.Tier);

            frame = inforVisualEquipment.Item1;
            item.SetColorName(inforVisualEquipment.Item3);
            item.SetQualityFull(inforVisualEquipment.Item2);
        }
        item.Init(data, frame);
        item.InitInventory(itemRef, null);
        item.InitIcon(item.icon.sprite,item.frame.sprite);
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


        item.InitInventory(itemRef, data);



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
        // Đăng ký sự kiện mỗi khi load scene mới
        UnityEngine.SceneManagement.SceneManager.sceneLoaded += OnSceneLoaded;
    }
    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        loadScene.gameObject.SetActive(false);
      
    }

    private void Start()
    {
        var listPanel = transform.GetComponentsInChildren<PanelGroup>(true);
        var listSingleTon = transform.GetComponentsInChildren<IUISingleton>(true);
        foreach (var item in listPanel)
        {
            item.Init();
        }
        foreach (var item in listSingleTon)
        {
            item.Init();
        }
    }
    [SerializeField]public int activePopupCount = 0; // Biến đếm số lượng popup đang mở

    public void RegisterOpenPopup()
    {
        activePopupCount++;
        if (commonMask != null)
        {
            commonMask.gameObject.SetActive(true);
        }
    }

    public void RegisterClosePopup()
    {
        activePopupCount--;

        // Chỉ khi KHÔNG CÒN popup nào mở (count <= 0) thì mới được tắt mask
        if (activePopupCount <= 0)
        {
            activePopupCount = 0; // Đảm bảo không bị âm số
            if (commonMask != null)
            {
                commonMask.gameObject.SetActive(false);
            }
        }
    }
}
