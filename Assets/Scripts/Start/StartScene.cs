using DG.Tweening;
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
            Application.targetFrameRate = 60;
        }

        public void SetStatusText(string text)
        {
            statusText.text = text;
        }

        public void OnConnectResult(ConnectResult connectResult)
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
        }

        private (int delay, string message)? ParseMessage(string text)
        {
            if (!text.StartsWith('['))
            {
                return null;
            }

            int end = text.IndexOf(']');
            if (end < 0)
            {
                return null;
            }

            if (!int.TryParse(text.Substring(1, end - 1), out var delay))
            {
                return null;
            }

            string message = text[(end + 1)..].Trim();
            return (delay, message);
        }

        public void OnCheckVersionResult(CheckVersionResult result)
        {
            var parse = ParseMessage(result.Message);
            statusText.text = !parse.HasValue ? result.Message : parse.Value.message;
            if (result.Success)
            {
                var delay = parse?.delay ?? 0;
                DOVirtual.DelayedCall(delay, () =>
                {
                    Net.AuthenticatedSuccess();
                    SceneManager.LoadSceneAsync("MainMenu");
                });
            }
        }

        public void OnDisconnect()
        {
            statusText.text = disconnectMessage;
        }

        public void OnReconnectResult(Result result)
        {
        }
    }
}