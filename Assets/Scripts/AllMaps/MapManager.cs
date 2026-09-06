using CreateCharacter;
using LongDH.LongDHModules;
using LongDH.Script.ObjectPool;
using Network;
using Network.Callback;
using Network.ClientState;
using Network.Extensions;
using Network.Models;
using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using ComboSkillResource.NPCData;
using LongDHModules;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

namespace AllMaps
{
    public interface IActionMap
    {
        void ActiveEventMap(Map map);
    }
    public class MapManager : MonoBehaviour, IAllMapsCallback
    {
        [SerializeField] public Map[] maps;
        [SerializeField] private Player player;
        [SerializeField] private GameObject playerPrefab;
        private Map current;
        private Dictionary<Map, List<Map>> graph;

        public bool isDelayAfterLoadScene = true;

        public Map FindMap(EMapType eMap)
        {
            foreach (Map map in maps)
            {
                if(map is Stronghold strongHold && strongHold.eCurrentMapType == eMap)
                {
                    return map;
                }    
            } 
            return null;
                
        }    
        private GameObject SetIconEvent(Map map , GameObject prefabIcon)
        {
            GameObject iconAction = null;

            if (map is Stronghold)
            {
                iconAction = prefabIcon.Spawn(map.transform.GetChild(1).transform);

            }
            else
            {
                iconAction = prefabIcon.Spawn(map.transform.GetChild(0).transform);

            }
            return iconAction;
        }
        public Map SetDataOnMap(ActionMapResourceSO Data,int nodeId)
        {
            Map map = FindMap(nodeId);
          
            map.SetData(Data, SetIconEvent(map, Data.prefabIcon));

            return map;
        }
        public void SetDataOnMap(GameObject prefabIcon, Map map)
        {
            map.SetData(null, SetIconEvent(map, prefabIcon));
        }
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
            var state = new Network.ClientState.AllMaps(this);
            Client.Instance.ChangeState(state);
            SetUpGraph();
            var characterData = Client.Instance.CharacterData;
            var entryMapId = characterData.Progression.IdCurrentNode;
            var map = FindMap(entryMapId);
            current = map;
            player.SetPosition(map.PlayerPosition);
            var appearance = characterData.Appearance;
            player.Init(characterData.Gender, characterData.CanFly, appearance);
            player.SetName(characterData.Name,new Color32(255, 213, 79, 255));
            player.SetInstance();
            current.OnPlayerEnter();
            Pool.Set(this);
        }
        
        private void Start()
        {
            Observer.Notify(EventList.GamePlay.UPDATE_PLAYER_STATS);
            isDelayAfterLoadScene = true;
            StartCoroutine(DelayTime(0.01f));
        }

        private IEnumerator DelayTime(float time)
        {
            yield return new WaitForSeconds(time);
            isDelayAfterLoadScene = false;

        }
        public void PlayerMoveTo(Map map)
        {
            if (CameraFollow.Instance.target != player.transform) return;
            if (isDelayAfterLoadScene) return;
            if (map == current) return;
            if (player.IsMoving) return;
            Client.Instance.Send(MessageTag.MoveToMap, map.Id);
        }

        private void Move(int[] path)
        {
            var listMapPath = path.Select(x => FindMap(x)).ToList();
            var map = FindMap(path.Last());
            var start = FindMap(path.First());
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

            player.DoMove(listNodes, OnMoveToMapCompleted);
        }

        private void MoveOtherPlayer(Player player, int[] path)
        {
            var listMapPath = path.Select(x => FindMap(x)).ToList();
            var start = FindMap(path.First());
            player.SetPosition(start.PlayerPosition);
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
            player.DoMove(listNodes, () =>
            {
                player.DestroySelf();
            });
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
        [Button]
        private void FixVisualStrongHoldText(float scale)
        {
            foreach (var item in maps)
            {
                if(item is Stronghold strongHold)
                {
                    strongHold.transform.GetChild(0).transform.localScale = Vector3.one * scale;
                }    
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
                    OnGetCharacterOnlineThisNode(result.CharactersOnline);
                    Observer.Notify(EventList.GamePlay.TIME_ADJUST);
                    Observer.Notify(EventList.GamePlay.MOVE_MAP_SUCCESS);

                }
            });
        }

        public void OnGetCharacterOnlineThisNode(GetCharacterOnlineThisNodeResult result)
        {
            NetworkManager.Instance.Invoke(() =>
            {
                if (!result.Success)
                {
                    NotifyGroup.Instance.Show(result.Message);
                }
                else
                {
                    OnGetCharacterOnlineThisNode(result.CharactersOnline);
                }
            });
        }

        public void OnPlayerMoving(PlayerMoving playerMoving)
        {
            var player = Instantiate(playerPrefab).GetComponentInChildren<Player>();
            player.Init(playerMoving.Gender, playerMoving.IsSwordRiding, playerMoving.Appearance);
            player.SetName(playerMoving.Name, new Color32(129, 212, 250, 255));
            MoveOtherPlayer(player, playerMoving.Path);
        }


        private void OnGetCharacterOnlineThisNode(CharacterSnapshot[] characters)
        {
          
            Debug.Log($"Có {characters.Length} players online trong node");
            List<CharacterStatsSecure> characterStatsSecures = new List<CharacterStatsSecure>();
            foreach (var character in characters)
            {
                characterStatsSecures.Add(CharacterStatsSecureBuilder.FromCharacterSnapshot(character));
            }
            List<NPCData> npcOnCurrentMap = Pool.Get<NPCManager>().npcDatas.Where(x => x.charStats.idNode == current.Id).ToList();
            foreach (var character in npcOnCurrentMap)
            {

                characterStatsSecures.Add(character.charStats);
            }
            Pool.Get<UINPCJiaoHu>().ShowNPCList(characterStatsSecures);
        }
        #if UNITY_EDITOR

        [Button]
        public void ChangeSprites()
        {
            var renderers = FindObjectsByType<SpriteRenderer>(
                FindObjectsInactive.Include,
                FindObjectsSortMode.None
            );
            var res = GetAllSpritesInFolder("Assets/Texture2D");
            foreach (var sr in renderers)
            {
                if(sr.sprite == null) continue; 
                var spriteName = sr.sprite.name;
                var newSprite = res.FirstOrDefault(x => x.name == spriteName);
                if (newSprite == null)
                {
                    Debug.Log($"Sprite {spriteName} not found",sr);
                    continue;
                }
                sr.sprite = newSprite;
            }
        }

        public static List<Sprite> GetAllSpritesInFolder(string folderPath)
        {
            List<Sprite> result = new List<Sprite>();

            string[] guids = AssetDatabase.FindAssets("t:Texture2D", new[] { folderPath });

            foreach (string guid in guids)
            {
                string path = AssetDatabase.GUIDToAssetPath(guid);

                System.Object[] assets =
                    AssetDatabase.LoadAllAssetRepresentationsAtPath(path);

                foreach (var obj in assets)
                {
                    if (obj is Sprite sprite)
                    {
                        result.Add(sprite);
                    }
                }
            }
            return result;
        }
#endif

    }
}