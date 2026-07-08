using Network;
using Network.Callback;
using Network.Models;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Start
{
    public class StartScene : MonoBehaviour, IUnauthenticatedClientCallback
    {
        [SerializeField] private Text statusText;
        [SerializeField] private string conneting;
        [SerializeField] private string checkingVersion;
        [SerializeField] private string disconnectMessage;
        private NetworkManager Net => NetworkManager.Instance;

        private void Start()
        {
            statusText.text = conneting;
            _ = Net.Connect(this);
        }

        public void SetStatusText(string text)
        {
            statusText.text = text;
        }
        public void OnConnectResult(ConnectResult connectResult)
        {
            Net.Invoke(() =>
            {
                if (connectResult.Success)
                {
                    Net.SetIdentity(connectResult.Id, connectResult.Key);
                    statusText.text = checkingVersion;
                }
                else
                {
                    statusText.text = connectResult.Message;
                }
            });
        }

        public void OnCheckVersionResult(CheckVersionResult result)
        {
            Net.Invoke(() =>
            {
                statusText.text = result.Message;
                if (result.Success)
                {
                    Net.AuthenticatedSuccess();
                    SceneManager.LoadSceneAsync("MainMenu");
                }
            });
        }
        public void OnDisconnect()
        {
            Net.Invoke(() => { statusText.text = disconnectMessage; });
        }
    }
}