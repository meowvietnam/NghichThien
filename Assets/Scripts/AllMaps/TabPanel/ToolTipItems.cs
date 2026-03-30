using AllMaps;
using LongDH.LongDHModules;
using Sirenix.OdinInspector;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
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
    public static ToolTipItems Instance { get; private set; }

    private void Awake()
    {
        Instance = this;
        gameObject.SetActive(false);
    }
    public void InitShow(BaseItemData itemData,Sprite frameSprite, Sprite qualityFull , Color colorName , ETypeInventoryItem eTypeItemShow)
    {
        gameObject.SetActive(true);
        SetEventButton(btnMask, () =>
        {
            gameObject.SetActive(false);
        });
        
        this.itemData = itemData;
        this.nameText.text = itemData.nameItem;
        InitIcon(this.itemData.spriteItem, frameSprite);
        SetQualityFull(qualityFull);
        SetColorName(colorName);

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
    private void InitInforSkill()
    {
        void SetTextKeyword(Image prefabKeyword , int i , string content)
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
                prefabKeyword.transform.parent.GetChild(0).gameObject.SetActive(false);
            }
        }
        if (itemData is BaseSkill skill)
        {
            qualityText.text = skill.GetTextTierSkill();
            typeText.text = skill.GetTextTypeSkill();
            descriptionText.text = skill.GetDesc(ThanThong.GetDaiCanhGioi(Pool.Get<Player>().characterStatsBase.level));
            var listEKeyword = skill.GetEKeywordSkillHave();
            var listEBuffKeyword = skill.GetEKeywordBuffSkillHave(1);
            ResetKeyword(prefabLeftInfor);
            ResetKeyword(prefabRightInfor);

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
   
    private void InitInforItem()
    {

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
