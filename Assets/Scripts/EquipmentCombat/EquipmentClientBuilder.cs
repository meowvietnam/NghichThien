using System.Collections.Generic;
using System.Linq;
using CreateCharacter;

public class EquipmentClientBuilder
{
    private readonly EquipmentClient _client = new EquipmentClient();

    public EquipmentClientBuilder WithId(ushort id)
    {
        _client.id = new SecureInt(id);
        return this;
    }

    public EquipmentClientBuilder WithName(string name)
    {
        _client.nameEquip = name;
        return this;
    }

    public EquipmentClientBuilder WithEffects(IEnumerable<Effect> effects)
    {
        _client.listEffectEquip = effects?
            .Select(effect => new EquipmentClient.EffectClient
            {
                Type = effect.Type,
                Value = new SecureInt(effect.Value)
            })
            .ToList() ?? new List<EquipmentClient.EffectClient>();

        return this;
    }

    public EquipmentClient Build()
    {
        return _client;
    }

    public static EquipmentClient FromEquipment(Equipment equipment)
    {
        if (equipment == null)
        {
            return new EquipmentClient();
        }

        return new EquipmentClientBuilder()
            .WithId(equipment.Id)
            .WithName(equipment.Name)
            .WithEffects(equipment.Effects)
            .Build();
    }
}
