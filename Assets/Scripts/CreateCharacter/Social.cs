using System;
using System.Collections.Generic;
using System.Linq;
using AllMaps;
using Network;
using Network.Extensions;
using Sirenix.OdinInspector;
using UnityEngine;

namespace CreateCharacter
{
    [Serializable]
    public class Social : IMemorySerializable<Social>
    {
        [field: SerializeField] public List<Mail> Mails { get; set; } = new();
        [field: ShowInInspector] public HashSet<Relationship> Relationships { get; set; } = new();

        public void Deserialize(IReader reader, ref Social value)
        {
            value.Mails = reader.ReadArray<Mail>().ToList();
            value.Relationships = reader.ReadArray<Relationship>().ToHashSet();
        }

        public int ByteSize()
        {
            return Mails.ByteSize() + Relationships.ByteSize();
        }

        public void Serialize(IWriter writer)
        {
            writer.WriteArray(Mails);
            writer.WriteArray(Relationships);
        }

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