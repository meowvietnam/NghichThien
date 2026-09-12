using Network;
using Network.Core;
using UnityEngine;

namespace Clan
{
    public class ClanEntryController : BaseBehaviour
    {
        [SerializeField] private UnityEngine.UI.Button button;
        [SerializeField] private ClanIntroPanel clanIntroPanel;
        [SerializeField] private ClanPanel clanPanel;

        private void Start()
        {
            SetEventButton(button, Open);
        }

        private ClanHandler Handler() => Client.Instance.CoreModule.GetHandler<ClanHandler>();

        public void Open()
        {
            var clanHandler = Handler();
            if (clanHandler == null)
            {
                Debug.LogWarning("ClanHandler chưa được khởi tạo", this);
                return;
            }

            if (clanHandler.CurrentClan != null)
            {
                if (clanPanel != null) clanPanel.Show();
            }
            else
            {
                if (clanIntroPanel != null) clanIntroPanel.Show();
            }
        }
    }
}
