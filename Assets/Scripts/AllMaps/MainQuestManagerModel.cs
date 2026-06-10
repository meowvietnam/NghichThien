using AllMaps;
using LongDH.LongDHModules;
using LongDHModules;
using Network;
using UnityEngine;
using Network.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
namespace AllMaps
{
    public class MainQuestManagerModel : IMemorySerializable<MainQuestManagerModel>
    {
        public List<MainQuestResource> MainQuestResource { get; set; }

        public int ByteSize()
        {
            return MainQuestResource.ByteSize();
        }

        public void Deserialize(IReader reader, ref MainQuestManagerModel value)
        {
            value.MainQuestResource = reader.ReadArray<MainQuestResource>().ToList();
        }

        public void Serialize(IWriter writer)
        {
            writer.WriteArray(MainQuestResource);
        }
    }
}
