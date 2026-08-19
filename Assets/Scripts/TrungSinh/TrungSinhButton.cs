using CreateCharacter;
using LongDHModules;
using Network;
using Network.Extensions;
using Network.Extensions.MemoryPack;
using Network.Models;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace TrungSinh
{
    public class TrungSinhButton : BaseBehaviour
    {
        [SerializeField] private Button button;

        private void Start()
        {
            SetEventButton(button, ShowTrungSinhPanel);
        }

        private async void ShowTrungSinhPanel()
        {
            var client = Client.Instance;
            var reader = await client.RequestWait(MessageTag.GetTrungSinhConfigRequest);
            var getResult = reader.MemRead<GetTrungSinhConfig>();
            if (!getResult.Success)
            {
                NotifyGroup.Instance.Show(getResult.Message);
                return;
            }

            Pool.Get<PopupConfirmAction>().ShowPopConfirmAction(getResult.Config.GetText(), ToTrungSinh);
        }

        private async void ToTrungSinh()
        {
            var client = Client.Instance;
            var reader = await client.RequestWait(MessageTag.ToTrungSinhRequest);
            var result = reader.MemRead<Result>();
            if (result.Success)
            {
                SceneManager.LoadScene("TrungSinh");
            }
            else
            {
                NotifyGroup.Instance.Show(result.Message);
            }
        }
    }
}