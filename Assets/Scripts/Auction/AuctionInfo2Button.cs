using CreateCharacter;
using Network;
using Network.Extensions;
using Network.Extensions.MemoryPack;
using Network.Models;
using UnityEngine;
using UnityEngine.UI;

namespace Auction
{
    public class AuctionInfo2Button : BaseBehaviour
    {
        [SerializeField] private Button button;
        [SerializeField] private InfoPanel2 infoPanel;

        private void Start()
        {
            SetEventButton(button, OnClick);
        }

        private void OnDestroy()
        {
            button.onClick.RemoveAllListeners();
        }
        private async void OnClick()
        {
            var client = Client.Instance;
            var reader = await client.RequestWait(MessageTag.GetAuctionInfo2Request);
            var result = reader.MemRead<GetAuctionInfoResult>();
            NetworkManager.Instance.Invoke(() =>
            {
                if (result.Success)
                {
                    infoPanel.Show();
                    infoPanel.Init(result.AuctionInfo);
                }
                else
                {
                    NotifyGroup.Instance.Show(result.Message);
                }
            });
        }
    }
}
