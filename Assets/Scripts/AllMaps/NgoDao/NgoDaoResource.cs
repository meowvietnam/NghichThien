using Sirenix.OdinInspector;
using System.Collections.Generic;
using AllMaps.NgoDao;
using UnityEngine;

[CreateAssetMenu(fileName = nameof(NgoDaoResource), menuName = "Ngộ Đạo/" + nameof(NgoDaoResource))]
public class NgoDaoResource : BaseData
{
    [ShowInInspector]
    public List<NgoDaoList> ngoDao = new List<NgoDaoList>(5);
    [System.Serializable]
    public class NgoDaoList
    {
        public List<NgoDao> list;
    }
    [SerializeField] public BaseSkill.ETypeSkill eTypeSkill;


    protected void OnValidate()
    {
        id = (int)eTypeSkill;
    }
    public static string[] CamNgoText = new string[]
    {
        "Sơ khuy môn kính",
        "Lược hữu tiểu thành",
        "Dung hội quán thông",
        "Đạo chi thật cảnh",
        "Đại đạo đã thành",

    };
    public static int[] CamNgoRequire = new int[]
    {
        1000,
        8000,
        30000,
        70000,
        150000,

    };
    public int NgoDaoAtLevel(NgoDao ngoDaoSO)
    {
        for (int i = 0; i < ngoDao.Count; i++)
        {
            foreach (var item in ngoDao[i].list)
            {
                if(item == ngoDaoSO)
                {
                    return i+1;
                }
            }
        }
        return 0;
            
    }   
    public static int GetLevelCamNgo(int value)
    {
        int levelIndex = -1;
        for (int i = 0; i < CamNgoRequire.Length; i++)
        {
            if ( value >= CamNgoRequire[i])
            {
                levelIndex = i;
            }
            else
            {
                // Vì mảng đã sắp xếp tăng dần, nếu gặp mốc lớn hơn currentValue 
                // thì không cần kiểm tra các mốc sau nữa.
                break;
            }
        }
        return levelIndex + 1;
    }
}
