using System.Collections.Generic;
using LuyenDan;
using MemoryPack;

namespace Network.Models
{
    /// <summary>
    /// Client → Server: request to convert 4 đan phương (recipes) into 1 đan lô (furnace).
    /// </summary>
    [MemoryPackable]
    public partial class ConvertDanLuRequest
    {
        /// <summary>
        /// The recipe ItemRefs to consume (grouped by Id, Count indicates quantity used).
        /// Total count across all entries must equal 4.
        /// </summary>
        public List<ItemRef> RecipeItems { get; set; }
    }
}
