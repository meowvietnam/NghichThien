using System;
using System.Linq;
using UnityEngine;

namespace AllMaps
{
    public class Map : MonoBehaviour
    {
        [SerializeField] private MapManager mapManager;
        [field: SerializeField] public int Id { get; private set; }
        [SerializeField] private Node[] nodes;
        [field: SerializeField] public Vector2 PlayerPosition { get; private set; }
        public Node[] Nodes => nodes;
        public Node FindNodeByMap(Map map)
        {
            return nodes.FirstOrDefault(node => node.NextMap == map);
        }
        private void OnMouseDown()
        {
            mapManager.PlayerMoveTo(this);
        }
    }
}