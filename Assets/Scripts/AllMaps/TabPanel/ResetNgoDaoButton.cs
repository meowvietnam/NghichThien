using Base;
using CreateCharacter;
using LongDHModules;
using Network;
using Network.Extensions;
using Network.Extensions.MemoryPack;
using Network.Models;
using UnityEngine;
using UnityEngine.UI;

namespace AllMaps.NgoDaoTabPanel
{
    public class ResetNgoDaoButton : BaseBehaviour
    {
        [SerializeField] private Button button;
        private void Start()
        {
            SetEventButton(button, OnClick);
        }

        private void OnClick()
        {
            var popup = Pool.Get<PopupConfirmAction>();
            popup.ShowPopConfirmAction("Bạn có chắc chắn muốn sử dụng 500 Tiên ngọc để Tái Ngộ Đạo? Sau khi xác nhận, tiến trình Ngộ Đạo hiện tại sẽ được đặt lại và hoàn lại toàn bộ Ngộ đạo điểm.", Confirm);
        }

        private async void Confirm()
        {
            var client = Client.Instance;
            var reader = await client.RequestWait(MessageTag.ResetNgoDaoRequest);
            var result = reader.MemRead<ResetNgoDaoResult>();
            var notifyGroup = NotifyGroup.Instance;
            notifyGroup?.Show(result.Message);
            if (result.Success)
            {
                var charData = client.CharacterData;
                charData.MoneyVip = result.MoneyVip;
                charData.Progression.NgoDaoDiem = result.NewNgoDaoDiem;
                charData.InventoryManager.NgoDaos = result.NgoDaos;
                charData.Stats.ApplyFrom(result.Stats);
                Pool.Get<TabPanel>().InitNgoDao();
            }
        }
    }
}
