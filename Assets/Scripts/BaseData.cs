using UnityEngine;
using Sirenix.OdinInspector;

public class BaseData : ScriptableObject
{
    [field: SerializeField] protected int id;
    public int Id => id;
    [SerializeField] public string nameItem;
    [SerializeField]
    public Sprite spriteItem;
    
    public virtual void SetId()
    {
        id = int.Parse(spriteItem.name);
    }

}
