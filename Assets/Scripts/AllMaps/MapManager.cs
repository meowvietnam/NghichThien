using System;
using System.Collections.Generic;
using System.Linq;
using Algorithm;
using Base;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.UI;

namespace AllMaps
{
    public class MapManager : MonoBehaviour
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
        public void PlayerMoveTo(Map map)
        {
            if(map == current) return;
            if(player.IsMoving) return;
            var listMapPath = BFSPath.FindPath(graph,current, map);
            if(listMapPath == null) return;
            var start = current;
            current.OnPlayerExit();
            current = map;
            var listNodes = new List<Node>();
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
            player.DoMove(listNodes,OnMoveToMapCompleted);
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
    }
}
