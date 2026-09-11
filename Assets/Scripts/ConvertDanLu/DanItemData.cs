using LuyenDan;

namespace ConvertDanLu
{
    public class DanItemData
    {
        public ItemRef ItemRef { get; }
        public IDanItemData ItemData { get; }

        public DanItemData(ItemRef itemRef, IDanItemData itemData)
        {
            ItemRef = itemRef;
            ItemData = itemData;
        }
    }
}
