using Network.Core;
using UnityEngine;
using ClanModel = Network.Models.Clans.Clan;

namespace Clan
{
    public class ClanIntroPanel : ClanPopup
    {
        [SerializeField] private CreateClanPanel createClanPanel;
        [SerializeField] private UnityEngine.UI.Button createClanButton;
        [SerializeField] private ClanListPanel clanListPanel;
        [SerializeField] private UnityEngine.UI.Button clanListButton;
        [SerializeField] private ClanPanel clanPanel;

        protected override void Start()
        {
            base.Start();
            SetEventButton(createClanButton, OpenCreateClan);
            SetEventButton(clanListButton, OpenClanList);
        }

        private void OpenCreateClan()
        {
            if (createClanPanel != null) createClanPanel.Show();
        }

        private void OpenClanList()
        {
            if (clanListPanel != null) clanListPanel.Show();
        }

        protected override void BindClanEvents(ClanHandler handler)
        {
            handler.ClanChanged += OnClanChanged;
        }

        protected override void UnbindClanEvents(ClanHandler handler)
        {
            handler.ClanChanged -= OnClanChanged;
        }

        private void OnClanChanged(ClanModel clan)
        {
            if (clan == null) return;
            Hide(0f);
            if (clanPanel != null) clanPanel.Show();
        }
    }
}
