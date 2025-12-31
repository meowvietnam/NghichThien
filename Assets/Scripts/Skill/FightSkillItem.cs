using UnityEngine;
using UnityEngine.UI;

public class FightSkillItem : BaseItemUI
{
    [SerializeField] private Image recommentHighLight;
    [SerializeField] private Image selectHighLight;
    [SerializeField] private Image maskCoolDown;

    public ThanThong ThanThongData => (ThanThong)itemData;

    public bool isLock => maskCoolDown.gameObject.activeSelf;
    public void SetLock( bool isLock)
    {
        maskCoolDown.gameObject.SetActive(isLock);
    }
    public override void InitItemData(BaseItemData itemData)
    {
        base.InitItemData(itemData);
        icon.gameObject.SetActive(itemData != null);
        SetLock(itemData == null);


    }
    public override void OnPointerUp()
    {
        base.OnPointerUp();
        SetSelectHighlight(true);
    }
    public void SetSelectHighlight(bool isActive)
    {
        selectHighLight.gameObject.SetActive(isActive);
    }
    public void ShowRecommentHighlight(bool isActive)
    {
        recommentHighLight.gameObject.SetActive(isActive);
    }    




}
