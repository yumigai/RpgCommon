using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using System.Linq;

public abstract class BaseBattleSceneMng : MonoBehaviour
{

    public enum SEQUENCE
    {
        ENCOUNT,
        ADVANCE,
        TURN_START, //演出・全体ターン開始
        UNIT_READY, //ユニットターン開始
        COMMAND, //コマンド入力待ち
        PROCESS, //処理
        ACTION, //演出中
        SELECT_TARGET, //ターゲット選択
        CHOICE, //アイテム・スキル選択
        INPUT_WAIT, //プレイヤー入力待ち
    }

    /// <summary>
    /// ターゲット選択サイド
    /// </summary>
    private enum SELECT_SIDE
    {
        NON,
        PARTY,
        ENEMY,
    }

    public enum ADVANCED
    {
        PLAYER,
        ENEMY,
        EVEN,
    }

    private const string regenerate_effect = "Regenerate";

    private const string AdvanceEffect = "Blow";
    private const string PoisonEffect = "PoisonTurn";
    private const string ParalyzeEffect = "ParalyzeTurn";

    private const int ADVANCE_HIT_EFFECT_NUM = 3;

    [SerializeField]
    public PartyPlatesMng PlayerParty;

    [SerializeField]
    public EnemyPlatesMng EnemyParty;

    [SerializeField]
    public Camera EffectCamera;

    [SerializeField]
    public GameObject CommandPanel;

    [SerializeField]
    public FrameImageMng CommandUnit;

    [SerializeField]
    public GameObject TargettPanel;

    [SerializeField]
    public GameObject ChoicePanel;

    [SerializeField]
    public MultiUseScrollMng ChoiceList;

    [SerializeField]
    public Image EncountScreen;

    [SerializeField]
    public GameObject ResultBoard;

    [SerializeField]
    public GameObject LoseBoard;

    [SerializeField]
    public NumberAnimationMng ResultExp;

    [SerializeField]
    public NumberAnimationMng ResultMoney;

    [SerializeField]
    public Text ResultNowMoney;

    [SerializeField]
    public MultiUseScrollMng ResultItemList;

    [SerializeField]
    public CharaImgGaugeMng ExpGauges;

    [SerializeField]
    public EffectMng DestroyEffect;

    [SerializeField]
    public Text MessageText;

    [SerializeField]
    public Text TurnText;

    [SerializeField]
    protected CharaImgGroupMng ActionSortGroup;

    [SerializeField]
    public GameObject SceneBase;

    protected BattleProc.RESULT BattleResult = BattleProc.RESULT.CONTINUE;

    private BattleProc.ActionData CommandData;

    private SELECT_SIDE SelectSide = SELECT_SIDE.NON;

    private CharaPlateMng SelectedUnit;

    private CharaPlateMng BeforeTargetPlayer;

    private CharaPlateMng BeforeTargetEnemy;

    private Dictionary<string, EffectMng> MainEffects;

    protected static SEQUENCE Sequence;

    ////前シーケンス（キャンセルで戻る場合に使用）
    //protected static SEQUENCE BeforeSequence;

    public static ADVANCED Advance;

    public static string BattleBgm;

    public static string BossBgm;

    public static BaseBattleSceneMng Singleton;

    public QuestTran Quest {
        get {
            return SaveMng.Quest;
        }
    }
    public BattleProc.ActionData Log {
        get {
            return BattleProc.BattleAction;
        }
    }

    private List<CharaPlateMng> SelectSideMembers {
        get {
            return SelectSide == SELECT_SIDE.PARTY ? PlayerParty.Members : EnemyParty.Members;
        }
    }

    /// <summary>
    /// 直近の攻撃後者
    /// ※現在の攻撃者とは違う
    /// </summary>
    //public PartyPlatesMng Attacked {
    //    get {
    //        return Log.Atk.Type == UnitMast.TYPE.PLAYER ? PlayerParty : EnemyParty;
    //    }
    //}

    ///// <summary>
    ///// 直近の防御後者
    ///// </summary>
    //public PartyPlatesMng Defended {
    //    get {
    //        return Log.Atk.Type == UnitMast.TYPE.PLAYER ? EnemyParty : PlayerParty;
    //    }
    //}

    /// <summary>
    /// ユニットプレート取得
    /// </summary>
    /// <param name="type"></param>
    /// <param name="id"></param>
    /// <returns></returns>
    public CharaPlateMng getUnitPlate(UnitStatusTran unit) {
        var party = unit.Type == UnitMast.TYPE.PLAYER ? PlayerParty : EnemyParty;
        return party.getMember(unit.Id);
    }

