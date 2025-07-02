using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkillLeanMast : MasterCmn
{
    public enum TYPE
    {
        UNIT,
        SOUL,
        ITEM,
        ALL
    }
    public int Id;
    public TYPE Type;
    public string UserTag;
    public string SkillTag;
    public int LeanLevel;

    public static IReadOnlyList<SkillLeanMast> List;

    public static void load() {
        List = load<SkillLeanMast>();
    }

}
