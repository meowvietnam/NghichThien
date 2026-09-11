using UnityEngine;

namespace CreateCharacter.CharStats
{
    public abstract class Step : MonoBehaviour
    {
        [SerializeField] private GameObject thisObject;

        public void SetActive(bool isActive)
        {
            thisObject.SetActive(isActive);
        }

        public abstract void OnEnter();
        public abstract void OnExit();
        public abstract bool Validate();
    }
}