    public GameObject MessageBoard {
        get {
            return MessageText.transform.parent.gameObject;
        }
    }

    void Awake() {
        Singleton = this;
        init();
    }

    protected void init() {
        ResultBoard.gameObject.SetActive(false);
        LoseBoard.gameObject.SetActive(false);
        EncountScreen.gameObject.SetActive(false);
        MainEffects = new Dictionary<string, EffectMng>();
        var eff_list = EffectCamera.transform.parent.GetComponentsInChildren<EffectMng>();
        foreach (var eff in eff_list) {
            MainEffects.Add(eff.name, eff);
        }
        SceneBase.SetActive(SceneManagerWrap.NowScheneIs(CmnConst.SCENE.BattleScene));
    }

    private void Start() {
    }

    public void OnEnable() {

        initSetting();
    }

    protected virtual void initSetting() {
        Sequence = SEQUENCE.ENCOUNT;
        MessageBoard.SetActive(false);
        CommandPanel.SetActive(false);
        CommandUnit.gameObject.SetActive(false);
        showTargetPanel(false);
        ChoicePanel.SetActive(false);
        ResultBoard.gameObject.SetActive(false);
        LoseBoard.gameObject.SetActive(false);
        EncountScreen.gameObject.SetActive(true);
        //BrokenFilter.Fade = 1f;
        //BluePrintFilter.enabled = false;
        BattleResult = BattleProc.RESULT.CONTINUE;
        PlayerParty.CreatePlate(GameConst.Path.IMG_CHARA_BUSTUP);
        EnemyParty.initPlate();
        PlayerParty.setCallback((int index, CharaPlateMng plate) => { selectPartyUnit(plate); }, (int index, CharaPlateMng plate) => { enterPointerPartyUnit(plate); });
        EnemyParty.setCallback((int index, CharaPlateMng plate) => { selectEnemy(plate); }, (int index, CharaPlateMng plate) => { enterPointerEnemy(plate); });
        EnemyParty.Members.ForEach(it => it.showParameter(false));
        BattleProc.Init();
        TurnText.text = "01";
        StartCoroutine(EncountAnimation());
    }

    protected abstract IEnumerator EncountAnimation();

    public void Update() {

        if (BattleResult == BattleProc.RESULT.CONTINUE) {
            //BattleResultに変化があった場合、この中でreturnはしないこと（result処理に入れなくなる）

            switch (Sequence) {
                case SEQUENCE.ENCOUNT:
                break;
                case SEQUENCE.ADVANCE:
                advanceAction();
                break;
                case SEQUENCE.TURN_START:
                turnStart();
                break;
                case SEQUENCE.UNIT_READY:
                unitReady();
                break;
                case SEQUENCE.PROCESS:
                    if (BattleProc.ActionUnit.Hp > 0) {

                        BattleResult = BattleProc.battle(CommandData);
                        StartCoroutine(battleAction());
                    } else {
                        BattleResult = BattleProc.endProcess();
                        nextTurn();
                    }
                    CommandData = null;

                    break;
                case SEQUENCE.COMMAND:
                    command();
                    break;
                case SEQUENCE.CHOICE:
                    choice();
                    break;
                case SEQUENCE.ACTION:
                break;
                case SEQUENCE.SELECT_TARGET:
                break;
                case SEQUENCE.INPUT_WAIT:
                break;
            }

            if (BattleResult != BattleProc.RESULT.CONTINUE) {
                //バトル終了（CONTINUEのループから状態が変更したので１回しか行われない想定
                StartCoroutine(ResultProcess());
            }
        }
    }

    private void advanceAction() {

        Sequence = SEQUENCE.ACTION;

        EnemyParty.Members.ForEach(it => it.showParameter(true));

        switch (Advance) {
            case ADVANCED.EVEN:
            Sequence = SEQUENCE.TURN_START;
            break;
            case ADVANCED.PLAYER:
            //advanceDamageProcess()
            StartCoroutine(advanceAttack(EnemyParty));
            break;
            case ADVANCED.ENEMY:
            StartCoroutine(advanceAttack(PlayerParty));
            break;
        }
    }

