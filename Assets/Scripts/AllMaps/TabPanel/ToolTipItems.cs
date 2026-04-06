using AllMaps;
using LongDH.LongDHModules;
using LuyenDan;
using LuyenDan.Inventory;
using Network;
using Sirenix.OdinInspector;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
public enum EActionShowItemInfor
{
    None,
    Buy,
    Sell,
}
public class ToolTipItems : BaseBehaviour
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
    [SerializeField]
    private IItemData iItemData;
    [SerializeField] private Text descTextItem;
    [SerializeField] private EActionShowItemInfor eActionShowItem;
    public static ToolTipItems Instance { get; private set; }


    private void Awake()
    {
        this.gameObject.SetActive(true);
        Instance = this;
        this.gameObject.SetActive(false);
    }
    public void InitShow(BaseItemData itemData,Sprite frameSprite, Sprite qualityFull , Color colorName , ETypeInventoryItem eTypeItemShow , ItemRef itemRef, IItemData iItemData , EActionShowItemInfor eTypeActionShow = EActionShowItemInfor.None)
    {

        this.gameObject.SetActive(true);
        SetEventButton(btnMask, () =>
        {
            this.gameObject.SetActive(false);
        });
        priceText.transform.parent.gameObject.SetActive(false);
        prefabIconRequire.transform.parent.parent.gameObject.SetActive(false);
        descTextItem.transform.parent.gameObject.SetActive(false);
        this.itemData = itemData;
        this.itemRef = itemRef;
        this.iItemData = iItemData;
        this.nameText.text = itemData.nameItem;
        this.eActionShowItem = eTypeActionShow;
        InitIcon(this.itemData.spriteItem, frameSprite);
        SetQualityFull(qualityFull);
        SetColorName(colorName);
        ResetKeyword(prefabLeftInfor);
        ResetKeyword(prefabRightInfor);
        // Gán dữ liệu TRƯỚC
        if (eTypeItemShow == ETypeInventoryItem.Skill)
        {
            InitInforSkill();
        }
        else if (eTypeItemShow == ETypeInventoryItem.Item)
        {
            InitInforItem();
        }

        // cụm này force rebuild trc mới dãn đúng
        // và for 3 lần tắt đi bật lại contentsize cũng mới dãn đúng 
       
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
        yield return null; // Đợi thêm 1 frame nữa để các Layout con kịp co dãn theo Layout cha

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
            qualityText.text = skill.GetTextTierSkill();
            typeText.text = skill.GetTextTypeSkill();
            descriptionText.text = skill.GetDesc(ThanThong.GetDaiCanhGioi(Pool.Get<Player>().characterStatsBase.level));
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
            // Cộng thêm nội dung vào chính nó và xuống dòng
            descTextItem.text += "\n";
        }
        else
        {
            // Tương tự cho descriptionText
            descriptionText.text += "\n";
        }
    }    
    private void InitInforItem()
    {
        //<color=#ffb143>【Yêu cầu điểm số】</color>
     
        if (itemData is BaseSkill)
        {
            InitInforSkill();

        }
        if (itemRef!= null)
        {
            switch (itemRef.Type)
            {
                case ItemType.Herb:
                    typeText.text = "Dược Liệu";

                    descriptionText.text = "Thảo dược có thể dùng luyện chế đan dược";
                    if (iItemData != null && iItemData is HerbData duocLieu)
                    {
                        descTextItem.transform.parent.gameObject.SetActive(true);

                        descTextItem.text = "<color=#ffb143>【Chủ Dược】 </color>"+$"{duocLieu.ChuDuoc} " + "<color=#ffb143>【Phụ Dược】 </color>" + $"{duocLieu.PhuDuoc} " + "<color=#ffb143>【Dược Dẫn】 </color>" + $"{duocLieu.ThuocDan}";

                    }
                    qualityText.text = GameConfig.TextWithTier[iItemData.Tier];


                    break;
                case ItemType.Furnace:
                    typeText.text = "Lò Luyện Đan";
                    descriptionText.text = "Đan lô vật phẩm bắt buộc để có thể luyện chế đan dược";
                    descTextItem.transform.parent.gameObject.SetActive(true);
                    descTextItem.text = $"<color=#ffb143>【Bền Bỉ】</color> " +$"{Client.Instance.CharacterData.InventoryManager.Furnaces.FirstOrDefault(x => x.Ref.Id == itemRef.Id).Durability + "/" + 100}";
                    qualityText.text = GameConfig.TextWithTier[iItemData.Tier];

                    break;
                case ItemType.LinhDan:
                    string text = "Đan Dược";
                    if (iItemData != null &&iItemData is LinhDan linhDan)
                    {
                        descTextItem.transform.parent.gameObject.SetActive(true);
                        switch (linhDan.ETypeDan)
                        {
                            case ETypeDan.DanCombat:
                                text += " - Chiến Đấu";
                                break;
                            case ETypeDan.DanBuff:
                                text += " - Tăng Chỉ Số"; 
                                break;
                            case ETypeDan.DanDotPha:
                                text += " - Đột Phá";
                                break;
                            case ETypeDan.DanOther:
                                text += " - Khác";
                                break;
                            default:
                                break;
                        }
                        descTextItem.text = $"{BaseSkill.GetColoredText("【Đan Độc】", "#d36cd3")} {linhDan.DanDoc}";
                        descriptionText.text = linhDan.Description;

                    }
                    typeText.text = text;
                    qualityText.text = GameConfig.TextWithTier[iItemData.Tier];

                    break;
                case ItemType.ThanThong:
                    typeText.text = "Sách Thần Thông";
                    break;
                case ItemType.CongPhap:
                    typeText.text = "Sách Công Pháp";
                    break;
                case ItemType.VuKhi:
                    break;
                case ItemType.PhongCu:
                    break;
                case ItemType.TrangSuc:
                    break;
                case ItemType.Recipe:
                    typeText.text = "Đan Phương";
                    descriptionText.text = $"Công thức luyện chế {iItemData.Name}";
                    qualityText.text = GameConfig.TextWithTier[iItemData.Tier];

                    break;
                default:
                    break;
            }
        }
        if (eActionShowItem == EActionShowItemInfor.None || eActionShowItem == EActionShowItemInfor.Sell)
        {
            SetPriceText(GameConfig.GetPriceItemSell(itemRef, iItemData));
        }
        else
        {
            SetPriceText(GameConfig.GetPriceItemBuy(itemRef, iItemData));

        }

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
}
