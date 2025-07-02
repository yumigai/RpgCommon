using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class ReadySceneMng : MonoBehaviour
{

    [SerializeField]
    private MultiUseScrollMng AreaLists;

    [SerializeField]
    private GamePadListRecivMng ListRecive;


    private void Start() {

        var list = StageMast.List.Where(it => SaveMng.Status.DiscoveryStage.IndexOf(it.Id) >= 0).ToArray();

        List<MultiUseListMng> mngs = AreaLists.makeList(list);
        foreach (var mng in mngs) {
            mng.Callback = pushStage;
        }

        ListRecive.initSetupWithFrameEnd();

    }

    public void pushStage(MultiUseListMng mng) {
        CommonProcess.showConfirm(mng.Name.text, pushJumpStageScene);
        StageFieldSceneMng.SelectedStage = System.Array.FindIndex(StageMast.List, it => it.Id == mng.Id);
    }

    public void pushChangeParty() {

    }
    public void pushJumpPartyEdit() {

    }

    public void pushJumpStageScene(object obj) {

        SaveMng.Quest.Init(StageFieldSceneMng.SelectedStage, SaveMng.Status.getActiveMembers());

        switch (StageFieldSceneMng.StageData.Kind) {
            case StageMast.KIND.MAIN_MISSION:
            case StageMast.KIND.SUB_MISSION:
            SceneManagerWrap.LoadAndNowLoading(CmnConst.SCENE.QuestScene);
            break;
            case StageMast.KIND.STORY:
            BaseStorySceneMng.StoryOrder(StageFieldSceneMng.StageData.Story, CmnConst.SCENE.HomeScene);
            break;
            case StageMast.KIND.BOSS:
            SaveMng.Quest.Enemys = new List<UnitStatusTran>();
            //SaveMng.Quest.Enemys.Add(StageFieldSceneMng.StageData.en);
            BaseStorySceneMng.StoryOrder(StageFieldSceneMng.StageData.Story, CmnConst.SCENE.BattleScene);
            break;
        }

    }

}