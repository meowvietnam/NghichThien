using System;
using System.Collections.Generic;
using System.Linq;
using CreateCharacter;
using LuyenDan.Inventory;
using Network;
using Network.Callback;
using Network.Core;
using Network.Models;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

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
        private void Start()
        {
            var client = Client.Instance;
            client.ChangeState(new Network.ClientState.LuyenDan(this));
            var inventoryHandler = client.CoreModule.GetHandler<InventoryHandler>();
            inventoryHandler.GetInventory(OnGetInventoryResult);
        }

        private void OnGetInventoryResult(GetInventoryResult result)
        {
            if (!result.Success)
            {
                NotifyGroup.Instance.Show(result.Message);
                return;
            }
            var inventoryManager = result.InventoryManager.DeepClone();
            var myFurnaces = inventoryManager.Furnaces;
            var myHerbs = inventoryManager.Herbs;
            var myRecipes = inventoryManager.Recipes;
            FurnaceInventory.Init(myFurnaces);
            HerbInventory.Init(myHerbs);
            var datas = myRecipes.Select(x =>
            {
                var recipe = Resources.FindRecipe(x.Id);
                if (recipe == null)
                {
                    Debug.Log($"Recipe {x} not found");
                }
                return recipe;
            }).ToList();
            RecipeListUI.Init(datas);
            ToolTipItems.Init();
        }

        public void SelectDanLu(FurnaceInventoryItemUI furnace)
        {
            AddFurnace.SetActive(false);
            FurnaceInventory.Close();
            Alchermy.SelectDanLu(furnace);
        }
        public void OnLuyenDanCompleted(LuyenDanResult result)
        {
            if (result.Success)
            {
                Alchermy.OnLuyenDanCompleted(result.NewDurability);
                HerbInventory.Refresh();
                RecipeListUI.Refresh();
                AlchermyResult.SetData(result.ItemRefs);
                AlchermyResult.SetDescription(result.Message);
                AlchermyResult.SetActive(true);
            }
            else
            {
                NotifyGroup.Instance.Show(result.Message);
            }
        }
    }
}
