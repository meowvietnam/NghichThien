using System.Collections.Generic;
using System.Text;
using Base;
using CreateCharacter;
using CreateCharacter.CharStats;
using CreateCharacter.States;
using LongDH.LongDHModules;
using LongDHModules;
using Network;
using Network.Extensions;
using Network.Extensions.MemoryPack;
using Network.Models;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace TrungSinh
{
    public class TrungSinhSystem : AbstractCreateCharacterSystem
    {
        [SerializeField] private SkinResources skinResources;
        private TrungSinhConfig config;

        private void Start()
        {
            Pool.Get<UIManager>().gameObject.SetActive(false);
            Client.Instance.ChangeState(new Network.ClientState.TrungSinh());
            GetTrungSinhConfig();
        }

        private async void GetTrungSinhConfig()
        {
            var client = Client.Instance;
            var reader = await client.RequestWait(MessageTag.GetTrungSinhConfigRequest);
            var result = reader.MemRead<GetTrungSinhConfig>();
            if (result.Success)
            {
                config = result.Config;
                var charData = client.CharacterData;
                createCharacterPanel.Init(new SelectStatsState(createCharacterPanel), skinResources, config.ThienPhu,
                    charData, config.Stats);
            }
            else
            {
                NotifyGroup.Instance.Show(result.Message);
            }
        }

        public override void BackToSelectChar()
        {
        }

        public override void OnSetNameResult(SetCharNameResult result)
        {
        }

        public override void Final(List<SelectionModel> selectionModels)
        {
            var stringBuilder = new StringBuilder();
            var money = createCharacterPanel.SelectStats.Money;
            if (money > 0)
            {
                stringBuilder.AppendLine($"Bạn vẫn còn {money} điểm chưa sử dụng");
            }

            stringBuilder.AppendLine($"Sử dụng {Utils.FormatCount(config.MoneyVip)} Tiên ngọc để trùng sinh?");
            popupConfirmAction.ShowPopConfirmAction(stringBuilder.ToString(), () => OnConfirm(selectionModels));
        }

        private async void OnConfirm(List<SelectionModel> selectionModels)
        {
            var client = Client.Instance;
            progressBar.SetText("Đang trùng sinh");
            progressBar.DoShow(0.5f, 0.5f, null);
            var reader = await client.MemRequestWait(MessageTag.TrungSinhRequest, selectionModels);
            var result = reader.MemRead<TrungSinhResult>();
            progressBar.DoKill(() =>
            {
                if (result.Success)
                {
                    var charData = result.Wrapper.Character;
                    client.SetChar(charData);
                    Back();
                }
                else
                {
                    NotifyGroup.Instance.Show(result.Message);
                }
            });
        }

        public override void OnApplyStatsResult(ApplyStatsResult result)
        {
        }

        public override void ToSetPlayerNamePanel(IPanel panel)
        {
        }

        private void Back()
        {
            Pool.Get<UIManager>().gameObject.SetActive(true);
            Observer.Notify(EventList.GamePlay.LOAD_SCENE, "AllMaps");
        }

        public override void SelectStatsBack()
        {
            Back();
        }
    }
}