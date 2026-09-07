using System;
using Network;
using Network.Extensions;
using UnityEngine;
using Sirenix.OdinInspector;

public class BaseData : ScriptableObject, IMemorySerializable<BaseData>
{
    [field: SerializeField] public int Id { get; protected set; }
    [SerializeField] public string nameItem;
    [SerializeField] public Sprite spriteItem;

    public virtual void SetId()
    {
        if (int.TryParse(spriteItem.name, out var id))
        {
            Id = id;
        }
    }

    public void Deserialize(IReader reader, ref BaseData value)
    {
        value.Id = reader.Read<int>();
        value.nameItem = reader.Read();
    }

    public virtual int ByteSize()
    {
        return Id.ByteSize() + nameItem.ByteSize();
    }

    public virtual void Serialize(IWriter writer)
    {
        writer.Write(Id);
        writer.Write(nameItem);
    }
}