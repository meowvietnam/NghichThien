using DG.Tweening;
using LongDH.LongDHModules;
using System.Linq;
using UnityEngine;
public class BuffEffect : MonoBehaviour
{
    [SerializeField] public Animator anim;
    [SerializeField] public EBuffAndDeBuff eBuff;

    protected virtual void Awake()
    {
        Observer.AddObserver(EventList.Combat.ADD_BUFF_UI, CharacterAddBuff);

    }
    private void CharacterAddBuff(params object[] data)
    {
        CharacterCombat currentTurn = (CharacterCombat)data[0];
        EBuffAndDeBuff eBuffParam = (EBuffAndDeBuff)data[1];
        int newValue = (int)data[2];
        int oldValue = (int)data[3];

        if (currentTurn == null || eBuffParam != eBuff) return;
        
        BuffEffect[] allEffects = currentTurn.GetComponentsInChildren<BuffEffect>(true);
        Transform child = null;
        foreach (var effect in allEffects)
        {
            // Kiểm tra xem BuffEffect này có đúng loại (eBuffParam) không
            if (effect.eBuff == eBuffParam)
            {
                child = effect.transform.GetChild(0);
                break;
            }
        }

        if (child == null) return;
       
        if(newValue <= 0)
        {
            child.gameObject.SetActive(false);
        }
        else
        {
            if(!child.gameObject.activeSelf || (eBuffParam != EBuffAndDeBuff.KiemKhi && eBuffParam != EBuffAndDeBuff.HoTrao && eBuffParam != EBuffAndDeBuff.SucThe))
            {
                child.gameObject.SetActive(false);
                child.gameObject.SetActive(true);
            }    
           
        }    

    }
    public void ShowEffect()
    {
        var child = transform.GetChild(0);
        child.gameObject.SetActive(false);
        child.gameObject.SetActive(true);
    }


}
