using LongDH.LongDHModules;
using Sirenix.OdinInspector;
using System.Collections.Generic;
using UnityEngine;

public class CombatManager : MonoBehaviour
{
    [SerializeField] private CharacterCombat char1;
    [SerializeField] private CharacterCombat char2;

    public CharacterCombat currentTurn;

    private void Awake()
    {
        char1.Init(new CharacterStats()
        {
            percentKim = 20,
            percentMoc = 20,
            percentThuy = 20,
            percentHoa = 20,
            percentTho = 20,

        });
        char2.Init(new CharacterStats());
        Observer.AddObserver(EventList.GamePlay.END_TURN_COMBAT, EndTurn);

    }
    [Button]
    public void EndTurn(params object[] data)
    {
        if (data[0] is List<SlotLinhKhi> linhKhiTieuTan)
        {
            foreach (var item in linhKhiTieuTan)
            {
                Debug.Log(item.eLinhKhi + "cmmm" + item.count);
                currentTurn.TieuTanLinhKhi(item.eLinhKhi, item.count);
            }
        }    
        if(currentTurn == char1)
        {
            currentTurn = char2;
        } 
        else
        {
            currentTurn = char1;
        }
        NewTurn();
    }    
    [Button]
    public void NewTurn()
    {
        currentTurn.HapThuLinhKhi();
        // param 0 là bool isPlayer == char1 
        Observer.Notify(EventList.GamePlay.NEW_TURN_COMBAT, currentTurn == char1? true: false,currentTurn);


    }


    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
