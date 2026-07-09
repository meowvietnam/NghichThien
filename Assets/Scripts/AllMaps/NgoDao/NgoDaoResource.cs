using Sirenix.OdinInspector;
using System.Collections.Generic;
using System.Linq;
using AllMaps.NgoDao;
using LongDHModules;
using Network;
using Network.Extensions;
using UnityEngine;

[CreateAssetMenu(fileName = nameof(NgoDaoResource), menuName = "Ngộ Đạo/" + nameof(NgoDaoResource))]
public class NgoDaoResource : BaseData, IMemorySerializable<NgoDaoResource>
{
    [ShowInInspector] public List<NgoDaoList> ngoDao = new(5);

    [System.Serializable]
    public class NgoDaoList
    {
        public List<NgoDao> list;
    }

    [SerializeField] public BaseSkill.ETypeSkill eTypeSkill;

    public override void SetId()
    {
        Id = (int)eTypeSkill;
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
                if (item == ngoDaoSO)
                {
                    return i + 1;
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
            if (value >= CamNgoRequire[i])
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

    public override int ByteSize()
    {
        return base.ByteSize() + ngoDao.Count.ByteSize()
                               + ngoDao.Sum(x => x.list.Select(y => y.Id).ByteSize())
                               + eTypeSkill.ByteSize();
    }
    public void Deserialize(IReader reader, ref NgoDaoResource value)
    {
        BaseData baseValue = value;
        baseValue.Deserialize(reader, ref baseValue);

        var manager = Pool.Get<GameManager>();
        var count = reader.Read<int>();
        value.ngoDao = new List<NgoDaoList>(count);
        for (var i = 0; i < count; i++)
        {
            var ids = reader.ReadArray<int>();
            var list = ids
                .Select(id => manager == null ? null : manager.GetNgoDaoWithId(id))
                .Where(x => x != null)
                .ToList();
            value.ngoDao.Add(new NgoDaoList { list = list });
        }

        value.eTypeSkill = reader.Read<BaseSkill.ETypeSkill>();
    }
    public override void Serialize(IWriter writer)
    {
        base.Serialize(writer);
        writer.Write(ngoDao.Count);
        foreach (var nd in ngoDao)
        {
            var ngoDaoList = nd.list;
            writer.WriteArray(ngoDaoList.Select(x => x.Id));
        }
        writer.Write(eTypeSkill);
    }
}
