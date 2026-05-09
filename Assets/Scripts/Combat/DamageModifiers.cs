using System;
using UnityEngine;

public interface IDamageModifier
{
    int Calculate();
}

public abstract class BaseDamageModifier : IDamageModifier
{
    protected readonly IDamageModifier _innerModifier;
    protected int _baseDamage;

    protected BaseDamageModifier(IDamageModifier innerModifier, int baseDamage)
    {
        _innerModifier = innerModifier;
        _baseDamage = baseDamage;
    }

    public virtual int Calculate()
    {
        return _innerModifier?.Calculate() ?? _baseDamage;
    }
}

public class PlainDamageModifier : IDamageModifier
{
    private readonly int _damage;

    public PlainDamageModifier(int damage)
    {
        _damage = damage;
    }

    public int Calculate() => _damage;
}

/// <summary>
/// Buff decorators - add damage based on sender's buffs
/// </summary>
public class OffensiveBuffDecorator : BaseDamageModifier
{
    private readonly EBuffAndDeBuff _buffType;
    private readonly CharacterCombat _sender;
    private readonly BaseSkill.ETypeSkill[] _eTypeSkils;

    public OffensiveBuffDecorator(
        IDamageModifier innerModifier,
        int baseDamage,
        CharacterCombat sender,
        EBuffAndDeBuff buffType,
        BaseSkill.ETypeSkill[] eTypeSkils)
        : base(innerModifier, baseDamage)
    {
        _sender = sender;
        _buffType = buffType;
        _eTypeSkils = eTypeSkils;
    }

    public override int Calculate()
    {
        int damage = base.Calculate();

        if (_sender != null && _eTypeSkils != null && _sender.buffAndDebuffCount.TryGetValue(_buffType, out SecureInt buffValue))
        {
            // Check if this buff applies to current skill types
            bool applies = false;
            foreach (var skillType in _eTypeSkils)
            {
                if (MatchesBuffType(_buffType, skillType))
                {
                    applies = true;
                    break;
                }
            }

            if (applies)
            {
                damage += buffValue;
            }
            if (_sender != null && _eTypeSkils != null && CanRemove(_buffType))
            {
                _sender.RemoveBuffAndDebuffCount(_buffType, buffValue.Value);
            }
        }

        return damage;
    }

   

    private bool MatchesBuffType(EBuffAndDeBuff buffType, BaseSkill.ETypeSkill skillType)
    {
        return (buffType == EBuffAndDeBuff.ChanKim && skillType == BaseSkill.ETypeSkill.Kim) ||
               (buffType == EBuffAndDeBuff.ChanMoc && skillType == BaseSkill.ETypeSkill.Moc) ||
               (buffType == EBuffAndDeBuff.ChanThuy && skillType == BaseSkill.ETypeSkill.Thuy) ||
               (buffType == EBuffAndDeBuff.ChanHoa && skillType == BaseSkill.ETypeSkill.Hoa) ||
               (buffType == EBuffAndDeBuff.ChanTho && skillType == BaseSkill.ETypeSkill.Tho) ||
               (buffType == EBuffAndDeBuff.ChanKiem && skillType == BaseSkill.ETypeSkill.Kiem) ||
               (buffType == EBuffAndDeBuff.ChanThan && skillType == BaseSkill.ETypeSkill.Than) ||
               (buffType == EBuffAndDeBuff.ChanKhi && skillType == BaseSkill.ETypeSkill.Khi) ||

               (buffType == EBuffAndDeBuff.NguKhiHiep && skillType == BaseSkill.ETypeSkill.Khi) ||
               (buffType == EBuffAndDeBuff.NguKimHiep && skillType == BaseSkill.ETypeSkill.Kim) ||
               (buffType == EBuffAndDeBuff.NguMocHiep && skillType == BaseSkill.ETypeSkill.Moc) ||
               (buffType == EBuffAndDeBuff.NguThuyHiep && skillType == BaseSkill.ETypeSkill.Thuy) ||
               (buffType == EBuffAndDeBuff.NguHoaHiep && skillType == BaseSkill.ETypeSkill.Hoa) ||
               (buffType == EBuffAndDeBuff.NguThoHiep && skillType == BaseSkill.ETypeSkill.Tho) ||
               (buffType == EBuffAndDeBuff.NguKiemHiep && skillType == BaseSkill.ETypeSkill.Kiem) ||
               (buffType == EBuffAndDeBuff.NguThanThucHiep && skillType == BaseSkill.ETypeSkill.Than) ||

               (buffType == EBuffAndDeBuff.Diem && skillType == BaseSkill.ETypeSkill.Hoa);
    }

