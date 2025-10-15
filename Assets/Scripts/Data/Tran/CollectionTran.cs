using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

[System.Serializable]
public class CollectionTran : CmnSaveProc.SaveClass
{
    public List<int> Archives = new List<int>();
    public List<int> Readed = new List<int>();
    public List<int> EndingIds = new List<int>();

    public bool IsCompleteEnding(){
        return StoryListMast.List.Count(it => it.Category == StoryListMast.CATEGORY.ENDING) == EndingIds.Count();
    }

    /// <summary>
    /// アーカイブ取得
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    public bool addArchive(int id) {
        return UtilToolLib.addId(id, ref Archives);
    }

    /// <summary>
    /// アーカイブ閲覧済み
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    public bool addReaded(int id) {
        return UtilToolLib.addId(id, ref Readed);
    }

    public bool addEnging(int id) {
        return UtilToolLib.addId(id, ref EndingIds);
    }
}
