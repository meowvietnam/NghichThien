using System;
using UnityEngine;
using UnityEngine.UI;

public class BaseBehaviour : MonoBehaviour
{
    public static void SetEventButton(Button btn , Action action)
    {
        if (btn == null)
        {
            return;
        }
        btn.onClick.RemoveAllListeners();
        btn.onClick.AddListener(() =>
        {
            action?.Invoke();
            SoundManager.Instance?.PlaySoundButtonClick();
        });
    }    
}
