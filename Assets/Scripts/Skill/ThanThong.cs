using DG.Tweening;
using LongDH.LongDHModules;
using LongDH.Script.ObjectPool;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
public abstract class ThanThong : BaseSkill
{
    [System.Serializable]
    public class RequireActive 
    {
        public ELinhKhi linhKhiRequire;
        public int count;
    }
    [SerializeField]protected List<RequireActive> requireActives;
    public virtual List<RequireActive> GetRequireActive(CharacterCombat caster)
    {
        return requireActives;
    }
    public ELinhKhi[] LinhKhiTypes =>
    requireActives
        .Where(r => r.linhKhiRequire != ELinhKhi.None)
        .Select(r => r.linhKhiRequire)
        .ToArray();
    protected override ETypeItem typeItem
    {
        get => ETypeItem.ThanThong;
    }
    public float timeDelayTakeDame = 0;
    public override abstract string GetDesc(int level);
    public override string GetDesc()
    {
        return "";
    }

    public void HauKeVoLuc(CharacterCombat caster)
    {
        caster.AddThanThongCoolDown(this, 1);

    }
    [SerializeField] protected GameObject prefabEffectSkill;
    public abstract override void Execute(CharacterCombat caster, CharacterCombat target, params object[] data);

    public static int GetIndexLevelThanThong(CharacterCombat caster)
    {
        return GetIndexLevelThanThong(caster.characterStatsCurrent);
    }
    public static int GetIndexLevelThanThong(CharacterStats characterStatsCurrent)
    {
       
        int indexLevel = GetLevelThanThong(characterStatsCurrent) - 1;
        return indexLevel;
    }
    public static int GetLevelThanThong(CharacterStats characterStatsCurrent)
    {
        int level = characterStatsCurrent.level;
        int daiCanhGioi = GetDaiCanhGioi(level);
        return daiCanhGioi;
    }
    public static int GetDaiCanhGioi(int level)
    {
        int daiCanhGioi = ((level - 1) / 3) + 1;
        return daiCanhGioi;
    }
    public bool IsCanUseLinhKhiRequire(CharacterCombat caster)
    {
        // 1. Thu thập tất cả các loại khí hiện có của caster vào một danh sách tạm
        // 1. Khởi tạo Dictionary chứa tài nguyên hiện có của caster
        var availableKhis = new Dictionary<ELinhKhi, int>
        {
            { ELinhKhi.Kim,  caster.khiKim },
            { ELinhKhi.Moc,  caster.khiMoc },
            { ELinhKhi.Thuy, caster.khiThuy },
            { ELinhKhi.Hoa,  caster.khiHoa },
            { ELinhKhi.Tho,  caster.khiTho },
            { ELinhKhi.Ma,   caster.khiMa }
        };

        // 2. Tách và xử lý các yêu cầu cụ thể (Fixed Type) trước để ưu tiên tài nguyên
        var requirements = GetRequireActive(caster).ToList();
        var fixedRequires = requirements.Where(r => r.linhKhiRequire != ELinhKhi.None);
        var noneRequires = requirements.Where(r => r.linhKhiRequire == ELinhKhi.None);

        foreach ( var requirement in fixedRequires)
        {
            if(!availableKhis.ContainsKey(requirement.linhKhiRequire) || availableKhis[requirement.linhKhiRequire] < requirement.count)
            {
                return false;
            }
            else if(availableKhis[requirement.linhKhiRequire] >= requirement.count)
            {
                availableKhis[requirement.linhKhiRequire] -= requirement.count;
            }    
        }
        foreach (var requirement in noneRequires)
        {
            if (!availableKhis.Any(x => x.Value >= requirement.count))
            {
                return false;
            }
            else
            {
                ELinhKhi linhKhi = availableKhis.FirstOrDefault(x => x.Value >= requirement.count).Key;
                availableKhis.Remove(linhKhi);

            }
        }
        return true;
    }

   
    public virtual bool IsCanUse(CharacterCombat caster , CharacterCombat target)
    {
        return caster.congPhapDynamic.Count(x => x.congPhap is DietLinhQuyet) <= DietLinhQuyet.require-1;
    }    
   
    protected virtual void ShowSkillEffectParticle(CharacterCombat target, float delay, float cameraSize = 50f) // particle thì chỉnh camerasize còn base thì chỉnh scale thôi
    {
        if (prefabEffectSkill == null) return;
        
        GameObject effect = prefabEffectSkill.Spawn(target.transform);
        effect.transform.SetParent(null);
        effect.transform.localEulerAngles = prefabEffectSkill.transform.localEulerAngles;
        effect.transform.localScale = new Vector3(1,1,1);
        int signX = target.transform.localEulerAngles.y != 0 ? -1 : 1;
        if (signX < 0)
        {
            effect.transform.GetChild(0).transform.localEulerAngles = new Vector3(effect.transform.GetChild(0).transform.localEulerAngles.x, effect.transform.GetChild(0).transform.localEulerAngles.y,-180);

        }
        else
        {
            effect.transform.GetChild(0).transform.localEulerAngles = new Vector3(effect.transform.GetChild(0).transform.localEulerAngles.x, effect.transform.GetChild(0).transform.localEulerAngles.y, 0);

        }

        Pool.Get<CombatManager>().cameraEffectSkill.orthographicSize = cameraSize;

        DOVirtual.DelayedCall(delay, () =>
        {
            effect.gameObject.Recycle();
        });

    }
    protected virtual void ShowSkillEffectWorld(CharacterCombat target, float delay) // particle thì chỉnh camerasize còn base thì chỉnh scale thôi
    {
        if (prefabEffectSkill == null) return;

        GameObject effect = prefabEffectSkill.Spawn(target.transform);
        effect.transform.SetParent(null);
        effect.transform.localEulerAngles = prefabEffectSkill.transform.localEulerAngles;
        int signX = target.transform.localEulerAngles.y != 0 ? -1 : 1;
        effect.transform.localScale = new Vector3(1 * signX, 1, 1);


        Pool.Get<CombatManager>().cameraEffectSkill.orthographicSize = 50;

        DOVirtual.DelayedCall(delay, () =>
        {
            effect.gameObject.Recycle();
        });

    }
    protected virtual void ShowSkillEffectNoTarget(float delay)// base thì chỉnh scale thôi
    {
        if (prefabEffectSkill == null) return;

        GameObject effect = prefabEffectSkill.Spawn();
        effect.transform.localEulerAngles = prefabEffectSkill.transform.localEulerAngles;
        effect.transform.localScale = Vector3.one;
        effect.transform.localPosition = Vector3.zero;

        Pool.Get<CombatManager>().cameraEffectSkill.orthographicSize = 50;

        DOVirtual.DelayedCall(delay, () =>
        {
            effect.gameObject.Recycle();
        });

    }
}
