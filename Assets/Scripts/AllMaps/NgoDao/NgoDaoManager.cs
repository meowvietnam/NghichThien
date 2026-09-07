using System;
using System.Collections.Generic;
using System.Linq;
using Network;
using Network.Extensions;
using UnityEngine;

namespace AllMaps.NgoDao
{
    [Serializable]
    public class NgoDaoManager : IMemorySerializable<NgoDaoManager>
    {
        [field: SerializeField] public List<NgoDaoResource> NgoDaoResources { get; set; }

        public void Deserialize(IReader reader, ref NgoDaoManager value)
        {
            value.NgoDaoResources = reader.ReadArray<NgoDaoResource>().ToList();
        }

        public int ByteSize()
        {
            return NgoDaoResources.ByteSize();
        }

        public void Serialize(IWriter writer)
        {
            writer.WriteArray(NgoDaoResources);
        }
    }
}