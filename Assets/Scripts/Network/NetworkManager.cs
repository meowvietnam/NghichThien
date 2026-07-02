using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using CreateCharacter;
using Network.Callback;
using Network.ClientState;
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
        [field: SerializeField] public string Version { get; private set; }
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
        public async Task Connect(IUnauthenticatedClientCallback callback)
        {
            try
            {
                var tcpClient = new System.Net.Sockets.TcpClient();
                await tcpClient.ConnectAsync(host, port);
                unauthenticatedClient = new UnauthenticatedClient(tcpClient, callback, false);
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

        public void AuthenticatedSuccess(IAuthCallback callback)
        {
            client = new Client(unauthenticatedClient);
            client.ChangeState(new AuthState(callback));
        }

        public void Invoke(Action action)
        {
            lock (actionQueue)
            {
                actionQueue.Enqueue(action);
            }
        }

        [Button]
        public void ShowState()
        {
            Debug.Log($"{GetClientState()}");
        }

        private IClientState GetClientState()
        {
            if(client == null) return null;
            return client.State;
        }
    }
}