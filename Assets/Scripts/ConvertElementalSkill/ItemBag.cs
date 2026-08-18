using Base;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

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