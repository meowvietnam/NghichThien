using UnityEngine;
using UnityEngine.UI;
public class PlayerUICombatItem : BaseItemUI
{
    [SerializeField] private Image recommentHighLight;
    [SerializeField] private Image maskCoolDown;
    [SerializeField] private Button btnShowInfor;
    [SerializeField] private Text cdText;
    [SerializeField] private Image fullQuality;

    public bool isLock => maskCoolDown.gameObject.activeSelf || cdText.transform.parent.gameObject.activeSelf;
    public override void InitItemData(BaseItemData itemData, Sprite frameSprite)
    {
        base.InitItemData(itemData, frameSprite);
        icon.gameObject.SetActive(itemData != null);
        frame.gameObject.SetActive(itemData != null);
        fullQuality.gameObject.SetActive(itemData != null);

        btnShowInfor.gameObject.SetActive(itemData != null);

        SetLock(itemData == null);
        btnShowInfor.onClick.AddListener(() =>
        {
            EventHolder();
        });

    }
    public void SetFullQuality(Sprite sprite)
    {
        fullQuality.sprite = sprite;
        fullQuality.SetNativeSize();
    }    
    public void SetLock(bool isLock)
    {
        maskCoolDown.gameObject.SetActive(isLock);
    }
    public void SetCoolDown(bool isActive, int cd)
    {
        cdText.gameObject.transform.parent.gameObject.SetActive(isActive);
        cdText.text = cd.ToString();
    }
    public override void OnPointerUp()
    {
        if (isLock) return;
        base.OnPointerUp();
    }

    public void ShowRecommentHighlight(bool isActive)
    {
        recommentHighLight.gameObject.SetActive(isActive);
    }
}
