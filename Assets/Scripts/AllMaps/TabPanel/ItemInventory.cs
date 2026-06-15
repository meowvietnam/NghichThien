using Base;
using LongDH.LongDHModules;
using LongDHModules;
using LuyenDan;
using LuyenDan.Inventory;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
public class ItemInventory : BaseItemUI
{
    [SerializeField] private GameObject hasItem;
    [SerializeField] public Image qualityFull;
    [SerializeField] private Text textCount;
    [SerializeField] public Text textName;
    [SerializeField] private GameObject leftUpMask;
    [SerializeField] private Button btnShowInfor;
    [SerializeField] public ItemRef itemRef;
    [SerializeField] public IItemData iItemData;
    [SerializeField] public int count;
    [SerializeField] public int countRequire;

    public override void InitIcon(Sprite spriteIcon, Sprite frameSprite)
    {
        base.InitIcon(spriteIcon, frameSprite);
        if(itemRef != null)
        {
            if (itemRef.Type == ItemType.VuKhi)
            {
                icon.rectTransform.localScale = new Vector3(1f / 3f, 1f / 3f, 1f);
            }
            else if (itemRef.Type == ItemType.PhongCu || itemRef.Type == ItemType.TrangSuc)
            {
                icon.rectTransform.localScale = new Vector3(1f / 2.5f, 1f / 2.5f, 1f);
            }
            else
            {
                icon.rectTransform.localScale = Vector3.one;

            }
        }
    }

