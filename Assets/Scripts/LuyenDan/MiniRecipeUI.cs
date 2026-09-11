using System.Collections.Generic;
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
        private bool isCraftable;

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
                .Where(x => x != null && !x.IsDefault())
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

            phuduoc.text = string.Join(", ", phuduocs.Select(x => $"{x.PhuDuoc.Name}x{x.Ref.Count}"));
            var thuocdanRef = data.DuocDan;
            var thuocdan = r.FindHerb(thuocdanRef.Id);
            this.thuocdan.text = $"{thuocdan.Name}x{thuocdanRef.Count}";
        }

        public bool IsCraftable()
        {
            var chuduoc = data.ChuDuoc;
            var phuduoc1 = data.PhuDuoc1;
            var phuduoc2 = data.PhuDuoc2;
            var duocdan = data.DuocDan;
            var items = LuyenDanPanel.Instance.HerbInventory.ItemsSnapshot;
            if (!IsSatisfied(chuduoc, items))
            {
                return false;
            }

            if (!IsSatisfied(phuduoc1, items))
            {
                return false;
            }

            if (!IsSatisfied(phuduoc2, items))
            {
                return false;
            }

            if (!IsSatisfied(duocdan, items))
            {
                return false;
            }

            return true;
        }

        private bool IsSatisfied(RecipeData.Herbs herbs, List<ItemSnapshot> items)
        {
            if (herbs == null || herbs.IsDefault())
            {
                return true;
            }

            var item = items.FirstOrDefault(x => x.Id == herbs.Id);
            if (item == null)
            {
                return false;
            }

            return item.Count >= herbs.Count;
        }

        public void UpdateCraftale(bool isCraftable)
        {
            this.isCraftable = isCraftable;
            craftale.SetActive(isCraftable);
        }

        private void OnValueChanged(bool active)
        {
            if (!isCraftable)
            {
                return;
            }

            if (active)
            {
                LuyenDanPanel.Instance.Alchermy.SelectReceipe(data);
            }
        }
    }
}