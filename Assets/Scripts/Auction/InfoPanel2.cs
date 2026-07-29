using LongDH.LongDHModules;
using Network.Models;
using UnityEngine;
using UnityEngine.UI;

namespace Auction
{
    public class InfoPanel2 : InfoPanel
    {
        [SerializeField] private Button backButton;

        private void Start()
        {
            SetEventButton(backButton, OnBack);
        }

        private void OnBack()
        {
            Hide();
        }
        private void OnDestroy()
        {
            backButton.onClick.RemoveAllListeners();
        }
        public override void Init(AuctionInfo auctionInfo)
        {
            SetConfirmButtonInteractable(auctionInfo.IsOpen);
            base.Init(auctionInfo);
        }

        protected override void Confirm()
        {
            Observer.Notify(EventList.GamePlay.LOAD_SCENE, "Auction");
        }
    }
}
