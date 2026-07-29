using System;
using Network.Models;
using UnityEngine;

namespace Auction
{
    [Serializable]
    public class AuctionSessionStatusData
    {
        [field: SerializeField] public string Name { get; private set; }
        [field: SerializeField] public AuctionSessionStatus Status { get; private set; }
        [field: SerializeField] public Color Color { get; private set; }
        [field: SerializeField] public float Alpha { get; private set; }
    }
}