using UnityEngine;
using UnityEngine.UI;

namespace LuyenKhi
{
    public class EquipmentToggle : MonoBehaviour
    {
        [SerializeField] private EquipmentType type;
        [SerializeField] private Toggle toggle;
        [SerializeField] private SelectTypePage selectTypePage;

        private void OnValidate()
        {
            toggle = GetComponent<Toggle>();
            selectTypePage = GetComponentInParent<SelectTypePage>();
        }
        private void Start()
        {
            toggle.onValueChanged.AddListener(OnToggle);
        }
        private void OnToggle(bool isOn)
        {
            if (isOn)
            {
                selectTypePage.SelectEquipment(type);
            }
        }
    }
}