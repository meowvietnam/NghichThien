using System;
using System.Collections.Generic;
using System.Linq;
using LongDH.LongDHModules;
using Network;
using Network.Extensions;
using UnityEditor;
using UnityEngine;
using UnityEngine.EventSystems;
using LongDH.Script.ObjectPool;
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
        public EnemyData enemyDataCurrent;
        public GameObject iconVisualAction;

        public Node[] Nodes => nodes;
        public Node FindNodeByMap(Map map)
        {
            return nodes.FirstOrDefault(node => node.NextMap == map);
        }
        private void OnMouseDown()
        {
            if (IsPointerOverUIElement())
            {
                Debug.Log("k move duoc do over UI");               
                return;
            }
            mapManager.PlayerMoveTo(this);
        }
        public bool IsPointerOverUIElement()
        {
            var eventData = new PointerEventData(EventSystem.current);
            eventData.position = Input.mousePosition; // Hoặc dùng Input.GetTouch(0).position nếu là Mobile

            List<RaycastResult> results = new List<RaycastResult>();
            EventSystem.current.RaycastAll(eventData, results);

            // Nếu results.Count > 0 nghĩa là có UI đang nằm tại vị trí chuột/touch
            return results.Count > 0;
        }
        public virtual void SetData(EnemyData enemyDataCurrent, GameObject iconVisualAction)
        {
            this.enemyDataCurrent = enemyDataCurrent;
            this.iconVisualAction = iconVisualAction;
        }    
        public virtual void ResetData()
        {
            enemyDataCurrent = null;
            if(iconVisualAction != null)
            {
                iconVisualAction.Recycle();
                iconVisualAction = null;
            }    
        }    
        public virtual void OnPlayerEnter()
        {
            var currenEnemy = enemyDataCurrent;
            if (currenEnemy != null)
            {
                ResetData();
                Pool.Get<NpcManager>().OnGoToEnemy(currenEnemy);
                Pool.Get<GameManager>().GoToCombat(new GameManager.CharCombatInit { eTypeVisual = currenEnemy.eTypeVisualChar, charStats = currenEnemy.charStats });
            }
           
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