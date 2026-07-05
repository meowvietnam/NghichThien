using System;
using System.Collections.Generic;
using System.Linq;
using AllMaps;
using MemoryPack;
using Network;
using Network.Extensions;
using Network.Extensions.MemoryPack;
using Sirenix.OdinInspector;
using UnityEngine;

namespace CreateCharacter
{
    [Serializable]
    [MemoryPackable]
    public partial class Social
    {
        [field: SerializeField] public List<Mail> Mails { get; set; } = new();
        [field: ShowInInspector] public HashSet<Relationship> Relationships { get; set; } = new();
        public int GetRelationshipWithNPC(int npcId)
        {
            var r = new Relationship()
            {
                Npc = (ushort)npcId
            };
            if (Relationships.TryGetValue(r, out var relationship))
            {
                return relationship.Points;
            }
            return 0;
        }
      
        public Relationship GetRelationship(ushort npcId)
        {
            var relationshipRef = new Relationship()
            {
                Npc = npcId,
            };
            if (Relationships.TryGetValue(relationshipRef, out var relationship))
            {
                return relationship;
            }
            if (!Relationships.Contains(relationshipRef))
            {
                Relationships.Add(relationshipRef);
            }
            return relationshipRef;
        }
        public void RemoveRelationship(Relationship relationship)
        {
            Relationships.Remove(relationship);
        }
        public void RelationshipAddValue(Relationship relationship, int value)
        {
            var newValue = relationship.Points + value;
            relationship.Points = (ushort)newValue;
        }
        public void RelationshipSetValue(ushort npcId, int value)
        {
            var relationship = GetRelationship(npcId);
            relationship.Points = (ushort)value;
         
        }
        public void RelationshipAddValue(ushort npcId, int value)
        {
            var relationship = GetRelationship(npcId);
            RelationshipAddValue(relationship, value);
        }
    }
}