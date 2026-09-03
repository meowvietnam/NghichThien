using System;
using LuyenKhi;
using UnityEngine;
using UnityEngine.UI;

public class RandomLuyenKhiButton : BaseBehaviour
{
    [SerializeField] private RandomLuyenKhi randomLuyenKhi;
    [SerializeField] private Button button;

    private void Start()
    {
        SetEventButton(button, OnClick);
    }

    private void OnClick()
    {
        randomLuyenKhi.Show();
    }
}
