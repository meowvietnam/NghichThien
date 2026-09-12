using Network.Core;
using Network.Models.Clans;
using UnityEngine;
using ClanModel = Network.Models.Clans.Clan;

namespace Clan
{
    public class CreateClanPanel : ClanPopup
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
