using System;
using System.Collections.Generic;
using System.Linq;
using Algorithm;
using Base;
using LongDH.LongDHModules;
using Network;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.UI;

namespace AllMaps
{
    public class MapManager : MonoBehaviour , INetworkEvent.IMoveOnMap
    {
        [SerializeField] private Map[] maps;
        [SerializeField] private Player player;
        [SerializeField] private int entryMapId;
        private Map current;
        private Dictionary<Map, List<Map>> graph;
        public Map FindMap(int id)
        {
            return maps.FirstOrDefault(map => map.Id == id);
        }
        private void SetUpGraph()
        {
            graph = new Dictionary<Map, List<Map>>();
            foreach (var map in maps)
            {
                graph[map] = map.Nodes
                    .Select(x => x.NextMap)
                    .ToList();
            }
        }
        private void Awake()
        {
            SetUpGraph();
            var map = FindMap(entryMapId);
            current = map;
            player.SetPosition(map.PlayerPosition);
            player.Init(Gender.Male,true);
        }
        List<Node> listNodes = new List<Node>();

        public void PlayerMoveTo(Map map)
        {
            if (listNodes.Count > 0) return;
            if(map == current) return;
            if(player.IsMoving) return;
            var listMapPath = BFSPath.FindPath(graph,current, map);
            if(listMapPath == null) return;
            var start = current;
            current.OnPlayerExit();
            current = map;
            listNodes.Clear();
            for (var i = 1; i < listMapPath.Count; i++)
            {
                var m = listMapPath[i];
                var node = start.FindNodeByMap(m);
                if (node == null)
                {
                    continue;
                }
                start = m;
                listNodes.Add(node);
            }
            if (Client.Instance.CharacterData.CurrentEnergy < listNodes.Count) return;
            Observer.Notify(EventList.Network.MOVE_ON_MAP,map.Id,this);
#if UNITY_EDITOR
            player.DoMove(listNodes, OnMoveToMapCompleted);
#endif

        }
        private void OnMoveToMapCompleted()
        {
            current.OnPlayerEnter();

        }

        [Button]
        private void FixVisual()
        {
            var listBtn = transform.GetComponentsInChildren<Button>(true);
            foreach (var item in listBtn)
            {
                item.transition = Selectable.Transition.ColorTint;
            }
        }

        public void SuccessMoveOnMap()
        {
            player.DoMove(listNodes, () =>
            {
                OnMoveToMapCompleted();
                SetPos();

            });
            listNodes.Clear();

        }

        private void SetPos()
        {
            // reset lại pos đúng data trên server
            var map = FindMap(Client.Instance.CharacterData.IdCurrentNode);
            player.transform.position = map.PlayerPosition;
        }   
        public void FailMoveOnMap(string message)
        {
            SetPos();
        }
    }
}
