using System;
using Base;
using CreateCharacter;
using MemoryPack;

namespace Network.Models
{
    [MemoryPackable]
    [Serializable]
    public partial class PlayerAuctionModel
    {
        public int PlayerId { get; set; }
        public int CharId { get; set; }
        public string Name { get; set; }
        public Gender Gender { get; set; }
        public Appearance Appearance { get; set; }
        public PlayerAuctionStatus Status { get; set; }
        public int Amount { get; set; }

        public void ResetNewSession()
        {
            Amount = 0;
            Status = PlayerAuctionStatus.Guest;
        }
    }
}