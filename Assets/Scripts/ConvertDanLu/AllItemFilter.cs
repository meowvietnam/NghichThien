namespace ConvertDanLu
{
    /// <summary>
    /// Filter "Toàn bộ" – shows all items regardless of Tier.
    /// </summary>
    public class AllItemFilter : BaseFilter
    {
        public override bool Filter(ItemBag item)
        {
            return true;
        }
    }
}
