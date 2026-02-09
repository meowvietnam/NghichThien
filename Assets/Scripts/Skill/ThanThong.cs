using DG.Tweening;
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
        int level = characterStatsCurrent.level;
        int daiCanhGioi = ((level - 1) / 3) + 1;
        int indexLevel = daiCanhGioi - 1;
        return indexLevel;
    }
    public bool IsCanUseLinhKhiRequire(CharacterCombat caster)
    {
        foreach (var require in requireActives)
        {
            if (require.linhKhiRequire == ELinhKhi.None)
            {
                // Kiểm tra xem có CÓ ÍT NHẤT MỘT loại nào đủ count hay không
                bool hasAnyTypeEnough = caster.khiKim >= require.count ||
                                        caster.khiMoc >= require.count ||
                                        caster.khiThuy >= require.count ||
                                        caster.khiHoa >= require.count ||
                                        caster.khiTho >= require.count ||
                                        caster.khiMa >= require.count;

                if (!hasAnyTypeEnough) return false;
            }
            else
            {
                // Nếu yêu cầu loại cụ thể thì phải đúng loại đó
                int availableCount = caster.GetLinhKhi(require.linhKhiRequire);
                if (availableCount < require.count) return false;
            }
        }
        return true;
    }

   
    public virtual bool IsCanUse(CharacterCombat caster , CharacterCombat target)
    {
        return true;
    }    
    protected virtual void ShowSkillEffect(CharacterCombat target , float delay , Vector3 addPos = default)
    {
        if (prefabEffectSkill == null) return;
        var posCenter = Vector3.up * 500;
        posCenter += addPos;
        var effect = prefabEffectSkill.Spawn(target.transform);
        effect.transform.localPosition = posCenter;
        effect.transform.localScale = prefabEffectSkill.transform.localScale;
        DOVirtual.DelayedCall(delay, () =>
        {
            effect.gameObject.Recycle();
        });

    }
    protected virtual void ShowSkillEffectParticle(CharacterCombat target, float delay,float cameraSize,bool isNullParent, Vector3 addPos = default)
    {
        if (prefabEffectSkill == null) return;
        
        GameObject effect = !isNullParent ? prefabEffectSkill.Spawn(target.transform) : prefabEffectSkill.Spawn();
        effect.transform.localEulerAngles = prefabEffectSkill.transform.localEulerAngles;

        if (!isNullParent)
        {
            var posCenter = Vector3.up * 500;
            posCenter += addPos;
            effect.transform.localPosition = posCenter;

        }
        else
        {
            effect.transform.localPosition = Vector3.zero + addPos;
            int scaleX = (int)target.currentCharGender.transform.localScale.x;
            int signX = scaleX / Mathf.Abs(scaleX);
            if(signX<0)
            {
                effect.transform.localEulerAngles = new Vector3(effect.transform.localEulerAngles.x,effect.transform.localEulerAngles.y,effect.transform.localEulerAngles.z-180);

            }   
           
        }

        Camera.main.orthographicSize = cameraSize;

        DOVirtual.DelayedCall(delay, () =>
        {
            effect.gameObject.Recycle();
        });

    }
}
