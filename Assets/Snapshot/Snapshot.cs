using System;
using System.IO;
using Sirenix.OdinInspector;
using UnityEngine;

namespace  Snapshot
{
    public class Snapshot : MonoBehaviour
    {
        private void Start()
        {
            DontDestroyOnLoad(gameObject);
        }
        [Button]
        public void Capture()
        {
            string timestamp = DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss");
            string fileName = $"screen-capture-{timestamp}.png";
            var filePath = Path.Combine(Application.dataPath, "Snapshot", fileName);
            ScreenCapture.CaptureScreenshot(filePath);
            Debug.Log("Captured screenshot");
        }
    }
}