    private IEnumerator advanceAttack(PartyPlatesMng target) {

        //for( var i = 0; i < target.Members.Count(); i++){
        if (target.Members != null) {
            foreach (var val in target.Members) {
                BattleProc.freeCrash(-BattleProc.SPECIAL_CRASH_ADD_SUB, val.Unit);
                Vector3 eff_posi = val.transform.position;
                for (int i = 0; i < ADVANCE_HIT_EFFECT_NUM; i++) {
                    if (val.Shaper != null) {
                        eff_posi = val.Shaper.randomPosition();
                    }
                    MainEffects[AdvanceEffect].effect(eff_posi);
                    yield return new WaitForSeconds(0.1f);
                }
                damageReaction(val, 0, val.transform.position);
                yield return new WaitForSeconds(0.2f);
            }
        }

        yield return new WaitForSeconds(1f);

        Sequence = SEQUENCE.TURN_START;

    }

    private void turnStart() {
        BattleProc.ready();
        ActionSortGroup.CreateGroup(BattleProc.Battlers.FindAll(it => it.IsAlive));
        TurnText.text = (BattleProc.BattleData.Turn + 1).ToString("D2");
        Sequence = SEQUENCE.UNIT_READY;
    }

    /// <summary>
    /// ユニット行動開始
    /// </summary>
    private void unitReady() {

        CharaPlateMng chara = getUnitPlate(BattleProc.ActionUnit);

        if (chara != null) {
            chara.turnStart();
        }

        Sequence = SEQUENCE.COMMAND;
    }

    /// <summary>
    /// コマンド入力受付待ち
    /// </summary>
    private void command() {

        if (BattleProc.ActionUnit.IsAlive && BattleProc.ActionUnit.Tactics == AiProc.TACTICS.COMMAND) {
            CommandUnit.Img.sprite = BattleProc.ActionUnit.getStandImage();
            CommandUnit.updImageSize();
            CommandUnit.gameObject.SetActive(true);
            readyCommand();
        } else {
            Sequence = SEQUENCE.PROCESS;
        }
    }

    /// <summary>
    /// コマンド入力準備共通
    /// </summary>
    private void readyCommand() {
        CommandPanel.SetActive(true);
        CommandData = new BattleProc.ActionData();
        CommandData.Atk = BattleProc.ActionUnit;
        Sequence = SEQUENCE.INPUT_WAIT;
    }

    /// <summary>
    /// スキル・アイテム選択
    /// </summary>
    private void choice() {
        if (BattleProc.ActionUnit.IsAlive && BattleProc.ActionUnit.Tactics == AiProc.TACTICS.COMMAND) {

            ChoiceList.clear();

            if (CommandData.Action == AiProc.ACTION.ITEM) {
                var items = SaveMng.Items.Where(it => it.Mst.Category == ItemMast.CATEGORY.CONSUMABLE);

                foreach (var item in items) {
                    var item_num = string.Format("x{0}",item.Num);
                    ChoiceList.makeListItem(item.Id, item.Name, GameConst.Path.ICON_ITEM_PATH + item.Icon, item_num, item.Detail, choiceListItem);
                }
            } else {
                var sikills = BattleProc.ActionUnit.Skills;
                foreach (var skill in sikills) {
                    var mp_cost = string.Format("MP:{0}", skill.Cost);
                    ChoiceList.makeListItem(skill.Id, skill.Name, GameConst.Path.ICON_SKILL_PATH + skill.Icon, skill.Detail, mp_cost, choiceListItem);
                }
            }

            ChoicePanel.SetActive(true);

            ChoiceList.ReadyInputGamePad(true);

            Sequence = SEQUENCE.INPUT_WAIT;
        } else {
            Sequence = SEQUENCE.PROCESS;
        }
    }

    /// <summary>
    /// スキル・アイテム決定
    /// </summary>
    public void choiceListItem(int id) {

        PowerMast power;

        if (CommandData.Action == AiProc.ACTION.ITEM) {
            var item = SaveMng.ItemData.getData(id);
            CommandData.Item = item;
            power = item.Mst;
        } else {
            var skill = BattleProc.ActionUnit.getSkill(id);
            CommandData.Skill = skill;
            power = skill;
        }

        SelectSide = power.Side == PowerMast.SIDE.TARGET ? SELECT_SIDE.ENEMY : SELECT_SIDE.PARTY;

        //BeforeSequence = Sequence;
        Sequence = SEQUENCE.SELECT_TARGET;

        CommandUnit.gameObject.SetActive(false);
        ChoicePanel.SetActive(false);

        showTargetPanel(true, power.isTargettAll);
    }

