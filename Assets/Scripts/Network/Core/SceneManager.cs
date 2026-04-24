using System;
using Network.Extensions;
using Network.Models;
using UnityEngine;

namespace Network.Core
{
    public class SceneManager : IClientHandler
    {
        private Action<ChangeSceneResult> onChangeSceneResult;
        public void OnEnter()
        {
        }

        public void Handle(Message<MessageTag> message)
        {
            var tag = message.Tag;
            var reader = message.Reader;
            if (tag == MessageTag.ChangeSceneResult)
            {
                var result = reader.Read<ChangeSceneResult>();
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

        public void ChangeScene(string sceneName,Action<ChangeSceneResult> callback)
        {
            onChangeSceneResult = callback;
            Client.Instance.Send(MessageTag.ChangeScene,sceneName);
        }
        private static readonly MessageTag[] handlers = { MessageTag.ChangeSceneResult };
        public MessageTag[] HandleTag => handlers;
    }
}