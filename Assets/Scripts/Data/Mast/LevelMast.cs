using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelMast : MasterCmn {

    //最大レベル
    public const int MAX_LV = 99;

    public int Lv;
    public int Exp;
    public int GetExp;
    public int GetMoney;
    public int[] Potential;

    public static LevelMast[] List;

    public static void load() {
        List = load<LevelMast>();
    }

    /// <summary>
    /// 次のLvUPに必要なEXP
    /// </summary>
    /// <param name="lv"></param>
    /// <returns></returns>
    public static int GetNextExp(int lv) {
        if (lv < LevelMast.List.Length) {
            return LevelMast.List[lv].Exp;
        }
        return 0;
    }

    /// <summary>
    /// レベル・レアリティ（才能）に相当するパラメータ取得
    /// </summary>
    /// <param name="lv"></param>
    /// <param name="rare"></param>
    /// <returns></returns>
    public static int GetParam(int lv, int rare) {

        lv = Mathf.Clamp(lv, 0, List.Length);
        rare = Mathf.Clamp(rare, 0, List[lv].Potential.Length );
       
       return List[lv].Potential[rare];

    }
}

