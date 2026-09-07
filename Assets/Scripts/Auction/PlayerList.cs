using System.Collections.Generic;
using System.Linq;
using Network;
using Network.Models;
using UnityEngine;

namespace Auction
{
    public class PlayerList : MonoBehaviour
    {
        [SerializeField] private Player playerPrefab;
        private List<Player> players;
        public int PlayerCount => players.Count;

        private List<Player> GetAllPlayers()
        {
            return GetComponentsInChildren<Player>(true)
                .ToList();
        }

        public bool IsMe(int charId)
        {
            return AuctionPanel.Instance.Me.CharId == charId;
        }

        public void Init(PlayerAuctionModel[] models)
        {
            players = GetAllPlayers();
            var neededCount = models.Length - players.Count;
            if (neededCount < 0)
            {
                var start = players.Count + neededCount - 1;
                for (int i = start; i < players.Count; i++)
                {
                    players[i].SetActive(false);
                }

                players.RemoveRange(start, -neededCount);
            }
            else
            {
                for (int i = 0; i < neededCount; i++)
                {
                    var player = Instantiate(playerPrefab, transform);
                    players.Add(player);
                }
            }

            for (int i = 0; i < players.Count; i++)
            {
                var player = players[i];
                var model = models[i];
                player.SetData(model, IsMe(model.CharId));
                player.SetActive(true);
            }

            SortPlayers();
        }


        public bool TryReplacePlayer(int playerId, PlayerAuctionModel model)
        {
            var player = players.FirstOrDefault(x => x.Model.PlayerId == playerId);
            if (player == null)
            {
                return false;
            }

            player.SetData(model, IsMe(model.CharId));
            return true;
        }

        public bool TryRemovePlayer(int playerId)
        {
            var index = players.FindIndex(x => x.Model.PlayerId == playerId);
            if (index < 0)
            {
                return false;
            }

            var player = players[index];
            player.SetActive(false);
            players.RemoveAt(index);
            return true;
        }

        public bool TryGetByStatus(PlayerAuctionStatus status, out Player player)
        {
            var findPlayer = players.FirstOrDefault(x => x.Model.Status == status);
            player = findPlayer;
            return player != null;
        }

        public bool TryGetPlayer(int playerId, out Player player)
        {
            var findPlayer = players.FirstOrDefault(x => x.Model.PlayerId == playerId);
            player = findPlayer;
            return player != null;
        }

        public void AddPlayer(PlayerAuctionModel model)
        {
            var player = GetAllPlayers().FirstOrDefault(x => !x.IsActive());
            if (player == null)
            {
                player = Instantiate(playerPrefab, transform);
            }

            player.SetData(model, IsMe(model.CharId));
            player.SetActive(true);
            players.Add(player);
            SortPlayers();
        }

        private void SortPlayers()
        {
            players = players.OrderBy(x => x.Model.PlayerId).ToList();
            for (var i = 0; i < players.Count; i++)
            {
                players[i].SetSiblingIndex(i);
            }
        }

        public void ResetNewSession()
        {
            foreach (var player in players)
            {
                player.ResetNewSession();
            }
        }
    }
}