    /// <summary>
    /// スキル・アイテム選択キャンセル
    /// </summary>
    public void choiceCansel() {
        ChoicePanel.SetActive(false);
        readyCommand();
    }

    /// <summary>
    /// 演出処理
    /// </summary>
    private IEnumerator battleAction() {

        Sequence = SEQUENCE.ACTION;

        //既に対象ユニットのターンは終わってるので
        CharaPlateMng chara = getUnitPlate(Log.Atk);

        Vector3 posi = changeScreenPosi(chara.EffectPoint.position);

        if (Log.IsBurst || Log.IsStan) {
            var txt = "";
            if (SaveMng.IsJp) {
                txt = Log.IsBurst ? "は朦朧としている！" : "は痺れて動けない！";
            } else {
                txt = Log.IsBurst ? " is in a daze!" : " is numb and can't move!";
            }
            txt = chara.Unit.Name + txt;
            ShowMessage(txt);

            if (Log.IsStan) {
                MainEffects[ParalyzeEffect].effect(posi);
            }
            
            yield return new WaitForSeconds(0.6f);

        } else {
            if (Log.Action == AiProc.ACTION.GUARD) {
                chara.guard();
            } else {
                chara.action();

                if (Log.Skill != null || Log.Item != null) {
                    MessageText.text = Log.Skill != null ? Log.Skill.Name : Log.Item.Name;
                    MessageBoard.SetActive(true);
                }

                for (int i = 0; i < Log.Def.Count; i++) {
                    yield return new WaitForSeconds(0.2f);
                    hit(i);
                }

                yield return new WaitForSeconds(0.4f);
                chara.actionEnd();
            }
        }

        if (Log.removeBuff) {
            chara.showBreak(chara.Unit.IsCrash);
            buffIconUpdate(chara);
        }

        chara.setData();

        if (slipHpReaction(chara)) {
            yield return new WaitForSeconds(0.4f);
        }

        yield return new WaitForSeconds(0.6f);
        MessageBoard.SetActive(false);

        ActionSortGroup.RemoveUnit(chara.Unit.Id);
        ActionSortGroup.UpdateGroup(BattleProc.Battlers);

        nextTurn();
    }

    /// <summary>
    /// ターン更新判定・実行
    /// </summary>
    private void nextTurn() {
        if (BattleProc.isTurnStart()) {
            Sequence = SEQUENCE.TURN_START;
        } else {
            Sequence = SEQUENCE.UNIT_READY;
        }
    }

    /// <summary>
    /// リジェネ・毒処理
    /// </summary>
    /// <param name="chara"></param>
    private bool slipHpReaction(CharaPlateMng chara) {
        var s = Log.SlipHp;
        if (s != 0) {
            Vector3 posi = changeScreenPosi(chara.EffectPoint.position);
            if (s < 0) {
                MainEffects[PoisonEffect].effect(posi);
                damageReaction(chara, -s, posi);
            } else {
                healReaction(chara, s);
            }
            return true;
        }
        return false;
    }

    /// <summary>
    /// ターゲット選択
    /// </summary>
    /// <param name="party"></param>
    /// <param name="plate"></param>
    private void changeTarget(CharaPlateMng plate) {

        if ( SelectSide != SELECT_SIDE.NON || Sequence == SEQUENCE.SELECT_TARGET ) {
            if (CommandData.Power == null || !CommandData.Power.isTargettAll) {
                SelectSideMembers.ForEach(it => it.Target.SetActive(false));
                plate.Target.SetActive(true);
                SelectedUnit = plate;
            }
            
        }
    }

    /// <summary>
    /// ヒット/スキル・アイテム効果処理
    /// </summary>
    /// <param name="i"></param>
    /// <param name="delay"></param>
    /// <returns></returns>
    public void hit(int i) {

        UnitStatusTran def = Log.Def[i];

        CharaPlateMng tgt = getUnitPlate(def);

        Vector3 posi = changeScreenPosi(tgt.EffectPoint.position);

        attackEffectSet(posi);

        StartCoroutine( buffIconEffectSet(tgt) );

        if (Log.Atk.Type == def.Type) {
            //ターゲットが自陣営
            switch (Log.Action) {
                case AiProc.ACTION.ITEM:
                case AiProc.ACTION.SKILL:
                    healReaction(tgt, Log.Values[i]);
                break;
            }
        } else {
            //ターゲットが相手陣営
                if (i < Log.IsHit.Count && Log.IsHit[i]) {

                    showDamageInfo(tgt, Log.ElementAdj[i], Log.IsCritical[i] );

                    damageReaction(tgt, Log.Values[i], posi);

                    tgt.showBreak(tgt.Unit.IsCrash);

                } else {
                    tgt.miss();
                }
        }
    }

