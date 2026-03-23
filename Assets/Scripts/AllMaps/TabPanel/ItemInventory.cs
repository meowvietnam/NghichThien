using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ItemInventory : BaseItemUI
{
    public enum ETypeInventory {
    
        None,
        Skill,
        Item,
    
    }


    [SerializeField] private GameObject hasItem;
    [SerializeField] private Image qualityFull;
    [SerializeField] private Text textCount;
    [SerializeField] public Text textName;
    [SerializeField] private GameObject leftUpMask;

    public void InitInventory(ETypeInventory type)
    {
       
        if(itemData == null)
        {
            hasItem.SetActive(false);
            return;
        }
        else
        {
            hasItem.SetActive(true);


        }
        textName.text = itemData.nameItem;
        switch (type)
        {
            case ETypeInventory.None:
                break;
            case ETypeInventory.Skill:
                textCount.gameObject.SetActive(false);
                leftUpMask.gameObject.SetActive(false);

                break;
            case ETypeInventory.Item:
                textCount.gameObject.SetActive(true);

                //leftUpMask.gameObject.SetActive(true);
                break;
            default:
                break;
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
        icon.transform.parent.gameObject.AddComponent<RectMask2D>().softness = new Vector2Int(25,25);

    }
}
