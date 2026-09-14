using System;
using CreateCharacter;
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

        private bool isOpening;

        private void Start()
        {
            SetEventButton(button, Open);
        }

        private ClanHandler Handler() => Client.Instance.CoreModule.GetHandler<ClanHandler>();

        public async void Open()
        {
            if (isOpening) return;

            var clanHandler = Handler();
            if (clanHandler == null)
            {
                Debug.LogWarning("ClanHandler chưa được khởi tạo", this);
                NotifyGroup.Instance?.Show("Kết nối tông môn chưa sẵn sàng");
                return;
            }

            isOpening = true;
            if (button != null) button.interactable = false;
            try
            {
                var result = await clanHandler.RefreshClan();
                if (!result.Success)
                {
                    NotifyGroup.Instance?.Show(string.IsNullOrWhiteSpace(result.Message)
                        ? "Không thể tải thông tin tông môn"
                        : result.Message);
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
            catch (Exception exception)
            {
                Debug.LogException(exception, this);
                NotifyGroup.Instance?.Show("Không thể tải thông tin tông môn, vui lòng thử lại");
            }
            finally
            {
                isOpening = false;
                if (button != null) button.interactable = true;
            }
        }
    }
}
