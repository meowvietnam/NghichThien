using System.Linq;
using System.Threading.Tasks;
using CreateCharacter;
using Network;
using Network.ClientState;
using Network.Extensions;
using Network.Extensions.MemoryPack;
using Network.Models;

namespace DP
{
    public class DongPhuPanel : BaseBehaviour
    {
        public static DongPhuPanel Instance { get; private set; }
        public DongPhuInfo DongPhuInfo { get; private set; }
        private DongPhuShop Shop { get; set; }
        private DongPhuInventory Inventory { get; set; }
        private Client client;
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

        public DongPhuLevelConfig GetLevelConfig(DongPhuLevel level)
        {
            return DongPhuInfo.Config.Levels.First(x => x.Level == level);
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
