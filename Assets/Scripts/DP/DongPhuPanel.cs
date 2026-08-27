using System.Linq;
using System.Threading.Tasks;
using CreateCharacter;
using Network;
using Network.ClientState;
using Network.Extensions;
using Network.Extensions.MemoryPack;
using Network.Models;
using UnityEngine;

namespace DP
{
    public class DongPhuPanel : BaseBehaviour
    {
        public static DongPhuPanel Instance { get; private set; }
        public DongPhuInfo DongPhuInfo { get; private set; }
        private DongPhuShop Shop { get; set; }
        private DongPhuInventory Inventory { get; set; }
        private Client client;
        [field: SerializeField] public InventoryUI InventoryUI { get; set; }
        [field: SerializeField] public GameObject Selected { get; private set; }
        private void Start()
        {
            client = Client.Instance;
            var state = new DongPhuState(this);
            client.ChangeState(state);
            client.Send(MessageTag.GetDongPhuInfo);
        }

        public void OnGetDongPhuInfoResult(GetDongPhuInfoResult result)
        {
            if (!result.Success)
            {
                NotifyGroup.Instance.Show(result.Message);
            }
            else
            {
                Init(result.DongPhuInfo);
            }
        }
        public void HideInventory()
        {
            InventoryUI.Hide();
        }
        
        public void OnPlantResult(PlantResult result)
        {
            NotifyGroup.Instance.Show(result.Message);
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
            HideInventory();
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
        }
        
        public DongPhuLevelConfig GetLevelConfig(DongPhuLevel level)
        {
            return DongPhuInfo.Config.Levels.First(x => x.Level == level);
        }

        public void OpenInventoryWithContext(IOpenInventoryContext context)
        {
            
        }
        public void Init(DongPhuInfo info)
        {
            Shop = null;
            Inventory = null;
            DongPhuInfo = info;
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

    }
}