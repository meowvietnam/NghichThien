using UnityEngine;
using UnityEngine.UI;

namespace ConvertElementalSkill
{
    public class SelectSkillType : BaseBehaviour
    {
        [SerializeField] private SkillType skillType;
        [SerializeField] private Button button;
        public SkillType SkillType => skillType;
        [field: SerializeField] public Sprite Icon { get; private set; }
        [field: SerializeField] public string Name { get; private set; }

        private void Start()
        {
            SetEventButton(button, OnSelected);
        }

        private void OnSelected()
        {
            ConvertElementalSkillPanel.Instance.OnSelectSkillType(this);
        }

        private void OnDestroy()
        {
            button.onClick.RemoveAllListeners();
        }
    }
}