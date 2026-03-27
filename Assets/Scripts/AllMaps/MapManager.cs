using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Algorithm;
using Base;
using CreateCharacter;
using Network;
using Network.Callback;
using Network.Extensions;
using Network.Models;
using Sirenix.OdinInspector;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

namespace AllMaps
{
    public class MapManager : MonoBehaviour, IAllMapsCallback
    {
        [SerializeField] private Map[] maps;
        [SerializeField] private Player player;
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
            Client.Instance.ChangeState(new Network.ClientState.AllMaps(this));
            SetUpGraph();
            var characterData = Client.Instance.CharacterData;
            var entryMapId = characterData.IdCurrentNode;
            var map = FindMap(entryMapId);
            current = map;
            player.SetPosition(map.PlayerPosition);
            player.Init(characterData.Gender,false);
        }
        public void PlayerMoveTo(Map map)
        {
            if(map == current) return;
            if(player.IsMoving) return;
            Client.Instance.Send(MessageTag.MoveToMap,map.Id);
            // var listMapPath = BFSPath.FindPath(graph,current, map);
            // if(listMapPath == null) return;
            // var start = current;
            // current.OnPlayerExit();
            // current = map;
            // var listNodes = new List<Node>();
            // for (var i = 1; i < listMapPath.Count; i++)
            // {
            //     var m = listMapPath[i];
            //     var node = start.FindNodeByMap(m);
            //     if (node == null)
            //     {
            //         continue;
            //     }
            //     start = m;
            //     listNodes.Add(node);
            // }
            // player.DoMove(listNodes,OnMoveToMapCompleted);
        }

        private void Move(int[] path)
        {
            var listMapPath = path.Select(x => FindMap(x)).ToList();
            var map = FindMap(path.Last());
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

        public void OnMoveToMapResult(MoveToMapResult result)
        {
            NetworkManager.Instance.Invoke(() =>
            {
                if (!result.Success)
                {
                    NotifyGroup.Instance.Show(result.Message);
                }
                else
                {
                    Move(result.Path);
                }
            });
        }
    }
}
