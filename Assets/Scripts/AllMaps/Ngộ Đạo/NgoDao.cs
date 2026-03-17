using UnityEngine;
[CreateAssetMenu(fileName = nameof(NgoDao), menuName = "Ngộ Đạo/" + nameof(NgoDao))]
public abstract class NgoDao : BaseData
{
    [SerializeField] public int require;
    [SerializeField] public BaseSkill.ETypeSkill[] eTypeRequire;


    public abstract string GetDes();
   
}
