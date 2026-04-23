using System.Linq;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.UI;

namespace LuyenKhi
{
    public class LuyenKhiPage : MonoBehaviour
    {
        [SerializeField] private GameObject selfObject;
        [SerializeField] private Image iconImage;
        [SerializeField] private Text grade;
        
        public void SetActive(bool active)
        {
            selfObject.SetActive(active);
        }
        public void SelectSubEquipment(SubEquipmentType type)
        {
            var icon = LuyenKhiPanel.Instance.Resources.EquipmentIcons.First(x => x.Type == type);
            iconImage.sprite = icon.Icon;
            iconImage.SetNativeSize();
        }
    }
}