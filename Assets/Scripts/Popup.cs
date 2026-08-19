using DG.Tweening;
using LongDH.LongDHModules;
using System;
using System.Collections;
using LongDHModules;
using UnityEngine;
using UnityEngine.UI;

public class Popup : BaseBehaviour
{
    [SerializeField] protected Image commonMask ;
    [SerializeField] protected Button btnClose;
    [SerializeField] protected float scaleBase =1f;
    [SerializeField]
    protected Action actionClose;
    public bool isIgnoreOtherPopup = false;
    public virtual void Show(float delay = 0.5f,Action actionComplete = null)
    {
       
        gameObject.SetActive(true);
        transform.localScale = Vector3.zero;
        
        transform.DOScale(Vector3.one * scaleBase, delay).OnComplete(() =>
        {
            actionComplete?.Invoke();
        });
        SetEventButton(btnClose, () =>
        {
            ActionBtnClose();
        });
        if(commonMask == null)
        {
            var uiManager = Pool.Get<UIManager>();
            if (!isIgnoreOtherPopup)
            {

                uiManager.RegisterOpenPopup(this);

            }
            else
            {
                uiManager.commonMask.gameObject.SetActive(true);
            }    
        }    
        else
        {
            commonMask.gameObject.SetActive(true);
        }    

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
        if (commonMask == null)
        {   var uiManager = Pool.Get<UIManager>();
            if(uiManager != null)
            {
                uiManager.RegisterClosePopup();
            }
        }
        else
        {
            commonMask.gameObject.SetActive(false);
        }

    }
    // Coroutine này giờ không cần nằm trong Popup nữa (hoặc giữ lại nhưng chạy ké qua UI Manager)
    private IEnumerator DelayFrameOnUIManager()
    {
        yield return null;
        Pool.Get<UIManager>().RegisterClosePopup();
    }

}
