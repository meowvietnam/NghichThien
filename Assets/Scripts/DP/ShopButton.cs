using System;
using DP;
using UnityEngine;
using UnityEngine.UI;

public class ShopButton : BaseBehaviour
{
    [SerializeField] private Button button;
    private void Start()
    {
        SetEventButton(button, OpenShop);
    }

    private void OpenShop()
    {
        DongPhuPanel.Instance.OpenShop();
    }
}
