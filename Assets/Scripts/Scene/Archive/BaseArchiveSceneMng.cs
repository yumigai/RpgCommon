using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseArchiveSceneMng : Base2DSceneMng
{

    public enum CATEGORY {
        STORY,
        DOCUMENT,
        RECORD,
        HELP,
        CONFIG
    }

    [SerializeField]
    private MultiUseScrollMng ArchiveList;

    [SerializeField]
    private GamePadListRecivMng MenuRecive;

    [SerializeField]
    private GamePadListRecivMng ListRecive;

    private CATEGORY SelectedCategory;

    new private void Start() {
        base.Start();
        Init();
    }

    private void Init() {
        pushCloseProcess = backToMenu;
        ArchiveList.ListItem.GetComponent<MultiUseListMng>().TagLabel.gameObject.SetActive(false);
    }

    public void backToMenu() {
        if (GamePadListRecivMng.ActiveGamePadList == MenuRecive) {
            SceneManagerWrap.loadBefore();
        } else {
            ArchiveList.clear();
            GamePadListRecivMng.ActiveGamePadList = MenuRecive;
        }
    }

    public void changeCategory(int category) {
        changeCategory((CATEGORY)category);
    }

    public void changeCategory(CATEGORY category) {

        SelectedCategory = category;

        switch (category) {
            case CATEGORY.STORY:
            makeStoryList(StoryListMast.KIND.STORY, "再生");
            break;
            case CATEGORY.DOCUMENT:
            makeStoryList(StoryListMast.KIND.BOOK, "読む");
            break;
            case CATEGORY.RECORD:
            showRecord();
            break;
            case CATEGORY.HELP:
            ArchiveList.makeList(HelpMast.List);
            break;
        }

    }

    public void pushCategoryMenu() {

        if(SelectedCategory != CATEGORY.RECORD) {
            if(ArchiveList.activeCount() > 0) {
                ListRecive.initSetupWithFrameEnd(true);
            }
        }
        
    }


    public void makeStoryList(StoryListMast.KIND kind, string button_txt, bool show_unknown = true) {
        ArchiveList.clear();
        for (int i = 0; i < StoryListMast.List.Length; i++) {
            if (StoryListMast.List[i].Kind == kind) {
                int id = StoryListMast.List[i].Id;
                if (SaveMng.Status.Archives.IndexOf(id) >= 0) {
                    MultiUseListMng listitem = ArchiveList.makeListItemMasterId(i, StoryListMast.List[i], button_txt);

                    if (SaveMng.Status.Readed.IndexOf(id) >= 0) {
                        // Newアイコン
                        listitem.TagLabel.gameObject.SetActive(false);
                    }
                } else if (show_unknown) {
                    MultiUseListMng listitem = ArchiveList.makeListItemMasterId(i, StoryListMast.List[i], "未発見", MultiUseListMng.BUTTON.LOCK);
                    listitem.Name.text = "未発見";
                    if (listitem.Detail != null) {
                        listitem.Detail.text = "";
                    }
                    listitem.TagLabel.gameObject.SetActive(false);
                }
            }
        }

        ArchiveList.ListItem.SetActive(false);
    }

    public void pushList(MultiUseListMng mng) {

        switch (SelectedCategory) {
            case CATEGORY.STORY:
            jumpStory(mng);
            break;
            case CATEGORY.DOCUMENT:
            showDocument(mng);
            break;
            case CATEGORY.RECORD:
            break;
            case CATEGORY.HELP:
            break;
        }
    }

    void jumpStory(MultiUseListMng mng) {
        if (SaveMng.Status.Archives.IndexOf(mng.Id) < 0) {
            return;
        }

        CommonProcess.playClickSe();
        SaveMng.Status.Readed.Add(mng.Id);
        BaseStorySceneMng.StoryOrder(StoryListMast.List[mng.Index].Tag, SceneManagerWrap.NowScene);
    }

    void showDocument(MultiUseListMng mng) {

        if (SaveMng.Status.Archives.IndexOf(mng.Id) < 0) {
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


    public void showRecord() {
        ArchiveList.clear();

        var stageClear =ArchiveList.makeListItem();
        var archives = ArchiveList.makeListItem();
        stageClear.Name.text = "クリアステージ数:" + SaveMng.Status.ClearStage.Count.ToString();
        archives.Name.text = "アーカイブ数:" + SaveMng.Status.Archives.Count.ToString();

        ArchiveList.ListItem.SetActive(false);
    }
}
