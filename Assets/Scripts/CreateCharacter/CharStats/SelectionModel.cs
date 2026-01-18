using System.Collections.Generic;

namespace CreateCharacter.CharStats
{
    
    public class SelectionModel
    {
        public int Id { get; private set; }
        public List<SelectionGroup> Groups { get; private set; }

        public SelectionModel(int id)
        {
            Groups = new List<SelectionGroup>();
        }
    }
}