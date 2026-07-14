using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using CreateCharacter;
using Network.Callback;
using Network.ClientState;
using Network.Core;
using Network.Models;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Network
{
    public class NetworkManager : MonoBehaviour
    {
        [SerializeField] private string host;
        [SerializeField] private int port;
        [SerializeField] private string connectFailMessage;
        [SerializeField] private string platform;
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
            private set
            {
            }
        }
        private void Awake()
        {
            Debug.Log($"Main thread id: {Thread.CurrentThread.ManagedThreadId}");
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
            try
            {
                var tcpClient = new System.Net.Sockets.TcpClient();
                await tcpClient.ConnectAsync(host, port);
                unauthenticatedClient = new UnauthenticatedClient(tcpClient, callback, reconnect);
            }
            catch (Exception e)
            {
                Debug.Log(e);
                callback.OnConnectResult(new ConnectResult() { Success = false, Message = connectFailMessage });
            }
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
            if(client == null) return null;
            return client.State;
        }
    }
}