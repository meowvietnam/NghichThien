using UnityEngine;

namespace Network
{
    [CreateAssetMenu(fileName = "ServerConfig", menuName = "Configs/ServerConfig")]
    public class ServerConfig : ScriptableObject
    {
        [field: SerializeField] public string Host { get; private set; }
        [field: SerializeField] public int Port { get; private set; }
    }
}
