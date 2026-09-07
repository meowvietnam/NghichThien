using UnityEngine;

public class MoneyResources : ScriptableObject
{
    [field: SerializeField] public Sprite MoneyIcon { get; private set; }
    [field: SerializeField] public Sprite MoneyVipIcon { get; private set; }
    public static MoneyResources instance;

    public static MoneyResources Instance
    {
        get
        {
            if (instance == null)
            {
                instance = Resources.Load<MoneyResources>("MoneyResources");
            }

            return instance;
        }
    }
}