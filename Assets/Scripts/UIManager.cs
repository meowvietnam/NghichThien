using System;
using AllMaps.ItemProvider;
using Base;
using LongDH.LongDHModules;
using LuyenDan;
using LuyenDan.Inventory;
using LuyenKhi;
using Network;
using Network.Core;
using Sirenix.OdinInspector;
using System.Linq;
using AllMaps.ui_rank;
using LongDHModules;
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Skill;
using IAP;
using DG.Tweening;

public interface IUISingleton
{
    void Init();
}

public class UIManager : BaseBehaviour
{
    [SerializeField] public Sprite[] spriteQualityFulls;
    [SerializeField] public Sprite[] spriteFrames;
    [SerializeField] public Color[] colorQualitys;
    [SerializeField] public Sprite spriteTTInven;
    [SerializeField] public Sprite spriteCPInven;
    [SerializeField] private Sprite spriteRecipeInven;
    [SerializeField] public GameObject loadScene;
    [SerializeField] public Button btnBXH;
    [SerializeField] public Button btnThuongHoi;
    [SerializeField] public Button btnGacha;
    [SerializeField] private Button convertBtn;
    [SerializeField] public Button btnListIconShow;
    [SerializeField] private bool isShowListIcon = true;
    [SerializeField] private RectTransform[] listIcons;
    private Sequence listIconTween;
    public void SetActiveListIcon(bool active)
    {
        foreach (var listIcon in listIcons)
        {
            listIcon.gameObject.SetActive(active);
        }
    }
    /// <summary>
    /// Abstract Factory để lấy equipment từ các collection khác nhau
    /// </summary>
#if UNITY_EDITOR
    [Button]
    public void GetQualities()
    {
        var qualities = CoreConfig.Instance.Qualities;
        spriteQualityFulls = qualities.Select(x => x.Background).ToArray();
        colorQualitys = qualities.Select(x => x.Color).ToArray();
        spriteFrames = qualities.Select(x => x.IconUp).ToArray();
        EditorUtility.SetDirty(this);
    }
#endif
    public (Sprite, Sprite, Color) GetFrameAndQualityFullAndColorNameSkill(BaseSkill data)
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

    public void InitVisualItemSkill(ItemInventory item, BaseSkill data, ItemRef itemRef = null)
    {
        Sprite frame = null;
        if (data != null)
        {
            var inforVisualSkill = GetFrameAndQualityFullAndColorNameSkill(data);
            frame = inforVisualSkill.Item1;

            item.SetColorName(inforVisualSkill.Item3);
            item.SetQualityFull(inforVisualSkill.Item2);
            if (itemRef == null || itemRef.Type != ItemType.ThanThong && itemRef.Type != ItemType.CongPhap)
            {
                if (data is ThanThong thanThong)
                {
                    itemRef = new ItemRef
                    {
                        Id = (ushort)data.Id,
                        Type = ItemType.ThanThong,
                        Count = 1,
                    };
                }
                else if (data is CongPhap congPhap)
                {
                    itemRef = new ItemRef
                    {
                        Id = (ushort)data.Id,
                        Type = ItemType.CongPhap,
                        Count = 1,
                    };
                }
            }
        }


        item.Init(data, frame);
        item.InitInventory(itemRef, null);
        item.InitIcon(item.icon.sprite, item.frame.sprite);
    }

    public void InitVisualEquipment(ItemInventory item, BaseItemData data, ItemRef itemRef = null)
    {
        Sprite frame = null;
        CreateCharacter.Equipment equipment = null;
        if (itemRef != null)
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
            equipment = ItemProviderFactory.GetProvider<CreateCharacter.Equipment>(typeCurrent)
                .GetDataItemByIdRef(itemRef.Id);
        }

        if (equipment == null)
        {
            Debug.Log("equipment null");
        }

