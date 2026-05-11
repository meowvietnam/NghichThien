using AllMaps;
using AllMaps.ItemProvider;
using LongDH.LongDHModules;
using LuyenDan;
using LuyenDan.Inventory;
using Network;
using Sirenix.OdinInspector;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Base;
using CreateCharacter;
using UnityEngine;
using UnityEngine.UI;
using CongPhap = Skill.CongPhap;
using ThanThong = Skill.ThanThong;

public enum EActionShowItemInfor
{
    None,
    Buy,
    Sell,
}
public class ToolTipItems : BaseBehaviour , ISingleton
{
    [SerializeField] private Button btnMask;
    [SerializeField] private Image icon;
    [SerializeField] private Image frame;
    [SerializeField] private Image qualityFull;
    [SerializeField] private Text nameText;
    [SerializeField] private Text typeText;
    [SerializeField] private Text qualityText;
    [SerializeField] private Text descriptionText;
    [SerializeField] private Sprite[] spriteLinhKhis;
    [SerializeField] private Image prefabIconRequire;
    [SerializeField] private Text priceText;
    [SerializeField] private BaseItemData itemData;
    [SerializeField]
    private GameObject tipObj;
    [SerializeField]
    private Image prefabLeftInfor;
    [SerializeField]
    private Image prefabRightInfor;
    [SerializeField]
    private ItemRef itemRef;
    private IItemData iItemData;
    [SerializeField]
    private FurnaceData furnaceData;
    public FurnaceData FurnaceData
    {
        get
        {
            // Nếu _furnaceData null thì tạo mới một cái rồi trả về
            return furnaceData ??= itemRef.Type == ItemType.Furnace ? Client.Instance.CharacterData.InventoryManager.Furnaces.FirstOrDefault(x => x.SelfId == itemRef.Id) : null;
        }
    }

    [SerializeField] private Text descTextItem;
    [SerializeField] private EActionShowItemInfor eActionShowItem;
    public static ToolTipItems Instance { get; private set; }
    private CharacterData CharData => Client.Instance.CharacterData;
   

   
    public void ShowInfoItem(IItemData itemData,ItemRef itemRef, FurnaceData furnaceData = null)
    {
        iItemData = itemData;
        this.itemRef = itemRef;
        this.furnaceData = furnaceData;
        var tier = itemData.Tier;
        var quality = CoreConfig.Instance.FindQuality(tier);
        var frame = quality.IconUp;
        var background = quality.Background;
        var icon = itemData.Icon;
        var color = quality.Color;
        var name = itemData.Name;
        nameText.text = name;
        eActionShowItem = EActionShowItemInfor.None;
        gameObject.SetActive(true);
        InitIcon(icon, frame);
        SetQualityFull(background);
        SetColorName(color);
        SetQualityText(quality);
        ResetKeyword(prefabLeftInfor);
        ResetKeyword(prefabRightInfor);
        InitInforItem();
        RefreshTooltipLayout();
    }
    public void InitShow(ItemInventory itemInventory,Sprite frameSprite, Sprite qualityFull , Color colorName , ETypeInventoryItem eTypeItemShow , ItemRef itemRef, IItemData iItemData , EActionShowItemInfor eTypeActionShow = EActionShowItemInfor.None)
    {
        if (itemInventory.itemData == null) return;
        gameObject.SetActive(true);
        priceText.transform.parent.gameObject.SetActive(false);
        prefabIconRequire.transform.parent.parent.gameObject.SetActive(false);
        descTextItem.transform.parent.gameObject.SetActive(false);
        itemData = itemInventory.itemData;
        this.itemRef = itemRef;
        this.iItemData = iItemData;
        nameText.text = itemData.nameItem;
        eActionShowItem = eTypeActionShow;
        InitIcon(itemInventory.icon.sprite, frameSprite);
        SetQualityFull(qualityFull);
        SetColorName(colorName);
        ResetKeyword(prefabLeftInfor);
        ResetKeyword(prefabRightInfor);
        if (iItemData != null)
        {
            SetQualityText(iItemData.Tier);
        }
        if (eTypeItemShow == ETypeInventoryItem.Skill)
        {
            InitInforSkill();
        }
        else if (eTypeItemShow == ETypeInventoryItem.Item)
        {
            InitInforItem();
        }
        RefreshTooltipLayout();        
    }

