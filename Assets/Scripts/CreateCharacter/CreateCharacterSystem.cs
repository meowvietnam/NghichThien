using UnityEngine;

namespace CreateCharacter
{
    public class CreateCharacterSystem : MonoBehaviour
    {
        [SerializeField] private Transform createAvatarParent;
        [SerializeField]
        private CreateAvatar createAvatarPrefab;
        [SerializeField] private int totalSlots = 3;
        [SerializeField] private SkinResources skinResources;
        [SerializeField] private CreateCharacterPanel createCharacterPanel;
        private void Start()
        {
            createCharacterPanel.Init(skinResources);
        }
    }
}
