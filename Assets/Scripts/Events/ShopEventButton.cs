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
        protected async override  void OnClick()
        {
            var reader = await Client.Instance.MemRequestWait(MessageTag.GetEventRequest, EventId);
            var result = reader.MemRead<GetEventResult>();
            if (result.Success)
            {
                var @event = result.Event;
                if (currentPanel == null)
                {
                    currentPanel = Instantiate(panelPrefab, containerPanel);
                }
                currentPanel.Init(@event);
                currentPanel.Show();
            }
            else
            {
                NotifyGroup.Instance.Show(result.Message);
            }
        }
    }
}
