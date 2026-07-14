using CreateCharacter;
using Network.Callback;
using Network.Models;
using UnityEngine;
using LuyenDan;
using Sirenix.OdinInspector;
using UnityEngine.UI;

namespace Network
{
    public class ReconnectManager : BaseBehaviour, IUnauthenticatedClientCallback
    {
        [SerializeField] private bool autoReconnect;
        [SerializeField] private NetworkManager networkManager;
        [SerializeField] private GameObject connectionCanvas;
        [SerializeField] private ProgressBar progressBar;
        [SerializeField] private Button exitGameButton;
        [SerializeField] private string disconnectMessage;
        [SerializeField] private string reconnectingMessage = "Đang kết nối lại...";
        [SerializeField] private string reconnectAuthenticatingMessage = "Đang xác thực kết nối...";
        [SerializeField] private string reconnectWaitingResultMessage = "Đang chờ phản hồi từ máy chủ...";
        [SerializeField] private string reconnectRetryMessage = "Thử kết nối lại sau giây lát...";
        [SerializeField] private string reconnectSuccessMessage = "Kết nối lại thành công";
        [SerializeField] private string reconnectFailedMessage = "Không thể kết nối lại";
        [SerializeField] private int maxTry;
        [SerializeField] private float retryDelay;
        private int currentTry;
        private bool isReconnecting;
        private bool waitingReconnectResult;
        private Client registeredClient;

        private void Awake()
        {
            DontDestroyOnLoad(connectionCanvas);
            SetEventButton(exitGameButton, ExitGame);
            exitGameButton?.gameObject.SetActive(false);
        }

        private void ExitGame()
        {
            Application.Quit();
        }

        public void OnConnectResult(ConnectResult connectResult)
        {
            Debug.Log(connectResult.Message);
            if (!isReconnecting || !waitingReconnectResult)
            {
                return;
            }

            if (connectResult.Success)
            {
                return;
            }

            waitingReconnectResult = false;
            RetryOrFail(connectResult.Message);
        }

        public void OnCheckVersionResult(CheckVersionResult result)
        {
        }

        public void OnReconnectResult(Result result)
        {
            if (!isReconnecting)
            {
                return;
            }

            waitingReconnectResult = false;
            if (result.Success)
            {
                CompleteReconnect(result.Message);
                return;
            }

            RetryOrFail(result.Message);
        }

        public void OnDisconnect()
        {
            if (Client.Instance != null && Client.Instance.IsLogout)
            {
                return;
            }

            if (isReconnecting)
            {
                if (waitingReconnectResult)
                {
                    SetProgressText(reconnectWaitingResultMessage);
                }

                return;
            }

            ShowConnectionUi(disconnectMessage);
            if (autoReconnect)
            {
                StartReconnect();
            }
        }

        public void Register(Client client)
        {
            if (registeredClient != null)
            {
                registeredClient.OnDisconnect -= OnDisconnect;
            }

            registeredClient = client;
            client.OnDisconnect += OnDisconnect;
        }

        public void StartReconnect()
        {
            if (isReconnecting)
            {
                return;
            }

            isReconnecting = true;
            currentTry = 0;
            ShowConnectionUi(reconnectingMessage);
            exitGameButton?.gameObject.SetActive(false);
            progressBar?.DoShow(0.5f, 0, null);
            StartReconnectAttempt();
        }

        private async void StartReconnectAttempt()
        {
            if (!isReconnecting)
            {
                return;
            }

            var totalTry = maxTry > 0 ? maxTry : 3;
            if (currentTry >= totalTry)
            {
                FailReconnect(reconnectFailedMessage);
                return;
            }

            currentTry++;
            waitingReconnectResult = true;
            SetProgressText(BuildProgressMessage(reconnectingMessage, currentTry, totalTry));
            await GetNetworkManager().Connect(this, true);

            if (!isReconnecting || !waitingReconnectResult)
            {
                return;
            }

            SetProgressText(BuildProgressMessage(reconnectAuthenticatingMessage, currentTry, totalTry));
            var net = GetNetworkManager();
            var request = new Reconnect(net.Id, net.Key);
            if (!net.SendReconnect(request))
            {
                waitingReconnectResult = false;
                RetryOrFail(reconnectFailedMessage);
                return;
            }

            SetProgressText(BuildProgressMessage(reconnectWaitingResultMessage, currentTry, totalTry));
        }

        private void RetryOrFail(string message)
        {
            var totalTry = maxTry > 0 ? maxTry : 3;
            if (currentTry >= totalTry)
            {
                FailReconnect(string.IsNullOrEmpty(message) ? reconnectFailedMessage : message);
                return;
            }

            var retryMessage = string.IsNullOrEmpty(message) ? reconnectRetryMessage : $"{message}\n{reconnectRetryMessage}";
            SetProgressText(BuildProgressMessage(retryMessage, currentTry + 1, totalTry));
            Invoke(nameof(StartReconnectAttempt), retryDelay);
        }

        private void CompleteReconnect(string message)
        {
            CancelInvoke(nameof(StartReconnectAttempt));
            isReconnecting = false;
            waitingReconnectResult = false;
            currentTry = 0;
            SetProgressText(string.IsNullOrEmpty(message) ? reconnectSuccessMessage : message);
            exitGameButton?.gameObject.SetActive(false);
            progressBar?.DoKill(HideConnectionUi);
            Notify(string.IsNullOrEmpty(message) ? reconnectSuccessMessage : message);
        }

        private void FailReconnect(string message)
        {
            CancelInvoke(nameof(StartReconnectAttempt));
            isReconnecting = false;
            waitingReconnectResult = false;
            SetProgressText(string.IsNullOrEmpty(message) ? reconnectFailedMessage : message);
            connectionCanvas?.SetActive(true);
            progressBar?.SetActive(true);
            exitGameButton?.gameObject.SetActive(true);
            progressBar?.DoKill(() => { }, false);
            Notify(string.IsNullOrEmpty(message) ? reconnectFailedMessage : message);
        }

        private void ShowConnectionUi(string message)
        {
            connectionCanvas?.SetActive(true);
            progressBar?.SetActive(true);
            exitGameButton?.gameObject.SetActive(false);
            SetProgressText(message);
        }

        private void HideConnectionUi()
        {
            connectionCanvas?.SetActive(false);
        }

        private void SetProgressText(string message)
        {
            if (progressBar != null)
            {
                progressBar.SetText(message);
            }
        }

        private string BuildProgressMessage(string message, int tryIndex, int totalTry)
        {
            return $"{message} ({tryIndex}/{totalTry})";
        }

        private NetworkManager GetNetworkManager()
        {
            return networkManager != null ? networkManager : NetworkManager.Instance;
        }

        private void Notify(string message)
        {
            if (!string.IsNullOrEmpty(message) && NotifyGroup.Instance != null)
            {
                NotifyGroup.Instance.Show(message);
            }
        }

        private void OnDestroy()
        {
            CancelInvoke(nameof(StartReconnectAttempt));
            if (registeredClient != null)
            {
                registeredClient.OnDisconnect -= OnDisconnect;
            }
        }
    }
}
