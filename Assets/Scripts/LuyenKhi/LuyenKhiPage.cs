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
        [SerializeField] private Text gradeDetail;
        [SerializeField] private ButtonLuyenKhi buttonLuyenKhi;
        [field: SerializeField, ReadOnly] public ArtifactType CurrentSelectedType { get; private set; } 
        public void Init()
        {
            SetGrade(0);
        }
        public void SetActive(bool active)
        {
            selfObject.SetActive(active);
        }
        public void SelectArtifactType(ArtifactType type)
        {
            CurrentSelectedType = type;
            var icon = LuyenKhiPanel.Instance.Resources.EquipmentIcons.First(x => x.Type == type);
            iconImage.sprite = icon.Icon;
            iconImage.SetNativeSize();
        }

        public void SetGrade(int grade)
        {
            this.grade.text = grade.ToString();
            var resources = LuyenKhiPanel.Instance.Resources;
            var equipmentGradeConfig = resources.EquipmentGrades.Last(x => x.MinGrade <= grade);
            var tier = equipmentGradeConfig.Tier;
            gradeDetail.text = equipmentGradeConfig.Name;
            gradeDetail.color = resources.EquipmentTiers.First(x => x.Tier == tier).Color;
            buttonLuyenKhi.SetInteractable(grade > 0);
        }
    }
}