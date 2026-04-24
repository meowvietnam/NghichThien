using System;
using System.Collections.Generic;
using System.Linq;
using CreateCharacter;
using LuyenDan.Inventory;
using Network;
using Network.Callback;
using Network.Core;
using Network.Models;
using UnityEngine;
namespace LuyenDan
{
    public class LuyenDanPanel : BaseBehaviour, ILuyenDanCallback
    {
        private static LuyenDanPanel instance;

        public static LuyenDanPanel Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = FindAnyObjectByType<LuyenDanPanel>();
                }

                return instance;
            }
        }

        [field: SerializeField] public LuyenDanResources Resources { get; private set; }
        [field: SerializeField] public FurnaceInventory FurnaceInventory { get; private set; }
        [field: SerializeField] public HerbInventory HerbInventory { get; private set; }
        [field: SerializeField] public AddFurnace AddFurnace { get; private set; }
        [field: SerializeField] public Alchermy Alchermy { get; private set; }
        [field: SerializeField] public RecipeListUI RecipeListUI { get; private set; }
        [field: SerializeField] public AlchermyResult AlchermyResult { get; private set; }
        [field: SerializeField] public ProgressBar ProgressBar { get; private set; }
        [field: SerializeField] public ToolTipItems ToolTipItems { get; private set; }

        private InventoryHandler inventoryHandler;

        private void Start()
        {
            var client = Client.Instance;
            client.ChangeState(new Network.ClientState.LuyenDan(this));
            inventoryHandler = client.CoreModule.GetHandler<InventoryHandler>();
            inventoryHandler.OnInventoryUpdated += OnInventoryUpdated;
            ToolTipItems.Init();
            inventoryHandler.GetInventory();
        }

        private void OnDestroy()
        {
            if (inventoryHandler != null)
                inventoryHandler.OnInventoryUpdated -= OnInventoryUpdated;
        }
        private void OnInventoryUpdated(InventoryManager inventoryManager)
        {
            NetworkManager.Instance.Invoke(() =>
            {
                Alchermy.ClearAllSlot();
                InitInventory(inventoryManager);
            });
        }

        private void InitInventory(InventoryManager inventoryManager)
        {
            var cloned = inventoryManager.DeepClone();
            FurnaceInventory.Init(cloned.Furnaces);
            HerbInventory.Init(cloned.Herbs);
            var datas = cloned.Recipes.Select(x =>
            {
                var recipe = Resources.FindRecipe(x.Id);
                if (recipe == null)
                {
                    Debug.Log($"Recipe {x} not found");
                }
                return recipe;
            }).ToList();
            RecipeListUI.Init(datas);
        }

        public void SelectDanLu(FurnaceInventoryItemUI furnace)
        {
            AddFurnace.SetActive(false);
            FurnaceInventory.Close();
            Alchermy.SelectDanLu(furnace);
        }
        private void OnLuyenDanCompleted(LuyenDanResult result)
        {
            if (result.Success)
            {
                ProgressBar.DoKill(() =>
                {
                    AlchermyResult.SetData(result.ItemRefs);
                    AlchermyResult.SetDescription(result.Message);
                    AlchermyResult.SetActive(true);
                });
            }
            else
            {
                ProgressBar.DoKill(() =>
                {
                    NotifyGroup.Instance.Show(result.Message);
                });
                
            }
        }
        public void LuyenDanResult(LuyenDanResult result)
        {
            NetworkManager.Instance.Invoke(() => { OnLuyenDanCompleted(result); });
        }
    }
}