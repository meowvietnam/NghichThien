using UnityEngine;

namespace LuyenKhi
{
    public class Equipment : MonoBehaviour
    {
        [field: SerializeField] public EquipmentType Type { get; private set; }
        [SerializeField] private SelectTypePage selectTypePage;
        [SerializeField] private GameObject selfObject;

        public void SetActive(bool active)
        {
            selfObject.SetActive(active);
        }
    }
}