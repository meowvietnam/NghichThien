using System.Collections.Generic;
using System.Linq;
using MemoryPack;
using Network;
using Network.Extensions;

namespace CreateCharacter.CharStats
{
    [MemoryPackable]
    public partial class SelectionGroup
    {
        public int Id { get; private set; }
        public List<int> Items { get; private set; }

        public SelectionGroup(int id)
        {
            Id = id;
            Items = new List<int>();
        }

        [MemoryPackConstructor]
        public SelectionGroup()
        {
        }
    }
}