    private void SetQualityText(Tier tier)
    {
        var quality = CoreConfig.Instance.FindQuality(tier);
        SetQualityText(quality);
    }
    private void SetQualityText(Quality quality)
    {
        qualityText.text = quality.Name;
        qualityText.color = quality.Color;
    }
    private void RefreshTooltipLayout()
    {
        var fitter = tipObj.GetComponent<ContentSizeFitter>();
        if (fitter != null)
        {
            for (int i = 0; i < 3; i++)
            {
                Canvas.ForceUpdateCanvases();
                LayoutRebuilder.ForceRebuildLayoutImmediate(transform as RectTransform);
                fitter.enabled = false;
                fitter.enabled = true;
            }
        }
    }
    private IEnumerator DelayFrame()
    {
        yield return null;
    }
    void SetTextKeyword(Image prefabKeyword, int i, string content)
    {
        Image itemInfor = null;
        if (i < prefabKeyword.transform.parent.childCount)
        {
            itemInfor = prefabKeyword.transform.parent.GetChild(i).GetComponent<Image>();
        }
        else
        {
            itemInfor = Instantiate(prefabKeyword, prefabKeyword.transform.parent);

        }
        itemInfor.gameObject.SetActive(true);
        itemInfor.transform.GetChild(0).GetComponent<Text>().text = $"{content}";
    }
    void ResetKeyword(Image prefabKeyword)
    {
        for (int i = 0; i < prefabKeyword.transform.parent.childCount; i++)
        {
            prefabKeyword.transform.parent.GetChild(i).gameObject.SetActive(false);
        }
    }
    private void InitInforSkill()
    {
        if (itemData is BaseSkill skill)
        {
            qualityText.text = BaseSkill.GetTextTierSkill(skill.eTierSkill);
            typeText.text = skill.GetTextTypeSkill();
            if(skill is ThanThong)
            {
                descriptionText.text = skill.GetDesc(ThanThong.GetDaiCanhGioi(CharData.Progression.Level));
            }
            else if(skill is CongPhap)
            {
                descriptionText.text = skill.GetDesc(5);
            }
            var listEKeyword = skill.GetEKeywordSkillHave();
            var listEBuffKeyword = skill.GetEKeywordBuffSkillHave(1);
            for (int i = 0; i < listEKeyword.Length; i++)
            {
                SetTextKeyword(prefabLeftInfor, i, BaseSkill.GetDescKeyWord(listEKeyword[i]));
            }
            for (int i = 0; i < listEBuffKeyword.Length; i++)
            {
                SetTextKeyword(prefabRightInfor, i, BaseSkill.GetDescBuffOrDeBuff(listEBuffKeyword[i]));
            }
        }
        if (itemData is ThanThong thanThong)
        {
            prefabIconRequire.transform.parent.parent.gameObject.SetActive(true);
            for (int i = 0; i < thanThong.BaseRequireActive.Count; i++)
            {
                Image itemRequire = null;
                if (i < prefabIconRequire.transform.parent.childCount)
                {
                    itemRequire = prefabIconRequire.transform.parent.GetChild(i).GetComponent<Image>();
                }
                else
                {
                    itemRequire = Instantiate(prefabIconRequire, prefabIconRequire.transform.parent);
                }
                itemRequire.gameObject.SetActive(true);
                itemRequire.sprite = spriteLinhKhis[(int)thanThong.BaseRequireActive[i].linhKhiRequire];
                itemRequire.SetNativeSize();
                itemRequire.transform.GetChild(0).GetComponent<Text>().text = $"x{thanThong.BaseRequireActive[i].count}";
            }
        }
        else
        {
            prefabIconRequire.transform.parent.parent.gameObject.SetActive(false);
        }
    }
    public void SetPriceText(int price)
    {
        priceText.transform.parent.gameObject.SetActive(true);
        priceText.text = price.ToString();
        if (descTextItem.transform.parent.gameObject.activeSelf)
        {
            descTextItem.text += "\n";
        }
        else
        {
            descriptionText.text += "\n";
        }
    }    
    private void InitInforItem()
    {
        if (itemData is BaseSkill)
        {
            InitInforSkill();
        }

        if (itemRef == null) return;
        var disPlayer = ItemProviderFactory.GetProvider(itemRef.Type);
        if(disPlayer != null)
        {
            var ctx = BuildContext();
            disPlayer.Display(ctx);
        }
        else
        {
            //if (eActionShowItem == EActionShowItemInfor.None || eActionShowItem == EActionShowItemInfor.Sell)
            //    SetPriceText(GameManager.GetPriceItemSell(itemRef, iItemData));
            //else
            //    SetPriceText(GameManager.GetPriceItemBuy(itemRef, iItemData));
        }
    }
    private ToolTipContext BuildContext()
    {
        return new ToolTipContext
        {
            TypeText        = typeText,
            QualityText     = qualityText,
            DescriptionText = descriptionText,
            DescTextItem    = descTextItem,
            PrefabIconRequire = prefabIconRequire,
            PrefabLeftInfor   = prefabLeftInfor,
            PrefabRightInfor  = prefabRightInfor,
            SpriteLinhKhis  = spriteLinhKhis,
            ItemRef         = itemRef,
            FurnaceData = FurnaceData,
            IItemData = iItemData,
            BaseItemData = itemData,
            ActionShowItem  = eActionShowItem,
            SetPriceText    = SetPriceText,
        };
    }
    private void InitIcon(Sprite spriteIcon, Sprite frameSprite)
    {
        if (spriteIcon != null)
        {
            icon.sprite = spriteIcon;
        }
        if (frameSprite != null)
        {
            frame.sprite = frameSprite;
        }
    }
    public void SetColorName(Color color)
    {
        nameText.color = color;
        qualityText.color = color;
    }
    public void SetQualityFull(Sprite sprite)
    {
        qualityFull.sprite = sprite;
        qualityFull.SetNativeSize();
    }
    public void Init()
    {
        gameObject.SetActive(true);
        Instance = this;
        gameObject.SetActive(false);
        SetEventButton(btnMask, () =>
        {
            gameObject.SetActive(false);
        });
    }
}
