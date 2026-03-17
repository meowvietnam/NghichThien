using CreateCharacter.CharStats;
using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;


public class BtnInforStats : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] private string infoName;
    [SerializeField] private StatType statType;
    [SerializeField, TextArea] private string info;

    public void OnPointerEnter(PointerEventData eventData)
    {
        var tooltip = Tooltip.Instance;
        tooltip.SetActive(true);
        tooltip.SetTextInfo(infoName, info);
        tooltip.SetScale(Vector2.one);
        tooltip.SetPosition(eventData.position - new Vector2(100, -40));
    }
    public void OnPointerExit(PointerEventData eventData)
    {
        var tooltip = Tooltip.Instance;
        tooltip.SetActive(false);
    }
}
