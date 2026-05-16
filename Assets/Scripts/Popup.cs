using DG.Tweening;
using LongDH.LongDHModules;
using System;
using UnityEngine;
using UnityEngine.UI;

public class Popup : BaseBehaviour
{
    [SerializeField] protected Image commonMask ;
    [SerializeField] protected Button btnClose;
    [SerializeField] protected float scaleBase =1f;
    [SerializeField]
    protected Action actionClose;
  public virtual void Show(float delay = 0.5f,Action actionComplete = null)
    {
        if(commonMask == null)
        {
            commonMask =  Pool.Get<UIManager>().commonMask;
        }
        gameObject.SetActive(true);
        transform.localScale = Vector3.zero;
        if(commonMask != null)
        {
            commonMask?.gameObject.SetActive(true);
        }
        transform.DOScale(Vector3.one * scaleBase, delay).OnComplete(() =>
        {
            actionComplete?.Invoke();
        });
        SetEventButton(btnClose, () =>
        {
            ActionBtnClose();
        });
    }
    public void InitActionClose(Action action)
    {
        actionClose = action;
    }    
    protected virtual void ActionBtnClose()
    {
        Hide();
        actionClose?.Invoke();
    }    
    public void Hide(float delay = 0, Action actionComplete = null)
    {

        actionComplete?.Invoke();
        gameObject.SetActive(false);

        if (commonMask != null)
        {
            commonMask?.gameObject.SetActive(false);
        }
    }

}
