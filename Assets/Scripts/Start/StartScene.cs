using Network;
using Network.Callback;
using Network.Models;
using UnityEngine;
using UnityEngine.UI;

namespace Start
{
    public class StartScene : MonoBehaviour, IUnauthenticatedClientCallback
    {
        [SerializeField] private Text statusText;
        [SerializeField] private string conneting;
        [SerializeField] private string checkingVersion;
        [SerializeField] private string versionValid;
        [SerializeField] private string versionInvalid;
        [SerializeField] private string disconnectMessage;
        [SerializeField] private AuthService authService;
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

        public void OnCheckVersionResult(bool result)
        {
            Net.Invoke(() =>
            {
                if (result)
                {
                    statusText.text = versionValid;
                    Net.AuthenticatedSuccess(authService);
                    authService.Login();
                }
                else
                {
                    statusText.text = versionInvalid;
                }
            });
        }
        public void OnDisconnect()
        {
            Net.Invoke(() => { statusText.text = disconnectMessage; });
        }
    }
}