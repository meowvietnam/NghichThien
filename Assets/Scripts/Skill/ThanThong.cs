using DG.Tweening;
using LongDH.LongDHModules;
using LongDH.Script.ObjectPool;
using System;
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
    public List<RequireActive> BaseRequireActive => requireActives;
    public virtual List<RequireActive> GetRequireActive(CharacterCombat caster)
    {
        return requireActives;
    }
    public ELinhKhi[] LinhKhiTypes =>
    requireActives
        .Where(r => r.linhKhiRequire != ELinhKhi.None)
        .Select(r => r.linhKhiRequire)
        .ToArray();
   
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
        return GetIndexLevelThanThong(caster.characterStatsTemp);
    }
    public static int GetIndexLevelThanThong(CharacterStatsSecure characterStatsCurrent)
    {
       
        int indexLevel = GetDaiCanhGioi(characterStatsCurrent) - 1;
        return indexLevel;
    }
    public static int GetDaiCanhGioi(CharacterCombat caster)
    {
        int daiCanhGioi = GetDaiCanhGioi(caster.characterStatsTemp);
        return daiCanhGioi;
    }
    public static int GetDaiCanhGioi(CharacterStatsSecure characterStatsCurrent)
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
    public List<LinhKhiTieuTan> GetLinhKhiTieuTanIfPossible(CharacterCombat caster)
    {
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

        List<LinhKhiTieuTan> consumedList = new List<LinhKhiTieuTan>();

        // 2. Tách yêu cầu
        var requirements = GetRequireActive(caster).ToList();
        var fixedRequires = requirements.Where(r => r.linhKhiRequire != ELinhKhi.None);
        var noneRequires = requirements.Where(r => r.linhKhiRequire == ELinhKhi.None);

        // 3. Xử lý Fixed Type (Yêu cầu đích danh loại khí)
        foreach (var requirement in fixedRequires)
        {
            if (!availableKhis.ContainsKey(requirement.linhKhiRequire) || availableKhis[requirement.linhKhiRequire] < requirement.count)
            {
                return null; // Không đủ điều kiện trả về null thay vì false
            }

            availableKhis[requirement.linhKhiRequire] -= requirement.count;

            // Thêm vào danh sách tiêu tán
            consumedList.Add(new LinhKhiTieuTan
            {
                eLinhKhi = requirement.linhKhiRequire,
                count = requirement.count
            });
        }

        // 4. Xử lý None Type (Loại khí bất kỳ)
        foreach (var requirement in noneRequires)
        {
            // Tìm loại khí đầu tiên có đủ số lượng (Ưu tiên loại có ít để giữ lại loại nhiều, hoặc ngược lại tùy logic bạn muốn)
            // Ở đây lấy loại đầu tiên tìm thấy thỏa mãn >= count
            var match = availableKhis.FirstOrDefault(x => x.Value >= requirement.count);

            if (match.Value < (int)requirement.count)
            {
                return null; // Không có loại khí nào đủ số lượng
            }

            ELinhKhi selectedKey = match.Key;
            availableKhis[selectedKey] -= requirement.count;

            // Thêm vào danh sách tiêu tán
            consumedList.Add(new LinhKhiTieuTan
            {
                eLinhKhi = selectedKey,
                count = requirement.count
            });
        }

        return consumedList; // Trả về danh sách nếu thỏa mãn tất cả
    }
    public List<LinhKhiTieuTan> GetLinhKhiTieuTanTuongDong(CharacterCombat caster)
    {
        var baseKhis = new Dictionary<ELinhKhi, int>
    {
        { ELinhKhi.Kim,  caster.khiKim },
        { ELinhKhi.Moc,  caster.khiMoc },
        { ELinhKhi.Thuy, caster.khiThuy },
        { ELinhKhi.Hoa,  caster.khiHoa },
        { ELinhKhi.Tho,  caster.khiTho },
        { ELinhKhi.Ma,   caster.khiMa }
    };

        var requirements = GetRequireActive(caster).ToList();
        var fixedRequires = requirements.Where(r => r.linhKhiRequire != ELinhKhi.None).ToList();
        var noneRequires = requirements.Where(r => r.linhKhiRequire == ELinhKhi.None).ToList();

        // 1. Trừ các yêu cầu cố định trước
        List<LinhKhiTieuTan> fixedConsumed = new List<LinhKhiTieuTan>();
        foreach (var req in fixedRequires)
        {
            if (baseKhis[req.linhKhiRequire] < (int)req.count) return null;
            baseKhis[req.linhKhiRequire] -= (int)req.count;
            fixedConsumed.Add(new LinhKhiTieuTan { eLinhKhi = req.linhKhiRequire, count = req.count });
        }

        // Nếu không có yêu cầu None, trả về kết quả Fixed luôn
        if (noneRequires.Count == 0) return CombatManager.IsTuongDong(fixedConsumed) ? fixedConsumed : null;

        // 2. TÌM KIẾM SỰ TƯƠNG ĐỒNG: Thử từng loại linh khí để làm "hệ chủ đạo" cho None
        // Theo logic IsTuongDong của bạn: Nếu Fixed đã có 1 loại, ta nên ưu tiên dùng loại đó cho None
        ELinhKhi? fixedType = fixedConsumed.Select(x => x.eLinhKhi).Distinct().Count() == 1
                              ? fixedConsumed[0].eLinhKhi : (ELinhKhi?)null;

        List<ELinhKhi> priorities = Enum.GetValues(typeof(ELinhKhi))
                                        .Cast<ELinhKhi>()
                                        .Where(e => e != ELinhKhi.None)
                                        .OrderByDescending(e => e == fixedType) // Ưu tiên hệ giống hệ Fixed đã có
                                        .ToList();

        foreach (var candidate in priorities)
        {
            var tempKhis = new Dictionary<ELinhKhi, int>(baseKhis);
            List<LinhKhiTieuTan> tempNoneConsumed = new List<LinhKhiTieuTan>();
            bool canFillAll = true;

            foreach (var req in noneRequires)
            {
                if (tempKhis[candidate] >= (int)req.count)
                {
                    tempKhis[candidate] -= (int)req.count;
                    tempNoneConsumed.Add(new LinhKhiTieuTan { eLinhKhi = candidate, count = req.count });
                }
                else
                {
                    canFillAll = false;
                    break;
                }
            }

            if (canFillAll)
            {
                var finalResult = fixedConsumed.Concat(tempNoneConsumed).ToList();
                // Kiểm tra lại bằng logic IsTuongDong của bạn để chắc chắn
                if (CombatManager.IsTuongDong(finalResult))
                {
                    return finalResult;
                }
            }
        }

        // 3. Nếu không thể tạo Tương Đồng, lấy đại (Logic cũ) hoặc trả về null tùy game design
        // Ở đây mình chọn: Nếu không thể tạo tương đồng thì vẫn trả về kết quả hợp lệ bất kỳ
        return null;
    }
    public List<LinhKhiTieuTan> GetLinhKhiETuongQuan(CharacterCombat caster, ETuongQuan targetQuan)
    {
        var requirements = GetRequireActive(caster).ToList();
        int totalSlots = requirements.Count;

        // 1. Phân loại yêu cầu
        var fixedReqs = requirements.Where(r => r.linhKhiRequire != ELinhKhi.None).ToList();
        var noneReqs = requirements.Where(r => r.linhKhiRequire == ELinhKhi.None).ToList();

        // 2. Lấy danh sách các hệ linh khí hợp lệ (Kim, Mộc, Thủy, Hỏa, Thổ)
        var allElements = Enum.GetValues(typeof(ELinhKhi))
            .Cast<ELinhKhi>()
            .Where(e => e != ELinhKhi.None && e != ELinhKhi.Ma) // Thường Ma không nằm trong ngũ hành tương sinh
            .ToList();

        // 3. Duyệt qua tất cả các tổ hợp có thể có của các ô "None"
        // Nếu chỉ có 1-2 ô None, việc duyệt này rất nhanh.
        // Chúng ta sẽ thử từng nhóm hệ (Distinct Elements) từ 1 đến 3 loại hệ.

        // Tạo danh sách các ứng viên hệ dựa trên các ô Fixed đã có
        var fixedTypes = fixedReqs.Select(r => r.linhKhiRequire).Distinct().ToList();

        // Thuật toán: Thử kết hợp các hệ vào các ô None
        // Để tối ưu, ta tìm các bộ (Set) từ 1 đến 3 hệ trước
        for (int k = 1; k <= 3; k++)
        {
            var combos = GetCombinations(allElements, k);
            foreach (var combo in combos)
            {
                // Kiểm tra xem combo này có chứa tất cả các hệ bắt buộc (Fixed) không
                if (!fixedTypes.All(f => combo.Contains(f))) continue;

                // Thử phân bổ các hệ trong combo này vào các ô None
                // Để đạt Tương Sinh/Khắc, ta chỉ cần ít nhất 1 đại diện của mỗi hệ trong combo
                // Cách đơn giản nhất: Gán mỗi hệ trong combo vào ít nhất 1 ô, các ô còn lại lấy hệ đầu tiên
                var candidateList = GenerateCandidate(fixedReqs, noneReqs, combo);
                if (candidateList == null) continue;

                // Kiểm tra logic ETuongQuan của bạn
                if (CombatManager.GetETuongQuan(candidateList) == targetQuan)
                {
                    // Kiểm tra xem nhân vật có đủ linh khí không
                    if (HasEnoughLinhKhi(caster, candidateList))
                    {
                        return candidateList;
                    }
                }
            }
        }

        return null; // Không tìm được bộ nào thỏa mãn
    }
    // Hàm hỗ trợ tạo danh sách tiêu tán thử nghiệm
    private List<LinhKhiTieuTan> GenerateCandidate(List<RequireActive> fixedReqs, List<RequireActive> noneReqs, List<ELinhKhi> combo)
    {
        var result = new List<LinhKhiTieuTan>();

        // Thêm các ô cố định
        foreach (var f in fixedReqs)
            result.Add(new LinhKhiTieuTan { eLinhKhi = f.linhKhiRequire, count = f.count });

        // Phân bổ các hệ trong combo vào các ô None
        for (int i = 0; i < noneReqs.Count; i++)
        {
            // Đảm bảo dùng hết các hệ trong combo, còn thừa thì dùng hệ cuối cùng
            ELinhKhi selected = (i < combo.Count) ? combo[i] : combo[combo.Count - 1];
            result.Add(new LinhKhiTieuTan { eLinhKhi = selected, count = noneReqs[i].count });
        }

        // Gộp nhóm lại để GetETuongQuan chạy đúng (vì nó dùng Distinct)
        return result.GroupBy(x => x.eLinhKhi)
                     .Select(g => new LinhKhiTieuTan { eLinhKhi = g.Key, count = g.Sum(x => (int)x.count) })
                     .ToList();
    }

    // Kiểm tra kho đồ thực tế
    private bool HasEnoughLinhKhi(CharacterCombat caster, List<LinhKhiTieuTan> needed)
    {
        foreach (var item in needed)
        {
            int available = item.eLinhKhi switch
            {
                ELinhKhi.Kim => caster.khiKim,
                ELinhKhi.Moc => caster.khiMoc,
                ELinhKhi.Thuy => caster.khiThuy,
                ELinhKhi.Hoa => caster.khiHoa,
                ELinhKhi.Tho => caster.khiTho,
                _ => 0
            };
            if (available < (int)item.count) return false;
        }
        return true;
    }

    // Hàm lấy tổ hợp (Helper)
    private IEnumerable<List<T>> GetCombinations<T>(List<T> list, int k)
    {
        if (k == 0) yield return new List<T>();
        else
        {
            for (int i = 0; i < list.Count; i++)
            {
                foreach (var rest in GetCombinations(list.Skip(i + 1).ToList(), k - 1))
                {
                    var result = new List<T> { list[i] };
                    result.AddRange(rest);
                    yield return result;
                }
            }
        }
    }
    public bool IsCanUseLinhKhiRequire(CharacterCombat caster)
    {
        return GetLinhKhiTieuTanIfPossible(caster) != null;
    }

   
    public virtual bool IsCanUse(CharacterCombat caster , CharacterCombat target)
    {
        return caster.congPhapDynamic.Count(x => x.congPhap is DietLinhQuyet) <= DietLinhQuyet.require-1;
    }

    protected GameObject ShowSkillEffectParticleBase(CharacterCombat caster, float delay, float cameraSize = 50f, bool isDestroy = true) // particle thì chỉnh camerasize còn base thì chỉnh scale thôi
    {
        var actionControl = GetActionControlEffectParticleBase();
        var effect = ShowSkillEffectParticle(caster, delay, cameraSize , isDestroy , actionControl.Item1,actionControl.Item2);
        
        return effect;

    }
    protected virtual (Action<Transform,Transform>,Action<Transform, Transform>) GetActionControlEffectParticleBase()
    {
        
        return ((childEffect,childPrefab) => {
            childEffect.transform.localPosition = new Vector3(childPrefab.transform.localPosition.x * -1, childPrefab.transform.localPosition.y, childPrefab.transform.localPosition.z);
            
        }, (childEffect, childPrefab) => {
            childEffect.transform.localPosition = childPrefab.transform.localPosition;
        }
        );
    }    
    protected virtual GameObject ShowSkillEffectParticle(CharacterCombat caster, float delay, float cameraSize = 50f ,bool isDestroy = true , Action<Transform, Transform> actionControlChildEffectIf = null, Action<Transform, Transform> actionControlChildEffectElse = null) // particle thì chỉnh camerasize còn base thì chỉnh scale thôi
    {
        if (prefabEffectSkill == null) return null;
        
        GameObject effect = prefabEffectSkill.Spawn(caster.transform);
        effect.transform.SetParent(null);
        effect.transform.localEulerAngles = prefabEffectSkill.transform.localEulerAngles;
        effect.transform.localScale = new Vector3(1,1,1);
        var childEffect = effect.transform.GetChild(0);
        var childPrefab = prefabEffectSkill.transform.GetChild(0);
        int signX = caster.transform.localEulerAngles.y != 0 ? -1 : 1;
        if (signX < 0)
        {
            childEffect.transform.localScale = new Vector3(childPrefab.localScale.x * -1, childPrefab.localScale.y, childPrefab.localScale.z);
            actionControlChildEffectIf?.Invoke(childEffect,childPrefab);
        }
        else
        {
            childEffect.transform.localScale = new Vector3(childPrefab.localScale.x, childPrefab.localScale.y, childPrefab.localScale.z);
            actionControlChildEffectElse?.Invoke(childEffect,childPrefab);

        }
        if(cameraSize == 50 )
        {
            effect.SetLayerRecursiveWithCamera(Pool.Get<CombatManager>().cameraEffectSkillLock50.cullingMask);

        }
        else if (cameraSize == 5)
        {
            effect.SetLayerRecursiveWithCamera(Pool.Get<CombatManager>().cameraEffectSkillLock5.cullingMask);

        }
        else
        {
            effect.SetLayerRecursiveWithCamera(Pool.Get<CombatManager>().cameraEffectSkillDynamic.cullingMask);
            Pool.Get<CombatManager>().cameraEffectSkillDynamic.orthographicSize = cameraSize;

        }
        var mainPS = effect.transform.GetChild(0).GetComponent<ParticleSystem>();

        if (mainPS != null)
        {
            // CÁCH SỬA CHÍNH Ở ĐÂY:
            // Thêm tham số true vào hàm Play để bao gồm tất cả các con lồng bên trong (withChildren)
            mainPS.Play(true);
        }
        if(isDestroy)
        {
            DOVirtual.DelayedCall(delay, () =>
            {
                effect.gameObject.Recycle();
            });
        }
        effect.transform.localPosition = new Vector3(effect.transform.localPosition.x, effect.transform.localPosition.y, 0);
        return effect;

    }
    protected virtual GameObject ShowSkillEffectWorld(CharacterCombat target, float delay , bool isDestroy = true) // particle thì chỉnh camerasize còn base thì chỉnh scale thôi
    {
        if (prefabEffectSkill == null) return null;

        GameObject effect = prefabEffectSkill.Spawn(target.transform);
        effect.transform.SetParent(null);
        effect.transform.localEulerAngles = prefabEffectSkill.transform.localEulerAngles;
        int signX = target.transform.localEulerAngles.y != 0 ? -1 : 1;
        effect.transform.localScale = new Vector3(1 * signX, 1, 1);


        effect.SetLayerRecursiveWithCamera(Pool.Get<CombatManager>().cameraEffectSkillLock50.cullingMask);
        effect.transform.localPosition = new Vector3(effect.transform.localPosition.x, effect.transform.localPosition.y, 0);

        if (isDestroy)
        {
            DOVirtual.DelayedCall(delay, () =>
            {
                effect.gameObject.Recycle();
            });
        }
        return effect;

    }
    protected virtual void ShowSkillEffectNoTarget(float delay)// base thì chỉnh scale thôi
    {
        if (prefabEffectSkill == null) return;

        GameObject effect = prefabEffectSkill.Spawn();
        effect.transform.localEulerAngles = prefabEffectSkill.transform.localEulerAngles;
        effect.transform.localScale = Vector3.one;
        effect.transform.localPosition = Vector3.zero;

        effect.SetLayerRecursiveWithCamera(Pool.Get<CombatManager>().cameraEffectSkillLock50.cullingMask);

        DOVirtual.DelayedCall(delay, () =>
        {
            effect.gameObject.Recycle();
        });

    }
 
}
