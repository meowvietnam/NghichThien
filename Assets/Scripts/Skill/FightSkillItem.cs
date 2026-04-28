using Skill;
using UnityEngine;
using UnityEngine.UI;

public class FightSkillItem : BaseItemUI
{
    [SerializeField] private Image recommentHighLight;
    [SerializeField] private Image maskCoolDown;
    [SerializeField] private Button btnShowInfor;
    [SerializeField] private Text cdText;

    public ThanThong ThanThongData => (ThanThong)itemData;

    public bool isLock => maskCoolDown.gameObject.activeSelf || cdText.transform.parent.gameObject.activeSelf;
    public void SetLock( bool isLock)
    {
        maskCoolDown.gameObject.SetActive(isLock);
    }
    public override void InitItemData(BaseItemData itemData, Sprite frameSprite)
    {
        base.InitItemData(itemData, frameSprite);
        icon.gameObject.SetActive(itemData != null);
        btnShowInfor.gameObject.SetActive(itemData != null);

        SetLock(itemData == null);
        btnShowInfor.onClick.AddListener(() =>
        {
            EventHolder();
        });

    }
    public void SetCoolDown(bool isActive , int cd)
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
