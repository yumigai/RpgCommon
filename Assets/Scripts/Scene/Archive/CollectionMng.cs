using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CollectionMng : StoryListMng
{

    override public void pushList(MultiUseListMng mng)
    {
        if (SaveMng.Status.Archives.IndexOf(mng.Id) < 0){
            return;
        }

        CommonProcess.playClickSe();
        
        string path = CmnConst.Path.ARCHIVE_TXT + StoryListMast.List[mng.Index].Tag;
        string txt = UtilToolLib.loadText(path);
        txt = txt.Replace("@player@", SaveMng.Conf.PlayerName);
        ConfirmWindowCmn wnd = CommonProcess.showMessage(mng.Name.text, txt);
        mng.TagLabel.gameObject.SetActive(false);
        SaveMng.Status.Readed.Add(mng.Id);
    }


}
