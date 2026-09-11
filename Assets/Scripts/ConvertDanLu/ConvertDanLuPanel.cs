using System.Linq;
using CreateCharacter;
using LongDHModules;
using LuyenDan;
using Network;
using Network.ClientState;
using Network.Extensions.MemoryPack;
using Network.Models;
using Sirenix.OdinInspector;
using UnityEngine;

namespace ConvertDanLu
{
    /// <summary>
    /// Root panel for converting 4 đan phương (recipes) of any Tier mix into 1 đan lô (furnace).
    /// Result Tier is randomly determined based on the proportion of each Tier placed.
    ///
    /// Source: Client.Instance.CharacterData.InventoryManager.Recipes
    /// Result: Client.Instance.CharacterData.InventoryManager.Furnaces
    /// </summary>
    public class ConvertDanLuPanel : BaseBehaviour
    {
        public static ConvertDanLuPanel Instance { get; private set; }

        [field: SerializeField] public ConvertDanLuPage Page { get; private set; }
        [field: SerializeField] public Bag Bag { get; private set; }
        [field: SerializeField] public ResultPage ResultPage { get; private set; }

        [SerializeField] private ProgressBar progressBar;

        private void Awake()
        {
            Instance = this;
        }

        private void Start()
        {
            var client   = Client.Instance;
            var charData = client.CharacterData;
            client.ChangeState(new ConvertDanLuState());

            charData.OnInventoryUpdated += OnInventoryUpdated;
            Init(charData.InventoryManager);
        }

        private void OnInventoryUpdated(InventoryManager inventoryManager)
        {
            NetworkManager.Instance.Invoke(() => Init(inventoryManager));
        }

        private void Init(InventoryManager inventory)
        {
            // Clone recipes so bag counts don't conflict with live inventory
            var recipes = inventory.Recipes.Select(r => r.DeepClone()).ToHashSet();
            Bag.Init(recipes);
            Page.Init();
        }

        /// <summary>
        /// Sends a ConvertDanLuRequest with the 4 placed recipe ItemRefs.
        /// On success: decreases recipe counts in inventory, adds the resulting furnace.
        /// </summary>
        [Button]
        public async void Convert()
        {
            var filledSlots = Page.PutItems.GetFilledSlots();
            if (filledSlots.Length != PutItems.RequiredCount) return;

            // Group by recipe Id to send compact ItemRefs with counts
            var recipeItems = filledSlots
                .Select(s => s.DanData.ItemRef)
                .GroupBy(r => new { r.Id, r.Type })
                .Select(g => new ItemRef
                {
                    Id    = g.Key.Id,
                    Type  = g.Key.Type,
                    Count = g.Sum(r => r.Count)
                })
                .ToList();

            var request = new ConvertDanLuRequest
            {
                RecipeItems = recipeItems
            };

            var client = Client.Instance;
            progressBar.SetText("Đang chuyển đổi...");
            progressBar.DoShow(0.5f, 0.25f, null);

            var reader = await client.MemRequestWait(MessageTag.ConvertDanLuRequest, request);
            var result = reader.MemRead<ConvertDanLuResult>();

            if (result.Success)
            {
                var inventory = client.CharacterData.InventoryManager;

                // Decrease consumed recipes
                foreach (var consumed in result.RecipeItems)
                {
                    inventory.Decrease(consumed);
                }

                // Add the new furnace
                inventory.Furnaces.Add(result.FurnaceResult);
            }

            NetworkManager.Instance.Invoke(() =>
            {
                if (result.Success)
                {
                    Page.PutItems.Clear();
                    Page.InfoManager.Clear();
                    ResultPage.SetItem(result.FurnaceResult);
                }

                progressBar.DoKill(() =>
                {
                    NotifyGroup.Instance.Show(result.Message);
                    if (result.Success)
                    {
                        ResultPage.Show();
                    }
                });
            });
        }

        private void OnDestroy()
        {
            var charData = Client.Instance?.CharacterData;
            if (charData != null)
            {
                charData.OnInventoryUpdated -= OnInventoryUpdated;
            }

            Instance = null;
            Pool.Get<GameManager>().Cleanup();
        }
    }
}
