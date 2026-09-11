using System;
using UnityEngine;

namespace AllMaps
{
    [Serializable]
    public class Node
    {
        [field: SerializeField] public Map NextMap { get; private set; }
        [field: SerializeField] public MapMoveNode MoveNode { get; private set; }

        public Node(Map nextMap, MapMoveNode moveNode)
        {
            NextMap = nextMap;
            MoveNode = moveNode;
        }
    }
}