using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;
using System;
using UnityEngine.SceneManagement;
using System.Linq;

public class BaseStageFieldSceneMng : MonoBehaviour
{
    //public const float NEED_HOLD_TIME_STAGE_CLEAR = 5f;

    //public const float ENEMY_DISPERSION = 0.9F;

    public const string ENEMY_MODEL_PATH = "prefab/Enemy/";

    public const string STAGE_PREFAB_PATH = CmnConfig.RESOURCE_PREFAB + "Stage/";

    //private const float HOLD_KEY_POINT_UP = 5;

    //private const float HOLD_KEY_POINT_DONW = 2;

    public enum Tag
    {
        Hero,
        StageRoom,
    }

    [SerializeField]
    public Transform EnemyBase;

    //[SerializeField]
    //public Transform ObjectBase;

    [SerializeField]
    public string[] BGMS;

    [SerializeField]
    public Transform ButtomLine;

    public List<FieldEnemyMng> Enemys;

    [SerializeField]
    public EffectMng HitEffect;
    [SerializeField]
    public EffectMng CriticalEffect;
    [SerializeField]
    public EffectMng GunFireEffect;
    [SerializeField]
    public EffectMng DestroyEffect;

    [SerializeField]
    public GameObject KeyPrefab;

    [SerializeField]
    public GameObject GatePrefab;

    //[SerializeField]
    //public JemMng[] DefaultJems;

    [SerializeField]
    public AudioClip CriticalSe;
    [SerializeField]
    public AudioClip FinishAttackSe;

    [SerializeField]
    public AudioClip GameClearSe;

    [SerializeField]
    public AudioClip GameOverSe;

    [SerializeField]
    public GameObject[] TreasurePrefab;

    [SerializeField]
    public FieldUIMng MainUI;

    [SerializeField]
    private StageAreaMng StageAreaPrefab;

    [SerializeField]
    private Effct3DtoUIMng EventCountEffect;

    [SerializeField]
    private MenuSceneMng MenuScene;

    //[SerializeField]
    //private Image EncountScreen;

    [SerializeField]
    public GameObject SceneBase;

    [System.NonSerialized]
    public int NowDestroy;

    [System.NonSerialized]
    public int GetKey;

    [System.NonSerialized]
    public StageAreaMng StageArea;

    [System.NonSerialized]
    public bool IsReady = false; //初期準備

    [System.NonSerialized]
    public bool IsBattle; //コマンドバトル式でのみ使用

    private CharacterMng EncountEnemy;

    /// <summary>
    /// ステージ開始時刻
    /// </summary>
    private float StartTime;

    /// <summary>
    /// 終了フラグ（ステージクリア・ゲームオーバー）
    /// </summary>
    private bool IsFinished;

    ///// <summary>
    ///// キー長押し時間
    ///// </summary>
    //private float HoldKeyTime = 0f;

    public static int SelectedStage = 0;

    public static BaseStageFieldSceneMng Singleton;

    public Image EncountScreen {
        get {
            return BaseBattleSceneMng.Singleton.EncountScreen;
        }
    }

    public static StageMast StageData {
        get {
            return StageMast.List == null ? null : StageMast.List[SelectedStage];
        }
    }

    private static QuestTran Quest {
        get {
            return SaveMng.Quest;
        }
    }

    void Awake() {
        Physics.gravity = new Vector3(0, -30, 0);
        SceneManagerWrap.LoadSceneAsync(GameConst.SCENE.BattleScene.ToString(), LoadSceneMode.Additive);
        Singleton = this;
    }

    void Start() {
        init();
        Resume();
        IsReady = true;

    }

    private void OnEnable() {
        if (IsReady) {
            Resume();
        }
    }

    private void FixedUpdate() {
        //if(HoldKeyTime > 0) {
        //    HoldKeyTime -= HOLD_KEY_POINT_DONW;
        //    HoldKeyTime = Mathf.Clamp(HoldKeyTime, 0, HoldKeyTime);
        //}
    }

