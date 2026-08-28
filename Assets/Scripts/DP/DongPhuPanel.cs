using System;
using System.Linq;
using System.Threading.Tasks;
using Base;
using ConvertElementalSkill;
using CreateCharacter;
using DG.Tweening;
using LuyenDan;
using Network;
using Network.ClientState;
using Network.Extensions;
using Network.Extensions.MemoryPack;
using Network.Models;
using Unity.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace DP
{
    [Serializable]
    public class DongPhuBackground
    {
        [field:SerializeField] public DongPhuLevel Level { get; private set; }
        [field: SerializeField] public Sprite Background { get; private set; }
    }
    public class DongPhuPanel : BaseBehaviour
    {
        public static DongPhuPanel Instance { get; private set; }
        public DongPhuInfo DongPhuInfo { get; private set; }
        private DongPhuShop Shop { get; set; }
        private DongPhuInventory Inventory { get; set; }
        private Client client;
        [field: SerializeField] public InventoryUI InventoryUI { get; set; }
        [field: SerializeField] public GameObject Selected { get; private set; }
        [field: SerializeField] public ButtonGroup ButtonGroup { get; private set; }
        [SerializeField] private Text textInfo;
        [SerializeField] private GameObject likeButton;
        [SerializeField] private GardenUI gardenUI;
        [SerializeField] private Text likeText;
        [SerializeField] private Shop shop;
        [SerializeField] private Button closeButton;
        [field: SerializeField] public SelectNum SelectNum { get; private set; }
        [SerializeField] private ProgressBar progressBar;
        [SerializeField] private VisitDongPhuPanel visitDongPhuPanel;
        [field: SerializeField] public NotifyGroup NotifyGroup { get; private set; }
        [field: SerializeField] public ToolTipItems ToolTipItems { get; private set; }
        [SerializeField] private Image background;
        [SerializeField] private DongPhuBackground[] backgrounds;
        public SlotUI CurrentSelectedSlot { get; private set; }

        private void Start()
        {
            Instance = this;
            ToolTipItems.Init();
            progressBar.SetText("Đang tải Động phủ...");
            progressBar.DoShow(0.25f, 0.15f, null);
            SetEventButton(closeButton, Close);
            client = Client.Instance;
            var state = new DongPhuState(this);
            client.ChangeState(state);
            client.Send(MessageTag.GetDongPhuInfo);
        }

        private void UpdateLevel()
        {
            var slots = DongPhuInfo.DongPhu.Garden.Slots;
            var config = DongPhuInfo.Config;
            var level = DongPhuInfo.DongPhu.Level;
            UpdateTextInfo();
            foreach (var slot in slots)
            {
                if (slot.IsLocked)
                {
                    if (config.IsLevelGreaterOrEqual(level, slot.UnlockLevel))
                    {
                        slot.SetIsLocked(false);
                        var slotUI = gardenUI.FindSlot(slot.SlotIndex);
                        slotUI.PlayEffect();
                    }
                }
            }
        }

        private void Close()
        {
            shop.Hide();
            InventoryUI.Hide();
            closeButton.gameObject.SetActive(false);
        }

        public bool CanWateringHelp()
        {
            var level = GetLevelConfig(DongPhuInfo.MyLevel);
            return DongPhuInfo.CurrentWateringHelpCount < level.MyMaxWaterHelpPerDay;
        }

        public void OnGetDongPhuInfoResult(GetDongPhuInfoResult result)
        {
            if (!result.Success)
            {
                NotifyGroup.Show(result.Message);
            }
            else
            {
                Init(result.DongPhuInfo);
            }
        }

        public void HideInventory()
        {
            InventoryUI.Hide();
            closeButton.gameObject.SetActive(true);
        }

        public DongPhuLevelConfig? GetMyNextLevel()
        {
            var levels = DongPhuInfo.Config.Levels;
            var levelIndex = Array.FindIndex(levels, x => x.Level == DongPhuInfo.MyLevel);
            if (levelIndex < 0)
            {
                return null;
            }

            var nextLevelIndex = levelIndex + 1;
            if (nextLevelIndex >= levels.Length)
            {
                return null;
            }

            return levels[nextLevelIndex];
        }

        public void OnPlantResult(PlantResult result)
        {
            NotifyGroup.Show(result.Message);
            if (!result.Success)
            {
                return;
            }

            var updatedSeed = result.UpdatedSeed;
            if (Inventory.Seeds.TryGetValue(updatedSeed, out var seed))
            {
                seed.SetCount(updatedSeed.Count);
                if (seed.Count <= 0)
                {
                    Inventory.Seeds.Remove(updatedSeed);
                }
            }

            Selected.gameObject.SetActive(false);
            OnPlant(result.PlantSeed);
        }

        public void OnPlantNotify(PlantNotify notify)
        {
            OnPlant(notify.PlantSeed);
        }

        private void OnPlant(PlantSeed plantSeed)
        {
            var garden = DongPhuInfo.DongPhu.Garden;
            garden.Seeds.Add(plantSeed);
            var slot = garden.GetSlot(plantSeed.SlotIndex);
            slot.InvokeOnPlant(plantSeed);
            var slotUI = gardenUI.FindSlot(plantSeed.SlotIndex);
            slotUI.PlayEffect();
        }

        public DongPhuLevelConfig GetLevelConfig(DongPhuLevel level)
        {
            return DongPhuInfo.Config.Levels.First(x => x.Level == level);
        }

        public void SetCurrentSelectedSlot(SlotUI slotUI)
        {
            CurrentSelectedSlot = slotUI;
            if (slotUI == null)
            {
                Selected.SetActive(false);
                ButtonGroup.Hide();
                return;
            }

            var hasSeed = slotUI.HasSeed();
            if (hasSeed)
            {
                ButtonGroup.Show();
            }
            else
            {
                ButtonGroup.Hide();
            }

            var selectedTf = Selected.transform;
            selectedTf.SetParent(slotUI.transform);
            selectedTf.localPosition = Vector3.zero;
            Selected.SetActive(true);
        }

        public async void OpenInventoryWithContext(IOpenInventoryContext context)
        {
            if (Inventory == null)
            {
                var inventory = await GetInventory();
                InventoryUI.Init(inventory);
            }

            shop.Hide();
            InventoryUI.Show(context).OnComplete(() => { closeButton.gameObject.SetActive(true); });
        }

        public void UpdateTextInfo()
        {
            var dongPhu = DongPhuInfo.DongPhu;
            var level = GetLevelConfig(dongPhu.Level);
            textInfo.text =
                $"Động phủ của <color=#FFD166>{dongPhu.Name}</color> [<color=#{ColorUtility.ToHtmlStringRGB(level.Color)}>{level.Name}</color>]";
            
            var bg = backgrounds.FirstOrDefault(x => x.Level == dongPhu.Level);
            if (bg == null)
            {
                return;
            }
            background.sprite = bg.Background;
            Debug.Log(textInfo.text);
        }

        public void Init(DongPhuInfo info)
        {
            progressBar.DoKill(() => { });
            Shop = null;
            Inventory = null;
            DongPhuInfo = info;
            UpdateTextInfo();
            UpdateLike();
            gardenUI.Init(DongPhuInfo.DongPhu.Garden);
            ButtonGroup.Init(IsMe());
        }

        private void UpdateLike()
        {
            var canLike = DongPhuInfo.CanLike;
            likeButton.SetActive(canLike);
            var like = DongPhuInfo.DongPhu.LikeCount;
            likeText.text = Utils.FormatCount(like);
        }

        public bool IsMe()
        {
            return DongPhuInfo.DongPhu.CharId == client.CharacterData.Id;
        }

        public async Task<DongPhuShop> GetShop()
        {
            if (Shop == null)
            {
                var reader = await client.RequestWait(MessageTag.GetDongPhuShopRequest);
                Shop = reader.MemRead<DongPhuShop>();
            }

            return Shop;
        }

        public async Task<DongPhuInventory> GetInventory()
        {
            if (Inventory == null)
            {
                var reader = await client.RequestWait(MessageTag.GetDongPhuInventoryRequest);
                Inventory = reader.MemRead<DongPhuInventory>();
            }

            return Inventory;
        }

        public void OnHarvestSlotResult(HarvestSlotResult result)
        {
            NotifyGroup.Show(result.Message);
            if (result.Success)
            {
                var charData = Client.Instance.CharacterData;
                var rewards = result.Rewards;
                foreach (var reward in rewards)
                {
                    reward.Claim(charData);
                }

                InventoryUI.UpdateMoney();
                OnHarvest(result.SlotIndex);
                ButtonGroup.UpdateUI();
            }
        }

        public void OnHarvestSlotNotify(HarvestSlotNotify notify)
        {
            OnHarvest(notify.SlotIndex);
        }

        private void OnHarvest(ushort slotIndex)
        {
            var garden = DongPhuInfo.DongPhu.Garden;
            var slot = garden.GetSlot(slotIndex);
            var seed = garden.GetPlantSeed(slotIndex);
            if (slot == null)
            {
                Debug.Log("Slot null!!!");
                return;
            }

            if (seed == null)
            {
                Debug.Log("Seed null!");
                return;
            }

            garden.Seeds.Remove(seed);
            slot.InvokeOnHarvest(seed);
            var slotUI = gardenUI.FindSlot(slotIndex);
            slotUI.PlayEffect();
        }

        public void OnWateringHelpNotify(WateringHelpNotify notify)
        {
            OnWateringHelp(notify.SlotIndex, notify.ReduceTime);
        }

        public void OnWateringHelpResult(WateringHelpResult result)
        {
            NotifyGroup.Show(result.Message);
            if (result.Success)
            {
                var charData = Client.Instance.CharacterData;
                var rewards = result.Rewards;
                foreach (var reward in rewards)
                {
                    reward.Claim(charData);
                }

                DongPhuInfo.CurrentWateringHelpCount = result.CurrentWateringHelpCount;
                InventoryUI.UpdateMoney();
                OnWateringHelp(result.SlotIndex, result.ReduceTime);
                ButtonGroup.UpdateUI();
            }
        }

        private void OnWateringHelp(ushort slotIndex, TimeSpan reduceTime)
        {
            var garden = DongPhuInfo.DongPhu.Garden;
            var slot = garden.GetSlot(slotIndex);
            var seed = garden.GetPlantSeed(slotIndex);
            if (slot == null)
            {
                Debug.Log("Slot null!!!");
                return;
            }

            if (seed == null)
            {
                Debug.Log("Seed null!");
                return;
            }

            seed.EstimateHarvestAt -= reduceTime;
            var slotUI = gardenUI.FindSlot(slotIndex);
            slotUI.PlayEffect();
        }

        public async void OnBuySeedResult(BuySeedResult result)
        {
            NotifyGroup.Show(result.Message);
            if (result.Success)
            {
                if (Inventory == null)
                {
                    await GetInventory();
                    InventoryUI.Init(Inventory);
                }

                var charData = Client.Instance.CharacterData;
                charData.Stats.LinhThach = result.NewLinhThach;
                InventoryUI.UpdateMoney();
                var seed = result.Seed;
                var updatedSeed = result.UpdatedSeed;
                Inventory.AddOrUpdate(seed);
                Shop.UpdateSeed(updatedSeed);
            }
        }

        public async void OpenShop()
        {
            await GetShop();
            shop.Init(Shop);
            InventoryUI.Hide();
            shop.Show().OnComplete(() => { closeButton.gameObject.SetActive(true); });
        }

        public void CloseShop()
        {
            shop.Hide();
            closeButton.gameObject.SetActive(false);
        }

        public void OnLikeDongPhuResult(LikeDongPhuResult result)
        {
            if (result.Success)
            {
                DongPhuInfo.CanLike = result.CanLike;
                DongPhuInfo.DongPhu.LikeCount = result.NewLike;
                UpdateLike();
            }
            else
            {
                NotifyGroup.Show(result.Message);
            }
        }

        public void OnLikeDongPhuNotify(LikeDongPhuNotify notify)
        {
            DongPhuInfo.DongPhu.LikeCount = notify.NewLike;
            UpdateLike();
        }

        public void OnUpgradeDongPhuResult(UpgradeDongPhuResult result)
        {
            NotifyGroup.Show(result.Message);
            if (result.Success)
            {
                DongPhuInfo.MyLevel = result.NewLevel;
                if (IsMe())
                {
                    DongPhuInfo.DongPhu.Level = result.NewLevel;
                }

                client.CharacterData.MoneyVip = result.NewMoneyVip;
                InventoryUI.UpdateMoney();
                UpdateLevel();
            }
        }

        public void OnUpgradeDongPhuNotify(UpgradeDongPhuNotify notify)
        {
            DongPhuInfo.DongPhu.Level = notify.NewLevel;
            UpdateLevel();
        }

        public async void Visit(string dongPhuName)
        {
            var request = new VisitDongPhu()
            {
                Name = dongPhuName
            };
            var client = Client.Instance;
            progressBar.SetText("Đang tìm Động phủ...");
            progressBar.DoShow(0.25f, 0.15f, null);
            var reader = await client.MemRequestWait(MessageTag.VisitDongPhuRequest, request);
            progressBar.DoKill(() => { });
            var result = reader.MemRead<VisitDongPhuResult>();
            if (result.Success)
            {
                visitDongPhuPanel.AddHistory(dongPhuName);
                SceneManager.LoadScene("DongPhu");
            }
            else
            {
                NotifyGroup.Show(result.Message);
            }
        }

        public void OnReloadShopResult(ReloadDpShopResult result)
        {
            NotifyGroup.Show(result.Message);
            if (result.Success)
            {
                Shop = result.Shop;
                shop.Init(Shop);
            }
        }
    }
}