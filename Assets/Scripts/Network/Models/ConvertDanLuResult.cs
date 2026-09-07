using System.Collections.Generic;
using LuyenDan;
using MemoryPack;

namespace Network.Models
{
    /// <summary>
    /// Server → Client: result of a ConvertDanLuRequest.
    /// On success: FurnaceSelfIds lists the consumed furnace SelfIds;
    ///             ItemResult is the newly obtained đan phương (recipe) ItemRef.
    /// </summary>
    [MemoryPackable]
    public partial class ConvertDanLuResult : Result
    {
        /// <summary>SelfIds of the furnaces that were consumed.</summary>
        public List<ushort> FurnaceSelfIds { get; set; }

        /// <summary>The recipe (đan phương) ItemRef received.</summary>
        public ItemRef ItemResult { get; set; }
    }
}