    /// <summary>
    /// ダメージ演出
    /// </summary>
    /// <param name="ch"></param>
    /// <param name="damage"></param>
    void damageReaction(CharaPlateMng ch, int damage, Vector3 posi) {

        ch.damage(damage);
        ch.setData();

        if (ch.Unit.Hp <= 0) {
            StartCoroutine(waitToAction(() => { DestroyEffect.effect(posi); ch.gameObject.SetActive(false); }, 1f));
        }
    }

    /// <summary>
    /// 回復演出
    /// </summary>
    /// <param name="ch"></param>
    /// <param name="val"></param>
    void healReaction(CharaPlateMng ch, int val) {
        ch.heal(val);
        ch.setData();
    }

    void showDamageInfo(CharaPlateMng plate, float element, bool crit) {
        if (element == BattleProc.ELEMENT_SUB_CULC) {
            //耐性補正
            plate.showRegist(true);
            TimeInvokeMng.TimerAction(()=> { plate.showRegist(false); }, 1f, this.gameObject);
        }
        if (element == BattleProc.ELEMENT_ADD_CULC) {
            //弱点補正
            plate.showWeak(true);
            TimeInvokeMng.TimerAction(() => { plate.showWeak(false); }, 1f, this.gameObject);
        }
        if (crit) {
            plate.showCritical(crit);
            TimeInvokeMng.TimerAction(() => { plate.showCritical(false); }, 1f, this.gameObject);
        }
    }

    /// <summary>
    /// バフアイコン更新
    /// </summary>
    /// <param name="plate"></param>
    void buffIconUpdate(CharaPlateMng plate) {

        UtilToolLib.AllObjectActive(plate.BuffIcons, false);

        foreach (var buf in plate.Unit.Buff) {
            if (buf.Turn > 0 && plate.BuffIcons[(int)buf.Type] != null) {
                plate.BuffIcons[(int)buf.Type].SetActive(true);
                if (buf.Value > 0) {
                    plate.BuffIcons[(int)buf.Type].GetComponent<Image>().color = plate.EffectBuff.BuffColor;
                } else if(buf.Value < 0){
                    plate.BuffIcons[(int)buf.Type].GetComponent<Image>().color = plate.EffectBuff.DeBuffColor;
                }
            }
        }
    }

    /// <summary>
    /// エフェクト表示
    /// </summary>
    /// <param name="damage"></param>
    /// <param name="posi"></param>
    protected void attackEffectSet(Vector3 posi) {

        if (Log.Power != null && MainEffects.ContainsKey(Log.Power.Effect)) {
            MainEffects[Log.Power.Effect]?.effect(posi);

        } else {

            var weapon = Log.Atk.EquipWeapon;

            if (weapon == null || weapon.Mst == null || weapon.Mst.Effect == "") {
                //無装備
                MainEffects[Log.Atk.Mst.AtkEffect].effect(posi);
            } else {
                //武器ごとのエフェクト
                MainEffects[weapon.Mst.Effect].effect(posi);
            }
        }
    }

    /// <summary>
    /// バフ用のアイコンエフェクト
    /// </summary>
    /// <param name="tgt"></param>
    /// <returns></returns>
    protected IEnumerator buffIconEffectSet(CharaPlateMng tgt) {

        if (Log.Power != null) {
            if (Log.Power.BuffTypes != null && Log.Power.BuffTypes.Length > 0 && Log.Power.BuffTypes[0] != BuffTran.TYPE.NON) {
                foreach (var buffType in Log.Power.BuffTypes) {
                    tgt.EffectBuff.EffectStart(buffType, Log.Power.BuffPower);
                    yield return new WaitForSeconds(1f);
                }
                buffIconUpdate(tgt);
            }
        }
    }

    /// <summary>
    /// ディレイ関数(TimeInvokeMngに変更する予定）
    /// </summary>
    /// <param name="action"></param>
    /// <param name="delay"></param>
    /// <returns></returns>
    private IEnumerator waitToAction(System.Action action, float delay) {
        yield return new WaitForSeconds(delay);
        action();
    }

    /// <summary>
    /// メッセージ表示
    /// </summary>
    /// <param name="txt"></param>
    private void ShowMessage(string txt) {
        MessageText.text = txt;
        MessageBoard.SetActive(true);
        waitToAction(() => { MessageBoard.SetActive(false); }, 1f);
    }

