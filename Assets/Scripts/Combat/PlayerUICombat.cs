using CreateCharacter;
using DG.Tweening;
using LongDH.LongDHModules;
using NUnit.Framework.Interfaces;
using Sirenix.OdinInspector;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using LongDHModules;
using UnityEngine;
using UnityEngine.UI;
using ThanThong = Skill.ThanThong;

public class PlayerUICombat : BaseBehaviour
{
    public enum EStateControl
    {
        None,
        UseSkill,
        DeleteLinhKhi,
    }

    public List<SlotLinhKhi> nodes; // các nút cần sắp xếp
    public List<SlotLinhKhi> linhKhiRemains; // các nút cần sắp xếp
    public List<SlotLinhKhi> currentNodes; // các nút cần sắp xếp
    public Sprite[] spriteUILinhKhis;

    public float radius = 120f; // bán kính vòng tròn
    public Button btnConfirm;
    public Button btnCancel;
    public Button btnEndRound;
    public int maxLinhKhi => player.totalKhi;
    public int CurrentLinhKhi => linhKhiRemains.Sum(item => item.count);

    public EStateControl eStateControl = EStateControl.None;

    public List<FightSkillItem> fightSkills;
    public PlayerUICombatItem equipmentWeapon;

    public FightSkillItem fightSkillSelect;
    public Image tuongSinhImg;
    public Image tuongKhacImg;
    private CharacterCombat player => CombatManager.char1;
    public Text textTip;
    public ThanThong[] currentThanThong => player.thanThongEquip.ToArray(); // test
    public CombatManager CombatManager => transform.GetComponentInParent<CombatManager>();

    public SkillTip skillTip;

    public GameObject linhKhiSelectUI;
    [SerializeField] private Text textLinhKhiUser;

    public void SetTextLinhKhiUser()
    {
        textLinhKhiUser.text = $"{CurrentLinhKhi}/{maxLinhKhi}";
    }

    public void SetStateControl(EStateControl eState)
    {
        eStateControl = eState;
        if (eState == EStateControl.None)
        {
            btnEndRound.transform.parent.gameObject.SetActive(true);
            btnConfirm.transform.parent.gameObject.SetActive(false);
            ShowNodes(0);
            if (fightSkillSelect != null)
            {
                fightSkillSelect.SetSelectHighlight(false);
                fightSkillSelect.ShowRecommentHighlight(false);
            }

            fightSkillSelect = null;
            tuongSinhImg.gameObject.SetActive(false);
            tuongKhacImg.gameObject.SetActive(false);
            CheckFightSkillCanUse();
            textTip.transform.parent.parent.transform.DOLocalMove(new Vector3(0, -50, 0), 0.5f);
        }
        else
        {
            btnEndRound.transform.parent.gameObject.SetActive(false);
            btnConfirm.transform.parent.gameObject.SetActive(true);
        }
    }

    private List<LinhKhiTieuTan> GetLinhKhiTieuTans(List<SlotLinhKhi> slotLinhKhis)
    {
        List<LinhKhiTieuTan> linhKhiTieuTan = new List<LinhKhiTieuTan>();
        foreach (var item in slotLinhKhis)
        {
            linhKhiTieuTan.Add(new LinhKhiTieuTan
            {
                count = item.count,
                eLinhKhi = item.eLinhKhi,
            });
        }

        return linhKhiTieuTan;
    }

    private void CheckTuongQuan()
    {
        if (eStateControl != EStateControl.UseSkill) return;
        switch (CombatManager.GetETuongQuan(GetLinhKhiTieuTans(currentNodes)))
        {
            case ETuongQuan.TuongSinh:
                tuongSinhImg.gameObject.SetActive(true);
                tuongKhacImg.gameObject.SetActive(false);

                break;
            case ETuongQuan.TuongKhac:
                tuongSinhImg.gameObject.SetActive(false);
                tuongKhacImg.gameObject.SetActive(true);
                break;
            case ETuongQuan.None:
                tuongSinhImg.gameObject.SetActive(false);
                tuongKhacImg.gameObject.SetActive(false);
                break;
            default:
                break;
        }
    }

