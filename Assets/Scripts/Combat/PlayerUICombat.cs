using LongDH.LongDHModules;
using Sirenix.OdinInspector;
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
    public float radius = 120f;         // bán kính vòng tròn
    public Button btnConfirm;
    public Button btnCancel;
    public Button btnEndRound;
    public int maxLinhKhi;
    public int CurrentLinhKhi => linhKhiRemains.Sum(item => item.count);

    public EStateControl eStateControl = EStateControl.None;

    public void SetStateControl(EStateControl eState)
    {
        eStateControl = eState;
        if(eState == EStateControl.None)
        {
            btnEndRound.transform.parent.gameObject.SetActive(true);
            btnConfirm.transform.parent.gameObject.SetActive(false);
            ShowNodes(0);

        }
        else
        {
            btnEndRound.transform.parent.gameObject.SetActive(false);
            btnConfirm.transform.parent.gameObject.SetActive(true);
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

                    SetStateControl(EStateControl.None);
                    break;
                case EStateControl.DeleteLinhKhi:
                    if (CurrentLinhKhi <= maxLinhKhi)
                    {
                        Observer.Notify(EventList.GamePlay.END_TURN_COMBAT, currentNodes);
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
                for (int i = 0; i < item.count; i++)
                {
                    item.actionPointerDown?.Invoke();
                }
            }
            SetStateControl(EStateControl.None);
        });
        btnEndRound.onClick.AddListener(() =>
        {
            SetStateControl(EStateControl.DeleteLinhKhi);
            if (CurrentLinhKhi >= maxLinhKhi)
            {
                ShowNodes(6);
            }
            else
            {
                btnConfirm.onClick.Invoke();
            }


        });
        Observer.AddObserver(EventList.GamePlay.NEW_TURN_COMBAT, NewTurn);
        foreach (var item1 in linhKhiRemains)
        {
            item1.Init(() =>
            {
                Debug.Log(currentNodes.Count);
                foreach (var item2 in currentNodes)
                {
                    if(!item2.isLock && (item2.count <= 0 || item2.eLinhKhi == item1.eLinhKhi))
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
            });
        }
        foreach (var item1 in nodes)
        {
            item1.Init(() =>
            {
                
                foreach (var item2 in linhKhiRemains)
                {
                    if (!item2.isLock && (item2.eLinhKhi == item1.eLinhKhi))
                    {
                        item2.AddCount(1);
                        item1.AddCount(-1);
                        break;
                    }
                }
            });
        }
    }
    public Sprite GetSpriteLinhKhi(ELinhKhi eLinhKhi)
    {
        if(eLinhKhi == ELinhKhi.None) return null;
        return spriteUILinhKhis[(int)eLinhKhi -1];
    }
    public void NewTurn(params object[] data)
    {
        if (!(bool)data[0]) return;
        CharacterCombat player = (CharacterCombat)data[1];
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

    }
    [Button]
    public void ShowNodes(int count)
    {
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
