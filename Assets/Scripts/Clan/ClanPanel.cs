using System.Collections.Generic;
using Network.Core;
using Network.Models.Clans;
using ClanModel = Network.Models.Clans.Clan;

namespace Clan
{
    public class ClanPanel : ClanPopup
    {
        public ClanModel Data => Handler?.CurrentClan;

        protected override void BindClanEvents(ClanHandler handler)
        {
            handler.ClanChanged += OnClanChanged;
            handler.MemberAdded += OnMemberAdded;
            handler.MemberRemoved += OnMemberRemoved;
            handler.ContributionChanged += OnContributionChanged;
            handler.ShopChanged += OnShopChanged;
            handler.MemberPointsChanged += OnMemberPointsChanged;
        }

        protected override void UnbindClanEvents(ClanHandler handler)
        {
            handler.ClanChanged -= OnClanChanged;
            handler.MemberAdded -= OnMemberAdded;
            handler.MemberRemoved -= OnMemberRemoved;
            handler.ContributionChanged -= OnContributionChanged;
            handler.ShopChanged -= OnShopChanged;
            handler.MemberPointsChanged -= OnMemberPointsChanged;
        }

        protected override void OnShown()
        {
            OnClanChanged(Data);
        }

        protected virtual void OnClanChanged(ClanModel clan)
        {
            if (clan == null)
            {
                Hide();
                return;
            }
            
        }

        protected virtual void OnMemberAdded(ClanMember member)
        {
        }

        protected virtual void OnMemberRemoved(ClanRemoveMember removed)
        {
        }

        protected virtual void OnContributionChanged(ClanContribution contribution)
        {
        }

        protected virtual void OnShopChanged(IReadOnlyList<ClanShopItem> items)
        {
        }

        protected virtual void OnMemberPointsChanged(ClanMemberPointsChanged changed)
        {
        }
    }
}
