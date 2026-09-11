using System;
using Network.Models;
using UnityEngine;

namespace Auction
{
    [Serializable]
    public class StatusData
    {
        [field: SerializeField] public PlayerAuctionStatus Status { get; private set; }
        [field: SerializeField] public string Name { get; private set; }
        [field: SerializeField] public Sprite Icon { get; private set; }
        [field: SerializeField] public Color Color { get; private set; }
    }

    public class StatusResources : ScriptableObject
    {
        [field: SerializeField] public StatusData[] Datas { get; private set; }
    }
}