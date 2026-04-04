using System.Linq;
using System.Threading.Tasks;
using Network.Callback;
using Network.Extensions;
using Network.Models;
using UnityEngine;

namespace Network.ClientState
{
    public class AllMaps : IClientState
    {
        private IAllMapsCallback callback;
        public AllMaps(IAllMapsCallback callback)
        {
            this.callback = callback;
        }
        public void OnEnter()
        {
            var client = Client.Instance;
            client.Send(MessageTag.GetPlayerOnlineThisNode);
        }
        public void Handle(Message<MessageTag> message)
        {
            var tag = message.Tag;
            var reader = message.Reader;
            if (tag == MessageTag.MoveToMapResult)
            {
                var result = reader.Read<MoveToMapResult>();
                if (result.Success)
                {
                    var charData = Client.Instance.CharacterData;
                    charData.IdCurrentNode = result.Path.Last();
                    charData.CurrentEnergy = result.Energy;
                    charData.Stats.TuoiTac = result.NewAge;
                }
                callback.OnMoveToMapResult(result);
            }
            else if (tag == MessageTag.GetPlayerOnlineThisNodeResult)
            {
                var result = reader.Read<GetCharacterOnlineThisNodeResult>();
                callback.OnGetCharacterOnlineThisNode(result);
            }
        }
        public void OnLeave()
        {
        }

        public MessageState State  => MessageState.AllMaps;
    }
}