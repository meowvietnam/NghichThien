using UnityEngine;

namespace Auction
{
    [CreateAssetMenu(menuName = "Auction/Session Status Resource")]
    public class AuctionSessionStatusResource : ScriptableObject
    {
        [field: SerializeField] public AuctionSessionStatusData[] Datas { get; private set; }
    }
}