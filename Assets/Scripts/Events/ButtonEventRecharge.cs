using System;
using CreateCharacter;
using Network;
using Network.Extensions.MemoryPack;
using Network.Models;
using UnityEngine;
using UnityEngine.UI;

namespace Events
{
    public class ButtonEventRecharge : ButtonEvent
    {
        [SerializeField] private EventRechargePanel panelPrefab;
        private EventRechargePanel currentPanel;

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