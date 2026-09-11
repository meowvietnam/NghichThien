using System;
using System.Linq;
using MemoryPack;

namespace Events
{
    [MemoryPackable]
    public partial class ItemEvent : IEquatable<ItemEvent>
    {
        public ushort Id { get; set; }
        public int Count { get; set; }

        public override int GetHashCode()
        {
            return Id;
        }

        public bool Equals(ItemEvent? other)
        {
            if (other is null) return false;
            if (ReferenceEquals(this, other)) return true;
            return Id == other.Id;
        }

        public ItemEventData GetItemEventData()
        {
            return EventResources.Instance.Items.FirstOrDefault(x => x.Id == Id);
        }
    }
}