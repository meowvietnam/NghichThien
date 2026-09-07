using System.Collections.Generic;
using System.Linq;
using MemoryPack;
using Network;
using Network.Extensions;
using UnityEngine;

namespace CreateCharacter.CharStats
{
    [MemoryPackable]
    public partial class SelectionModel
    {
        public int Id { get; private set; }
        public List<SelectionGroup> Groups { get; private set; }

        public SelectionModel(int id)
        {
            Id = id;
            Groups = new List<SelectionGroup>();
        }

        [MemoryPackConstructor]
        public SelectionModel()
        {
        }
    }
}