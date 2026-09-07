using System.Collections.Generic;
using MemoryPack;

namespace Network.Models
{
    /// <summary>
    /// Client → Server: request to convert 4 đan lô (furnaces, identified by SelfId) into 1 đan phương (recipe).
    /// </summary>
    [MemoryPackable]
    public partial class ConvertDanLuRequest
    {
        /// <summary>SelfId list of the 4 furnaces to consume.</summary>
        public List<ushort> FurnaceSelfIds { get; set; }
    }
}
