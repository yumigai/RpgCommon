using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class StoryListMng : MultiUseScrollMng {

	public const string NEXT_SECENE = "StoryScene";

    public void makeList(StoryListMast.KIND kind, string button_txt, bool show_unknown = true )
    {
        for (int i = 0; i < StoryListMast.List.Length; i++)
        {
            if (StoryListMast.List[i].Kind == kind)
            {
                int id = StoryListMast.List[i].Id;
                if (SaveMng.Status.Archives.IndexOf(id) >= 0)
                {
                    MultiUseListMng listitem = makeListItemMasterId(i, StoryListMast.List[i], button_txt);

                    if (SaveMng.Status.Readed.IndexOf(id) >= 0)
                    {
                        // Newアイコン
                        listitem.TagLabel.gameObject.SetActive(false);
                    }
                }
                else if(show_unknown)
                {
                    MultiUseListMng listitem = makeListItemMasterId(i, StoryListMast.List[i], "未発見", MultiUseListMng.BUTTON.LOCK);
                    listitem.Name.text = "未発見";
                    if (listitem.Detail != null) {
                        listitem.Detail.text = "";
                    }
                    listitem.TagLabel.gameObject.SetActive(false);
                }
            }
        }

        ListItem.SetActive(false);
    }

	override public void pushList( MultiUseListMng mng ){

        if (SaveMng.Status.Archives.IndexOf( mng.Id) < 0)
        {
            return;
        }

        CommonProcess.playClickSe ();
        SaveMng.Status.Readed.Add(mng.Id);
        BaseStorySceneMng.StoryOrder( StoryListMast.List[mng.Index].Tag, SceneManagerWrap.NowScene );
	}

}
