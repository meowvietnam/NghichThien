using Sirenix.OdinInspector;
using System;
using UnityEngine;
using UnityEngine.UI;
using LongDH.LongDHModules;
using LongDHModules;

public class PopupConfirmAction : Popup,IUISingleton
{
    [SerializeField] private Button btnConfirm;
    [SerializeField] private Text contentText;

    protected void Awake()
    {
       
    }
    [Button]
    public void ShowPopConfirmAction(string content,Action actionConfirm)
    {
        Show();
        contentText.text = content;
        SetEventButton(btnConfirm, () =>
        {
            Hide();
            actionConfirm?.Invoke();
        });
        var fitter = contentText.GetComponent<ContentSizeFitter>();
        if (fitter != null)
        {
            for (int i = 0; i < 3; i++)
            {
                Canvas.ForceUpdateCanvases();
                LayoutRebuilder.ForceRebuildLayoutImmediate(transform as RectTransform);
                fitter.enabled = false;
                fitter.enabled = true;
            }
        }
    }

    public void Init()
    {
        Pool.Set(this);
        gameObject.SetActive(false);
    }
}
