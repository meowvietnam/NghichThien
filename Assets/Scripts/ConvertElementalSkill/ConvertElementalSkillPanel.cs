using System.Linq;
using System.Threading.Tasks;
using CreateCharacter;
using LuyenDan;
using Network;
using Network.ClientState;
using Network.Extensions.MemoryPack;
using Network.Models;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.UI;

namespace ConvertElementalSkill
{
    public class ConvertElementalSkillPanel : BaseBehaviour
    {
        [SerializeField] private Button switchButton;
        public static ConvertElementalSkillPanel Instance { get; private set; }
        [field: SerializeField] public ConvertElementalSkillPage Page { get; private set; }
        [field: SerializeField] public Bag Bag { get; private set; }
        [field: SerializeField] public ResultPage ResultPage { get; private set; }
        public ToolTipItems ToolTipItems => ToolTipItems.Instance;
        [SerializeField] private GameObject selectTypeObject;
        [SerializeField] private ProgressBar progressBar;
        private SkillType currentSelectedSkill = SkillType.ThanThong;
        public string CurrentSelectedSkillName { get; private set; }

        private void Awake()
        {
            Instance = this;
        }

        private void Start()
        {
            var client = Client.Instance;
            var charData = client.CharacterData;
            client.ChangeState(new ConvertElementalSkillState());
            SetEventButton(switchButton, OnSwitch);
            charData.OnInventoryUpdated += OnInventoryUpdated;
        }

        private void OnInventoryUpdated(InventoryManager inventoryManager)
        {
            NetworkManager.Instance.Invoke(() => { Init(inventoryManager); });
        }

        private void OnSwitch()
        {
            selectTypeObject.SetActive(true);
            Page.SetActive(false);
        }

        public void OnSelectSkillType(SelectSkillType selectSkillType)
        {
            selectTypeObject.SetActive(false);
            Page.SetActive(true);
            Page.SetIcon(selectSkillType.Icon);
            Page.SetName(selectSkillType.Name);
            currentSelectedSkill = selectSkillType.SkillType;
            CurrentSelectedSkillName = selectSkillType.Name;
            var inventory = Client.Instance.CharacterData.InventoryManager;
            Init(inventory);
        }
        [Button]
        public async void Convert()
        {
            var availableSlots = Page.PutItems.GetAvailableSlots();
            var itemConverts = availableSlots.Select(x => x.SkillData.ItemRef)
                .GroupBy(x => new {x.Id,x.Type})
                .Select(x => new ItemRef()
                {
                    Id = x.Key.Id,
                    Type = x.Key.Type,
                    Count = (ushort)x.Sum(y => y.Count)
                })
                .ToHashSet();
            var request = new Network.Models.ConvertElementalSkill()
            {
                ItemConverts = itemConverts,
            };
            var client = Client.Instance;
            progressBar.SetText("Đang chuyển đổi...");
            progressBar.DoShow(0.5f, 0.25f, null);
            var reader = await client.MemRequestWait(MessageTag.ConvertElementalSkillRequest, request);
            var result = reader.MemRead<ConvertElementalSkillResult>();
            if (result.Success)
            {
                var items = result.ItemConverts;
                var inventory = client.CharacterData.InventoryManager;
                foreach (var item in items)
                {
                    inventory.Decrease(item);
                }

                inventory.Add(result.ItemResult);
            }
            NetworkManager.Instance.Invoke(() =>
            {
                if (result.Success)
                {
                    Page.PutItems.Clear();
                    Page.RateManager.Clear();
                    Bag.AddItem(result.ItemResult);
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

        private void Init(InventoryManager inventory)
        {
            var itemSet = currentSelectedSkill == SkillType.ThanThong ? inventory.ThanThongs : inventory.CongPhaps;
            var items = itemSet.Select(x => x.DeepClone()).ToHashSet();
            Bag.Init(items);
            Page.Init();
        }

        private void OnDestroy()
        {
            switchButton.onClick.RemoveAllListeners();
            var charData = Client.Instance.CharacterData;
            if (charData != null)
            {
                charData.OnInventoryUpdated -= OnInventoryUpdated;
            }

            Instance = null;
        }
    }
}