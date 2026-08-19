using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using CreateCharacter;
using Firebase.Analytics;
using LuyenDan;
using Network.Callback;
using Network.ClientState;
using Network.Core;
using Network.Extensions.MemoryPack;
using Network.Models;
using Newtonsoft.Json;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Network
{
    public class NetworkManager : MonoBehaviour
    {
        [ShowInInspector, ReadOnly] private string host => serverConfig?.Host ?? string.Empty;
        [ShowInInspector, ReadOnly] private int port => serverConfig?.Port ?? 0;
        [SerializeField] private ServerConfig serverConfig;
        [SerializeField] private string connectFailMessage;
        [SerializeField] private float connectTimeoutSeconds = 10f;
#if UNITY_IPHONE
        [ShowInInspector, ReadOnly] private string platform => "ios";
#else
        [ShowInInspector, ReadOnly] private string platform => "apk";
#endif
        [ShowInInspector] public string Version => $"{platform}_v{Application.version}";
        [SerializeField] private ReconnectManager reconnectManager;

        private Queue<Action> actionQueue = new();
        public static NetworkManager Instance { get; private set; }
        private UnauthenticatedClient unauthenticatedClient;
        private Client client;
        [ShowInInspector, ReadOnly] public int Id { get; private set; }
        [ShowInInspector, ReadOnly] public string Key { get; private set; }

        [ShowInInspector, TabGroup("Character"), HideLabel, InlineProperty]
        public CharacterData CharacterData
        {
            get => client?.CharacterData;
            private set { }
        }

        private void Awake()
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            StartCoroutine(LoadGameManager());
        }

        private IEnumerator LoadGameManager()
        {
            var req = Resources.LoadAsync<GameManager>("GameManager");
            yield return req;
            var prefab = req.asset as GameManager;
            InstantiateAsync(prefab);
        }

        public async Task Connect(IUnauthenticatedClientCallback callback, bool reconnect = false)
        {
            System.Net.Sockets.TcpClient tcpClient = null;
            try
            {
                unauthenticatedClient = null;
                tcpClient = new System.Net.Sockets.TcpClient();
                var connectTask = tcpClient.ConnectAsync(host, port);
                var timeoutMilliseconds = Mathf.Max(1, connectTimeoutSeconds) * 1000f;
                var completedTask = await Task.WhenAny(connectTask, Task.Delay((int)timeoutMilliseconds));
                if (completedTask != connectTask)
                {
                    tcpClient.Close();
                    _ = connectTask.ContinueWith(task => { }, TaskContinuationOptions.OnlyOnFaulted);
                    throw new TimeoutException($"Kết nối quá thời gian chờ sau {connectTimeoutSeconds:0.##} giây");
                }

                await connectTask;
                unauthenticatedClient = new UnauthenticatedClient(tcpClient, callback, reconnect);
            }
            catch (Exception e)
            {
                tcpClient?.Close();
                callback.OnConnectResult(new ConnectResult() { Success = false, Message = connectFailMessage });
            }
        }

        public bool SendReconnect(Reconnect reconnect)
        {
            if (unauthenticatedClient == null)
            {
                return false;
            }

            unauthenticatedClient.TcpClient.MemSend(MessageTag.Reconnect, reconnect);
            return true;
        }

        private void Update()
        {
            while (actionQueue.Count > 0)
            {
                actionQueue.Dequeue().Invoke();
            }
        }

        public void SetIdentity(int id, string key)
        {
            Id = id;
            Key = key;
        }
        
        public void AuthenticatedSuccess()
        {
            client = new Client(unauthenticatedClient);
            reconnectManager.Register(client);
        }

        public void ReconnectSuccess()
        {
            client.TcpClient.Reconnect(unauthenticatedClient.TcpClient);
        }
    
        public void Invoke(Action action)
        {
            lock (actionQueue)
            {
                actionQueue.Enqueue(action);
            }
        }
        private IClientState GetClientState()
        {
            if (client == null) return null;
            return client.State;
        }
    }
}