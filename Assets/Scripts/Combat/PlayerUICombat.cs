using DG.Tweening;
using LongDH.LongDHModules;
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
    private CharacterCombat player = null;
    public Text textTip;
    public ThanThong[] thanthongtest;// test
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
            switch (eStateControl)
            {
                case EStateControl.None:

                    break;
                case EStateControl.UseSkill:
                    Observer.Notify(EventList.GamePlay.PLAY_SKILL, fightSkillSelect.ThanThongData, GetLinhKhiTieuTans(currentNodes));
                    SetStateControl(EStateControl.None);
                    break;
                case EStateControl.DeleteLinhKhi:
                    if (CurrentLinhKhi <= maxLinhKhi)
                    {

                        Observer.Notify(EventList.GamePlay.END_TURN_COMBAT, GetLinhKhiTieuTans(currentNodes));
                        SetStateControl(EStateControl.None);

                    }

                    break;
                default:
                    break;
            }

        });
        btnCancel.onClick.AddListener(() =>
        {
            foreach (var item in currentNodes)
            {
                item.SetLock(false);
                int count = item.count;
                for (int i = 0; i < count; i++)
                {
                    item.actionPointerDown?.Invoke();
                }
            }
            SetStateControl(EStateControl.None);
        });
        btnEndRound.onClick.AddListener(() =>
        {
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
        Observer.AddObserver(EventList.GamePlay.HAPTHU_LINHKHI, HapThuLinhKhi);

        foreach (var item1 in linhKhiRemains)
        {
            item1.Init(() =>
            {
                Debug.Log(currentNodes.Count);
                if (item1.count <= 0 || item1.isLock) return;

                if (CurrentLinhKhi <= maxLinhKhi && eStateControl == EStateControl.DeleteLinhKhi) return;
                foreach (var item2 in currentNodes)
                {
                    if (item2.isLock) continue;

                    if (!item2.isLock && (item2.count <= 0 || item2.eLinhKhi == item1.eLinhKhi))
                    {
                        if (item2.count <= 0)
                        {
                            item2.eLinhKhi = item1.eLinhKhi;
                            item2.icon.sprite = GetSpriteLinhKhi(item2.eLinhKhi);
                            item1.icon.SetNativeSize();
                        }
                        item2.AddCount(1);
                        
                        item1.AddCount(-1);
                        break;
                    }
                }
                textTip.text = $"{CurrentLinhKhi - maxLinhKhi}";
                CheckTuongQuan();

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
                        item2.AddCount(1);
                        item1.AddCount(-1);
                    }
                }
                CheckTuongQuan();
                textTip.text = $"{CurrentLinhKhi - maxLinhKhi}";

            });
        }
        for (var s = 0; s < fightSkills.Count; s++)
        {
            var skill = fightSkills[s];
            Action actionHolder = new Action(() =>
            {

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
                        : linhKhiRemains.FirstOrDefault(l => l.count >= require.count);

                    // --- Nếu tìm thấy trong linhKhiRemains ---

                    if (match != null)
                    {
                       
                        for (int i = 0; i < require.count; i++)
                            match.actionPointerDown?.Invoke();
                        
                        if(hasSpecificType)
                        {
                            var node = currentNodes.FirstOrDefault(n => n.eLinhKhi == require.linhKhiRequire && !n.isLock);
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
    public void CheckFightSkillCanUse()
    {
        foreach (var item in fightSkills)
        {
            if (item.ThanThongData == null) continue;
            item.SetLock(false);

            var listRequire = item.ThanThongData.requireActives;
            listRequire = listRequire
            .OrderBy(x => x.linhKhiRequire == ELinhKhi.None)
            .ToList();

            foreach (var require in listRequire)
            {
                bool hasSpecificType = require.linhKhiRequire != ELinhKhi.None;

                // --- Tìm đối tượng phù hợp ---
                var match = hasSpecificType
                    ? linhKhiRemains.FirstOrDefault(l => l.eLinhKhi == require.linhKhiRequire && l.count >= require.count)
                    : linhKhiRemains.FirstOrDefault(l => l.count >= require.count );

                // --- Nếu tìm thấy trong linhKhiRemains ---

                if (match == null)
                {
                    Debug.Log("CheckFightSkillCanUse  lock"  );
                    item.SetLock(true);
                    break;

                }
               


            }
        }
        foreach (var item in fightSkills)
        {
            if (item.ThanThongData == null) continue;

            if (player.thanThongCoolDown.Any(x => x.Key == item.ThanThongData))
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
            if (item.ThanThongData == null) continue;

            if (item.isLock == false)
            {
                if(item.ThanThongData is BaseSkill.LienKich)
                {
                    if(CombatManager.IsLienKich(item.ThanThongData,player.thanThongOldUse))
                    {
                        item.ShowRecommentHighlight(true);
                        break;

                    }
                }    
                else if(item.ThanThongData is BaseSkill.TuongSinh)
                {
                    if (CombatManager.GetETuongQuan(GetLinhKhiTieuTans(linhKhiRemains)) == ETuongQuan.TuongSinh)
                    {
                        item.ShowRecommentHighlight(true);
                        break;

                    }
                }
                else if (item.ThanThongData is BaseSkill.TuongKhac)
                {
                    if (CombatManager.GetETuongQuan(GetLinhKhiTieuTans(linhKhiRemains)) == ETuongQuan.TuongKhac)
                    {
                        item.ShowRecommentHighlight(true);
                        break;

                    }
                }
                else if (item.ThanThongData is BaseSkill.TuongDong)
                {
                    if (CombatManager.IsTuongDong(GetLinhKhiTieuTans(linhKhiRemains)))
                    {
                        item.ShowRecommentHighlight(true);
                        break;

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
    public void HapThuLinhKhi(params object[] data)
    {
        CharacterCombat player = (CharacterCombat)data[0];
        if(player != this.player ) return;
        ELinhKhi linhKhi = (ELinhKhi)data[1];
        int count = (int)data[2];
        foreach (var item in linhKhiRemains)
        {
           if(item.eLinhKhi == linhKhi)
           {
                item.AddCount(count);
                break;
           }
        }

    }
    public void NewTurn(params object[] data)
    {
        if (!(bool)data[0]) return;
        CharacterCombat player = (CharacterCombat)data[1];
        
        if (player == null) return;
        this.player = player;
        SetStateControl(EStateControl.None);
        maxLinhKhi = player.totalKhi;
        foreach (var item in linhKhiRemains)
        {
            switch (item.eLinhKhi)
            {
                case ELinhKhi.None:
                    break;
                case ELinhKhi.Kim:
                    item.AddCount(player.khiKim);
                    break;
                case ELinhKhi.Moc:
                    item.AddCount(player.khiMoc);

                    break;
                case ELinhKhi.Thuy:
                    item.AddCount(player.khiThuy);

                    break;
                case ELinhKhi.Hoa:
                    item.AddCount(player.khiHoa);

                    break;
                case ELinhKhi.Tho:
                    item.AddCount(player.khiTho);

                    break;
                case ELinhKhi.Ma:
                    item.AddCount(player.khiMa);

                    break;
                default:
                    break;
            }
        }
        CheckFightSkillCanUse();

    }
    [Button]
    public void ShowNodes(int count)
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
