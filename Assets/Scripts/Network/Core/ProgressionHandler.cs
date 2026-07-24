using CreateCharacter;
using LongDH.LongDHModules;
using Network.Extensions;
using Network.Extensions.MemoryPack;
using Network.Models;

namespace Network.Core
{
    public class ProgressionHandler : IClientHandler
    {
        public void OnEnter()
        {
            Observer.AddObserver(EventList.Network.SPECIAL_LEVELUP, SpecialLevelUp);
        }

        private async void SpecialLevelUp(object[] data)
        {
            var callback = data[0] as INetworkEvent.ISpecialLevelUp;
            var client = Client.Instance;
            var reader = await client.RequestWait(MessageTag.SpecialLevelUpRequest);
            var result = reader.MemRead<SpecialLevelUpResult>();
            if (result.Success)
            {
                var snapshot = result.StatsSnapshot;
                var charData = client.CharacterData;
                charData.Stats.ApplyFrom(snapshot);
                var inventory = charData.InventoryManager;
                var buffItems = result.BuffItems;
                foreach (var buffItem in buffItems)
                {
                    inventory.Decrease(buffItem);
                }
                var camNgos = result.CamNgos;
                inventory.CamNgos =  camNgos;
                var currentExp = result.CurrentExp;
                charData.Progression.CurrentExp = currentExp;
            }
            NetworkManager.Instance.Invoke(() =>
            {
                if (result.Success)
                {
                    callback?.SuccessSpecialLevelUp();
                }
                else
                {
                    callback?.FailSpecialLevelUp(result.Message);
                }
            });
        }

        public void Handle(Message<MessageTag> message)
        {
            var tag = message.Tag;
            var reader = message.Reader;
            var charData = Client.Instance.CharacterData;
            if (tag == MessageTag.LevelUpdated)
            {
                var levelUpdated = reader.MemRead<LevelUpdated>();
                var snapshot = new CharacterSnapshot(charData);
                levelUpdated.ApplyTo(charData);
                NetworkManager.Instance.Invoke(() => { Observer.Notify(EventList.Network.LEVEL_UP, snapshot); });
            }
            else if (tag == MessageTag.ExperienceReward)
            {
                var experienceReward = reader.MemRead<ExperienceReward>();
                charData.Progression.CurrentExp = experienceReward.NewExp;
                NetworkManager.Instance.Invoke(() =>
                {
                    var notifyGroup = NotifyGroup.Instance;
                    if (notifyGroup != null)
                    {
                        notifyGroup.Show($"Bạn nhận được {experienceReward.AddExp} tu vi!");
                        Observer.Notify(EventList.GamePlay.UPDATE_PLAYER_STATS);
                    }
                });
            }
        }

        public void OnLeave()
        {
            Observer.RemoveObserver(EventList.Network.SPECIAL_LEVELUP, SpecialLevelUp);
        }

        public void OnDisconnect(IProtocol<MessageTag> protocol)
        {
        }

        private static readonly MessageTag[] handler = { MessageTag.LevelUpdated , MessageTag.ExperienceReward };
        public MessageTag[] HandleTag => handler;
    }
}