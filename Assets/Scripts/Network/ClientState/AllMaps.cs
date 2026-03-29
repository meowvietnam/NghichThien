using System.Linq;
using System.Threading.Tasks;
using Network.Callback;
using Network.Extensions;
using Network.Models;

namespace Network.ClientState
{
    public class AllMaps : IClientState
    {
        private IAllMapsCallback callback;
        public AllMaps(IAllMapsCallback callback)
        {
            this.callback = callback;
        }
        public async void OnEnter()
        {
            var client = Client.Instance;
            client.Send(MessageTag.GetPlayerOnlineThisNode);
            await Task.Delay(200);
            client.Send(MessageTag.GetMails);
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
                }
                callback.OnMoveToMapResult(result);
            }
            else if (tag == MessageTag.GetPlayerOnlineThisNodeResult)
            {
                var result = reader.Read<GetCharacterOnlineThisNodeResult>();
                callback.OnGetCharacterOnlineThisNode(result);
            }
            else if (tag == MessageTag.GetMailsResult)
            {
                var result = reader.Read<GetMailsResult>();
                if (result.Success)
                {
                    var charData = Client.Instance.CharacterData;
                    charData.Mails = result.Mails;
                }
                callback.OnGetMails(result);
            }
        }
        public void OnLeave()
        {
        }

        public MessageState State  => MessageState.AllMaps;
    }
}