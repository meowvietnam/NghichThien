using System;
using System.Linq;
using CreateCharacter;
using LongDH.LongDHModules;
using LuyenDan;
using Network;
using Network.Extensions.MemoryPack;
using Network.Models;
using UnityEngine;

namespace LuyenKhi
{
    public class RandomLuyenKhi : MonoBehaviour
    {
        [SerializeField] private SelectNum selectNum;
        [SerializeField] private TierToggle[] tierToggles;
        [SerializeField] private ProgressBar progressBar;

        private void Start()
        {
            selectNum.confirmAction.AddListener(OnConfirm);
            selectNum.SetMin(1);
            selectNum.SetMax(100);
            selectNum.SetValue(1);
            OnValueChanged(selectNum.GetValue());
        }

        private void OnValueChanged(int value)
        {
            var text = $"Luyện ngẫu nhiên {value} lần, hãy chọn độ hiếm vật liệu";
            selectNum.SetText(text);
        }

        private async void OnConfirm()
        {
            var tiers = tierToggles.Where(x => x.IsSelected()).Select(x => x.Tier).ToHashSet();
            Debug.Log(string.Join(", ", tiers));
            var count = selectNum.GetValue();
            var selectedType = LuyenKhiPanel.Instance.LuyenKhiPage.CurrentSelectedType;
            var request = new RandomLuyenKhiRequest()
            {
                Count = count,
                Tiers = tiers,
                SelectedType = selectedType
            };
            var client = Client.Instance;
            progressBar.SetText("Đang luyện khí...");
            progressBar.DoShow(0.25f, 0.15f, null);
            var reader = await client.MemRequestWait(MessageTag.RandomLuyenKhiRequest, request);
            var result = reader.MemRead<RandomLuyenKhiResult>();
            if (result.Success)
            {
                var charData = client.CharacterData;
                charData.Stats.TuoiTac = result.TuoiTac;
                Observer.Notify(EventList.GamePlay.UPDATE_PLAYER_STATS);
                charData.GetInventory();
            }

            progressBar.DoKill(() => { NotifyGroup.Instance.Show(result.Message); });
        }

        public void Show()
        {
            selectNum.ValueChanged += OnValueChanged;
            selectNum.Show(0.25f);
        }
    }
}