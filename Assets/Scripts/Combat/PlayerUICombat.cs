using CreateCharacter;
using DG.Tweening;
using LongDH.LongDHModules;
using NUnit.Framework.Interfaces;
using Sirenix.OdinInspector;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class PlayerUICombat : MonoBehaviour
{
    public enum EStateControl
    {
        None,
        UseSkill,
        DeleteLinhKhi,
    }
    public List<SlotLinhKhi> nodes;  // các nút cần sắp xếp
    public List<SlotLinhKhi> linhKhiRemains;  // các nút cần sắp xếp
    public List<SlotLinhKhi> currentNodes;  // các nút cần sắp xếp
    public Sprite[] spriteUILinhKhis;
    public Sprite[] spriteFrame;

    public float radius = 120f;         // bán kính vòng tròn
    public Button btnConfirm;
    public Button btnCancel;
    public Button btnEndRound;
    public int maxLinhKhi;
    public int CurrentLinhKhi => linhKhiRemains.Sum(item => item.count);

    public EStateControl eStateControl = EStateControl.None;

    public List<FightSkillItem> fightSkills;
    public FightSkillItem fightSkillSelect;
    public Image tuongSinhImg;
    public Image tuongKhacImg;
    private CharacterCombat player => CombatManager.char1;
    public Text textTip;
    public ThanThong[] thanthongtest;// test
    public CombatManager CombatManager => transform.GetComponentInParent<CombatManager>();

    public SkillTip skillTip;
    public void SetStateControl(EStateControl eState)
    {
        eStateControl = eState;
        if(eState == EStateControl.None)
        {
            btnEndRound.transform.parent.gameObject.SetActive(true);
            btnConfirm.transform.parent.gameObject.SetActive(false);
            ShowNodes(0);
            if(fightSkillSelect != null)
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
   
    private void Awake()
    {
        btnConfirm.onClick.AddListener(() =>
        {
            if (CombatManager.IsDelayEffectSkill) return;
            switch (eStateControl)
            {
                case EStateControl.None:

                    break;
                case EStateControl.UseSkill:
                    if (currentNodes.Any(x => x.countRequire > 0)) return;
                    Observer.Notify(EventList.GamePlay.PLAY_SKILL,player,CombatManager.char2, fightSkillSelect.ThanThongData, GetLinhKhiTieuTans(currentNodes));
                    SetStateControl(EStateControl.None);
                    break;
                case EStateControl.DeleteLinhKhi:
                    if (CurrentLinhKhi <= maxLinhKhi)
                    {

                        Observer.Notify(EventList.GamePlay.END_TURN_COMBAT,player,CombatManager.char2, GetLinhKhiTieuTans(currentNodes));
                        SetStateControl(EStateControl.None);

                    }

                    break;
                default:
                    break;
            }

        });
        btnCancel.onClick.AddListener(() =>
        {
            if (CombatManager.IsDelayEffectSkill) return;
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
        });
        btnEndRound.onClick.AddListener(() =>
        {
            if (CombatManager.IsDelayEffectSkill) return;
            SetStateControl(EStateControl.DeleteLinhKhi);
            if (CurrentLinhKhi > maxLinhKhi)
            {
                ShowNodes(6);
                textTip.transform.parent.parent.localPosition = new Vector3(370, 0, 0);
                textTip.transform.parent.parent.DOLocalMove(new Vector3(0, 0, 0), 0.5f);
                textTip.text = $"{CurrentLinhKhi - maxLinhKhi}";

            }
            else
            {
                btnConfirm.onClick.Invoke();
            }
          


        });
        Observer.AddObserver(EventList.GamePlay.NEW_TURN_COMBAT, NewTurn);
        Observer.AddObserver(EventList.GamePlay.ADJUST_LINHKHI, AdjustLinhKhi);

        foreach (var item1 in linhKhiRemains)
        {
            item1.Init(() =>
            {
                Debug.Log(currentNodes.Count);
                if (item1.count <= 0 || item1.isLock || (currentNodes.Any(x => x.eLinhKhi == item1.eLinhKhi && x.isLock == false && x.countRequire<=0) && eStateControl == EStateControl.UseSkill )) return;

                if (CurrentLinhKhi <= maxLinhKhi && eStateControl == EStateControl.DeleteLinhKhi) return;
                foreach (var item2 in currentNodes)
                {
                    if (item2.isLock || item2.countRequire <=0) continue;

                    if (!item2.isLock && (item2.count <= 0 || item2.eLinhKhi == item1.eLinhKhi) && item1.count >= item2.countRequire)
                    {
                        AddLinhKhiFromRemainToNode(item1,item2,item2.countRequire);
                        break;
                    }
                }
             

            });
        }
        foreach (var item1 in nodes)
        {
            item1.Init(() =>
            {
                if (item1.count <= 0 || item1.isLock ) return;

                foreach (var item2 in linhKhiRemains)
                {
                    if ((item2.eLinhKhi == item1.eLinhKhi))
                    {
                        AddLinhKhiFromNodeToRemain(item1,item2 , item1.count);
                    }
                }
              

            });
        }
        for (var s = 0; s < fightSkills.Count; s++)
        {
            var skill = fightSkills[s];
            Action actionHolder = new Action(() =>
            {
                skillTip.SetPosition(new Vector3(skill.transform.position.x,skillTip.transform.position.y,skillTip.transform.position.z));
                skillTip.ShowSkillTip(skill.ThanThongData.name,skill.ThanThongData.GetDesc(ThanThong.GetIndexLevelThanThong(player.characterStatsCurrent)),skill.ThanThongData.GetTextTypeSkill(),skill.ThanThongData.requireActives,skill.ThanThongData);
            });
            Action actionPointerUp = new Action(() =>
            {
                btnCancel.onClick?.Invoke();
                SetStateControl(EStateControl.UseSkill);
                ShowNodes(skill.ThanThongData.requireActives.Count);
                fightSkillSelect = skill;

                var listRequire = skill.ThanThongData.requireActives;
                listRequire = listRequire
                .OrderBy(x => x.linhKhiRequire == ELinhKhi.None)
                .ToList();

                foreach (var require in listRequire)
                {
                    bool hasSpecificType = require.linhKhiRequire != ELinhKhi.None;

                    // --- Tìm đối tượng phù hợp ---
                    var match = hasSpecificType
                        ? linhKhiRemains.FirstOrDefault(l => l.eLinhKhi == require.linhKhiRequire && l.count >= require.count)
                        : linhKhiRemains.FirstOrDefault(l => l.count >= require.count && !currentNodes.Any(x => x.eLinhKhi == l.eLinhKhi && x.isLock == false));

                    // --- Nếu tìm thấy trong linhKhiRemains ---

                    if (match != null)
                    {
                        SlotLinhKhi node = null;
                        foreach (var item in currentNodes)
                        {
                            if(item.count<=0)
                            {
                                node = item;
                                item.SetCountRequire(require.count,require.linhKhiRequire);
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
            ThanThong thanThong = s < thanthongtest.Length ? thanthongtest[s] : null;
            skill.Init(thanThong, thanThong != null ? spriteFrame[(int)(((int)thanThong.eTierSkill - 1) / 3)] : null, actionHolder, actionPointerUp); // test data 
        } 
            
       

      
    }
    public void AddLinhKhiFromRemainToNode(SlotLinhKhi from , SlotLinhKhi to, int count = 1)
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
    public void AddLinhKhiFromNodeToRemain(SlotLinhKhi from, SlotLinhKhi to , int count = 1)
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
    public void CheckFightSkillCanUse()
    {
        foreach (var item in fightSkills)
        {
            if (item.ThanThongData == null) continue;
            item.SetLock(false);

            if(!item.ThanThongData.IsCanUseLinhKhiRequire(player))
            {
                Debug.Log("CheckFightSkillCanUse  lock");
                item.SetLock(true);
            }    

           
        }
        foreach (var item in fightSkills)
        {
            if (item.ThanThongData == null) continue;
            if (!item.ThanThongData.IsCanUse(player,null))
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
                item.SetCoolDown(false,0);

            }


        }
        foreach (var item in fightSkills)
        {
            if (item.ThanThongData == null || player.thanThongOldUse == null) continue;
            item.ShowRecommentHighlight(false);
            if (item.isLock == false)
            {
                if(item.ThanThongData is BaseSkill.LienKich)
                {
                    if(CombatManager.IsLienKich(item.ThanThongData,player.thanThongOldUse.thanThong))
                    {
                        item.ShowRecommentHighlight(true);

                    }
                }    
                else if(item.ThanThongData is BaseSkill.TuongSinh)
                {
                    if (CombatManager.GetETuongQuan(GetLinhKhiTieuTans(linhKhiRemains)) == ETuongQuan.TuongSinh)
                    {
                        item.ShowRecommentHighlight(true);

                    }
                }
                else if (item.ThanThongData is BaseSkill.TuongKhac)
                {
                    if (CombatManager.GetETuongQuan(GetLinhKhiTieuTans(linhKhiRemains)) == ETuongQuan.TuongKhac)
                    {
                        item.ShowRecommentHighlight(true);

                    }
                }
                else if (item.ThanThongData is BaseSkill.TuongDong)
                {
                    if (CombatManager.IsTuongDong(GetLinhKhiTieuTans(linhKhiRemains)))
                    {
                        item.ShowRecommentHighlight(true);

                    }
                }
            }

        }
    }    
    public Sprite GetSpriteLinhKhi(ELinhKhi eLinhKhi)
    {
        if(eLinhKhi == ELinhKhi.None) return null;
        return spriteUILinhKhis[(int)eLinhKhi -1];
    }
    public void AdjustLinhKhi(params object[] data)
    {
        CharacterCombat player = (CharacterCombat)data[0];
        if(player != this.player ) return;
      
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

    }
    public void NewTurn(params object[] data)
    {
        if (!(bool)data[2])
        {
            this.gameObject.SetActive(false);
            return;
        }
        this.gameObject.SetActive(true);

        CharacterCombat player = (CharacterCombat)data[0];
        
        if (player == null) return;
        SetStateControl(EStateControl.None);
        maxLinhKhi = player.totalKhi;
        
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
            float angle = fullStep * i - 90f;  // -90 để nút đầu ở trên
            float rad = angle * Mathf.Deg2Rad;

            float x = Mathf.Cos(rad) * radius;
            float y = Mathf.Sin(rad) * radius;

            nodes[i].GetComponent<RectTransform>().anchoredPosition = new Vector2(x, y);
        }
    }
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