    protected void init() {
        IsReady = false;
        IsFinished = false;
        Enemys = new List<FieldEnemyMng>();
        NowDestroy = 0;

        makeStage();

        setRespawn();
        //int res_index = UnityEngine.Random.Range(0, RespawnMng.Posis.Count());
        FieldPlayerMng.hero().transform.position = RespawnMng.getRandom();// RespawnMng.Posis[res_index];
        CameraMng.Singleton.setLookTarget(FieldPlayerMng.Hero.transform);
        putEnemy();
        putGateObject();
        putKeyObject();


        MainUI.makeMiniMap(StageArea);

        BaseBattleSceneMng.BattleBgm = StageData.Chapter.BattleBgm;
        BaseBattleSceneMng.BossBgm = StageData.Chapter.BossBgm;
    }

    public void Resume() {

        Pauser.Resume();

        if (BaseBattleSceneMng.IsWin() && EncountEnemy != null) {
            Enemys.Remove((FieldEnemyMng)EncountEnemy);
            Destroy(EncountEnemy.gameObject);
        }

        if (IsBattle) {
            //バトル後なら敵を再配置
            Vector3[] posis = RespawnMng.getMiddleToLongPosis();
            Enemys.ForEach(it => setEnemyPosition(it.gameObject, posis));

            playBgm();
            IsBattle = false;
        }
    }

    /// <summary>
    /// 汎用ヒットエフェクト
    /// </summary>
    /// <param name="posi"></param>
    /// <param name="rote"></param>
    /// <param name="se"></param>
    public static void hitEffect(Vector3 posi, Quaternion rote, AudioClip se = null) {
        hitEffect(null, posi, rote, se);
    }

    public static void hitEffect(GameObject effect, Vector3 posi, Quaternion rote, AudioClip se) {
        if (effect == null) {
            Singleton.HitEffect.effect(posi, rote);
        } else {
            EffectMng.showEffect(posi, rote, effect, 1f);
        }
        if (se != null) {
            SoundMng.Instance.playSE(se);
        }
    }

    public static void fireEffect(Vector3 posi, Quaternion rotate) {
        Singleton.GunFireEffect.effect(posi, rotate);
    }

    public static void criticalEffect(Vector3 posi, Quaternion rotate) {
        Singleton.CriticalEffect.effect(posi, rotate);
    }

    protected void makeStage() {

        GameObject prefab = Resources.Load(STAGE_PREFAB_PATH + StageData.StagePrefab) as GameObject;
        StageAreaPrefab = prefab.GetComponent<StageAreaMng>();

        if (StageAreaPrefab.MaxRoomNum > 0) {
            StageAreaPrefab.MaxRoomNum = StageData.MaxRoomNum;
        }

        StageArea = Instantiate(StageAreaPrefab) as StageAreaMng;
        
        StageArea.transform.SetParent(SceneBase.transform);
        //StageArea.CallbackBaked = makedArea;
    }

    protected void setRespawn() {
        List<Vector3> resporns = new List<Vector3>();

        for (int i = 0; i < StageArea.Rooms.Count(); i++) {
            if (StageArea.Rooms[i].Respawns.Length > 1) {
                for (int j = 1; j < StageArea.Rooms[i].Respawns.Length; j++) {
                    Vector3 posi = StageArea.Rooms[i].Respawns[j].position;
                    resporns.Add(new Vector3(posi.x, posi.y + 0f, posi.z));
                }
            } else {
                Transform res_center = StageArea.Rooms[i].RewpawnCenter;
                if (res_center != null) {
                    resporns.Add(new Vector3(res_center.position.x, res_center.position.y, res_center.position.z));
                }
            }
        }
        RespawnMng.Posis = resporns.ToArray();
    }

    /// <summary>
    /// ナビメッシュ焼きこみ
    /// </summary>
    /// <returns></returns>
    //protected IEnumerator bakeStage() {
    //    yield return new WaitForEndOfFrame();
    //    StageArea.bakeNav();
    //}

    protected void playRandomBgm() {
        if (BGMS.Length > 0) {
            int index = UnityEngine.Random.Range(0, BGMS.Length);
            SoundMng.Instance.playBGM(BGMS[index]);
        }
    }

    /// <summary>
    /// ステージ生成コールバック
    /// </summary>
    //protected void makedArea() {
    //    //敵行動開始
    //    putEnemy();
    //    Enemys.ForEach(it => it.Ready());
    //}

    protected void playBgm() {
        //AudioClip bgm = Resources.Load<AudioClip>( GameConst.Path.MUSIC + StageData.Bgm);
        SoundMng.Instance.playBGM(StageData.Bgm);
    }


    protected void putEnemy() {
        putEnemy(StageArea.EnemyPrefab, StageData.EnemyNum);
    }

