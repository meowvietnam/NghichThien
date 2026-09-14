using System.Threading.Tasks;
using CreateCharacter;
using Network;
using Network.Extensions.MemoryPack;
using Network.Models;
using UnityEngine;

namespace Events
{
    public class ShopEventButton : ButtonEvent
    {
        [SerializeField] private ShopEventPanel panelPrefab;
        private ShopEventPanel currentPanel;

        protected override void OnClick()
        {
            _ = ShowEvent();
        }

        public async Task<bool> ShowEvent(bool notifyOnFailure = true)
        {
            var reader = await Client.Instance.MemRequestWait(MessageTag.GetEventRequest, EventId);
            var result = reader.MemRead<GetEventResult>();
            if (result.Success)
            {
                var @event = result.Event;
                if (@event?.GameEvent is not EventShop)
                {
                    return false;
                }

                if (currentPanel == null)
                {
                    currentPanel = Instantiate(panelPrefab, containerPanel);
                }

                currentPanel.Init(@event);
                currentPanel.Show();
                return true;
            }

            if (notifyOnFailure)
            {
                NotifyGroup.Instance.Show(result.Message);
            }

            return false;
        }
    }
}
