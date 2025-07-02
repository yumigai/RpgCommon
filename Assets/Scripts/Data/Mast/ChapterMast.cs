using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChapterMast : MulitiUseListMast
{
    public int NeedClearCount;
    public int SceneProgress; //全体のゲームを通した進行度（時系列的な）
    public string BattleBgm;
    public string BossBgm;

    public static ChapterMast[] List;

    public static void load() {
        List = load<ChapterMast>();
    }
}
