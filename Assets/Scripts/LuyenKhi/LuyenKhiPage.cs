using System.Linq;
using Network;
using Network.Models;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.UI;
using Network.Extensions;
using CreateCharacter;
namespace LuyenKhi
{
    public class LuyenKhiPage : MonoBehaviour
    {
        [SerializeField] private GameObject selfObject;
        [SerializeField] private Image iconImage;
        [SerializeField] private Text grade;
        [SerializeField] private Text gradeDetail;
        [SerializeField] private ButtonLuyenKhi buttonLuyenKhi;
        [SerializeField] private PutMaterials putMaterials;
        [field: SerializeField, ReadOnly] public ArtifactType CurrentSelectedType { get; private set; }

        public void Init()
        {
            SetGrade(0);
            putMaterials.Init();
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
            putMaterials.Recalculate();
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

        public void LuyenKhi()
        {
            var materials = putMaterials.GetSelectedMaterials();
            if (materials.Length == 0)
            {
                NotifyGroup.Instance.Show("Hãy chọn ít nhất 1 nguyên liệu");
                return;
            }
            var materialIds = materials.Select(x => (ushort)x.Id).ToArray();
            var request = new LuyenKhiRequest()
            {
                SelectedType = CurrentSelectedType,
                MaterialIds = materialIds,
            };
            var progressBar = LuyenKhiPanel.Instance.ProgressBar;
            Client.Instance.Send(MessageTag.LuyenKhi, request);
            progressBar.SetText("Đang luyện khí...");
            progressBar.DoShow(0.25f,0.15f,null);
        }
    }
}