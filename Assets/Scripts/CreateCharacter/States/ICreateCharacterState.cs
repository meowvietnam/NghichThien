namespace CreateCharacter.States
{
    public interface ICreateCharacterState
    {
        void OnEnter();
        void Next();
        void Back();
        void OnExit();
    }
}