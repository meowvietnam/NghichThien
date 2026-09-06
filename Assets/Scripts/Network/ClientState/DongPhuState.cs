using CreateCharacter;
using DP;
using LongDH.LongDHModules;
using Network.Extensions.MemoryPack;
using Network.Models;

using Application = UnityEngine.Application;

namespace Network.ClientState
{
    public class DongPhuState : IClientState
    {
        private DongPhuPanel panel;
        public DongPhuState(DongPhuPanel panel)
        {
            this.panel = panel;
        }
        public void OnEnter()
        {
        }

        public void Handle(Message<MessageTag> message)
        {
            var tag = message.Tag;
            var reader =  message.Reader;
            NetworkManager.Instance.Invoke(() =>
            {
                OnMessage(tag, reader);
            });
        }

        private void OnMessage(MessageTag tag, IReader reader)
        {
            if (tag == MessageTag.GetDongPhuInfoResult)
            {
                var result = reader.MemRead<GetDongPhuInfoResult>();
                panel.OnGetDongPhuInfoResult(result);
            }
            else if (tag == MessageTag.PlantResult)
            {
                var result = reader.MemRead<PlantResult>();
                panel.OnPlantResult(result);
            }
            else if (tag == MessageTag.PlantNotify)
            {
                var notify = reader.MemRead<PlantNotify>();
                panel.OnPlantNotify(notify);
            }
            else if(tag == MessageTag.HarvestSlotResult)
            {
                var result = reader.MemRead<HarvestSlotResult>();
                panel.OnHarvestSlotResult(result);
            }
            else if(tag == MessageTag.HarvetSlotNotify)
            {
                var notify = reader.MemRead<HarvestSlotNotify>();
                panel.OnHarvestSlotNotify(notify);
            }
            else if(tag == MessageTag.WateringHelpResult)
            {
                var result = reader.MemRead<WateringHelpResult>();
                panel.OnWateringHelpResult(result);
            }
            else if(tag == MessageTag.WateringHelpNotify)
            {
                var notify = reader.MemRead<WateringHelpNotify>();
                panel.OnWateringHelpNotify(notify);
            }
            else if(tag == MessageTag.LikeDongPhuResult)
            {
                var result = reader.MemRead<LikeDongPhuResult>();
                panel.OnLikeDongPhuResult(result);
            }
            else if(tag == MessageTag.LikeDongPhuNotify)
            {
                var notify = reader.MemRead<LikeDongPhuNotify>();
                panel.OnLikeDongPhuNotify(notify);
            }
            else if(tag == MessageTag.UpgradeDongPhuResult)
            {
                var result = reader.MemRead<UpgradeDongPhuResult>();
                panel.OnUpgradeDongPhuResult(result);
            }
            else if(tag == MessageTag.UpgradeDongPhuNotify)
            {
                var notify = reader.MemRead<UpgradeDongPhuNotify>();
                panel.OnUpgradeDongPhuNotify(notify);
            }
            else if(tag == MessageTag.ToAllMapFromDongPhu)
            {
                Observer.Notify(EventList.GamePlay.LOAD_SCENE, "AllMaps");
            }
            else if (tag == MessageTag.OpenLink)
            {
                var model = reader.MemRead<OpenLink>();
                var link = model.Link;
                Application.OpenURL(link);
            }
            else if (tag == MessageTag.ReloadDpShopResult)
            {
                var result = reader.MemRead<ReloadDpShopResult>();
                panel.OnReloadShopResult(result);
            }
        }
        public void OnLeave()
        {
        }

        public MessageState State => MessageState.DongPhuState;
    }
}