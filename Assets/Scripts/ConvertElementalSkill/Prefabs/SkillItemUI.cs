namespace ConvertElementalSkill.Prefabs
{
    public class SkillItemUI : BaseItemUI
    {
        protected override void OnShowInfo()
        {
            ToolTipItems.Instance.ShowInfoSkill(SkillData.ItemData);
        }
    }
}