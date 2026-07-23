using System.Linq;
using CreateCharacter;
using Network;
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
        public ToolTipItems ToolTipItems => ToolTipItems.Instance;
        [SerializeField] private GameObject selectTypeObject;
        private SkillType currentSelectedSkill = SkillType.ThanThong;
        public string CurrentSelectedSkillName { get; private set; }
        private void Awake()
        {
            Instance = this;
        }
        private void Start()
        {
            SetEventButton(switchButton, OnSwitch);
            Client.Instance.CharacterData.OnInventoryUpdated += OnInventoryUpdated;
            Init(Client.Instance.CharacterData.InventoryManager);
        }

        private void OnInventoryUpdated(InventoryManager inventoryManager)
        {
            NetworkManager.Instance.Invoke(() =>
            {
                Init(inventoryManager);
            });
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