    public void Init()
    {
        Observer.AddObserver(EventList.Combat.ADJUST_LINHKHI, SetTextLinhKhiUser);
        SetEventButton(btnConfirm, () =>
        {
            if (CombatManager.IsDelayGame) return;
            if (player.isAutoCastSkill) return;

            switch (eStateControl)
            {
                case EStateControl.None:

                    break;
                case EStateControl.UseSkill:
                    if (currentNodes.Any(x => x.countRequire > 0)) return;
                    Observer.Notify(EventList.Combat.PLAY_SKILL, player, CombatManager.char2,
                        fightSkillSelect.ThanThongData, GetLinhKhiTieuTans(currentNodes));
                    SetStateControl(EStateControl.None);
                    CheckFightSkillCanUse();
                    break;
                case EStateControl.DeleteLinhKhi:
                    if (CurrentLinhKhi <= maxLinhKhi || player.isSkipTieuTanLinhKhi == true)
                    {
                        this.gameObject.SetActive(false);
                        Observer.Notify(EventList.Combat.END_TURN_COMBAT, player, CombatManager.char2,
                            GetLinhKhiTieuTans(currentNodes));
                        SetStateControl(EStateControl.None);
                    }

                    break;
                default:
                    break;
            }
        });
        SetEventButton(btnCancel, () =>
        {
            if (CombatManager.IsDelayGame) return;
            if (player.isAutoCastSkill) return;

            foreach (var item in currentNodes)
            {
                if (item == null || item.eLinhKhi == ELinhKhi.None) continue;
                item.SetLock(false);
                int count = item.count;
                AddLinhKhiFromNodeToRemain(
                    item,
                    linhKhiRemains.FirstOrDefault(x => x.eLinhKhi == item.eLinhKhi),
                    count
                );
            }

            SetStateControl(EStateControl.None);
            CheckFightSkillCanUse();
        });

        SetEventButton(btnEndRound, () =>
        {
            ButtonEndRoundAction();
               
           
        });
        void ButtonEndRoundAction()
        {
            if (CombatManager.IsDelayGame) return;
            if (player.isAutoCastSkill) return;

            SetStateControl(EStateControl.DeleteLinhKhi);
            if (CurrentLinhKhi <= maxLinhKhi || player.isSkipTieuTanLinhKhi == true)
            {
                if (player.IsFullLinhKhi)
                {

                    Pool.Get<PopupConfirmAction>().ShowPopConfirmAction("Đã đạt tối đa số lượng linh khí (linh khí tối đa * 10). Không thể hấp thụ thêm linh khí", () =>
                    {
                        btnConfirm.onClick.Invoke();
                    });
                }
            }
            else
            {
                ShowNodes(6);
                textTip.transform.parent.parent.localPosition = new Vector3(370, 0, 0);
                textTip.transform.parent.parent.DOLocalMove(new Vector3(0, 0, 0), 0.5f);
                textTip.text = $"{CurrentLinhKhi - maxLinhKhi}";
            }

            CheckFightSkillCanUse();
        }
        Observer.AddObserver(EventList.Combat.NEW_TURN_COMBAT, NewTurn);
        Observer.AddObserver(EventList.Combat.ADJUST_LINHKHI, AdjustLinhKhi);
        Observer.AddObserver(EventList.Combat.PLAY_SKILL, CheckFightSkillCanUseObserver);

        foreach (var item1 in linhKhiRemains)
        {
            item1.Init(() =>
            {
                if (item1.count <= 0 || item1.isLock || currentNodes.All(x => x.isLock == true)) return;
                Debug.Log("lmm" + currentNodes.Count);

                if (CurrentLinhKhi <= maxLinhKhi && eStateControl == EStateControl.DeleteLinhKhi) return;
                Debug.Log("cmm" + currentNodes.Count);

                foreach (var item2 in currentNodes)
                {
                    if (item2.isLock || item2.countRequire <= 0) continue;

                    if (!item2.isLock && (item2.count <= 0 || item2.eLinhKhi == item1.eLinhKhi))
                    {
                        if (eStateControl == EStateControl.UseSkill && item1.count >= item2.countRequire)
                        {
                            AddLinhKhiFromRemainToNode(item1, item2, item2.countRequire);
                        }
                        else if (eStateControl == EStateControl.DeleteLinhKhi)
                        {
                            AddLinhKhiFromRemainToNode(item1, item2, 1);
                        }


                        break;
                    }
                }
            });
        }

        foreach (var item1 in nodes)
        {
            item1.Init(() =>
            {
                if (item1.count <= 0 || item1.isLock) return;

                foreach (var item2 in linhKhiRemains)
                {
                    if ((item2.eLinhKhi == item1.eLinhKhi))
                    {
                        AddLinhKhiFromNodeToRemain(item1, item2, item1.count);
                    }
                }
            });
        }

        for (var s = 0; s < fightSkills.Count; s++)
        {
            var skill = fightSkills[s];
            Action actionHolder = new Action(() =>
            {
                int daiCanhGioi = ThanThong.GetDaiCanhGioi(player.characterStatsTemp.level);
                var inforVisualSkill = Pool.Get<UIManager>()
                    .GetFrameAndQualityFullAndColorNameSkill((BaseSkill)skill.itemData);

                ToolTipItems.Instance.InitShowForCombat(skill.ThanThongData, inforVisualSkill.Item1,
                    inforVisualSkill.Item2, inforVisualSkill.Item3,
                    new LuyenDan.ItemRef
                        { Id = (ushort)skill.ThanThongData.Id, Type = LuyenDan.ItemType.ThanThong, Count = 1 },
                    EActionShowItemInfor.None, player);

                //skillTip.SetPosition(new Vector3(skill.transform.position.x, skillTip.transform.position.y, skillTip.transform.position.z));
                //skillTip.ShowSkillTip(skill.ThanThongData.nameItem,skill.ThanThongData is TranPhapTT tranPhapTT? tranPhapTT.GetDescTranPhap(player,daiCanhGioi) : skill.ThanThongData.GetDesc(daiCanhGioi), skill.ThanThongData.GetTextTypeSkill(), skill.ThanThongData.GetRequireActive(player), skill.ThanThongData, player.characterStatsTemp.level);
            });
            Action actionPointerUp = new Action(() =>
            {
                if (player.isAutoCastSkill) return;
                if (skill.isLock) return;

                btnCancel.onClick?.Invoke();
                SetStateControl(EStateControl.UseSkill);
                ShowNodes(skill.ThanThongData.GetRequireActive(player).Count);
                fightSkillSelect = skill;

                var listRequire = skill.ThanThongData.GetRequireActive(player);
                List<LinhKhiTieuTan> listSpecial = new List<LinhKhiTieuTan>();

                if (skill.ThanThongData is BaseSkill.TuongDong)
                {
                    listSpecial = skill.ThanThongData.GetLinhKhiTieuTanTuongDong(player);
                }
                else if (skill.ThanThongData is BaseSkill.TuongSinh)
                {
                    listSpecial = skill.ThanThongData.GetLinhKhiETuongQuan(player, ETuongQuan.TuongSinh);
                }
                else if (skill.ThanThongData is BaseSkill.TuongKhac)
                {
                    listSpecial = skill.ThanThongData.GetLinhKhiETuongQuan(player, ETuongQuan.TuongKhac);
                }

                if (listSpecial == null || listSpecial.Count <= 0 || listSpecial.Count < listRequire.Count)
                {
                    listSpecial = skill.ThanThongData.GetLinhKhiTieuTanIfPossible(player);
                }

                listRequire = listRequire
                    .OrderBy(x => x.linhKhiRequire == ELinhKhi.None)
                    .ToList();
                for (int i = 0; i < listRequire.Count; i++)
                {
                    ThanThong.RequireActive require = listRequire[i];
                    bool hasSpecificType = require.linhKhiRequire != ELinhKhi.None;

                    SlotLinhKhi match = null;
                    if (listSpecial != null && listSpecial.Count > 0)
                    {
                        var special = listSpecial[i];

                        // --- Tìm đối tượng phù hợp ---
                        match = linhKhiRemains.FirstOrDefault(l =>
                            l.eLinhKhi == special.eLinhKhi && l.count >= special.count);
                    }
                    else
                    {
                        // --- Tìm đối tượng phù hợp ---
                        match = hasSpecificType
                            ? linhKhiRemains.FirstOrDefault(l =>
                                l.eLinhKhi == require.linhKhiRequire && l.count >= require.count)
                            : linhKhiRemains.FirstOrDefault(l =>
                                l.count >= require.count &&
                                !currentNodes.Any(x => x.eLinhKhi == l.eLinhKhi && x.isLock == false));
                    }


                    // --- Nếu tìm thấy trong linhKhiRemains ---

                    if (match != null)
                    {
                        SlotLinhKhi node = null;
                        foreach (var item in currentNodes)
                        {
                            if (item.count <= 0)
                            {
                                node = item;
                                item.SetCountRequire(require.count, require.linhKhiRequire);
                                break;
                            }
                        }

                        AddLinhKhiFromRemainToNode(match, node, require.count);


                        if (hasSpecificType)
                        {
                            node?.SetLock(true);
                        }


                        continue;
                    }
                }
            });
            ThanThong thanThong = s < currentThanThong.Length ? currentThanThong[s] : null;
            var inforVisualSkill = Pool.Get<UIManager>().GetFrameAndQualityFullAndColorNameSkill(thanThong);

            skill.Init(thanThong, thanThong != null ? inforVisualSkill.Item1 : null);
            skill.InitAction(actionHolder, actionPointerUp);
            skill.SetFullQuality(inforVisualSkill.Item2);
        }

        skillTip.DoHide();
        if (player.characterStatsBase.vuKhi.Item != null)
        {
            equipmentWeapon.gameObject.SetActive(true);
            equipmentWeapon.btnShowInfor.gameObject.SetActive(false);
            var inforVisualEquipment = Pool.Get<UIManager>()
                .GetFrameAndQualityFullAndColorNameEquipment(player.characterStatsBase.vuKhi.Item.Tier);

            equipmentWeapon.Init(
                new BaseItemData
                {
                    name = player.characterStatsBase.vuKhi.nameEquip,
                    spriteItem = player.characterStatsBase.vuKhi.Item.Icon
                }, inforVisualEquipment.Item1);
            equipmentWeapon.InitAction(null, () =>
            {
                CombatManager.ExcuteWeapon();
                SetVisualCDEquipmentWeapon();
            });
            equipmentWeapon.SetFullQuality(inforVisualEquipment.Item2);
        }
        else
        {
            equipmentWeapon.gameObject.SetActive(false);
        }

        this.gameObject.SetActive(false);
    }

