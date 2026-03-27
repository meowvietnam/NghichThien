using System;
using System.Collections.Generic;
using System.Linq;
using Network;
using Network.Extensions;
using UnityEditor;
using UnityEngine;
using UnityEngine.EventSystems;
namespace AllMaps
{
    public class Map : BaseBehaviour, IMemorySerializable<Map>
    {
        
        [SerializeField] private MapManager mapManager;
        [field: SerializeField] public MapType MapType { get; set; }
        [field: SerializeField] public int Id { get; set; }
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

        public void Deserialize(IReader reader, ref Map value)
        {
            value.MapType = reader.Read<MapType>();
            value.Id = reader.Read<int>();
            value.nextIndex = reader.ReadArray<int>();
        }

        public int ByteSize()
        {
            return MapType.ByteSize() + Id.ByteSize() + nextIndex.ByteSize();
        }

        public void Serialize(IWriter writer)
        {
            writer.Write(MapType);
            writer.Write(Id);
            writer.WriteArray(nextIndex);
        }
    }
}