    private void OnEnable()
    {
        if (selectHighLight == null) return;
        selectHighLight.gameObject.SetActive(false);
    }
    public void InitCount(int count)
    {
        this.count = count;
        textCount.text = count.ToString();
        if (count >0 && !this.gameObject.activeSelf)
        {
            this.gameObject.SetActive(true);
        }
    }
    public void AddCount(int count)
    {
        this.count += count;
        textCount.text = this.count.ToString();
        if(count<= 0 && this.gameObject.activeSelf)
        {
            this.gameObject.SetActive(false);
        }
        else if (count > 0 && !this.gameObject.activeSelf)
        {
            this.gameObject.SetActive(true);
        }
    }
    public void InitCountRequire(int countRequire)
    {
        this.countRequire = countRequire;
        textCount.text = $"{this.count}/{this.countRequire}";
    }
    public void InitItemRef(ItemRef itemRef)
    {
        this.itemRef = itemRef;
    }
    public void InitInventory(ItemRef itemRefInven,IItemData iItemData , EActionShowItemInfor eActionShowItemInfor = EActionShowItemInfor.None)
    {

        this.itemRef = itemRefInven;
        this.iItemData = iItemData;
        if(itemData == null)
        {
            btnShowInfor?.gameObject.SetActive(false);
            textCount.gameObject.SetActive(false);
            textName.gameObject.SetActive(false);
            hasItem.SetActive(false);
            return;
        }
        else
        {
            hasItem.SetActive(true);
            btnShowInfor?.gameObject.SetActive(true);
            textName.gameObject.SetActive(true);

        }
        if (btnShowInfor != null)
        {
            SetEventButton(btnShowInfor, () =>
            {
                ToolTipItems.Instance.InitShow(this,frame.sprite,qualityFull.sprite,textName.color, itemRefInven, iItemData , eActionShowItemInfor);
            });
        }    
        textName.text = itemData.nameItem;

        if(Pool.Get<TabPanel>().gameObject.activeSelf)
        {
            textCount.gameObject.SetActive(true);
        }
        else
        {
            textCount.gameObject.SetActive(false);
        }    
        leftUpMask.gameObject.SetActive(false);
        if (itemRef != null)
        {
            if (itemRef.Type == ItemType.Furnace || itemRef.Type == ItemType.VuKhi || itemRef.Type == ItemType.PhongCu || itemRef.Type == ItemType.TrangSuc)
            {
                textCount.gameObject.SetActive(false);
            }
        }    
    }
    public override void OnPointerDown(PointerEventData eventData)
    {
        base.OnPointerDown(eventData);
        SetSelectHighlight(true);

    }
    public override void OnPointerUp()
    {
        SetSelectHighlight(false);

    }
    public void SetColorName(Color color)
    {
        textName.color = color;
    }
    public void SetQualityFull(Sprite sprite)
    {
        qualityFull.sprite = sprite;
        qualityFull.SetNativeSize();
    }
    [Button]
    private void FixVisualItemSkill()
    {
        frame.transform.SetParent(qualityFull.transform.parent);
        frame.transform.SetSiblingIndex(2);
        frame.transform.localScale = Vector3.one;
        qualityFull.transform.localScale = Vector3.one;

        icon.transform.parent.localScale = Vector3.one * 0.98f;
        icon.transform.localScale = Vector3.one;

        // Lưu ý: Không ghi đuôi .asset
        Sprite loadedSprite = Resources.Load<Sprite>("Sprites/MCS_icon_quality_bg");

        if (loadedSprite != null)
        {
            Image img = icon.transform.parent.GetComponent<Image>();
            img.sprite = loadedSprite;

            // Sửa lỗi: Không thể gán trực tiếp vào 'a' (color là struct)
            Color c = img.color;
            c.a = 1f;
            img.color = c;

            // Sửa lỗi: Cú pháp Destroy phải có ngoặc đơn ()
            var rectMask = icon.transform.parent.GetComponent<RectMask2D>();
            if (rectMask != null) DestroyImmediate(rectMask);

            // Chèn thêm Mask
            icon.transform.parent.gameObject.AddComponent<Mask>();
            icon.transform.parent.GetComponent<Mask>().showMaskGraphic = false;
        }
        else
        {
            Debug.LogError("Không tìm thấy Sprite! Hãy kiểm tra lại đường dẫn trong thư mục Resources.");
        }

    }
    [Button]
    private void GetComponentBag()
    {
        hasItem = transform.GetChild(1).gameObject;
        selectHighLight = transform.GetChild(2).GetComponent<Image>();
        textCount = transform.GetChild(1).transform.GetChild(4).GetComponent<Text>();
        icon = transform.GetChild(1).transform.GetChild(1).transform.GetChild(0).GetComponent<Image>();
        frame  = transform.GetChild(1).transform.GetChild(0).transform.GetChild(0).GetComponent<Image>();
        qualityFull = transform.GetChild(1).transform.GetChild(0).GetComponent<Image>();
        textName  = transform.GetChild(1).transform.GetChild(2).transform.GetChild(0).GetComponent<Text>();
        leftUpMask = transform.GetChild(1).transform.GetChild(3).gameObject;

    }
    [Button]
    private void GetComponentThanThong()
    {
        hasItem = transform.GetChild(1).gameObject;
        selectHighLight = transform.GetChild(3).GetComponent<Image>();
        textCount = transform.GetChild(1).transform.GetChild(4).GetComponent<Text>();
        icon = transform.GetChild(1).transform.GetChild(1).transform.GetChild(0).GetComponent<Image>();
        frame = transform.GetChild(1).transform.GetChild(0).transform.GetChild(0).GetComponent<Image>();
        qualityFull = transform.GetChild(1).transform.GetChild(0).GetComponent<Image>();
        textName = transform.GetChild(1).transform.GetChild(2).transform.GetChild(0).GetComponent<Text>();
        leftUpMask = transform.GetChild(1).transform.GetChild(3).gameObject;

    }
    [Button]
    private void GetComponentCongPhap()
    {
        hasItem = transform.GetChild(2).gameObject;
        selectHighLight = transform.GetChild(3).GetComponent<Image>();
        textCount = transform.GetChild(2).transform.GetChild(4).GetComponent<Text>();
        icon = transform.GetChild(2).transform.GetChild(1).transform.GetChild(0).GetComponent<Image>();
        frame = transform.GetChild(2).transform.GetChild(0).transform.GetChild(0).GetComponent<Image>();
        qualityFull = transform.GetChild(2).transform.GetChild(0).GetComponent<Image>();
        textName = transform.GetChild(2).transform.GetChild(2).transform.GetChild(0).GetComponent<Text>();
        leftUpMask = transform.GetChild(2).transform.GetChild(3).gameObject;

    }
    [Button]
    private void SetUpMask()
    {
        qualityFull.transform.localScale = Vector3.one * 1.03f;
        RectMask2D parent = icon.transform.parent.GetComponent<RectMask2D>();
        if(parent == null)
        {
            parent = parent.gameObject.AddComponent<RectMask2D>();

        }
        parent.softness =  new Vector2Int(25, 25);

    }
    [Button]
    private void FixVisualTextCount()
    {
        RectTransform rect = textCount.GetComponent<RectTransform>();
        rect.anchoredPosition = new Vector2(rect.anchoredPosition.x, -24f);
    }
    [Button]
    private void GetBtnShowInfor()
    {
        btnShowInfor = transform.GetChild(3).GetComponent<Button>();

    }
    [Button]
    private void SetHasItemFalse()
    {
       hasItem.gameObject.SetActive(false);

    }
    [Button]
    private void ReplaceSpriteBGNullItem()
    {
        // Lưu ý: Không ghi đuôi .asset
        Sprite loadedSprite = Resources.Load<Sprite>("newui/Sprites/MCS_TAB_wupin_st");

        if (loadedSprite != null)
        {
            transform.GetChild(0).GetChild(0).GetComponent<Image>().sprite = loadedSprite;
        }
        else
        {
            Debug.LogError("Không tìm thấy Sprite! Hãy kiểm tra lại đường dẫn trong thư mục Resources.");
        }
    }
}
