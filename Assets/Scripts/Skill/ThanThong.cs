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
       
        int indexLevel = GetLevelThanThong(characterStatsCurrent) - 1;
        return indexLevel;
    }
    public static int GetLevelThanThong(CharacterStats characterStatsCurrent)
    {
        int level = characterStatsCurrent.level;
        int daiCanhGioi = ((level - 1) / 3) + 1;
        return daiCanhGioi;
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
   
    protected virtual void ShowSkillEffectParticle(CharacterCombat target, float delay, float cameraSize = 50f) // particle thì chỉnh camerasize còn base thì chỉnh scale thôi
    {
        if (prefabEffectSkill == null) return;
        
        GameObject effect = prefabEffectSkill.Spawn(target.transform);
        effect.transform.SetParent(null);
        effect.transform.localEulerAngles = prefabEffectSkill.transform.localEulerAngles;
        effect.transform.localScale = new Vector3(1,1,1);
        int signX = target.currentCharGender.transform.localEulerAngles.y != 0 ? -1 : 1;
        if (signX < 0)
        {
            effect.transform.GetChild(0).transform.localEulerAngles = new Vector3(effect.transform.GetChild(0).transform.localEulerAngles.x, effect.transform.GetChild(0).transform.localEulerAngles.y,-180);

        }
        else
        {
            effect.transform.GetChild(0).transform.localEulerAngles = new Vector3(effect.transform.GetChild(0).transform.localEulerAngles.x, effect.transform.GetChild(0).transform.localEulerAngles.y, 0);

        }

        Camera.main.orthographicSize = cameraSize;

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
        int signX = target.currentCharGender.transform.localEulerAngles.y != 0 ? -1 : 1;
        effect.transform.localScale = new Vector3(1 * signX, 1, 1);


        Camera.main.orthographicSize = 50;

        DOVirtual.DelayedCall(delay, () =>
        {
            effect.gameObject.Recycle();
        });

    }
}
