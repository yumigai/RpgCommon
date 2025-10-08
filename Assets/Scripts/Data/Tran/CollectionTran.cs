using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

[System.Serializable]
public class CollectionTran 
{
    public List<int> Archives = new List<int>();
    public List<int> Readed = new List<int>();
    public List<int> EndingIds = new List<int>();

    public bool IsCompleteEnding(){
        return StoryListMast.List.Count(it => it.Category == StoryListMast.CATEGORY.ENDING) == EndingIds.Count();
    }
}
