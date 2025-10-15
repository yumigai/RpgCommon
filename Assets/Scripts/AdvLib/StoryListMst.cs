using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StoryListMast : MulitiUseListMast {

    public enum KIND
    {
        BOOK,
        STORY,
    }

    public enum CATEGORY{
        NORMAL, // drop ari
        EVENT, // tokusyu. drop shinai
        OPENING,
        ENDING,
    }

    public KIND Kind;

    public CATEGORY Category;

    public static StoryListMast[] List;

	public static void load(){
		List = load<StoryListMast>();
	}

    public static bool StoryOrder(string tag) {

        StoryListMast list = System.Array.Find(List, it => it.Tag == tag);
        if (list == null) {
            return false;
        }
        BaseStorySceneMng.StoryNameOrder = list.Tag;
        SaveMng.Collection.addArchive(list.Id);
        SaveMng.Collection.addReaded(list.Id);
        SaveMng.Collection.save();

        return true;
    }

}