    ///// <summary>
    ///// プロセス進行
    ///// </summary>
    ///// <returns></returns>
    //private IEnumerator nextProcess(float delay, SEQUENCE next) {
    //    yield return new WaitForSeconds(delay);
    //    Sequence = next;
    //}

    //private void healPlayerAll() {
    //    if (OrderHealEffectNum > 0) {
    //        for (int i = 0; i < PlayerParty.Members.Length; i++) {
    //            StartCoroutine(healPlayer(i, OrderHealEffectNum));
    //        }
    //        OrderHealEffectNum = 0;
    //    }
    //}

    //private IEnumerator healPlayer(int index, int num) {

    //    yield return new WaitForSeconds(0.1f);

    //    if (PlayerParty.Members[index].isActiveAndEnabled) {
    //        //Vector3 posi = EffectCamera.WorldToScreenPoint(PlayerParty.Members[index].EffectPoint.position);
    //        //posi = EffectCamera.ScreenToWorldPoint(posi);
    //        Vector3 posi = changeScreenPosi(PlayerParty.Members[index].EffectPoint.position);
    //        HealEffect.effect(posi, new Quaternion(), -1, OrderHealEffectNum.ToString());
    //    }
    //}

    /// <summary>
    /// ボス撃破時の連続爆発
    /// </summary>
    /// <param name="base_posi"></param>
    /// <param name="size"></param>
    /// <returns></returns>
    private IEnumerator explodeRush(Vector3 base_posi, Vector2 size) {

        for (int i = 0; i < 6; i++) {
            yield return new WaitForSeconds(0.5f);

            Vector3 posi = new Vector3(Random.Range(-size.x, size.x), Random.Range(-size.y, size.y));
            posi += base_posi;

            //MainAtkEffs[(int)MAIN_ATTACK.EXPLODE].effect(posi, new Quaternion(), -1, "");
        }
    }

    /// <summary>
    /// カメラ座標変換
    /// </summary>
    /// <param name="effect_point"></param>
    /// <returns></returns>
    public Vector3 changeScreenPosi(Vector3 effect_point) {
        return UtilToolLib.changeScreenPosi(EffectCamera, effect_point);
    }


    public void pushAttack() {
        SelectSide = SELECT_SIDE.ENEMY;
        Sequence = SEQUENCE.SELECT_TARGET;
        CommandData.Action = AiProc.ACTION.ATTACK;
        pushCommand();
        showTargetPanel(true, false);
    }

    public void pushGuard() {
        CommandData.Action = AiProc.ACTION.GUARD;
        pushCommand();
        CommandProcess();
    }

    public void pushItem() {
        CommandData.Action = AiProc.ACTION.ITEM;
        CommandPanel.SetActive(false);
        Sequence = SEQUENCE.CHOICE;
    }

    public void pushSkill() {
        CommandData.Action = AiProc.ACTION.SKILL;
        CommandPanel.SetActive(false);
        Sequence = SEQUENCE.CHOICE;
    }

    private void pushCommand() {
        CommandPanel.SetActive(false);
        CommandUnit.gameObject.SetActive(false);
    }

    /// <summary>
    /// ターゲットパネル表示
    /// </summary>
    /// <param name="is_all"></param>
    private void showTargetPanel(bool is_show, bool is_all = true) {

        if (is_show) {
            if (is_all) {
                foreach (var plate in SelectSideMembers) {
                    plate.Target.SetActive(true);
                }
            } else {
                var before_unit = SelectSide == SELECT_SIDE.ENEMY ? BeforeTargetEnemy : BeforeTargetPlayer;
                var member = SelectSideMembers; //SelectSide == SELECT_SIDE.ENEMY ? EnemyParty.Members : PlayerParty.Members;

                if ((before_unit == null || !before_unit.gameObject.activeSelf) && member.Count > 0) {
                    int index = member.FindIndex(it => it.gameObject.activeSelf);
                    changeTarget(member[index]);
                } else {
                    changeTarget(before_unit);
                }
            }
        } else {
            PlayerParty.Members.ForEach(it => it.Target.SetActive(false));
            EnemyParty.Members.ForEach(it => it.Target.SetActive(false));
        }

        TargettPanel.SetActive(is_show);
    }

