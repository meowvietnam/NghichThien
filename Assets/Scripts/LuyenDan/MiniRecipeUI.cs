using System.Linq;
using UnityEngine;
using UnityEngine.UI;

namespace LuyenDan
{
    public class MiniRecipeUI : MonoBehaviour
    {
        [SerializeField] private Toggle toggle;
        [SerializeField] private Text chuduoc;
        [SerializeField] private Text phuduoc;
        [SerializeField] private Text thuocdan;
        [SerializeField] private GameObject craftale;
        private RecipeData.Recipe data;
        public void Init(RecipeData.Recipe data, ToggleGroup toggleGroup)
        {
            this.data = data;
            toggle.group = toggleGroup;
            toggle.onValueChanged.AddListener(OnValueChanged);
            var r = LuyenDanPanel.Instance.Resources;
            var chuduocRef = data.ChuDuoc;
            var chuduoc = r.FindHerb(chuduocRef.Id);
            this.chuduoc.text = $"{chuduoc.Name}x{chuduocRef.Count}";
            RecipeData.Herbs[] phuduocRefs = { data.PhuDuoc1, data.PhuDuoc2 };
            var phuduocs = phuduocRefs
                .Where(x => x != null && x.Id != 0)
                .Select(x => new
                {
                    PhuDuoc = r.FindHerb(x.Id),
                    Ref = x
                })
                .ToArray();
            if (phuduocs.Any(i => i.PhuDuoc == null))
            {
                Debug.Log("Null phuduoc");
            }
            this.phuduoc.text = string.Join(", ", phuduocs.Select(x => $"{x.PhuDuoc.Name}x{x.Ref.Count}"));
            var thuocdanRef = data.DuocDan;
            var thuocdan = r.FindHerb(thuocdanRef.Id);
            this.thuocdan.text = $"{thuocdan.Name}x{thuocdanRef.Count}";
            
        }

        public void UpdateCraftale(bool isCraftable)
        {
            craftale.SetActive(isCraftable);
        }
        private void OnValueChanged(bool active)
        {
        }
    }
}