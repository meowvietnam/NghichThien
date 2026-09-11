using System.Collections.Generic;
using LuyenDan;
using MemoryPack;

namespace Network.Models
{
    /// <summary>
    /// Server → Client: result of a ConvertDanLuRequest.
    /// On success: RecipeItems lists the consumed recipe ItemRefs;
    ///             FurnaceResult is the newly obtained đan lô (FurnaceData).
    /// </summary>
    [MemoryPackable]
    public partial class ConvertDanLuResult : Result
    {
        /// <summary>The recipe ItemRefs that were consumed.</summary>
        public List<ItemRef> RecipeItems { get; set; }

        /// <summary>The đan lô (furnace) received as a result.</summary>
        public FurnaceData FurnaceResult { get; set; }
    }
}
