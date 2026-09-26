using System;
using System.Collections.Generic;
using System.Linq;
using LongDH.LongDHModules;
using Network;
using Network.Extensions;
using UnityEngine;
using UnityEngine.EventSystems;
using LongDH.Script.ObjectPool;

namespace AllMaps
{
    public class Map : BaseBehaviour, IMemorySerializable<Map>, IPointerClickHandler
    {
        [SerializeField] private MapManager mapManager;
        [field: SerializeField] public MapType MapType { get; set; }
        [field: SerializeField] public int Id { get; set; }
        [SerializeField] protected Node[] nodes;
        [SerializeField] protected int[] nextIndex;
        [field: SerializeField] public Vector2 PlayerPosition { get; private set; }
        public IActionMap actionMap;

        public GameObject iconVisualAction;

        public bool IsEmptyAction => actionMap == null && iconVisualAction == null;
        public Node[] Nodes => nodes;

        public Node FindNodeByMap(Map map)
        {
            return nodes.FirstOrDefault(node => node.NextMap == map);
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

        public void ResetDataAction()
        {
            actionMap = null;
            if (iconVisualAction != null)
            {
                iconVisualAction.Recycle();
                iconVisualAction = null;
            }

            for (int i = 0; i < transform.GetChild(0).childCount; i++)
            {
                transform.GetChild(0).GetChild(i).gameObject.SetActive(false);
            }
        }

        public virtual void SetData(IActionMap dataCurrent, GameObject iconVisualAction)
        {
            actionMap = dataCurrent;
            this.iconVisualAction = iconVisualAction;
        }

        public virtual void OnPlayerEnter()
        {
            Observer.Notify(EventList.GamePlay.USER_ENTER_MAP, this);
            if (actionMap != null)
            {
                actionMap.ActiveEventMap(this);
            }
            ResetDataAction();
        }

        public virtual void OnPlayerExit()
        {
        }

        public void CopyTo(Map target)
        {
            if (target == null) throw new ArgumentNullException(nameof(target));
            target.Id = this.Id;
            target.PlayerPosition = this.PlayerPosition;
            target.mapManager = this.mapManager;
            target.nextIndex = this.nextIndex;
            target.nodes = this.nodes;
            target.actionMap = this.actionMap;
            target.iconVisualAction = this.iconVisualAction;
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

        /// <summary>
        /// Bắn ra mỗi khi node bản đồ này được người chơi tap/click, TRƯỚC khi xử lý logic
        /// chọn node / di chuyển. Dùng cho hand-tut (Tutorials) để biết chính xác lúc nào
        /// người chơi đã chạm đúng vào node đang được khoanh sáng, không ảnh hưởng logic gốc.
        /// </summary>
        public event Action OnMapClicked;

        public void OnPointerClick(PointerEventData eventData)
        {
            OnMapClicked?.Invoke();

            if (mapManager.TryHandleNodeSelection(this)) return;
            mapManager.PlayerMoveTo(this);
        }
    }
}
