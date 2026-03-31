using CreateCharacter;
using LongDH.LongDHModules;
using Network;
using Network.Callback;
using Network.ClientState;
using Network.Extensions;
using Network.Models;
using Newtonsoft.Json;
using Sirenix.OdinInspector;
using System.Collections.Generic;
using System.IO;
using System.Linq;
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
        [SerializeField] private NPCData[] npcDatas;
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
            player.Init(characterData.Gender, false);
        }

        private void Start()
        {
            Observer.Notify(EventList.GamePlay.UPDATE_PLAYER_STATS);
            LoadNPCMap();
        }

        public void PlayerMoveTo(Map map)
        {
            if (map == current) return;
            if (player.IsMoving) return;
            Client.Instance.Send(MessageTag.MoveToMap, map.Id);
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

            player.DoMove(listNodes, OnMoveToMapCompleted);
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
                    OnGetCharacterOnlineThisNode(result.CharactersOnline);
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

        public void OnGetMails(GetMailsResult result)
        {
            NetworkManager.Instance.Invoke(() =>
            {
                if (!result.Success)
                {
                    NotifyGroup.Instance.Show(result.Message);
                }
                else
                {
                    var mails = result.Mails;
                    Debug.Log($"Bạn có {mails.Count} hộp thư");
                }
            });
        }

        private void OnGetCharacterOnlineThisNode(CharacterSnapshot[] characters)
        {
            //Test Data
            Debug.Log($"Có {characters.Length} players online trong node");
            List<CharacterStatsSecure> characterStatsSecures = new List<CharacterStatsSecure>();
            foreach (var character in characters)
            {
                characterStatsSecures.Add(new CharacterStatsSecure { tuoiTac = character.TuoiTac, hpMax = character.SinhMenh, level = character.Level, thoNguyen = character.ThoNguyen, tuChat = character.TuChat, ngoTinh = character.NgoTinh, tocDoBay = character.TocDoBay, thanThuc = character.ThanThuc, nameUser = character.Name, monPhai = character.MonPhai, thanThongEquip = character.ThanThongEquip.ConvertAll<SecureInt>(id => new SecureInt(id)), chuTu1Used = character.ChuTu1Used, chuTu2Used = character.ChuTu2Used, phuTu1Used = character.PhuTu1Used, phuTu2Used = character.PhuTu2Used, phuTu3Used = character.PhuTu3Used, phuTu4Used = character.PhuTu4Used });
            }
            List<NPCData> npcOnCurrentMap = npcDatas.Where(x => x.charStats.idNode == current.Id).ToList();
            foreach (var character in npcOnCurrentMap)
            {

                characterStatsSecures.Add(character.charStats);
            }

            //Test Data
            Pool.Get<UINPCJiaoHu>().ShowNPCList(characterStatsSecures);
           
        }
        private void SaveNPCMap()
        {
            Dictionary<int, int> dict = new Dictionary<int, int>();
            foreach (var npc in npcDatas)
            {
                dict.Add(npc.charStatsCreateRef.idChar.Value,npc.charStats.idNode);
            } 
            string json = JsonConvert.SerializeObject(dict);
            File.WriteAllText(Application.persistentDataPath + "/save.json", json);
        }
        private void NPCRandomOnMap(List<int> idNpcExcept)
        {
            foreach (var npc in npcDatas)
            {
                if(!idNpcExcept.Contains(npc.charStatsCreateRef.idChar))
                {
                    int randomMapId = maps[Random.Range(0, maps.Length)].Id;
                    npc.charStats.idNode = randomMapId;
                }    

            } 
                
            SaveNPCMap();
            
        }    

        private void LoadNPCMap()
        {

            string path = Application.persistentDataPath + "/save.json";

            if (File.Exists(path))
            {
                string json = File.ReadAllText(path);
                // Tiếp tục xử lý dữ liệu (JsonUtility.FromJson...)
                var dictLoadNPCMap = JsonConvert.DeserializeObject<Dictionary<int, int>>(json);

                List<int> npcExcept = new List<int>();
                foreach (var item in dictLoadNPCMap)
                {

                    npcDatas.FirstOrDefault(x => x.charStatsCreateRef.idChar == item.Key).charStats.idNode = item.Value;
                    npcExcept.Add(item.Key);
                }
                NPCRandomOnMap(npcExcept);
                Debug.Log("Đã load dữ liệu thành công!");
            }
            else
            {
                NPCRandomOnMap(new List<int> { });
                Debug.LogWarning("Không tìm thấy file save. Khởi tạo dữ liệu mới.");
                return;
            }
          

        }
    }
}