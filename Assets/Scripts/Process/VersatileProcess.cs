using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public static class VersatileProcess
{
    public static void nextDay() {

        if (GameConst.IS_TIME_LIMIT_SYSTEM) {
            SaveMng.Status.RestTimeLimit--;
            SaveMng.Status.save();
        }

        SaveMng.Quest = new QuestTran();
        SaveMng.Quest.save();
        SceneManagerWrap.LoadScene(CmnConst.SCENE.HomeScene);

    }

    public static T FindById<T>(this IReadOnlyList<T> list, int id) where T : MulitiUseListMast {
        return list.Where(it => it.Id == id).FirstOrDefault();
    }

    public static T FindByTag<T>(this IReadOnlyList<T> list, string tag) where T : MulitiUseListMast {
        return list.Where(it => it.Tag == tag).FirstOrDefault();
    }

    public static T FindById<T>(this List<T> list, int id) where T : MulitiUseListMast {
        return list.Where(it => it.Id == id).FirstOrDefault();
    }

    public static T FindByTag<T>(this List<T> list, string tag) where T : MulitiUseListMast {
        return list.Where(it => it.Tag == tag).FirstOrDefault();
    }
}
