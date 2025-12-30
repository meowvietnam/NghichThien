using System;
using System.Collections.Generic;
using System.Linq;
using Algorithm;
using UnityEngine;

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
        private void Start()
        {
            SetUpGraph();
            var map = FindMap(entryMapId);
            current = map;
            player.SetPosition(map.PlayerPosition);
            player.Init(Gender.Male);
        }
        public void PlayerMoveTo(Map map)
        {
            if(map == current) return;
            if(player.IsMoving) return;
            var listMapPath = BFSPath.FindPath(graph,current, map);
            if(listMapPath == null) return;
            var s = string.Join(",", listMapPath.Select(x => x.Id));
            Debug.Log(s);
            var start = current;
            current = map;
            var listNodes = new List<Node>();
            for (var i = 1; i < listMapPath.Count; i++)
            {
                var m = listMapPath[i];
                var node = start.FindNodeByMap(m);
                if (node == null)
                {
                    Debug.Log(i);
                    continue;
                }
                start = m;
                listNodes.Add(node);
            }
            player.DoMove(listNodes,OnMoveToMapCompleted);
        }
        private void OnMoveToMapCompleted()
        {
            
        }
    }
}
