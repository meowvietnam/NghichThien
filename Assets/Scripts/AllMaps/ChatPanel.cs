using LongDH.LongDHModules;
using UnityEngine;

public class ChatPanel : MonoBehaviour
{
    private void Awake()
    {
        Pool.Set(this);
    }
}