    private void SetVisualCDEquipmentWeapon()
    {
        equipmentWeapon.SetLock(player.isCooldownWeapon.Value);

        equipmentWeapon.SetCoolDown(player.isCooldownWeapon.Value, 1);
    }

    private void Awake()
    {
    }

    public void AddLinhKhiFromRemainToNode(SlotLinhKhi from, SlotLinhKhi to, int count = 1)
    {
        if (to.count <= 0)
        {
            to.eLinhKhi = from.eLinhKhi;
            to.icon.sprite = GetSpriteLinhKhi(to.eLinhKhi);
            to.icon.SetNativeSize();
        }

        to.AddCount(count);
        to.AddCountRequire(-count);
        from.AddCount(-count);
        textTip.text = $"{CurrentLinhKhi - maxLinhKhi}";
        CheckTuongQuan();
    }

    public void AddLinhKhiFromNodeToRemain(SlotLinhKhi from, SlotLinhKhi to, int count = 1)
    {
        to.AddCount(count);
        from.AddCountRequire(count);
        from.AddCount(-count, () =>
        {
            Debug.Log("tao chuyen none o day");
            from.eLinhKhi = ELinhKhi.None;
        });
        textTip.text = $"{CurrentLinhKhi - maxLinhKhi}";
        CheckTuongQuan();
    }

