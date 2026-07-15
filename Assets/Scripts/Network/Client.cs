using System;
using CreateCharacter;
using Network.ClientState;
using Network.Core;
using Network.Extensions;
using UnityEngine;

namespace Network
{
    public class Client : ClientConnection, IRequester<MessageTag>
    {
        public static Client Instance { get; private set; }
        private MessageStateHandler<MessageTag, MessageState> stateModule;
        public CoreMessageHandler<MessageTag> CoreModule { get; private set; }
        public UserData UserData { get; private set; }
        public CharacterData CharacterData { get; private set; }
        public IClientState State => stateModule.State as IClientState;
        public bool IsLogout { get; private set; }
        public event Action OnDisconnect;
        public Client(UnauthenticatedClient client)
        {
            var tcpClient = client.TcpClient;
            client.TransferProtocol(tcpClient, this);
            TcpClient = tcpClient;
            Instance = this;
            CoreModule = new CoreMessageHandler<MessageTag>();
            stateModule = new MessageStateHandler<MessageTag, MessageState>();
            var sessionHandler = new SessionHandler();
            CoreModule.RemoveHandler<SessionHandler>();
            CoreModule.AddHandler(sessionHandler);
            AddModule(CoreModule);
            AddModule(stateModule);
        }

        public void SetLogout(bool isLogout)
        {
            IsLogout = isLogout;
        }
        public void SetUserData(UserData userData)
        {
            UserData = userData;
        }

        public void ChangeState(IClientState state)
        {
            stateModule.ChangeState(state);
        }

        protected override void OnError(IProtocol<MessageTag> protocol, Exception ex)
        {
            Debug.LogException(ex);
        }

        protected override void OnDisconnected(IProtocol<MessageTag> protocol)
        {
            base.OnDisconnected(protocol);
            NetworkManager.Instance.Invoke(() =>
            {
                OnDisconnect?.Invoke();
            });
        }

        public void Send(Span<byte> data)
        {
            TcpClient.Send(data);
        }

        public void SetChar(CharacterData characterData)
        {
            CharacterData = characterData;
        }

        public void OnSelectedCharacter()
        {
            SetLogout(false);
            var module = CoreModule;
            var sceneManager = new SceneManager();
            module.RemoveHandler<SceneManager>();
            module.AddHandler(sceneManager);
            var skillHandler = new SkillHandler();
            module.RemoveHandler<SkillHandler>();
            module.AddHandler(skillHandler);
            
            var inventoryHandler = new InventoryHandler();
            module.RemoveHandler<InventoryHandler>();
            module.AddHandler(inventoryHandler);
            
            var monPhaiHandler = new MonPhaiHandler();
            module.RemoveHandler<MonPhaiHandler>();
            module.AddHandler(monPhaiHandler);

            var questHandler = new QuestHandler();
            module.RemoveHandler<QuestHandler>();
            module.AddHandler(questHandler);

            var combatHandler = new CombatHandler();
            module.RemoveHandler<CombatHandler>();
            module.AddHandler(combatHandler);

            var socialHandler = new SocialHandler();
            module.RemoveHandler<SocialHandler>();
            module.AddHandler(socialHandler);
            
            var shopHandler = new ShopHandler();
            module.RemoveHandler<ShopHandler>();
            module.AddHandler(shopHandler);
            
            var progressionHandler = new ProgressionHandler();
            module.RemoveHandler<ProgressionHandler>();
            module.AddHandler(progressionHandler);
            
            var adsHandler = new AdsHandler();
            module.RemoveHandler<AdsHandler>();
            module.AddHandler(adsHandler);
            
            var rankHandler = new RankHandler();
            module.RemoveHandler<RankHandler>();
            module.AddHandler(rankHandler);

            var inGameAuthHandler = new InGameAuthHandler();
            module.RemoveHandler<InGameAuthHandler>();
            module.AddHandler(inGameAuthHandler);
            
            var dailyHandler = new DailyHandler();
            module.RemoveHandler<DailyHandler>();
            module.AddHandler(dailyHandler);
            
            AdminPanel.Instance.StartListen();
        }
        public void AddRequest(MessageTag tag, Action<IReader> response, int timeoutMilliseconds = 0, Action<Exception> onError = null)
        {
            AddRequest(TcpClient ,tag, response, timeoutMilliseconds, onError);
        }
    }
}