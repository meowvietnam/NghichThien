using System.Linq;
using UnityEngine;

namespace LuyenKhi
{
    public class PutMaterials : MonoBehaviour
    {
        [SerializeField] public ItemSlot[] slots;
        private void OnValidate()
        {
            slots = GetComponentsInChildren<ItemSlot>();
        }
        public void Recalculate()
        {
            var matConfigs = LuyenKhiPanel.Instance.Resources.MaterialGradeConfigs;
            var slots = this.slots.Where(x => x.CurrentSelect != null).ToArray();
            var grade = 0;
            foreach (var slot in slots)
            {
                var tier = slot.CurrentSelect.Material.Tier;
                var matConfig = matConfigs.First(x => x.Tier == tier);
                grade += matConfig.Grade;
            }
            LuyenKhiPanel.Instance.LuyenKhiPage.SetGrade(grade);
        }
    }
}
