using System.Collections.Generic;
using System.Linq;
using System.Text;
using DG.Tweening;
using LuyenDan.Inventory;
using UnityEngine;
using UnityEngine.UI;

namespace LuyenDan
{
    public class Alchermy : MonoBehaviour
    {
        [SerializeField] private GameObject selfObject;
        [SerializeField] private Image furnaceImage;
        [SerializeField] private Text durabilityText;
        [SerializeField] private Sprite defaultFurnaceSprite;

        [SerializeField] private Ingredient main1;
        [SerializeField] private Ingredient sub1;
        [SerializeField] private Ingredient sub2;
        [SerializeField] private Ingredient primer;
        [SerializeField] private AlchermyFurnace furnaceSlot;
        [field: SerializeField] public SelectNum SelectHerbNum { get; private set; }
        [field: SerializeField] public SelectNum SelectLinhDanNum { get; private set; }
        [SerializeField] private RefineButton refineButton;
        public FurnaceInventoryItemUI SelectedFurnace { get; private set; }
        private AlchemyEntity pendingSelectionTarget;
        private InventoryItemUI pendingSelectionItem;
        private LuyenDanRequest currentRequest;
        private string phedan = "Phế Đan";
        public void SetCurrentSelected(AlchemyEntity target)
        {
            pendingSelectionTarget = target;
        }
        public void ConfirmHerbSelected()
        {
            pendingSelectionTarget?.Select(pendingSelectionItem);
            refineButton.UpdateCanRefine();
        }
        public void ConfirmLinhDanSelected()
        {
            var request = CreateRequest();
            currentRequest = request;
            //Send to server
            var result = new LuyenDanResult(true,
                "Luyện đan thành công",
                new List<ItemRef> { new(4010,ItemType.LinhDan,20),new(4020,ItemType.LinhDan,12) },
                SelectedFurnace.FData.Durability - 20,
                20);
            var progress = LuyenDanPanel.Instance.ProgressBar;
            progress.SetText("Đang tiến hành luyện đan...");
            progress.DoShow(1.75f,0.2f,null);
            DOVirtual.DelayedCall(3, () =>
            {
                progress.DoKill(() => LuyenDanPanel.Instance.OnLuyenDanCompleted(result));
            });
        }

        private LuyenDanRequest CreateRequest()
        {
            var chuduoc = new LuyenDanRequest.Herbs(main1);
            var phuduoc1 = new LuyenDanRequest.Herbs(sub1);
            var phuduoc2 = new LuyenDanRequest.Herbs(sub2);
            var duocdan = new LuyenDanRequest.Herbs(primer);
            var furnaceId = ((FurnaceInventoryItemUI)furnaceSlot.Item).FurnaceId;
            var count = SelectLinhDanNum.GetValue();
            var request = new LuyenDanRequest(chuduoc, phuduoc1, phuduoc2, duocdan, furnaceId,count);
            return request;
        }
        public void Select(InventoryItemUI item)
        {
            pendingSelectionItem = item;
            var count = item.ItemRef.Count;
            if (pendingSelectionTarget is Ingredient ingredient)
            {
                if (ingredient.Item == item)
                {
                    count += ingredient.Count;
                    SelectHerbNum.SetValue(ingredient.Count);
                }
                else
                {
                    SelectHerbNum.SetValue(1);
                }
            }
            SelectHerbNum.SetMax(count);
            var herb = LuyenDanPanel.Instance.Resources.FindHerb(item.ItemRef.Id);

            void UpdateText()
            {
                var value = SelectHerbNum.GetValue();
                var tier = herb.Tier;
                var colorStr = GetColorHexByTier(tier);
                var text = $"<b><color=#{colorStr}>{herb.Name}</color></b> <color=#00ff00>x{value}</color>";
                SelectHerbNum.SetText(text);
            }
            SelectHerbNum.ValueChanged += (value) =>
            {
                UpdateText();
            };
            UpdateText();
            SelectHerbNum.SetActive(true);
        }

        private string GetColorHexByTier(Tier tier)
        {
            var quality = LuyenDanPanel.Instance.Resources.Qualities.FirstOrDefault(q => q.Tier == tier);
            return quality != null ? ColorUtility.ToHtmlStringRGB(quality.Color) : "ffcc00";
        }

        public void SelectDanLu(FurnaceInventoryItemUI furnace)
        {
            SelectedFurnace = furnace;
            selfObject.SetActive(true);
            if (furnace != null)
            {
                furnaceImage.sprite = furnace.Data.BigIcon;
                SetDurabilityText(furnace.FData.Durability);
            }
            else
            {
                furnaceImage.sprite = defaultFurnaceSprite;
                SetDurabilityText(0);
            }
            furnaceSlot.Select(furnace);
            refineButton.UpdateCanRefine();
        }

