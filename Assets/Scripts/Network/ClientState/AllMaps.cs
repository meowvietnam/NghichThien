using System.Linq;
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
        public void OnEnter()
        {
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
                    charData.CurrentEnergy -= result.Energy;
                }
                callback.OnMoveToMapResult(result);
            }
        }
        public void OnLeave()
        {
        }

        public MessageState State  => MessageState.AllMaps;
    }
}