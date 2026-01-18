using System.Collections.Generic;

namespace CreateCharacter.CharStats
{
    public class SelectionGroup
    {
        public int Id { get; private set; }
        public List<int> Items { get; private set; }
        public SelectionGroup(int id) {
            Id = id;
            Items = new List<int>();
        }
    }
}