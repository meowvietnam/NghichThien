using CreateCharacter;
using Network;
using Network.Extensions.MemoryPack;
using Network.Models;
using UnityEngine;

namespace Events
{
    public class MilestoneEventButton : ButtonEvent
    {
        [SerializeField] private MilestoneEventPanel panelPrefab;
        private MilestoneEventPanel currentPanel;

        protected override void OnClick()
        {
            GetEvent();
        }

        private async void GetEvent()
        {
            var reader = await Client.Instance.MemRequestWait(MessageTag.GetEventRequest, EventId);
            var result = reader.MemRead<GetEventResult>();
            if (result.Success)
            {
                if (panelPrefab == null)
                {
                    panelPrefab = EventManager.Instance.MilestoneEventPanel;
                }

                if (currentPanel == null)
                {
                    currentPanel = Instantiate(panelPrefab, containerPanel);
                }

                currentPanel.Init(result.Event);
                currentPanel.Show();
            }
            else
            {
                NotifyGroup.Instance.Show(result.Message);
            }
        }
    }
}