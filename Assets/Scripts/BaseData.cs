using UnityEngine;
using Sirenix.OdinInspector;

public class BaseData : ScriptableObject
{
    [field: SerializeField, ReadOnly] protected int id;
    public int Id => id;
    [SerializeField] public string nameItem;
    [SerializeField]
    public Sprite spriteItem;
    
    public void SetId()
    {
        id = int.Parse(spriteItem.name);
    }

}
