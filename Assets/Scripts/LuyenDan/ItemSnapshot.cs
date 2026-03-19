using System;

namespace LuyenDan
{
    [Serializable]
    public class ItemSnapshot
    {
        public int Id;
        public int Count;

        public ItemSnapshot(int id, int count)
        {
            Id    = id;
            Count = count;
        }
        public bool IsEmpty => Count <= 0;
        public override string ToString() => $"[Id={Id}, Count={Count}]";
    }
}
