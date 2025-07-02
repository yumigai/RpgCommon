using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class BaseResultSceneMng : MonoBehaviour
{
    public const string CONFIRM_NEXT_JP = "次のステージに進みますか？";

    public const string CONFIRM_NEXT_EN = "next stage？";

    public const string CONFIRM_HOME_LAST_STAGE_JP = "エリアの最終ポイントに到達しました。\nホームに戻りますか？";

    public const string CONFIRM_HOME_LAST_STAGE_EN = "return home？";

    public const string CONFIRM_HOME_JP = "ホームに戻りますか？";

    public const string CONFIRM_HOME_EN = "return home？";

    [SerializeField]
    public Text DestoryEnemys;

    [SerializeField]
    public Text GetMoney;

    [SerializeField]
    public Text GetExp;

    [SerializeField]
    public Text[] RarityItemNum;

    //[SerializeField]
    //public GameObject RarityItemObject;

    //[SerializeField]
    //public Text[] RarityNumTexts = new Text[(int)GameConst.RARITY.ALL];

    //[SerializeField]
    //public GameObject[] RarityNumObjects = new GameObject[(int)GameConst.RARITY.ALL];

    public StageMast StageData { get { return BaseStageFieldSceneMng.StageData; } }

    public static bool IsSuccsess = false;

    private void Awake() {
    }

    private void Start() {
        resultSetting();
    }


    void resultSetting() {

        DestoryEnemys.text = SaveMng.Quest.DestroyNum.ToString();
        GetExp.text = SaveMng.Quest.GetExp.ToString();

        if (IsSuccsess) {
            GetMoney.text = SaveMng.Quest.GetMoney.ToString();
            for (int i = 0; i < (int)GameConst.RARITY.ALL; i++) {
                int num = SaveMng.Quest.CarryBag.Count(it => it.Rarity == i);
                RarityItemNum[i].text = num.ToString();
                RarityItemNum[i].transform.parent.gameObject.SetActive(num > 0);
            }
        }
    }

    public void finishResult() {

        if (!IsSuccsess) {
            VersatileProcess.nextDay();
            return;
        }

        bool isNext = false;

        if (StageData.NextIds.Length == 1 ) {
            //次のステージが一つのみ
            var index = StageMast.getStageIndex(StageData.NextId);
            if(index > 0) {
                StageMast.KIND next_kind = StageMast.List[index].Kind;
                if (StageData.Kind == next_kind 
                    && ( next_kind == StageMast.KIND.MAIN_MISSION || next_kind == StageMast.KIND.SUB_MISSION )) {
                    isNext = true;
                }
            }
        }

        if (isNext) {
            string txt = SaveMng.IsJp ? CONFIRM_NEXT_JP : CONFIRM_NEXT_EN;
            CommonProcess.showConfirm(txt, toNextStage, canselNextStage);
        } else {
            string txt = SaveMng.IsJp ? CONFIRM_HOME_LAST_STAGE_JP : CONFIRM_HOME_LAST_STAGE_EN;
            CommonProcess.showConfirm(txt, returnHome);
        }
    }

    public void toNextStage(object ob) {

        StageFieldSceneMng.SelectedStage = SaveMng.Quest.Continue(StageData.NextId); // StageMast.getStageIndex(StageData.NextId);
        SceneManagerWrap.LoadAndNowLoading(CmnConst.SCENE.QuestScene);
    }

    public void canselNextStage(object obj) {
        string txt = SaveMng.IsJp ? CONFIRM_HOME_JP : CONFIRM_HOME_EN;
        CommonProcess.showConfirm(txt, returnHome);
    }

    public void returnHome(object ob) {
        //獲得処理

        for (var i = 0; i < SaveMng.Quest.CarryBag.Count; i++) {
            //addItem処理を走らせるために一種類づづ追加
            SaveMng.ItemData.addItem(SaveMng.Quest.CarryBag[i], SaveMng.Quest.CarryBag[i].Num);
        }

        SaveMng.ItemData.save();
        SaveMng.Status.addMoney(SaveMng.Quest.GetMoney);
        SaveMng.Status.save();

        VersatileProcess.nextDay();

    }
}
