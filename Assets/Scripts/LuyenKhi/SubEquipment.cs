using System;
using UnityEngine;
using UnityEngine.UI;

namespace LuyenKhi
{
    public class SubEquipment : BaseBehaviour
    {
        [field: SerializeField] public SubEquipmentType Type { get; private set; }
        [SerializeField] private SelectTypePage selectTypePage;
        [SerializeField] private Button button;
        private void Start()
        {
            SetEventButton(button,OnClick);
        }
        private void OnClick()
        {
            selectTypePage.SelectSubEquipment(this);
        }

        private void OnDestroy()
        {
            button.onClick.RemoveAllListeners();
        }
    }
}