    protected void putEnemy(GameObject enemy_prefab, int num) {

        Vector3[] posis = RespawnMng.getMiddleToLongPosis();

        for (int j = 0; j < num; j++) {
            GameObject obj = Instantiate(enemy_prefab) as GameObject;
            obj.transform.parent = EnemyBase;
            obj.name = enemy_prefab.name + "_" +j;

            setEnemyPosition(obj, posis);

            FieldEnemyMng mng = obj.GetComponent<FieldEnemyMng>();
            Enemys.Add(mng);
            MainUI.addEnemy(mng);
        }
    }

    private void setEnemyPosition(GameObject obj, Vector3[] posis) {

        int posi_rand = UnityEngine.Random.Range(0, posis.Length);

        obj.transform.position = posis[posi_rand];
    }

    /// <summary>
    /// キーアイテム配置
    /// </summary>
    protected void putKeyObject() {
        if (StageData.Rule == StageMast.GAME_RULE.GET_TREASURE || StageData.Rule == StageMast.GAME_RULE.GET_KEY_AND_GOAL) {
            Vector3[] tmp = RespawnMng.getMiddleToLongPosis();
            List<Vector3> posis = tmp.ToList();

            for (int i = 0; i < StageData.NeedKeyNum; i++) {
                int index = UnityEngine.Random.Range(0, posis.Count);
                Vector3 posi = posis[index];
                //posi.y += RespawnMng.AIR_LOW;
                //ステージごとのキー設定が無ければ共通キーを使用する
                GameObject prefab = StageArea.KeyPrefab == null ? KeyPrefab : StageArea.KeyPrefab;
                GameObject obj = Instantiate(prefab) as GameObject;
                obj.transform.SetParent(SceneBase.transform);
                obj.transform.position = posi;
                if (posis.Count() > 1) {
                    //候補は最低一つは残す
                    posis.RemoveAt(index);
                }
            }
        }
    }

    /// <summary>
    /// ゴールオブジェクト配置
    /// </summary>
    protected void putGateObject() {
        if (StageData.Rule == StageMast.GAME_RULE.GOAL || StageData.Rule == StageMast.GAME_RULE.GET_KEY_AND_GOAL) {
            Vector3 posi = RespawnMng.getMiddleToLongPosiRandom();
            GameObject prefab = StageArea.KeyPrefab == null ? GatePrefab : StageArea.GatePrefab;
            GameObject obj = Instantiate(prefab) as GameObject;
            obj.transform.SetParent(SceneBase.transform);
            obj.transform.position = posi;
            MainUI.StageClearGuid.setClient(obj);
            MainUI.StageClearGuid.gameObject.SetActive(false);
        }
    }

    public void pushMelee() {
        FieldPlayerMng.Instance.pushMeleeAttack();
    }

    /// <summary>
    /// エンカウント
    /// </summary>
    /// <param name="attacker"></param>
    /// <param name="defender"></param>
    public void hitAndEncount(CharacterMng attacker, CharacterMng defender) {

        foreach (var it in Enemys) {
            if (it.MeleeWeapon != null) {
                it.MeleeWeapon.Clear();
            }
        }

        if (IsBattle) {
            return;
        }

        IsBattle = true;

        MenuScene.hideMenu();

        SoundMng.Instance.stopBGM();

        Pauser.Pause();

        bool player_adv = attacker.name == FieldPlayerMng.hero().name;

        EncountEnemy = player_adv ? defender : attacker;

        BaseBattleSceneMng.Advance = player_adv ? BaseBattleSceneMng.ADVANCED.PLAYER : BaseBattleSceneMng.ADVANCED.ENEMY;

        //Quest.Enemys = EnemyEncountMast.encount(StageData.EncountMapId, StageData.FieldSize);

        Quest.Enemys = new List<UnitStatusTran>();

        int field_size = StageData.FieldSize;

        var encounts = EnemyEncountMast.List.Where(it => it.MapId == StageData.EncountMapId);
        var enc_per = encounts.Select(it => it.Percent).ToArray();
        var enc_arr = encounts.ToArray();

        for (int i = field_size; i > 0; i--) {
            int index = UtilToolLib.getRateRandom(enc_per);
            var enc = enc_arr[index];

            UnitStatusTran tran = EnemyMast.getEnemy(enc.EnemyId);
            if (enc.Lv > 0) {
                tran.setLevel(enc.Lv);
            }

            Quest.Enemys.Add(tran);
        }

        StartCoroutine(changeBattleScene());

    }

