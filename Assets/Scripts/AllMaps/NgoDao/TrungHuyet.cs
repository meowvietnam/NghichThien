using LongDH.LongDHModules;
using System.Linq;
using UnityEngine;
using System;
using Network;
using Network.Extensions;

[CreateAssetMenu(fileName = nameof(TrungHuyet), menuName = "Ngộ Đạo/Ngộ Đạo Combat/" + nameof(TrungHuyet))]
public class TrungHuyet : NgoDaoCombat, IMemorySerializable<TrungHuyet>
{
    public override void ExecuteNgoDao(CharacterCombat caster, CharacterCombat target, params object[] data)
    {
        var debuffKeys = caster.buffAndDebuffCount.Keys.Where(key => BaseSkill.debuffWeights.Keys.Contains(key)).ToList();
        // 2. Kiểm tra xem có debuff nào để xóa không
        if (debuffKeys.Count > 0)
        {
            // 3. Chọn ngẫu nhiên một key trong danh sách debuff đã lọc
            var randomDebuffKey = debuffKeys[CombatStatic.IndexRandom(0, debuffKeys.Count)];
            // 4. Thực hiện xóa 1 Count
            caster.RemoveBuffAndDebuffCount(randomDebuffKey, 1);
        }
    }

    public override string GetDes()
    {
        return $"Mỗi lần sử dụng thần thông, ngẫu nhiên gỡ ra một tầng trạng thái tiêu cực";
    }

    public override void InitPosCallExecute(CharacterCombat caster)
    {
        Observer.AddObserver(EventList.Combat.PLAY_SKILL, ObserverEffect);
    }

    public override int ByteSize()
    {
        var byteSize = base.ByteSize();
        return byteSize;
    }

    public void Deserialize(IReader reader, ref TrungHuyet value)
    {
        NgoDaoCombat baseValue = value;
        baseValue.Deserialize(reader, ref baseValue);
    }

    public override void Serialize(IWriter writer)
    {
        base.Serialize(writer);
    }
}