    public void CheckFightSkillCanUseObserver(params object[] data)
    {
        CharacterCombat caster = (CharacterCombat)data[0];
        if (caster == player)
        {
            CheckFightSkillCanUse();
        }
    }

    public void CheckFightSkillCanUse()
    {
        foreach (var item in fightSkills)
        {
            if (item.ThanThongData == null) continue;
            item.SetLock(false);

            if (!item.ThanThongData.IsCanUseLinhKhiRequire(player))
            {
                Debug.Log("CheckFightSkillCanUse  lock");
                item.SetLock(true);
            }
        }

        foreach (var item in fightSkills)
        {
            if (item.ThanThongData == null) continue;
            if (!item.ThanThongData.IsCanUseGlobal(player, CombatManager.targetTurn))
            {
                item.SetLock(true);
                continue;
            }

            if (player.thanThongCoolDown.Any(x => x.Key == item.ThanThongData && x.Value > 0))
            {
                var x = player.thanThongCoolDown
                    .First(t => t.Key == item.ThanThongData);

                item.SetLock(true);
                item.SetCoolDown(true, x.Value);
            }
            else
            {
                item.SetCoolDown(false, 0);
            }
        }

        foreach (var item in fightSkills)
        {
            item.ShowRecommentHighlight(false);
            if (item.ThanThongData == null || CombatManager.IsDelayGame) continue;
            if (item.isLock == false)
            {
                if (player.thanThongCurrentUse != null && item.ThanThongData is BaseSkill.LienKich)
                {
                    if (CombatManager.IsLienKich(item.ThanThongData, player.thanThongCurrentUse.thanThong))
                    {
                        item.ShowRecommentHighlight(true);
                    }
                }

                if (item.ThanThongData is BaseSkill.TuongDong)
                {
                    if (item.ThanThongData.GetLinhKhiTieuTanTuongDong(player) != null)
                    {
                        item.ShowRecommentHighlight(true);
                    }
                }

                if (item.ThanThongData is BaseSkill.TuongKhac)
                {
                    if (item.ThanThongData.GetLinhKhiETuongQuan(player, ETuongQuan.TuongKhac) != null)
                    {
                        item.ShowRecommentHighlight(true);
                    }
                }

                if (item.ThanThongData is BaseSkill.TuongSinh)
                {
                    if (item.ThanThongData.GetLinhKhiETuongQuan(player, ETuongQuan.TuongSinh) != null)
                    {
                        item.ShowRecommentHighlight(true);
                    }
                }
                //else if(item.ThanThongData is BaseSkill.TuongSinh)
                //{
                //    if (CombatManager.GetETuongQuan(GetLinhKhiTieuTans(linhKhiRemains)) == ETuongQuan.TuongSinh)
                //    {
                //        item.ShowRecommentHighlight(true);

                //    }
                //}
                //else if (item.ThanThongData is BaseSkill.TuongKhac)
                //{
                //    if (CombatManager.GetETuongQuan(GetLinhKhiTieuTans(linhKhiRemains)) == ETuongQuan.TuongKhac)
                //    {
                //        item.ShowRecommentHighlight(true);

                //    }
                //}
                //else if (item.ThanThongData is BaseSkill.TuongDong)
                //{
                //    if (CombatManager.IsTuongDong(GetLinhKhiTieuTans(linhKhiRemains)))
                //    {
                //        item.ShowRecommentHighlight(true);

                //    }
                //}
            }
        }
    }

