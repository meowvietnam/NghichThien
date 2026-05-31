using System;
using UnityEngine;
using UnityEngine.UI;

public class MidnightResetTimer : MonoBehaviour
{
    public Text textTimeReloadShop;
    private double totalSecondsRemaining;
    private void OnEnable()
    {
        DateTime now = DateTime.Now;
        DateTime midnight = now.Date.AddDays(1);
        totalSecondsRemaining = (midnight - now).TotalSeconds;
    }


    void Update()
    {
        if (totalSecondsRemaining > 0)
        {
            totalSecondsRemaining -= Time.deltaTime;
            TimeSpan t = TimeSpan.FromSeconds(totalSecondsRemaining);
            string timeStr = string.Format("{0:D2}:{1:D2}:{2:D2}",
                t.Hours,
                t.Minutes,
                t.Seconds);
            textTimeReloadShop.text = "Làm mới sau: " + timeStr;
        }
        else
        {
            textTimeReloadShop.text = "Đang làm mới...";
        }
    }
}
