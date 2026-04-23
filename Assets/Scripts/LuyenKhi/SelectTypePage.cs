using UnityEngine;

namespace LuyenKhi
{
    public class SelectTypePage : MonoBehaviour, IBackContext
    {
        [field: SerializeField] private Equipment[] equipments;
        [SerializeField] private GameObject selfObject;
        public void SelectEquipment(EquipmentType type)
        {
            foreach (var equipment in equipments)
            {
                var active = equipment.Type == type;
                equipment.SetActive(active);
            }
        }

        public void SelectSubEquipment(SubEquipment subEquipment)
        {
            var type = subEquipment.Type;
            LuyenKhiPanel.Instance.SelectSubEquipment(type);
        }

        public void SetActive(bool active)
        {
            selfObject.SetActive(active);
        }
        public void Back()
        {
            var panel = LuyenKhiPanel.Instance;
            SetActive(false);
            panel.LuyenKhiPage.SetActive(true);
            panel.SetBackContext(panel);
        }
    }
}