    /// <summary>
    /// ターゲット選択状態から戻る
    /// </summary>
    public void closeTargetSelect() {
        showTargetPanel(false);
        //前の状態がスキルかアイテム選択なら選択シーケンスに戻す
        if (CommandData.Action == AiProc.ACTION.SKILL || CommandData.Action == AiProc.ACTION.ITEM) {
            CommandUnit.gameObject.SetActive(true);
            Sequence = SEQUENCE.CHOICE;
        } else {
            Sequence = SEQUENCE.COMMAND;
        }
        
    }

    /// <summary>
    /// パーティユニット選択
    /// </summary>
    /// <param name="plate"></param>
    private void selectPartyUnit(CharaPlateMng plate) {
        BeforeTargetPlayer = plate;
        selectUnit(plate);
    }

    /// <summary>
    /// 敵ユニット選択
    /// </summary>
    /// <param name="plate"></param>
    private void selectEnemy(CharaPlateMng plate) {
        BeforeTargetEnemy = plate;
        selectUnit(plate);
    }

    private void selectUnit(CharaPlateMng plate) {
        SelectedUnit = plate;
        if (CommandData.Power != null && CommandData.Power.isTargettAll) {
            CommandData.AddDef( SelectSide == SELECT_SIDE.PARTY ? PlayerParty.Units : EnemyParty.Units);
        } else {
            CommandData.AddDef(plate.Unit);
        }

        CommandProcess();
    }

    /// <summary>
    /// ゲームパッド用ユニット選択
    /// </summary>
    //public void selectUnitGamePad() {

    //    if (SelectSide == SELECT_SIDE.PARTY) {


    //        selectPartyUnit(SelectedUnit);
    //    } else {
    //        selectEnemy(SelectedUnit);
    //    }
    //}

    /// <summary>
    /// マウスエンターイベント/プレイヤーキャラ
    /// </summary>
    /// <param name="plate"></param>
    private void enterPointerPartyUnit(CharaPlateMng plate) {
        enterPointerCommon(SELECT_SIDE.PARTY, plate);
    }

    /// <summary>
    /// マウスエンターイベント/エネミー
    /// </summary>
    /// <param name="plate"></param>
    private void enterPointerEnemy(CharaPlateMng plate) {
        enterPointerCommon(SELECT_SIDE.ENEMY, plate);
    }

    /// <summary>
    /// マウスエンターイベント共通
    /// </summary>
    /// <param name="side"></param>
    /// <param name="party"></param>
    /// <param name="plate"></param>
    private void enterPointerCommon(SELECT_SIDE side, CharaPlateMng plate) {
        if (side == SelectSide && Sequence == SEQUENCE.SELECT_TARGET) {
            if (SelectedUnit != plate) {
                changeTarget(plate);
            }
        }
    }

    /// <summary>
    /// パッド・キーボード用対象選択
    /// </summary>
    public void ChangeNextTarget() {

        var members = SelectSideMembers;//SelectSide == SELECT_SIDE.PARTY ? PlayerParty.Members : EnemyParty.Members;

        if (members.Count < 2) {
            return;
        }

        //var index = SelectSide == SELECT_SIDE.PARTY ? members.IndexOf(SelectedUnit) : EnemyParty.Layout.GetIndex(SelectedUnit.transform);
        var index = members.IndexOf(SelectedUnit);

        index = Mathf.Clamp(index += GamePadButtonMng.AxisHorizontal, 0, members.Count - 1);

        if (SelectSide == SELECT_SIDE.ENEMY) {
            for (var i = 0; i < members.Count; i++) {
                if (members[index] != null && members[index].gameObject.activeSelf) {
                    break;
                } else {
                    index = Mathf.Clamp(index += GamePadButtonMng.AxisHorizontal, 0, members.Count - 1);
                    index = EnemyParty.Layout.GetIndex(index);
                }
            }
        }


        //if (SelectSide == SELECT_SIDE.ENEMY) {
        //    if (members[index] == null || !members[index].gameObject.activeSelf) {
        //        index = Mathf.Clamp(index += GamePadButtonMng.AxisHorizontal, 0, members.Count - 1);
        //        if (members[index] == null || !members[index].gameObject.activeSelf) {
        //            index = Mathf.Clamp(index += GamePadButtonMng.AxisHorizontal, 0, members.Count - 1);

        //        }
        //    }
        //}

        

        //if (SelectSide == SELECT_SIDE.ENEMY) {
        //    index = EnemyParty.Layout.GetIndex(index);
        //}

        changeTarget(members[index]);


        //if (SelectSide == SELECT_SIDE.PARTY) {

        //    if (PlayerParty.Members.Count < 2) {
        //        return;
        //    }

        //    var index = PlayerParty.Members.IndexOf(SelectedUnit);
        //    index = Mathf.Clamp(index += GamePadButtonMng.AxisHorizontal, 0, PlayerParty.Members.Count - 1);

        //    changeTarget(PlayerParty.Members[index]);

        //} else {
        //    //敵パーティの場合は選択対象の並び替え
        //    if (EnemyParty.Members.Count == 0) {
        //        return;
        //    }

        //    var index = EnemyParty.Members.IndexOf(SelectedUnit);

        //    //SiblingIndexの関係上、Membersの並びは画面上の見た目と一致しないのでシンプルにindexをAxisHorizontalで加算（減算）しない
        //    IEnumerable<CharaPlateMng> selection = Enumerable.Empty<CharaPlateMng>();

        //    if (GamePadButtonMng.AxisHorizontal > 0) {
        //        selection = EnemyParty.Members.Where(it => SelectedUnit.transform.localPosition.x < it.transform.localPosition.x);
        //    } else {
        //        selection = EnemyParty.Members.Where(it => SelectedUnit.transform.localPosition.x > it.transform.localPosition.x);
        //    }


        //    if (selection != null && selection.Count() > 0) {
        //        var units = selection.OrderBy(it => it.transform.localPosition.x);
        //        var unit = GamePadButtonMng.AxisHorizontal > 0 ? units.First() : units.Last();
        //        changeTarget(unit);
        //    }
        //}

    }

