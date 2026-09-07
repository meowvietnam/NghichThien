using Base;
using CreateCharacter.CharStats;
using LongDH.LongDHModules;
using System.Collections;
using LongDHModules;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;


public class BtnInforStats : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] private string infoName;
    [SerializeField] private StatType statType;
    [SerializeField] protected virtual string Infor => info;
    [SerializeField, TextArea] private string info;

    public void OnPointerEnter(PointerEventData eventData)
    {
        var tooltip = Pool.Get<TabPanel>().toolTipBtnStats;
        tooltip.SetActive(true);
        tooltip.SetTextInfo(infoName, Infor);
        tooltip.SetScale(Vector2.one * 1.5f);
        tooltip.SetPosition(eventData.position - new Vector2(100, -40));
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        var tooltip = Pool.Get<TabPanel>().toolTipBtnStats;
        tooltip.SetActive(false);
    }
}