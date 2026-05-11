using System;
using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;
using System.Linq;
using UnityEngine.UI;

public class CommonProcess : CmnBaseProcessMng
{

    [SerializeField]
    public GameObject InputTextPrefab;

    [System.NonSerialized]
    new public static CommonProcess Singleton;

    //public static bool IsQuest{ get{ return ( SaveMng.Quest == null || !SaveMng.Quest.IsFinished ); } }

    //public static Image SwapScreen;
    public static bool InitFinish = false;

    protected new void Awake() {

#if UNITY_STANDALONE_WIN
        //Screen.SetResolution(1920, 1080, false, 60);
#endif

        if (Singleton != null) {
            Destroy(this.gameObject);
            return;
        }

        Singleton = this;

        base.Awake();

        loadInit();

    }

    public static void loadInit() {

        HomeMessageMast.load();

        EventActionMast.load();

        WorldMast.load();

        ItemMast.load();

        RuneMast.load();

        StageMast.load();

        ChapterMast.load();

        ShopItemMast.load();

        TrapMast.load();

        EnemyMast.load();

        UnitMast.load();

        EnemyEncountMast.load();

        TreasureMast.load();

        SkillMast.load();

        SkillLeanMast.load();

        LevelMast.load();

        StoryListMast.load();

        HelpMast.load();

        SaveMng.loadAll();

        InitFinish = true;

    }

    new private void Start() {
        base.Start();
    }

    // Update is called once per frame
    new void Update() {
        base.Update();
    }

    public static GameObject getCommonObject() {
        return Singleton.gameObject;
    }

    public static GameConst.ELEMENT getRandomElement() {
        GameConst.ELEMENT ele = (GameConst.ELEMENT)UnityEngine.Random.Range(1, (int)GameConst.ELEMENT.All);
        return ele;
    }

    public static string getElementWord(GameConst.ELEMENT ele) {
        return SaveMng.IsJp ? GameConst.ELEMENT_JP[(int)ele] : GameConst.ELEMENT_EN[(int)ele];
    }

    public static Sprite getElementImage(GameConst.ELEMENT ele) {

        string path = CmnConst.Path.IMG_ELEMENT + ele.ToString();
        return Resources.Load<Sprite>(path);
    }

    public static void saveAll(System.DateTime now) {

        SaveMng.saveTime(now);
        SaveMng.Status.save();

        if (SaveMng.Quest != null) {
            SaveMng.Quest.save();
        }
        SaveMng.UnitData.save();
    }

    ///// <summary>
    ///// ドロップアイテム判定
    ///// </summary>
    ///// <param name="table_id"></param>
    ///// <returns></returns>
    //public static ItemMast judgeDropItem(int table_id) {

    //    var category = UtilToolLib.getRateRandom(GameConst.DROP_CATEGORY_PERCENT);

    //    var drops = System.Array.FindAll(DropItemMast.List, it => it.DropTableId == table_id);

    //    drops.Select(it => ItemMast.getItem(it.ItemId));

    //    var items = ItemMast.List.Where(it => (int)it.Category == category).Where(it => drops.Any(it2 => it.Id == it2.ItemId));
    //    var count = items.Count();
    //    if (count <= 0) {
    //        return null;
    //    }

    //    var index = UnityEngine.Random.Range(0, count);

    //    return items.ElementAt(index);
    //}

    // public IEnumerator FadeOutAnimation() {
    //     if(SwapScreen != null){
    //         yield return new WaitForSeconds(0.1f);
    //         StartCoroutine(CameraScreenShotMng.TakeAndGetScreenShot((Sprite sp) => {
    //             SwapScreen.sprite = sp;
    //             SwapScreen.gameObject.SetActive(true);
    //             //BaseBattleSceneMng.Singleton.SceneBase.SetActive(true);
    //             //SceneBase.SetActive(false);
    //         }));
    //     }
    // }

    // public IEnumerator FadeInAnimation() {
    //     if(SwapScreen != null){
    //         yield return new WaitForEndOfFrame();
    //         yield return new WaitForSeconds(1f);
    //         SwapScreen.CrossFadeAlpha(0, 3, true);
    //         for (var i = 0; i < 100; i++) {
    //             BrokenFilter.Fade -= 0.01f;
    //             yield return new WaitForSeconds(0.02f);
    //         }
    //     }
    //     SwapScreen.gameObject.SetActive(false);
    // }


    public void initSetting() {
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
        if (SaveMng.Units.Count == 0) {
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
        if (SaveMng.Status.DiscoveryStage.Count == 0) {
            var list = StageMast.List.Where(it => it.Feature.Contains(StageMast.FEATURE.INIT_STAGE));
            foreach (var stage in list) {
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