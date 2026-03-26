using System;
using UnityEngine;
using UnityEngine.UI;

public class BaseBehaviour : MonoBehaviour
{
   
    protected void SetEventButton(Button btn , Action action)
    {
        btn.onClick.RemoveAllListeners();
        btn.onClick.AddListener(() =>
        {
            action?.Invoke();
            //sound btn

        });
    }    
}
