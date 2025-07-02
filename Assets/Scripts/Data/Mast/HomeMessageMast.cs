using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HomeMessageMast : MasterCmn
{
    public enum MESSAGE_SITUATION {
        NORMAL,
        QUEST_WIN,
        QUEST_LOSE,
        HEAL_EVENT,
        FIRST_MESSAGE,
        HP_LOW,
        GAME_CLEAR,
    }

    /// <summary>
    /// 表示条件
    /// </summary>
    public enum TERMS
    {
        ANY,
        HIGH_LIKE, //好感度・高
        MAX_LIKE, //好感度・最大
    }

    public int Id;
    public string Tag;
    public string Back;

    public string[] Characters;
    public string[] Faces;
    public string[] Icons;
    public string[] Names;
    public string[] Kanas;
    public string[] Texts;

    public MESSAGE_SITUATION Situation;

    //public TERMS Terms;

    public uint MainCharaId;

    public int NeedPoint;

    public int[] ChapterIds = new int[0];

    public static HomeMessageMast[] List;

    public static void load()
    {
        List = load<HomeMessageMast>();
    }

}
