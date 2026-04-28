using CreateCharacter;
using Network.Callback;
using Network.Extensions;
using Network.Models;

namespace Network.ClientState
{
    public class LuyenKhi : IClientState
    {
        private ILuyenKhiCallback callback;

        public LuyenKhi(ILuyenKhiCallback callback)
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
            if (tag == MessageTag.LuyenKhiResult)
            {
                var result = reader.Read<LuyenKhiResult>();
                if (result.Success)
                {
                    var charData = Client.Instance.CharacterData;
                    charData.Stats.TuoiTac = result.NewTuoiTac;
                    charData.ApplyInventoryModify(result.Modify);
                }

                callback.OnLuyenKhiResult(result);
            }
            else if (tag == MessageTag.SetEquipmentNameResult)
            {
                var result = reader.Read<SetEquipmentNameResult>();
                if (result.Success)
                {
                    var charData = Client.Instance.CharacterData;
                    var inventory = charData.InventoryManager;
                    var equipment = new Equipment()
                    {
                        Id = result.Id,
                        SelfId = result.SelfId,
                        Name = result.Name
                    };
                    inventory.Update(equipment);
                    charData.InventoryChanged();
                }
                callback.OnSetEquipmentNameResult(result);
            }
        }

        public void OnLeave()
        {
        }
        public MessageState State => MessageState.LuyenKhi;
    }
}