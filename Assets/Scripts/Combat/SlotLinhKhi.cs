using Sirenix.OdinInspector;
using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;


public enum ELinhKhi
{
    None = 0,
    Kim,
    Moc,
    Thuy,
    Hoa,
    Tho,
    Ma
}
public class SlotLinhKhi : MonoBehaviour , IPointerDownHandler
{
    public SecureInt count;
    public ELinhKhi eLinhKhi;
    public Text textCount;
    public Image icon;
    public Action actionPointerDown;
    public bool isLock;
    public SecureInt countRequire;
    public void AddCountRequire(int countRequire)
    {
        this.countRequire += countRequire;

    }
    public void SetCountRequire(int countRequire , ELinhKhi eLinhKhi = ELinhKhi.None)
    {
        this.countRequire = countRequire;
        this.eLinhKhi = eLinhKhi;

    }
    public void SetLock(bool isLock)
    {
        this.isLock = isLock;
    }
    public void AddCount(int count , Action eventEndCount = null )
    {
        this.count += count;

        textCount.text = this.count.Value.ToString();
        if (this.count <= 0)
        {
            icon?.gameObject.SetActive(false);
            eventEndCount?.Invoke();
        }    
        else
        {
            icon?.gameObject.SetActive(true);
        }    
    }
    public void SetCount(int count, Action eventEndCount = null)
    {
        this.count = count;

        textCount.text = this.count.Value.ToString();
        if (this.count <= 0)
        {
            icon?.gameObject.SetActive(false);
            eventEndCount?.Invoke();
        }
        else
        {
            icon?.gameObject.SetActive(true);
        }
    }
    public void Init(Action action)
    {
        actionPointerDown = action;
    }
  
    public void OnPointerDown(PointerEventData eventData)
    {
        if (count <= 0 || isLock) return;
        actionPointerDown?.Invoke();
    }
}