    public Sprite GetSpriteLinhKhi(ELinhKhi eLinhKhi)
    {
        if (eLinhKhi == ELinhKhi.None) return null;
        return spriteUILinhKhis[(int)eLinhKhi - 1];
    }

    public void AdjustLinhKhi(params object[] data)
    {
        CharacterCombat player = (CharacterCombat)data[0];
        if (player != this.player) return;

        foreach (var item in linhKhiRemains)
        {
            switch (item.eLinhKhi)
            {
                case ELinhKhi.None:
                    break;
                case ELinhKhi.Kim:
                    item.SetCount(player.khiKim);
                    break;
                case ELinhKhi.Moc:
                    item.SetCount(player.khiMoc);

                    break;
                case ELinhKhi.Thuy:
                    item.SetCount(player.khiThuy);

                    break;
                case ELinhKhi.Hoa:
                    item.SetCount(player.khiHoa);

                    break;
                case ELinhKhi.Tho:
                    item.SetCount(player.khiTho);

                    break;
                case ELinhKhi.Ma:
                    item.SetCount(player.khiMa);

                    break;
                default:
                    break;
            }
        }

        CheckFightSkillCanUse();
    }

    public void NewTurn(params object[] data)
    {
        Debug.Log("NewTurn");
        CharacterCombat currentTurn = (CharacterCombat)data[0];
        if (currentTurn != player)
        {
            Debug.Log("NewTurn1");
            this.gameObject.SetActive(false);

            return;
        }

        this.gameObject.SetActive(true);
      
        if (currentTurn == null)
        {
            Debug.Log("NewTurn2");
            return;
        }

        SetStateControl(EStateControl.None);

        foreach (var item in linhKhiRemains)
        {
            switch (item.eLinhKhi)
            {
                case ELinhKhi.None:
                    break;
                case ELinhKhi.Kim:
                    item.SetCount(currentTurn.khiKim);
                    break;
                case ELinhKhi.Moc:
                    item.SetCount(currentTurn.khiMoc);

                    break;
                case ELinhKhi.Thuy:
                    item.SetCount(currentTurn.khiThuy);

                    break;
                case ELinhKhi.Hoa:
                    item.SetCount(currentTurn.khiHoa);

                    break;
                case ELinhKhi.Tho:
                    item.SetCount(currentTurn.khiTho);

                    break;
                case ELinhKhi.Ma:
                    item.SetCount(currentTurn.khiMa);

                    break;
                default:
                    break;
            }
        }

        CheckFightSkillCanUse();
        SetVisualCDEquipmentWeapon();
    }

