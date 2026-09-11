using System;
using Network.Extensions;
using UnityEngine;

namespace Network.Core
{
    public class SessionHandler : IClientHandler
    {
        private DateTime lastPong = DateTime.UtcNow;

        public void OnEnter()
        {
        }

        public void Handle(Message<MessageTag> message)
        {
            var tag = message.Tag;
            var client = Client.Instance;
            if (tag == MessageTag.Ping)
            {
                client.Send(MessageTag.Pong);
            }
            else if (tag == MessageTag.Pong)
            {
                lastPong = DateTime.UtcNow;
            }
        }

        public void OnLeave()
        {
        }

        public void OnDisconnect(IProtocol<MessageTag> protocol)
        {
        }

        private static readonly MessageTag[] handlers = { MessageTag.Ping, MessageTag.Pong };
        public MessageTag[] HandleTag => handlers;
    }
}