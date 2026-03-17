using System;
using System.Collections.Generic;
using System.Linq;
using LuyenDan.Inventory;
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
        [field: SerializeField] public Inventory.Inventory FurnaceInventory { get; private set; }
        [field: SerializeField] public Inventory.Inventory HerbInventory { get; private set; }
        [field: SerializeField] public AddFurnace AddFurnace { get; private set; }
        [field: SerializeField] public Alchermy Alchermy { get; private set; }
        [field: SerializeField] public RecipeListUI RecipeListUI { get; private set; }
        
        [SerializeField] public List<ItemRef> myFurnaces;
        [SerializeField] public List<ItemRef> myHerbs;

        [Button]
        private void GenMyHerbs()
        {
            myHerbs = Resources.HerbDatas
                .Where(x => Random.value > 0.2f)
                .Select(x =>
                {
                    var id = x.Id;
                    var count = Random.Range(1, 20);
                    var type = ItemType.Herb;
                    var @ref = new ItemRef(id, type, count);
                    return @ref;
                }).ToList();
        }
        private void Start()
        {
            FurnaceInventory.Init(myFurnaces);
            HerbInventory.Init(myHerbs);
        }

        public void SelectDanLu(FurnaceInventoryItemUI furnace)
        {
            AddFurnace.SetActive(false);
            FurnaceInventory.Close();
            Alchermy.SelectDanLu(furnace);
        }
    }
}
