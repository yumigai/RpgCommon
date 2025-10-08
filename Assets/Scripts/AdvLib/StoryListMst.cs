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
        NORMAL,
        EVENT,
        OPENING,
        ENDING,
    }

    public KIND Kind;

    public CATEGORY Category;

    public string UnitTag;

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
        SaveMng.Status.addArchive(list.Id);
        SaveMng.Status.addReaded(list.Id);
        SaveMng.Status.save();

        return true;
    }

}
