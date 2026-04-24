using System;
using System.Collections.Generic;
using System.Linq;
using CreateCharacter;
using LuyenDan.Inventory;
using Network;
using Sirenix.OdinInspector;
using UnityEngine;
using Random = UnityEngine.Random;

namespace LuyenDan
{
    public class LuyenDanPanel : MonoBehaviour
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
        // [Button]
        // private void GenMyHerbs()
        // {
        //     var myHerbs = Resources.HerbDatas
        //         .Where(x => Random.value > 0.2f)
        //         .Select(x =>
        //         {
        //             var id = x.Id;
        //             var count = Random.Range(1, 20);
        //             var type = ItemType.Herb;
        //             var @ref = new ItemRef(id, type, count);
        //             return @ref;
        //         }).ToList();
        // }
        private void Start()
        {
            var inventoryManager = Client.Instance.CharacterData.InventoryManager;
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
