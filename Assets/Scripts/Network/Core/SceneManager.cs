using System;
using Network.Extensions;
using Network.Extensions.MemoryPack;
using Network.Models;
using UnityEngine;

namespace Network.Core
{
    public class SceneManager : IClientHandler
    {
        private Action<ChangeSceneResult> onChangeSceneResult;
        public BossData CurrentBoss { get; private set; }

        public void OnEnter()
        {
        }

        public void Handle(Message<MessageTag> message)
        {
            var tag = message.Tag;
            var reader = message.Reader;
            if (tag == MessageTag.BossInfo)
            {
                CurrentBoss = reader.MemRead<BossData>();
            }
            else if (tag == MessageTag.ChangeSceneResult)
            {
                var result = reader.MemRead<ChangeSceneResult>();
                Debug.Log($"SceneName: {result.SceneName}, Success: {result.Success}");
                if (onChangeSceneResult != null)
                {
                    onChangeSceneResult(result);
                    onChangeSceneResult = null;
                }
            }
        }

        public void OnLeave()
        {
        }

        public void OnDisconnect(IProtocol<MessageTag> protocol)
        {
        }

        public void ChangeScene(string sceneName, Action<ChangeSceneResult> callback)
        {
            onChangeSceneResult = callback;
            Client.Instance.Send(MessageTag.ChangeScene, sceneName);
        }

        private static readonly MessageTag[] handlers = { MessageTag.ChangeSceneResult, MessageTag.BossInfo };
        public MessageTag[] HandleTag => handlers;
    }
}