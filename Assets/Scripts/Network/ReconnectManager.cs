using Network.Callback;
using Network.Models;
using UnityEngine;
using System;
namespace Network
{
    public class ReconnectManager : MonoBehaviour,IUnauthenticatedClientCallback
    {
        [SerializeField] public TryReconnect[] tryReconnects;
        private int currentTry = 0;
        public void OnConnectResult(ConnectResult connectResult)
        {
            
        }
        public void OnCheckVersionResult(CheckVersionResult result)
        {
        }
        public void OnReconnectResult(Result result)
        {
            
        }
        public void OnDisconnect()
        {
            
        }
        public void Register(Client client)
        {
            client.OnDisconnect += OnDisconnect;
        }
    }
}
