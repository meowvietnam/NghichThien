using System.Linq;
using System.Threading.Tasks;
using CreateCharacter;
using LongDHModules;
using LuyenDan;
using Network;
using Network.ClientState;
using Network.Extensions.MemoryPack;
using Network.Models;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.UI;

namespace ConvertDanLu
{
    /// <summary>
    /// Root panel for converting 4 đan lô (furnaces) of the same Tier into 1 đan phương (recipe).
    /// Mirrors ConvertElementalSkillPanel, adapted for Furnace → Recipe conversion.
    /// 
    /// Scene wiring:
    ///   - Page:       ConvertDanLuPage (holds the 4 slots + info manager)
    ///   - Bag:        Bag (scroll list of owned furnaces)
    ///   - ResultPage: ResultPage (pop-up showing the received recipe)
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
            // Each furnace is unique by SelfId — pass them all directly
            var furnaces = inventory.Furnaces.Select(f => f).ToHashSet();
            Bag.Init(furnaces);
            Page.Init();
        }

        /// <summary>
        /// Sends a ConvertDanLuRequest with the 4 placed furnace SelfIds.
        /// On success: removes the furnaces from inventory, adds the recipe,
        ///             clears the slots and shows the result page.
        /// </summary>
        [Button]
        public async void Convert()
        {
            var filledSlots = Page.PutItems.GetFilledSlots();
            if (filledSlots.Length != PutItems.RequiredCount) return;

            var selfIds = filledSlots
                .Select(s => (ushort)s.DanData.ItemRef.Id)
                .ToList();

            var request = new ConvertDanLuRequest
            {
                FurnaceSelfIds = selfIds
            };

            var client = Client.Instance;
            progressBar.SetText("Đang chuyển đổi...");
            progressBar.DoShow(0.5f, 0.25f, null);

            var reader = await client.MemRequestWait(MessageTag.ConvertDanLuRequest, request);
            var result = reader.MemRead<ConvertDanLuResult>();

            if (result.Success)
            {
                var inventory = client.CharacterData.InventoryManager;

                // Remove consumed furnaces by SelfId
                foreach (var selfId in result.FurnaceSelfIds)
                {
                    inventory.RemoveFurnace(selfId);
                }

                // Add the new recipe to inventory
                inventory.Add(result.ItemResult);
            }

            NetworkManager.Instance.Invoke(() =>
            {
                if (result.Success)
                {
                    Page.PutItems.Clear();
                    Page.InfoManager.Clear();
                    ResultPage.SetItem(result.ItemResult);
                }

                var message = result.Message;
                progressBar.DoKill(() =>
                {
                    NotifyGroup.Instance.Show(message);
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
