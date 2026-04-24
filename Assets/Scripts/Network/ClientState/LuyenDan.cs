using Network.Callback;
using Network.Core;
using Network.Extensions;
using Network.Models;

namespace Network.ClientState
{
    public class LuyenDan : IClientState
    {
        private ILuyenDanCallback callback;
        public LuyenDan(ILuyenDanCallback callback)
        {
            this.callback = callback;
        }
        public void OnEnter()
        {
        }

        public void Handle(Message<MessageTag> message)
        {
            var tag = message.Tag;
            var reader = message.Reader;
            if (tag == MessageTag.LuyenDanResult)
            {
                var result = reader.Read<LuyenDanResult>();
                if (result.Success)
                {
                    var client = Client.Instance;
                    client.CharacterData.Stats.TuoiTac = result.NewTuoiTac;
                    var inventoryHandler = client.CoreModule.GetHandler<InventoryHandler>();
                    inventoryHandler.SetInventory(result.Inventory);
                }
                callback.LuyenDanResult(result);
            }
        }

        public void OnLeave()
        {
        }

        public MessageState State => MessageState.LuyenDan;
    }
}