    private void CommandProcess() {

        showTargetPanel(false);

        Sequence = SEQUENCE.PROCESS;
    }

    /// <summary>
    /// バトル終了後の報酬プロセス
    /// </summary>
    protected void BattleEndProcess() {
        if (BattleResult == BattleProc.RESULT.WIN) {

            ItemProcess.addItem(SaveMng.Quest.CarryBag, BattleProc.Reward.GetItems);

            Quest.GetMoney += BattleProc.Reward.GetMoney;
            Quest.GetExp += BattleProc.Reward.GetExp;
            
            //Expはバトル完了時点で付与する（アイテムとお金は帰還時）
            UnitProcess.addExp(BattleProc.Reward.GetExp);

            BattleProc.Reward = new BattleProc.RewardData(); //念のためリセット


        } else {
            BattleProc.Reward = new BattleProc.RewardData(); //念のためリセット
            VersatileProcess.nextDay();
        }
        
    }

    abstract protected IEnumerator ResultProcess();

    /// <summary>
    /// バトル終了
    /// </summary>
    public void endToBattle() {
        BattleEndProcess();
        if(Quest.Stage.Kind == StageMast.KIND.BOSS){
            if (Quest.Stage.Feature.Contains(StageMast.FEATURE.LAST_STAGE)){
                BaseStorySceneMng.Order(this, Quest.Stage.GetAfterStory(), CmnConst.SCENE.EndingScene);
            }else{
                BaseStorySceneMng.Order(this, Quest.Stage.GetAfterStory(), CmnConst.SCENE.HomeScene);
            }
        }
        else
        {
            SceneBase.SetActive(false);
            StageFieldSceneMng.Singleton.SceneBase.SetActive(true);
        }
    }

    /// <summary>
    /// リザルト：ユニット経験値セット
    /// </summary>
    protected void setUnitExp() {
        for (int i = ExpGauges.transform.parent.childCount; i < Quest.ActiveParty.Count; i++) {
            GameObject obj = Instantiate(ExpGauges.gameObject) as GameObject;
            obj.transform.SetParent(ExpGauges.transform.parent);
            obj.transform.localScale = ExpGauges.transform.localScale;
        }

        var gauges = ExpGauges.transform.parent.GetComponentsInChildren<CharaImgGaugeMng>();
        for (int i = 0; i < gauges.Length; i++) {
            if (i < Quest.ActiveParty.Count) {
                var unit = Quest.ActiveParty[i];
                gauges[i].Init(unit.Img, unit.getNextLvupExp(), unit.Exp);
                gauges[i].gameObject.SetActive(true);
            } else {
                gauges[i].gameObject.SetActive(false);
            }
        }
    }

    /// <summary>
    /// 最新のバトルは勝利したか
    /// </summary>
    /// <returns></returns>
    public static bool IsWin() {
        return (Singleton != null && Singleton.BattleResult == BattleProc.RESULT.WIN);
    }
}
