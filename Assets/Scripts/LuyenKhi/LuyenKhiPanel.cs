using System.Collections.Generic;
using System.Linq;
using CreateCharacter;
using LuyenDan;
using LuyenDan.Inventory;
using Network;
using Network.Callback;
using Network.Models;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.UI;

namespace LuyenKhi
{
    public class LuyenKhiPanel : BaseBehaviour, IBackContext, ILuyenKhiCallback
    {
        private static LuyenKhiPanel instance;

        public static LuyenKhiPanel Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = FindAnyObjectByType<LuyenKhiPanel>();
                }

                return instance;
            }
        }

        [field: SerializeField] public LuyenKhiResources Resources { get; private set; }
        [field: SerializeField] public SelectTypePage SelectTypePage { get; private set; }
        [field: SerializeField] public LuyenKhiPage LuyenKhiPage { get; private set; }
        [field: SerializeField] public Bag Bag { get; private set; }
        [field: SerializeField] public ResultPage ResultPage { get; private set; }
        [field: SerializeField] public ProgressBar ProgressBar { get; private set; }
        [SerializeField] private Button changeTypeButton;
        [SerializeField] private Button backButton;
        private IBackContext backContext;

        private void Start()
        {
            SetBackContext(this);
            SetEventButton(changeTypeButton, OnChangeType);
            SetEventButton(backButton, OnBack);
            var client = Client.Instance;
            client.ChangeState(new Network.ClientState.LuyenKhi(this));
            var charData = client.CharacterData;
            charData.OnInventoryUpdated += OnInventoryUpdated;
            charData.GetInventory();
        }

        private void OnInventoryUpdated(InventoryManager inventory)
        {
            NetworkManager.Instance.Invoke(() =>
            {
                LuyenKhiPage.Init();
                var items = inventory.Materials.Select(x => x.DeepClone());
                Bag.Init(items);
            });
        }

        public void SetBackContext(IBackContext backContext)
        {
            this.backContext = backContext;
        }

        private void OnChangeType()
        {
            LuyenKhiPage.SetActive(false);
            SelectTypePage.SetActive(true);
            SetBackContext(SelectTypePage);
        }

        private void OnBack()
        {
            backContext.Back();
        }

        public void SelectArtifactType(ArtifactType type)
        {
            SelectTypePage.SetActive(false);
            LuyenKhiPage.SetActive(true);
            LuyenKhiPage.SelectArtifactType(type);
        }

        public void Back()
        {
        }

        public void OnLuyenKhiResult(LuyenKhiResult result)
        {
            NetworkManager.Instance.Invoke(() =>
            {
                NotifyGroup.Instance.Show(result.Message);
                if (result.Success)
                {
                    ProgressBar.DoKill(() => { ResultPage.Show(result.Item); });
                }
                else
                {
                    ProgressBar.DoKill(() => { });
                }
            });
        }

        public void OnSetEquipmentNameResult(SetEquipmentNameResult result)
        {
            NetworkManager.Instance.Invoke(() =>
            {
                NotifyGroup.Instance.Show(result.Message);
                if (result.Success)
                {
                    ResultPage.Hide();
                }
            });
        }

        private void OnDestroy()
        {
            var charData = Client.Instance.CharacterData;
            charData.OnInventoryUpdated -= OnInventoryUpdated;
        }
    }
}