        public void OnLuyenDanCompleted(int newDurability)
        {
            if (SelectedFurnace != null)
            {
                var data = SelectedFurnace.FData;
                data.SetDurability(newDurability);
                SetDurabilityText(data.Durability);
            }

            if (currentRequest != null)
            {
                void Deduct(Ingredient ingredient)
                {
                    if (ingredient.IsEmpty) return;
                    int consume = ingredient.Count * currentRequest.Count;
                    int remaining = ingredient.Item.ItemRef.Count + ingredient.Count - consume;

                    if (remaining >= ingredient.Count)
                    {
                        ingredient.Item.ItemRef.SetCount(remaining - ingredient.Count);
                    }
                    else
                    {
                        var itemRefObj = ingredient.Item.ItemRef;
                        ingredient.Select(null);
                        itemRefObj.SetCount(Mathf.Max(0, remaining));
                    }
                }
                Deduct(main1);
                Deduct(sub1);
                Deduct(sub2);
                Deduct(primer);
            }

            furnaceSlot.OnLuyenDanCompleted();
            main1.OnLuyenDanCompleted();
            sub1.OnLuyenDanCompleted();
            sub2.OnLuyenDanCompleted();
            primer.OnLuyenDanCompleted();

            if (SelectedFurnace != null && SelectedFurnace.FData.Durability <= 0)
                SelectDanLu(null);
            refineButton.UpdateCanRefine();
        }

        private void SetDurabilityText(int durability)
        {
            durabilityText.text = $"{durability}/100";
        }

        public void SelectReceipe(RecipeData.Recipe data)
        {
            var chuduoc = data.ChuDuoc;
            var phuduoc1 = data.PhuDuoc1;
            var phuduoc2 = data.PhuDuoc2;
            var duocdan = data.DuocDan;
            Select(main1, chuduoc);
            Select(sub1, phuduoc1);
            Select(sub2, phuduoc2);
            Select(primer, duocdan);
            refineButton.UpdateCanRefine();
        }

        private void Select(Ingredient ingredient, RecipeData.Herbs herb)
        {
            if (herb == null)
            {
                ingredient.Select(null);
            }
            else
            {
                var inventory = LuyenDanPanel.Instance.HerbInventory;
                ingredient.Select(inventory.GetItem(herb.Id), herb.Count);
            }
        }
        public void ClearAllSlot()
        {
            main1.Select(null);
            sub1.Select(null);
            sub2.Select(null);
            primer.Select(null);
            refineButton.UpdateCanRefine();
        }

        public bool CanRefine()
        {
            if (furnaceSlot.IsEmpty)
            {
                return false;
            }
            Ingredient[] v = {main1, sub1, sub2, primer};
            return v.Any(x => !x.IsEmpty);
        }

        public void Refine()
        {
            var items = LuyenDanPanel.Instance.HerbInventory.ItemsSnapshot;
            SelectLinhDanNum.SetMin(1);
            int max = int.MaxValue;
            void UpdateMax(Ingredient ingredient)
            {
                if (ingredient == null || ingredient.IsEmpty || ingredient.Count == 0) return;
                var itemObj = items.FirstOrDefault(x => x.Id == ingredient.Item.ItemRef.Id);
                int inventoryCount = itemObj?.Count ?? 0;
                int possible = inventoryCount / ingredient.Count;
                if (possible < max) max = possible;
            }
            UpdateMax(main1);
            UpdateMax(sub1);
            UpdateMax(sub2);
            UpdateMax(primer);

            if (SelectedFurnace != null)
            {
                int durability = SelectedFurnace.FData.Durability;
                if (durability < max) max = durability;
            }
            if (max == int.MaxValue || max < 1) max = 1;

            var (data,recipe) = LuyenDanPanel.Instance.RecipeListUI.FindBestRecipe(main1, sub1, sub2, primer);
            var hasData = data != null;
            string linhdanName = string.Empty;
            int baseDaysRequired = 0;
            Tier linhdanTier = Tier.NhatPham;
            if (hasData)
            {
                var linhdan = data.LinhDan;
                linhdanName = linhdan.Name;
                baseDaysRequired = (int)linhdan.Tier + 3;
                linhdanTier = linhdan.Tier;
            }
            else
            {
                linhdanName = phedan;
                baseDaysRequired = 3;
            }
            void UpdateText()
            {
                var value = SelectLinhDanNum.GetValue();
                var colorStr = hasData ? GetColorHexByTier(linhdanTier) : "aaaaaa";
                var builder = new StringBuilder($"<b><color=#{colorStr}>{linhdanName}</color></b> <color=#00ff00>x{value}</color>");
                builder.AppendLine();
                builder.Append($"<i><color=#aaaaaa>Tiêu hao</color> <color=#ff4444>{baseDaysRequired * value}</color> <color=#aaaaaa>ngày</color></i>");
                SelectLinhDanNum.SetText(builder.ToString());
            }
            SelectLinhDanNum.SetMax(max);
            SelectLinhDanNum.SetValue(1);
            UpdateText();
            SelectLinhDanNum.ValueChanged += (value) => UpdateText();
            SelectLinhDanNum.SetActive(true);
        }
    }
}