    private IEnumerator changeBattleScene() {
        yield return new WaitForSeconds(0.1f);
        StartCoroutine(CameraScreenShotMng.TakeAndGetScreenShot((Sprite sp) => {
            EncountScreen.sprite = sp;
            EncountScreen.gameObject.SetActive(true);
            BaseBattleSceneMng.Singleton.SceneBase.SetActive(true);
            SceneBase.SetActive(false);
        }));
    }

    public void forceChangeBattleScene() {
        StartCoroutine(changeBattleScene());
    }

    public void showMenu() {
        MenuScene.showMenu(SceneBase);
        //MenuBoard.ShowMenu(SceneBase);
    }

    /// <summary>
    /// ゴールに到達
    /// </summary>
    public void reachTheGoal() {
        switch (StageData.Rule) {
            case StageMast.GAME_RULE.GET_KEY_AND_GOAL:
            if (GetKey >= StageData.NeedKeyNum) {
                gameClear();
            }
            break;
            case StageMast.GAME_RULE.GOAL:
            gameClear();
            break;
        }
    }

    /// <summary>
    /// ゲームルール：全滅
    /// </summary>
    public void destroyEnemy() {
        if (StageData.Rule == StageMast.GAME_RULE.DESTROY_ALL && Enemys.Count() == 0) {
            gameClear();
        }
    }

    /// <summary>
    /// ゲームルール；収集
    /// </summary>
    public void getKey() {
        GetKey++;
        if (StageData.Rule == StageMast.GAME_RULE.GET_TREASURE && GetKey >= StageData.NeedKeyNum) {

            clearEnemys();

            gameClear();
        }
    }

    /// <summary>
    /// 敵キャラ全部削除
    /// </summary>
    private void clearEnemys() {
        for (int i = 0; i < Enemys.Count; i++) {
            EffectMng.showEffect(Enemys[i].HitEffectPoint.position, Enemys[i].transform.localRotation, DestroyEffect.EffectPrefab);
            Destroy(Enemys[i].gameObject);
        }
        Enemys.Clear();
    }

    public void gameClear() {
        if (IsFinished) {
            return;
        }
        IsFinished = true;

        SaveMng.Quest.NowFloorNum++;
        SoundMng.Instance.playSE(GameClearSe);

        if (SaveMng.Quest.NowFloorNum >= StageData.FloorNum) {
            BaseResultSceneMng.IsSuccsess = true;
            SaveMng.Status.ClearAndNext(StageData);
            SaveMng.Status.save();
            gameFinishProcess();
        } else {
            SceneManagerWrap.LoadAndNowLoading(CmnConst.SCENE.QuestScene);
        }
    }

    public void stageClearAction() {
        if (!CmnBaseProcessMng.IsPause && !IsFinished) {
            gameClear();
        }
    }

    ///// <summary>
    ///// EventTrriger.PointerUpで使用
    ///// </summary>
    //public void stageClearActionCansel() {

    //    MainUI.StageClearGuid.updGauge(0f);
    //}

    private void gameFinishProcess() {

        //CameraScreenShotMng.callRandomShot();
        //ScreenCapture.CaptureScreenshot(CameraScreenShotMng.SCREEN_NAME);
        StartCoroutine(waitResultScene());
    }

    public IEnumerator waitResultScene() {
        yield return new WaitForSeconds(5);

        //SaveMng.GameData.TotalDestroy += NowDestroy;

        //SaveMng.GameData.save();

        //SaveMng.Player.NowMaxHp = PlayerMng.hero().MaxLife;
        //SaveMng.Player.save();

        //ResultSceneMng.GetCoin = GetNowGameCoin;
        //ResultSceneMng.DestroyEnemy = NowDestroy;
        //ResultSceneMng.FinishTime = Time.fixedTime - StartTime;

        SceneManagerWrap.LoadScene(CmnConst.SCENE.ResultScene);
    }

    public void readyStageClear() {
        MainUI.StageClearGuid.gameObject.SetActive(true);
        MainUI.StageClearGuid.setPosition();
    }

    public void resetStageClear() {
        MainUI.StageClearGuid.gameObject.SetActive(false);
    }
}