        if (data == null && equipment != null)
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
        item.InitIcon(item.icon.sprite, item.frame.sprite);
    }

    public void InitVisualItemInventory(ItemInventory item, IItemData data, ItemRef itemRef,
        bool isIconSkillBase = false, EActionShowItemInfor eActionShowItemInfor = EActionShowItemInfor.None)
    {
        Sprite frame = null;
        BaseItemData baseItemData = null;
        if (data != null)
        {
            var quality = CoreConfig.Instance.FindQuality(data.Tier);
            frame = quality.IconUp;

            item.SetColorName(quality.Color);
            item.SetQualityFull(quality.Background);
            baseItemData = new BaseItemData { nameItem = data.Name, spriteItem = data.Icon };
            item.Init(baseItemData, frame);
        }

        if (itemRef != null && itemRef.Type == ItemType.Recipe)
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

        if (isIconSkillBase)
        {
            item.icon.rectTransform.sizeDelta = new Vector2(128, 128);
        }


        item.InitInventory(itemRef, data, eActionShowItemInfor);
    }

    public Image commonMask;
    public GameObject panelGroup;

    private Vector2 GetButtonListPos(RectTransform buttonRectTf)
    {
        Canvas.ForceUpdateCanvases();
        float maxWidth = 0;
        if (isShowListIcon)
        {
            foreach (var listRectTf in listIcons)
            {
                LayoutRebuilder.ForceRebuildLayoutImmediate(listRectTf);
                var width = listRectTf.rect.width;
                if (width > maxWidth)
                {
                    maxWidth = width;
                }
            }
        }
        var anchorPos = buttonRectTf.anchoredPosition;
        var posX = -(maxWidth + buttonRectTf.rect.width);
        anchorPos.x = posX;
        return anchorPos;
    }
    public void UpdateButtonListPos()
    {
        DOVirtual.DelayedCall(0.1f, () =>
        {
            var buttonRectTf = btnListIconShow.GetComponent<RectTransform>();
            var anchorPos = GetButtonListPos(buttonRectTf);
            buttonRectTf.anchoredPosition = anchorPos;
        });
    }
    private void Awake()
    {
        var existing = Pool.Get<UIManager>();
        if (existing != null && existing != this)
        {
            Destroy(gameObject);
            return;
        }

        Pool.Set(this);
        DontDestroyOnLoad(gameObject);
        UnityEngine.SceneManagement.SceneManager.sceneLoaded += OnSceneLoaded;
        SetEventButton(btnThuongHoi, () =>
        {
            var iapHandler = Pool.Get<IapHandler>();
            iapHandler.popupMain.Show();
        });
        SetEventButton(btnBXH, () => { Pool.Get<RankPanel>().DoShow(); });
        SetEventButton(btnListIconShow, () =>
        {
            listIconTween?.Kill();
            isShowListIcon = !isShowListIcon;
            listIconTween = DOTween.Sequence();
            foreach (var listRectTf in listIcons)
            {
                if (listRectTf == null)
                    continue;
                var width = listRectTf.rect.width;
                var btnRect = btnListIconShow.GetComponent<RectTransform>();
                var targetButtonPos = GetButtonListPos(btnRect);
                listIconTween.Join(
                    btnRect.DOAnchorPos(targetButtonPos, 0.3f)
                );
                var xValue = width;
                if (!isShowListIcon)
                {
                    listIconTween.Join(
                        listRectTf.DOAnchorPosX(xValue, 0.25f)
                    );
                }
                else
                {
                    listIconTween.Join(
                        listRectTf.DOAnchorPosX(0, 0.25f)
                    );
                }
            }
        });
        SetEventButton(btnGacha, () => { Pool.Get<GachaPanel>().SetActive(); });
        SetEventButton(convertBtn, () =>
        {
            Observer.Notify(EventList.GamePlay.LOAD_SCENE, "ConvertElementalSkill");
        });
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        loadScene.gameObject.SetActive(false);
    }

    private void Start()
    {
        Observer.AddObserver(EventList.Network.IN_GAME_LOGOUT, OnLogout);
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
        UpdateButtonListPos();
    }

    private void OnDestroy()
    {
        Observer.RemoveObserver(EventList.Network.IN_GAME_LOGOUT, OnLogout);
        UnityEngine.SceneManagement.SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnLogout()
    {
        Destroy(gameObject);
    }

    [SerializeField] public Popup currentPopup;

    public void RegisterOpenPopup(Popup popup)
    {
        if (currentPopup != null && currentPopup != popup)
        {
            currentPopup.Hide();
        }

        this.currentPopup = popup;
        if (commonMask != null)
        {
            commonMask.gameObject.SetActive(true);
        }
    }

    public void RegisterClosePopup()
    {
        if (currentPopup == null || (currentPopup != null && !currentPopup.gameObject.activeSelf))
        {
            if (commonMask != null)
            {
                commonMask.gameObject.SetActive(false);
            }
        }
    }
}