using DG.Tweening;
using UnityEngine;

namespace DP
{
    public class InventoryUI : BaseBehaviour
    {
        private IOpenInventoryContext context;
        [SerializeField] private DongPhuSeedUI seedUIPrefab;
        [SerializeField] private RectTransform container;
        private Tween tween;
        public void Init(DongPhuInventory inventory)
        {
            var seeds = inventory.Seeds;
            foreach (var seed in seeds)
            {
                OnAddSeed(seed);
            }

            inventory.OnAddSeed += OnAddSeed;
        }
        private void OnAddSeed(Seed seed)
        {
            var seedUi =  Instantiate(seedUIPrefab, container);
            seedUi.Init(seed.HerbId, seed.Count);
            seed.OnCountUpdated += (count) =>
            {
                if (count > 0)
                {
                    seedUi.SetCount(count);
                }
                else
                {
                    Destroy(seedUi.gameObject);
                }
            };
        }

        public void ShowInventory(IOpenInventoryContext context)
        {
            this.context = context;
        }
        public void Hide()
        {
            context?.OnClose();       
        }
    }
}