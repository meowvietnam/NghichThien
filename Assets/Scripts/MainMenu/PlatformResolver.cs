using System;
using UnityEngine;

namespace MainMenu
{
    public class PlatformResolver : MonoBehaviour
    {
        [SerializeField] private GameObject or;
        [SerializeField] private GameObject googleSignInButton;

        private void OnValidate()
        {
            Run();
        }

        public void Run()
        {
            #if UNITY_IPHONE 
            or.SetActive(false);
            googleSignInButton.SetActive(false);
            #else
            or.SetActive(true);
            googleSignInButton.SetActive(true);
            #endif
        }
    }
}