    [Button]
    public void ShowNodes(int count, int countRequire = 99999)
    {
        foreach (var item in currentNodes)
        {
            item.count = 0;
            item.eLinhKhi = ELinhKhi.None;
        }

        currentNodes.Clear();
        if (count > nodes.Count) count = nodes.Count;

        // ON/OFF
        for (int i = 0; i < nodes.Count; i++)
        {
            nodes[i].gameObject.SetActive(i < count);
            if (nodes[i].gameObject.activeSelf)
            {
                currentNodes.Add(nodes[i]);
                nodes[i].SetLock(false);
                nodes[i].AddCount(0);
                nodes[i].SetCountRequire(countRequire);
            }
        }

        // 1 node = giữa
        if (count == 1)
        {
            nodes[0].GetComponent<RectTransform>().anchoredPosition = Vector2.zero;
            return;
        }

        // -------------------------
        // CASE 1: 2 → 3 nodes = NỬA TRÊN (180° → 0°)
        // -------------------------
        if (count <= 3)
        {
            float startAngle = 180f;
            float endAngle = 0f;
            float angleStep = (startAngle - endAngle) / (count - 1);

            for (int i = 0; i < count; i++)
            {
                float angle = startAngle - angleStep * i;
                float rad = angle * Mathf.Deg2Rad;

                float x = Mathf.Cos(rad) * radius;
                float y = Mathf.Sin(rad) * radius;

                nodes[i].GetComponent<RectTransform>().anchoredPosition = new Vector2(x, y);
            }

            return;
        }

        // -------------------------
        // CASE 2: 4 → 6 nodes = FULL 360° chia đều
        // -------------------------
        float fullStep = 360f / count;

        for (int i = 0; i < count; i++)
        {
            float angle = fullStep * i - 90f; // -90 để nút đầu ở trên
            float rad = angle * Mathf.Deg2Rad;

            float x = Mathf.Cos(rad) * radius;
            float y = Mathf.Sin(rad) * radius;

            nodes[i].GetComponent<RectTransform>().anchoredPosition = new Vector2(x, y);
        }
    }

    public void HapThuLinhKhiBtnAction(int linhKhi)
    {
        player.HapThuLinhKhi((ELinhKhi)linhKhi, 1);
        linhKhiSelectUI.SetActive(false);
    }
}