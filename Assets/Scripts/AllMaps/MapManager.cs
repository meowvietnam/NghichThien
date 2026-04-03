using System.Collections.Generic;
using System.Linq;
using CreateCharacter;
using LongDH.LongDHModules;
using Network;
using Network.Callback;
using Network.ClientState;
using Network.Extensions;
using Network.Models;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.UI;

namespace AllMaps
{
    public class MapManager : MonoBehaviour, IAllMapsCallback
    {
        [SerializeField] private Map[] maps;
        [SerializeField] private Player player;
        [SerializeField] private MailPanel mailPanel;
        [SerializeField] private MailButton mailButton;
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
            player.Init(characterData.Gender, false);
        }

        private void Start()
        {
            Observer.Notify(EventList.GamePlay.UPDATE_PLAYER_STATS);
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
                    mailPanel.Init(mails);
                    mailButton.Init(mails);
                }
            });
        }

        public void MarkMailRead(int id)
        {
            NetworkManager.Instance.Invoke(() =>
            {
                mailPanel.MarkMailRead(id);
                mailButton.Refresh();
            });
        }

        public void RemoveMail(int id)
        {
            NetworkManager.Instance.Invoke(() =>
            {
                mailPanel.RemoveMail(id);
                mailButton.Refresh();
            });
        }

        private void OnGetCharacterOnlineThisNode(CharacterSnapshot[] characters)
        {
            //Test Data
            Debug.Log($"Có {characters.Length} players online trong node");
            //Test Data
            Pool.Get<UINPCJiaoHu>().ShowNPCList(new List<CharacterStatsSecure>
            {
                        new CharacterStatsSecure {tuoiTac = 15, hpMax = 200,level = 9,thoNguyen = 200,tuChat = 120,ngoTinh = 50,tocDoBay=20,thanThuc=30,nameUser = "Viêm Đế",monPhai = Base.EMonPhai.BachDeLau,thanThongEquip = new List<SecureInt> {874,527,417,878}, chuTu1Used =1 , chuTu2Used = 2 , phuTu1Used = 3, phuTu2Used =4,phuTu3Used=5,phuTu4Used=6},
                        new CharacterStatsSecure {tuoiTac = 15, hpMax = 500,level = 4,thoNguyen = 200,tuChat = 120,ngoTinh = 50,tocDoBay=20,thanThuc=30,nameUser = "Hàn Lão Ma",monPhai = Base.EMonPhai.None,thanThongEquip = new List<SecureInt> {427,309,131}, chuTu1Used =2 , chuTu2Used = 4, phuTu1Used = 7, phuTu2Used =9,phuTu3Used=10,phuTu4Used=12 },
                        new CharacterStatsSecure {tuoiTac = 15, hpMax = 700,level = 2,thoNguyen = 200,tuChat = 120,ngoTinh = 50,tocDoBay=20,thanThuc=30,nameUser = "Lệ Phi Vũ",monPhai = Base.EMonPhai.KimHongKiemPhai,thanThongEquip = new List<SecureInt> {874,527,417,878}, chuTu1Used =1 , chuTu2Used = 2 , phuTu1Used = 3, phuTu2Used =4,phuTu3Used=5,phuTu4Used=6},

            });
        }
    }
}