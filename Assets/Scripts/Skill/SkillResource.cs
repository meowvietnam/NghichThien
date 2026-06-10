using Sirenix.OdinInspector;
using Skill;
using System.Linq;
using UnityEditor;
using UnityEngine;
[CreateAssetMenu(menuName = "Skill/SkillResource")]

public class SkillResource : ScriptableObject
{
    private static SkillResource instance;

    public static SkillResource Instance
    {
        get
        {
            if (instance == null)
            {
                instance = Resources.Load<SkillResource>("Skill/SkillResource");
            }

            return instance;
        }
    }
    public ThanThong[] thanThongs;
    public CongPhap[] congPhaps;
   
}
