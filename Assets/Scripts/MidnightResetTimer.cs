using System;
using UnityEngine;
using UnityEngine.UI;

public class MidnightResetTimer : MonoBehaviour
{
    public Text textTimeReloadShop;
    private double totalSecondsRemaining;

    private void OnEnable()
    {        // 1. Tính toán t?ng s? giây còn l?i ngay khi m? Shop

        DateTime now = DateTime.Now;
        DateTime midnight = now.Date.AddDays(1);
        totalSecondsRemaining = (midnight - now).TotalSeconds;
    }
   

    void Update()
    {
        if (totalSecondsRemaining > 0)
        {
            // 2. Tr? th?i gian theo th?c t? (DeltaTime)
            totalSecondsRemaining -= Time.deltaTime;

            // 3. Chuy?n ??i s? giây thành ??nh d?ng H:M:S
            TimeSpan t = TimeSpan.FromSeconds(totalSecondsRemaining);
            string timeStr = string.Format("{0:D2}:{1:D2}:{2:D2}",
                t.Hours,
                t.Minutes,
                t.Seconds);

            // 4. C?p nh?t Text
            textTimeReloadShop.text = "Làm m?i sau: " + timeStr;
        }
        else
        {
            // 5. X? lý khi h?t th?i gian (Reset Shop)
            textTimeReloadShop.text = "?ang làm m?i...";
            // G?i hàm RefreshShop() c?a b?n ? ?ây
        }
    }
}
