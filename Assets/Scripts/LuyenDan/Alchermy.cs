using System.Collections.Generic;
using System.Linq;
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
            var result = new LuyenDanResult(true, "Luyện đan thành công", new List<ItemRef>() { new ItemRef(4010,ItemType.LinhDan,20) }, SelectedFurnace.ItemRef.Durability - 20);
            LuyenDanPanel.Instance.OnLuyenDanCompleted(result);
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
            SelectHerbNum.SetText(herb.Name);
            SelectHerbNum.SetActive(true);
        }

        public void SelectDanLu(FurnaceInventoryItemUI furnace)
        {
            SelectedFurnace = furnace;
            selfObject.SetActive(true);
            if (furnace != null)
            {
                furnaceImage.sprite = furnace.Data.BigIcon;
                SetDurabilityText(furnace.ItemRef.Durability);
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
                var itemRef = SelectedFurnace.ItemRef;
                itemRef.SetDurability(newDurability);
                SetDurabilityText(itemRef.Durability);
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

            if (SelectedFurnace != null && SelectedFurnace.ItemRef.Durability <= 0)
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
                int durability = SelectedFurnace.ItemRef.Durability;
                if (durability < max) max = durability;
            }
            if (max == int.MaxValue || max < 1) max = 1;

            var (data,recipe) = LuyenDanPanel.Instance.RecipeListUI.FindBestRecipe(main1, sub1, sub2, primer);
            string lindanName = data != null ? data.LinhDan.Name : "Phế Đan";

            SelectLinhDanNum.SetMax(max);
            SelectLinhDanNum.SetValue(1);
            SelectLinhDanNum.SetText(lindanName);
            SelectLinhDanNum.SetActive(true);
        }
    }
}