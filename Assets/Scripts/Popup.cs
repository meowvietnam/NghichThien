using DG.Tweening;
using System;
using UnityEngine;
using UnityEngine.UI;

public class Popup : BaseBehaviour
{
    [SerializeField] private GameObject commonMask;
    [SerializeField] private Button btnClose;
    [SerializeField] private float scaleBase =1f;
    public void Show(float delay = 0.5f,Action actionComplete = null)
    {
        gameObject.SetActive(true);
        transform.localScale = Vector3.zero;
        if(commonMask != null)
        {
            commonMask?.SetActive(true);
        }
        transform.DOScale(Vector3.one * scaleBase, delay).OnComplete(() =>
        {
            actionComplete?.Invoke();
        });
        SetEventButton(btnClose, () =>
        {
            Hide();
        });
    }
    public void Hide(float delay = 0, Action actionComplete = null)
    {
        transform.DOScale(Vector3.zero, delay).OnComplete(() =>
        {
            actionComplete?.Invoke();
            gameObject.SetActive(false);

            if (commonMask != null)
            {
                commonMask?.SetActive(false);
            }
        });
    }

}
