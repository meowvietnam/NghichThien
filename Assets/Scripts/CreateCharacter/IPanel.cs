using Base;

namespace CreateCharacter
{
    public interface IPanel : IFadable
    {
        void SetActive(bool active);
    }
}