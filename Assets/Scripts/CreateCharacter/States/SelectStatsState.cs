namespace CreateCharacter.States
{
    public class SelectStatsState : ICreateCharacterState
    {
        private CreateCharacterPanel createCharacterPanel;
        private SelectStats selectStats;

        public SelectStatsState(CreateCharacterPanel createCharacterPanel)
        {
            this.createCharacterPanel = createCharacterPanel;
            this.selectStats = createCharacterPanel.SelectStats;
        }

        public void OnEnter()
        {
            selectStats.SetActive(true);
            selectStats.FirstStep();
        }

        public void Next()
        {
            if (selectStats.ValidateStep())
            {
                selectStats.NextStep();
            }
        }

        public void Back()
        {
            selectStats.PreviousStep();
        }

        public void OnExit()
        {
            selectStats.SetActive(false);
        }
    }
}