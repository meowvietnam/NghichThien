using Network.Models;

namespace Network.Callback
{
    public interface ICreateCharacterCallback
    {
        void BackToSelectChar();
        void OnSetNameResult(SetCharNameResult result);
        void OnSetSkinsResult(SetSkinResult result);
        void OnApplyStatsResult(ApplyStatsResult result);
    }
}