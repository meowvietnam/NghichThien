using System;
using UnityEngine;
using UnityEngine.UI;

public class ToggleVisual : MonoBehaviour
{
    [SerializeField] private Toggle toggle;
    [SerializeField] private Image activeImage;
    [SerializeField] private Image inactiveImage;

    private void Start()
    {
        toggle.onValueChanged.AddListener(OnValueChanged);
        OnValueChanged(toggle.isOn);
    }

    private void OnValueChanged(bool isOn)
    {
        activeImage.enabled = isOn;
        inactiveImage.enabled = !isOn;
    }
}