using Sirenix.OdinInspector;
using System;
using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class BaseItemUI : MonoBehaviour, IPointerUpHandler , IPointerDownHandler
{
    [SerializeField] private float timeHolder = 1f;
    [SerializeField] protected Coroutine enumerator = null;
    [SerializeField] protected bool isHolder = false;
    [SerializeField] public Image icon;
    [SerializeField] protected Image frame;
    [SerializeField] protected Image progressHold;

    [SerializeField] protected BaseItemData itemData;
    [SerializeField] protected Action actionHolder;
    [SerializeField] protected Action actionPointerUp;

    public virtual void Init(BaseItemData itemData , Sprite frameSprite, Action actionHolder = null , Action actionPointerUp = null)
    {
        InitItemData(itemData, frameSprite);

        InitAction(actionHolder, actionPointerUp);
    }
    [Button]
    public virtual void InitItemData(BaseItemData itemData , Sprite frameSprite)
    {
        if (itemData == null)
        {
            return;
        }    
        this.itemData = itemData;
        InitIcon(this.itemData.spriteItem, frameSprite);

    }
    public  void InitIcon(Sprite spriteIcon, Sprite frameSprite)
    {
        if(spriteIcon != null)
        {
            icon.sprite = spriteIcon;
        }    
        if(frameSprite != null)
        {
            frame.sprite = frameSprite;

        }

    }
    public void InitAction(Action actionHolder = null, Action actionPointerUp = null)
    {
        this.actionHolder = actionHolder;
        this.actionPointerUp = actionPointerUp;
    }
    public virtual void EventHolder()
    {
        actionHolder?.Invoke();
        progressHold.gameObject.SetActive(false);   
    }    
    protected IEnumerator IECheckHolder()
    {
        progressHold.transform.parent.gameObject.SetActive(true);

        isHolder = false;
        progressHold.fillAmount = 0f;

        float timer = 0f;

        while (timer < timeHolder)
        {
            timer += Time.deltaTime;
            progressHold.fillAmount = timer / timeHolder;
            yield return null; // update mỗi frame
        }

        progressHold.fillAmount = 1f;
        isHolder = true;
    }
    public void OnPointerDown(PointerEventData eventData)
    {
        enumerator = StartCoroutine(IECheckHolder());
    }

    public virtual void OnPointerUp()
    {
        Debug.Log("cos goi pointer up item ui");
        actionPointerUp?.Invoke();
    }    
    public void OnPointerUp(PointerEventData eventData)
    {
        if(!isHolder)
        {
            StopCoroutine(enumerator);
            enumerator = null;
            OnPointerUp();
        }
        else
        {
            isHolder = false;
        }
        progressHold.transform.parent.gameObject.SetActive(false);

    }


}
