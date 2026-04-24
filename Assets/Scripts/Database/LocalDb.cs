using System.IO;
using SQLite;
using UnityEditor;
using UnityEngine;

namespace Database
{
    public class LocalDb : MonoBehaviour
    {
        [SerializeField] private string fileName;
        private string FilePath => Path.Combine(Application.dataPath, "Database", fileName + ".db");
        [InitializeOnLoadMethod]
        public static void Initialize()
        {
        }
    }
}