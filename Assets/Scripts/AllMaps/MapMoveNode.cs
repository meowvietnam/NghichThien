using UnityEngine;

namespace AllMaps
{
    public class MapMoveNode : MonoBehaviour
    {
        [field: SerializeField] public int StartNode { get; private set; }
        [field: SerializeField] public int FinishNode { get; private set; }
        [field: SerializeField] public Vector2[] NodesPosition { get; private set; }
    }
}
