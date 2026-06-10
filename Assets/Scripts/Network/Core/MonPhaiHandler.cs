using Assets.Scripts.Network.Models;
using Base;
using LongDH.LongDHModules;
using Network;
using Network.Core;
using Network.Extensions;
using Network.Models;
using System;
using System.Collections;
using UnityEngine;
using static INetworkEvent;

namespace Assets.Scripts.Network.Core
{
    public class MonPhaiHandler : IClientHandler
    {
        private static MessageTag[] handler =
                    { MessageTag.JoinMonPhaiResult
            };

        public MessageTag[] HandleTag => handler;
        private INetworkEvent.IJoinMonPhai joinMonPhaiCallback;

        public void Handle(Message<MessageTag> message)
        {
            var tag = message.Tag;
            var reader = message.Reader;
            if (tag == MessageTag.JoinMonPhaiResult)
            {
                var result = reader.Read<JoinMonPhaiResult>();
                if (result.Success)
                {
                    var charData = Client.Instance.CharacterData;
                    charData.MonPhai = result.TypeMonPhai;
                }
                NetworkManager.Instance.Invoke(() =>
                {

                    if (result.Success)
                    {
                        joinMonPhaiCallback.SuccessJoinMonPhai();
                    }
                    else
                    {
                        joinMonPhaiCallback.FailJoinMonPhai(result.Message);
                    }
                });
            }
        }

        public void OnDisconnect(IProtocol<MessageTag> protocol)
        {
        }

        public void OnEnter()
        {
            Observer.AddObserver(EventList.Network.JOIN_MONPHAI, JoinMonPhai);
        }
        private void JoinMonPhai(object[] data)
        {
            joinMonPhaiCallback = data[0] as INetworkEvent.IJoinMonPhai;
            var typeMonPhai = (EMonPhai)data[1];
            Client.Instance.Send(MessageTag.JoinMonPhai, typeMonPhai);
        }

        public void OnLeave()
        {

        }

       
    }
}