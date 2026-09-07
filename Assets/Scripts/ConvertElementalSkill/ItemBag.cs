using UnityEngine.EventSystems;

namespace ConvertElementalSkill
{
    public class ItemBag : BaseItemUI, IPointerEnterHandler, IPointerExitHandler
    {
        protected override void OnSelected()
        {
            ConvertElementalSkillPanel.Instance.Bag.OnSelected(this);
        }
    }
}