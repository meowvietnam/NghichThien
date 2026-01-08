using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace CreateCharacter
{
    public class ToggleGroupSkinGroups : MonoBehaviour
    {
        [SerializeField] private ToggleSkinGroup toggleSkinGroupPrefab;
        [SerializeField] private ToggleGroup toggleGroup;
        [SerializeField] private Transform content;
        [SerializeField] private ToggleGroupSkinCategoriesContainer toggleGroupSkinCategoriesContainer;
        [SerializeField] private ItemListGroupContainer itemListGroupContainer;
        public void Init(List<SkinGroup> skinGroups)
        {
            foreach (var skinGroup in skinGroups)
            {
                var toggleSkinGroup = Instantiate(toggleSkinGroupPrefab, content);
                toggleSkinGroup.Init(skinGroup,toggleGroup,OnToggleOn);
            }
        }
        private void OnToggleOn(ToggleSkinGroup toggleGroup)
        {
            toggleGroupSkinCategoriesContainer.Active(toggleGroup.Id);
            itemListGroupContainer.Active(toggleGroup.Id);
        }
    }
}