    private bool CanRemove(EBuffAndDeBuff buffType)
    {
        return buffType == EBuffAndDeBuff.ChanKim ||
               buffType == EBuffAndDeBuff.ChanMoc ||
               buffType == EBuffAndDeBuff.ChanThuy ||
               buffType == EBuffAndDeBuff.ChanHoa ||
               buffType == EBuffAndDeBuff.ChanTho ||
               buffType == EBuffAndDeBuff.ChanKiem ||
               buffType == EBuffAndDeBuff.ChanThan ||
               buffType == EBuffAndDeBuff.ChanKhi;
    }
}

/// <summary>
/// Defense debuff decorators - reduce damage based on defender's debuffs
/// </summary>
public class DefenseDebuffDecorator : BaseDamageModifier
{
    private readonly EBuffAndDeBuff _debuffType;
    private readonly CharacterCombat _defender;
    private readonly BaseSkill.ETypeSkill[] _eTypeSkils;

    public DefenseDebuffDecorator(
        IDamageModifier innerModifier,
        int baseDamage,
        CharacterCombat defender,
        EBuffAndDeBuff debuffType,
        BaseSkill.ETypeSkill[] eTypeSkils)
        : base(innerModifier, baseDamage)
    {
        _defender = defender;
        _debuffType = debuffType;
        _eTypeSkils = eTypeSkils;
    }

    public override int Calculate()
    {
        int damage = base.Calculate();

        if (_defender != null && _defender.buffAndDebuffCount.TryGetValue(_debuffType, out SecureInt debuffValue))
        {
            if (_eTypeSkils != null && MatchesDebuffType(_debuffType))
            {
                damage -= debuffValue;
            }
        }

        return Mathf.Clamp(damage, 0, int.MaxValue);
    }

    private bool MatchesDebuffType(EBuffAndDeBuff debuffType)
    {
        if (_eTypeSkils == null) return false;

        return (debuffType == EBuffAndDeBuff.NguKim && System.Array.Exists(_eTypeSkils, e => e == BaseSkill.ETypeSkill.Kim)) ||
               (debuffType == EBuffAndDeBuff.NguMoc && System.Array.Exists(_eTypeSkils, e => e == BaseSkill.ETypeSkill.Moc)) ||
               (debuffType == EBuffAndDeBuff.NguThuy && System.Array.Exists(_eTypeSkils, e => e == BaseSkill.ETypeSkill.Thuy)) ||
               (debuffType == EBuffAndDeBuff.NguHoa && System.Array.Exists(_eTypeSkils, e => e == BaseSkill.ETypeSkill.Hoa)) ||
               (debuffType == EBuffAndDeBuff.NguTho && System.Array.Exists(_eTypeSkils, e => e == BaseSkill.ETypeSkill.Tho));
    }
}


public class SuongGiaDecorator : BaseDamageModifier
{
    private readonly CharacterCombat _sender;

    public SuongGiaDecorator(IDamageModifier innerModifier, int baseDamage, CharacterCombat sender)
        : base(innerModifier, baseDamage)
    {
        _sender = sender;
    }

    public override int Calculate()
    {
        int damage = base.Calculate();

        if (_sender != null && _sender.buffAndDebuffCount.TryGetValue(EBuffAndDeBuff.SuongGia, out SecureInt suongGia))
        {
            damage -= suongGia;
        }
        if (_sender != null)
        {
            _sender.RemoveBuffAndDebuffCount(EBuffAndDeBuff.SuongGia, 1);
        }
        return Mathf.Clamp(damage, 0, int.MaxValue);
    }

   
}
public class GiamTonThuongDecorator : BaseDamageModifier
{
    private readonly CharacterCombat _defender;

    public GiamTonThuongDecorator(IDamageModifier innerModifier, int baseDamage, CharacterCombat defender)
        : base(innerModifier, baseDamage)
    {
        _defender = defender;
    }

    public override int Calculate()
    {
        int damage = base.Calculate();

        if (_defender != null && _defender.buffAndDebuffCount.TryGetValue(EBuffAndDeBuff.GiamTonThuong, out SecureInt giamTonThuong))
        {
            damage -= giamTonThuong;
        }

        return Mathf.Clamp(damage, 0, int.MaxValue);
    }
}
public class BuffDameDecorator : BaseDamageModifier
{
    private readonly CharacterCombat _character;
    private readonly EBuffAndDeBuff _type;

    public BuffDameDecorator(IDamageModifier innerModifier, int baseDamage, CharacterCombat character , EBuffAndDeBuff typeBuff)
        : base(innerModifier, baseDamage)
    {
        _character = character;
        _type = typeBuff;
    }

    public override int Calculate()
    {
        int damage = base.Calculate();

        if (_character != null && _character.buffAndDebuffCount.TryGetValue(_type, out SecureInt value))
        {
            damage += value;
        }

        return Mathf.Clamp(damage, 0, int.MaxValue);
    }
}

