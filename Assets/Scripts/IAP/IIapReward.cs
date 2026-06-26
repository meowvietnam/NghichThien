using CreateCharacter;
using MemoryPack;

namespace IAP
{
    public class IapUIContext
    {
        
    }
    [MemoryPackable]
    public partial class IapRewardTest : IIapReward
    {
        public void Execute(CharacterData charData)
        {
        }
        public void Refund(CharacterData charData)
        {
        }

        public void ShowUISuccess(IapUIContext context)
        {
        }
        public void InitUI(IapUIContext context)
        {
        }
    }
    [MemoryPackable]
    [MemoryPackUnion(100, typeof(IapRewardTest))]
    public partial interface IIapReward
    {
        void Execute(CharacterData charData);
        void Refund(CharacterData charData);
        void ShowUISuccess(IapUIContext context);
        void InitUI(IapUIContext context);
    }
}