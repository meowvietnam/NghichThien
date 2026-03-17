using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.EventSystems;
namespace AllMaps
{
    public class Map : MonoBehaviour
    {
        [SerializeField] private MapManager mapManager;
        [field: SerializeField] public int Id { get; private set; }
        [SerializeField] protected Node[] nodes;
        [SerializeField] protected int[] nextIndex;
        [field: SerializeField] public Vector2 PlayerPosition { get; private set; }
        public Node[] Nodes => nodes;
        public Node FindNodeByMap(Map map)
        {
            return nodes.FirstOrDefault(node => node.NextMap == map);
        }
        private void OnMouseDown()
        {
            if (EventSystem.current.IsPointerOverGameObject())
            {
                return;
            }
            mapManager.PlayerMoveTo(this);
        }
        public virtual void OnPlayerEnter()
        {

        }
        public virtual void OnPlayerExit()
        {
        }
        public void CopyTo(Map target)
        {
            target.Id = this.Id;
            target.PlayerPosition = this.PlayerPosition;
            target.mapManager = this.mapManager;
            target.nextIndex = this.nextIndex;
        }
    }
}