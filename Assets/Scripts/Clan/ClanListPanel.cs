using Network.Core;
using UnityEngine;
using ClanModel = Network.Models.Clans.Clan;

namespace Clan
{
    public class ClanListPanel : ClanPopup
    {
        [SerializeField] private ClanPanel clanPanel;

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
