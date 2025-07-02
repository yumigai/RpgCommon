using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class BaseInitSceneMng : MonoBehaviour {

    protected void Awake() {
        Application.targetFrameRate = 60;
        Time.timeScale = 1f;
    }

    // Use this for initialization
    protected void Start () {
        StartCoroutine(waitCommonProcess());
    }

    /// <summary>
    /// 共通処理が終わり次第開始
    /// InitSceneMngで初期処理（マスタ・セーブ読み込み）を
    /// 行わないのは各画面でのデバッグをやりやすくするため
    /// </summary>
    /// <returns></returns>
    IEnumerator waitCommonProcess() {
        while (!CommonProcess.InitFinish) {
            yield return new WaitForSeconds(0.1f);
        }

        if (!SaveMng.Status.IsFinishInitSetting) {
            initSetting();
        }

        SceneManagerWrap.LoadScene(CmnConst.SCENE.TitleScene, false);

        //if (SaveMng.Status.IsFinishOpening) {
        //    SceneManagerWrap.LoadScene(CmnConst.SCENE.TitleScene, false);
        //} else {
        //    SceneManagerWrap.LoadScene(CmnConst.SCENE.OpeningScene, false);
        //}
    }

    void initSetting() {
        statusSetting();
        setKeyCode();
        addInitUnit();
        addOpenStage();
        SaveMng.Status.IsFinishInitSetting = true;
        SaveMng.Status.save();
    }

    /// <summary>
    /// ステータス初期設定
    /// </summary>
    void statusSetting() {
        SaveMng.Status.RestTimeLimit = GameConst.DEFAULT_TIME_LIMIT;
    }

    /// <summary>
    /// 初期ユニット追加
    /// </summary>
    void addInitUnit() {
        if (SaveMng.Units.Count == 0 ) {
            foreach (var unit in UnitMast.List) {
                UnitStatusTran tran = UnitMast.getUnit(unit);
                if (tran.Mst.Feature.Contains(UnitMast.FEATURE.INIT_JOIN)
                    || tran.Mst.Feature.Contains(UnitMast.FEATURE.INIT_PARTY)) {
                    tran.Tactics = AiProc.TACTICS.COMMAND;
                    SaveMng.Units.Add(tran);
                    if (tran.Mst.Feature.Contains(UnitMast.FEATURE.INIT_PARTY)) {
                        SaveMng.Status.resetPartyAll(tran.Id);
                    }
                }
            }
            SaveMng.saveUnit();
        }
    }

    /// <summary>
    /// 初期ステージ解放
    /// </summary>
    void addOpenStage() {
        if(SaveMng.Status.DiscoveryStage.Count == 0) {
            var list = StageMast.List.Where(it => it.Feature.Contains(StageMast.FEATURE.INIT_STAGE));
            foreach(var stage in list) {
                SaveMng.Status.DiscoveryStage.Add(stage.Id);
            }
        }
    }

    void setKeyCode() {

        SaveMng.Key.Buttons = new string[(int)CmnConfig.GamePadButton.KeyAll];

        SaveMng.Key.Buttons[(int)CmnConfig.GamePadButton.Decision] = "A";
        SaveMng.Key.Buttons[(int)CmnConfig.GamePadButton.Cancel] = "B";
        SaveMng.Key.Buttons[(int)CmnConfig.GamePadButton.A] = "A";
        SaveMng.Key.Buttons[(int)CmnConfig.GamePadButton.B] = "B";
        SaveMng.Key.Buttons[(int)CmnConfig.GamePadButton.X] = "X";
        SaveMng.Key.Buttons[(int)CmnConfig.GamePadButton.Y] = "Y";
        SaveMng.Key.Buttons[(int)CmnConfig.GamePadButton.L1] = "L1";
        SaveMng.Key.Buttons[(int)CmnConfig.GamePadButton.R1] = "R1";
        SaveMng.Key.Buttons[(int)CmnConfig.GamePadButton.Back] = "Back";
        SaveMng.Key.Buttons[(int)CmnConfig.GamePadButton.Start] = "Start";
        SaveMng.Key.Buttons[(int)CmnConfig.GamePadButton.L3] = "L3";
        SaveMng.Key.Buttons[(int)CmnConfig.GamePadButton.R3] = "R3";

        SaveMng.Key.KeyBoards = new string[(int)CmnConfig.GamePadButton.KeyAll];

        SaveMng.Key.KeyBoards[(int)CmnConfig.GamePadButton.Decision] = "z";
        SaveMng.Key.KeyBoards[(int)CmnConfig.GamePadButton.Cancel] = "x";
        SaveMng.Key.KeyBoards[(int)CmnConfig.GamePadButton.A] = "z";
        SaveMng.Key.KeyBoards[(int)CmnConfig.GamePadButton.B] = "x";
        SaveMng.Key.KeyBoards[(int)CmnConfig.GamePadButton.X] = "c";
        SaveMng.Key.KeyBoards[(int)CmnConfig.GamePadButton.Y] = "v";
        SaveMng.Key.KeyBoards[(int)CmnConfig.GamePadButton.L1] = "f";
        SaveMng.Key.KeyBoards[(int)CmnConfig.GamePadButton.R1] = "g";
        SaveMng.Key.KeyBoards[(int)CmnConfig.GamePadButton.Back] = "r";
        SaveMng.Key.KeyBoards[(int)CmnConfig.GamePadButton.Start] = "t";
        SaveMng.Key.KeyBoards[(int)CmnConfig.GamePadButton.L3] = "b";
        SaveMng.Key.KeyBoards[(int)CmnConfig.GamePadButton.R3] = "n";

        SaveMng.Key.save();

    }
}
