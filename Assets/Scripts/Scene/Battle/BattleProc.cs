using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class BattleProc
{

    //public const int BATTLE_TURN_NUM = 100;

    /// <summary>
    /// シンプルに炎→土→水→炎の３すくみの場合
    /// </summary>
    public const bool USE_ELEMENT = false;

    /// <summary>
    /// 各ユニットが個別に弱点を持つ場合
    /// </summary>
    public const bool USE_WEAK_SYSTEM = true;

    public const float ELEMENT_ADD_CULC = 1.5f;

    public const float ELEMENT_SUB_CULC = 0.75f;

    /// <summary>
    /// クラッシュ時ダメージ補正
    /// </summary>
    public const int DAMAGE_CRASH_ADJ = 2;

    /// <summary>
    /// 基本クラッシュポイントの消費量・回復量（ダメージ・通常ターン経過）
    /// </summary>
    public const int NORMAL_CRASH_ADD_SUB = 1;

    /// <summary>
    /// 特殊クラッシュポイントの消費量・回復量（弱点ダメージ・ガード時）
    /// </summary>
    public const int SPECIAL_CRASH_ADD_SUB = 5;

    /// <summary>
    /// コマンドデータ
    /// </summary>
    public class ActionData
    {
        public AiProc.ACTION Action;
        public UnitStatusTran Atk;
        public List<UnitStatusTran> Def = new List<UnitStatusTran>();
        public List<int> Dmg = new List<int>();
        public List<bool> IsHit = new List<bool>();
        public SkillMast Skill;
        public ItemTran Item;
        public int SlipHp = 0;
        public bool IsStan = false;
        public bool IsBurst = false;
        public bool addBuff = false;
        public bool removeBuff = false;
    }

    public class RewardData
    {
        public int GetExp;
        public int GetMoney;
        public int RuneBonusExp; //ルーンボーナス増加分
        public int RuneBonusMoney; //ルーンボーナス増加分
        public List<ItemTran> GetItems = new List<ItemTran>();
    }

    public enum SPEED_INDEX
    {
        PLAYER_OR_ENEMY,
        PARTY_INDEX,
        AGI,
        LUCK,
        SPEED_FACTER,
        ALL
    }

    public enum TARGET
    {
        BASIC_INFIGHT, //random one, and back target half percent
        RANDOM_ONE_ALL,
        RANDOM_ONE_FRONT,
        FRONT_ALL,
        TARGET_ALL,
    }

    public enum RESULT
    {
        WIN,
        LOSE,
        CONTINUE,
        DRAW,
    }

    public static readonly int[][] TARGET_PERCENT = new int[][]{
        new int[]{ 100 },
        new int[]{ 60,  40 },
        new int[]{ 45,  35, 20 },
        new int[]{ 35,  30, 20, 15  },
        new int[]{ 30,  25, 20, 15, 10 },
        new int[]{ 25,  22, 18, 15, 12, 8 },
        new int[]{ 22,  20, 18, 15, 12, 8,  5 },
        new int[]{ 20,  18, 16, 14, 11, 9,  7,  5 },
    };

    private static AiProc PlayerAi;
    private static AiProc EnemyAi;

    public static ActionData BattleAction;

    public static RewardData Reward;

    public static UnitStatusTran ActionUnit {
        get {
            return BattleData.ActionRound < Battlers.Count ? Battlers[BattleData.ActionRound] : null;
        }
    }

    private static QuestTran Quest {
        get {
            return SaveMng.Quest;
        }
    }

    public static List<UnitStatusTran> Battlers {
        get {
            return Quest.Battlers;
        }
        private set {
            Quest.Battlers = value;
        }
    }
    public static QuestTran.BattleTran BattleData {
        get {
            return Quest.Battle;
        }
        private set {
            Quest.Battle = value;
        }
    }

    public static bool IsBattle() {
        if (Quest != null && Quest.Enemys != null && Quest.Enemys.Count > 0) {
            return true;
        }
        return false;
    }

    public static void Init() {
        BattleData = new QuestTran.BattleTran();
        Reward = new RewardData();
        Quest.ActiveParty.ForEach(it => it.CrashPower = it.Mst.MaxCrash);
        Quest.Enemys.ForEach(it => it.CrashPower = it.Mst.MaxCrash);
    }

    private static void turnStart() {

        if (Battlers == null || Battlers.Count == 0) {
            //初回と戦闘中の状態を保存する場合のみ発生する（放置RPGなど）
            Battlers = new List<UnitStatusTran>();
            Battlers.AddRange(Quest.ActiveParty);
            Battlers.AddRange(Quest.Enemys);
        }

        aiSet(true);

        foreach (UnitStatusTran val in Battlers) {
            int luck_add = val.Status.Luk / 2;
            val.speedDice = Random.Range(0, luck_add);
        }

        Battlers.Sort((a, b) => b.speedFacter - a.speedFacter);

        //Debug.Log("Sort");
        //var test = Battlers.Select(it => it.Name);
        //Debug.Log(string.Join(",", test).ToString());

    }

    private static void turnEnd() {

        BattleData.Turn++;
        BattleData.ActionRound = 0;

    }

    private static RESULT resultProcess() {
        RESULT result = judgeResult();
        if (result != RESULT.CONTINUE) {
            for (int i = 0; i < Battlers.Count; i++) {
                Battlers[i].endBattle();
            }
            Battlers = null;
        }
        return result;
    }


    private static void aiSet(bool reset = false) {
        if (PlayerAi == null || reset) {
            PlayerAi = new AiProc(Quest.ActiveParty, Quest.Enemys);
            EnemyAi = new AiProc(Quest.Enemys, Quest.ActiveParty);
        }
    }

    /// <summary>
    /// 戦闘準備
    /// </summary>
    public static void ready() {

        if (BattleData == null) {
            //基本的にここには来ない
            BattleData = new QuestTran.BattleTran();
        }

        if (isTurnStart()) {
            //初回と戦闘中の状態を保存する場合のみ発生する（放置RPGなど）
            turnStart();
        }

        //BattleAction = new ActionData();
        //BattleAction.Atk = ActionUnit;
    }

    /// <summary>
    /// バトル処理
    /// </summary>
    /// <returns></returns>
	public static RESULT battle(ActionData command = null) {

        aiSet(); //バトル中断してから再開した場合のため、AI取得し直し

        //ターン開始、バトルアクションレコード初期化
        if (command == null) {

            aiFightAction();
        } else {
            BattleAction = command;
        }

        if (judgeSkipTurn(ActionUnit)) {
            //バーストかスタンでターンスキップ
            return endProcess();
        }

        bool to_action = fightAction();

        if (!to_action) {
            return RESULT.CONTINUE;
        }

        //確定したアクションをユニットごとのアクションデータに記録する
        ActionUnit.ActionPlan = BattleAction.Action;

        slipDamage(ActionUnit);

        return endProcess();

    }

    public static RESULT endProcess() {

        if (ActionUnit.IsGuard) {
            //ガード中はクラッシュゲージ５回復
            ActionUnit.addBurst(SPECIAL_CRASH_ADD_SUB);
        } else {
            ActionUnit.addBurst(NORMAL_CRASH_ADD_SUB);
        }

        BattleAction.removeBuff = ActionUnit.battleTurn();

        BattleData.ActionRound++;

        if (BattleData.ActionRound >= Battlers.Count) {
            turnEnd();
        }

        return resultProcess();
    }

    private static RESULT judgeResult() {

        bool p_live = false;
        bool e_live = false;

        foreach (UnitStatusTran val in Quest.ActiveParty) {
            if (val.Status.Hp > 0) {
                p_live = true;
                break;
            }
        }

        foreach (UnitStatusTran val in Quest.Enemys) {
            if (val.Status.Hp > 0) {
                e_live = true;
                break;
            }
        }

        if (p_live) {
            if (e_live) {
                return RESULT.CONTINUE;
            } else {
                return RESULT.WIN;
            }
        } else {
            if (e_live) {
                return RESULT.LOSE;
            } else {
                return RESULT.DRAW;
            }
        }
    }

    /// <summary>
    /// 新ターンか
    /// </summary>
    /// <returns></returns>
    public static bool isTurnStart() {
        return BattleData.ActionRound == 0;
    }

    //private static void fightAll( ){

    //	for( int i = 0; i < Battlers.Count; i++ ){
    //           fightAction(i);
    //       }
    //}

    /// <summary>
    /// ユニット一体あたりの行動(AI)
    /// </summary>
    /// <param name="i"></param>
    public static bool aiFightAction() {

        BattleAction = new ActionData();
        BattleAction.Atk = ActionUnit;

        AiProc ai = null;

        if (ActionUnit.Type == UnitMast.TYPE.PLAYER) {
            ai = PlayerAi;
        } else if (ActionUnit.Type == UnitMast.TYPE.ENEMY) {
            ai = EnemyAi;
        }

        ai.think(ActionUnit);

        BattleAction.Action = ai.JudgeAction;
        BattleAction.Def = ai.ActionTargets;
        BattleAction.Skill = ai.UseSkill;


        //if(ai.SkillTargets!=null && ai.SkillTargets.Count > 0) {
        //    BattleAction.Def = ai.SkillTargets;
        //}

        return true;
    }

    /// <summary>
    /// マニュアル入力の行動
    /// </summary>
    /// <param name="command"></param>
    /// <returns></returns>
    public static bool fightAction() {

        switch (BattleAction.Action) {
            case AiProc.ACTION.ATTACK:
            if (BattleAction.Def.Count == 0) {
                weaponAttack(BattleAction.Atk);
            } else {
                weaponAttack(BattleAction.Atk, BattleAction.Def);
            }
            break;
            case AiProc.ACTION.SKILL:
            useSkill(BattleAction.Skill, BattleAction.Atk, BattleAction.Def);
            break;
            case AiProc.ACTION.GUARD:
            //ActionPlanでgurdを指定された時点でガード状態になるのでここでは何もしない
            break;
            case AiProc.ACTION.ITEM:
            useItem(BattleAction.Item, BattleAction.Atk, BattleAction.Def);
            break;
        }

        return true;
    }

    /// <summary>
    /// ターンスキップ判定
    /// （１回休み）
    /// </summary>
    /// <param name="unit"></param>
    /// <returns></returns>
    private static bool judgeSkipTurn(UnitStatusTran unit) {
        if (unit.IsCrash) {
            BattleAction.IsBurst = true;
            return true;
        }
        if (unit.isBuff(BuffTran.TYPE.STAN)) {
            var rand = Random.Range(0, 100);
            var buf = unit.getBuff(BuffTran.TYPE.STAN);
            if (rand < buf.Value) {
                BattleAction.IsStan = true;
                return true;
            }
        }
        return false;
    }

    private static void slipDamage(UnitStatusTran unit) {
        if (unit.isBuff(BuffTran.TYPE.HP_SLIP)) {
            var bf = unit.getBuff(BuffTran.TYPE.HP_SLIP);
            unit.damage(-(int)bf.Value);
            BattleAction.SlipHp = (int)bf.Value;
        }
    }

    /// <summary>
    /// 直接攻撃（対象ランダム）
    /// </summary>
    /// <param name="val"></param>
	private static void weaponAttack(UnitStatusTran val) {

        //int target_index;
        List<UnitStatusTran> targets;

        if (val.Status.Hp > 0) {

            if (val.Type == UnitMast.TYPE.PLAYER) {
                targets = Quest.Enemys;//.Cast<UnitStatusTran>().ToList();
            } else if (val.Type == UnitMast.TYPE.ENEMY) {
                targets = Quest.ActiveParty;
            } else {
                targets = new List<UnitStatusTran>();
            }

            int target_index = getTarget(val, targets.Count);
            if (target_index < 0) {
                return;
            }
            BattleAction.Def.Add(targets[target_index]);
            weaponAttack(val, targets[target_index]);

        }
    }

    /// <summary>
    /// 武器攻撃
    /// </summary>
    /// <param name="val"></param>
    /// <param name="targets"></param>
    private static void weaponAttack(UnitStatusTran val, List<UnitStatusTran> targets) {
        foreach (var tar in targets) {
            weaponAttack(val, tar);
        }
    }

    /// <summary>
    /// 武器攻撃
    /// </summary>
    /// <param name="val"></param>
    /// <param name="target"></param>
    private static void weaponAttack(UnitStatusTran val, UnitStatusTran target) {
        bool is_hit = false;
        int damage_val = physicalDamage(val, target, ref is_hit);

        BattleAction.Dmg.Add(damage_val);
        BattleAction.IsHit.Add(is_hit);

        isDead(target, val);
    }

    private static void useSkill(SkillMast skill, UnitStatusTran val, List<UnitStatusTran> targets) {

        if (val.Status.Hp > 0 && skill != null) {

            switch (skill.Spec) {
                case SkillMast.SPEC.ATTACK:
                skillAttack(skill, val, targets);
                break;
                default:
                PowerProcess.execPower(skill, val, targets);

                break;

            }
            val.Status.Mp -= skill.Cost;
        }

    }

    /// <summary>
    /// アイテム使用
    /// </summary>
    /// <param name="item"></param>
    /// <param name="val"></param>
    /// <param name="target"></param>
    private static void useItem(ItemTran item, UnitStatusTran val, List<UnitStatusTran> target) {
        if (val.Status.Hp > 0 && item != null) {

            var mst = item.Mst;

            switch (mst.Spec) {
                case SkillMast.SPEC.ATTACK:
                break;
                case SkillMast.SPEC.HEAL:
                break;
                case SkillMast.SPEC.STATUS_BUFF:
                break;

            }
            SaveMng.ItemData.lostItem(item, -1);
        }
    }

    /// <summary>
    /// AIスキル攻撃
    /// </summary>
    /// <param name="skill"></param>
    /// <param name="val"></param>
    /// <param name="targets"></param>
    //private static void aiSkillAttack(SkillMast skill, UnitStatusTran val, List<UnitStatusTran> targets) {

    //    if (skill.Target == SkillMast.TARGET.ANYTHING) {
    //        skillAttack(skill, val, targets);
    //    } else {
    //        int index = Random.Range(0, targets.Count);
    //        skillAttack(skill, val, targets[index]);
    //    }
    //}

    /// <summary>
    /// スキル攻撃・単体/複数
    /// </summary>
    /// <param name="skill"></param>
    /// <param name="val"></param>
    /// <param name="targets"></param>
    private static void skillAttack(SkillMast skill, UnitStatusTran val, List<UnitStatusTran> targets) {

        magicDamages(skill, val, ref targets);
        int dead_count = 0;
        for (int i = targets.Count - 1; i >= 0; i--) {
            if (targets[i] != null && isDead(targets[i], val)) {
                dead_count++;
            }
        }
    }

    ///// <summary>
    ///// スキル攻撃・単体
    ///// </summary>
    ///// <param name="skill"></param>
    ///// <param name="val"></param>
    ///// <param name="target"></param>
    ///// <returns></returns>
    //private static bool skillAttack(SkillMast skill, UnitStatusTran val, UnitStatusTran target) {
    //    magicDamage(skill, val, target);
    //    return isDead(target, val);
    //}

    /// <summary>
    /// 通常攻撃
    /// </summary>
    /// <param name="attacker"></param>
    /// <param name="defender"></param>
    /// <param name="is_hit"></param>
    /// <returns></returns>
    //んー、迷ったけど、変更しない引数についてはrefは使わん。混乱するし。小サイズの配列ならパフォーマスの影響大きいのは分かるけども
    private static int physicalDamage(UnitStatusTran attacker, UnitStatusTran defender, ref bool is_hit) {

        int hit = attacker.Status.Str + attacker.Status.luckJudge();
        int dodge = defender.Status.Agi / 4 + defender.Status.luckJudge();

        hit = calcBuffBonus(hit, attacker, BuffTran.TYPE.HIT);

        dodge = calcBuffBonus(dodge, defender, BuffTran.TYPE.SWAY);

        is_hit = hit >= dodge ? true : false;

        if (is_hit) {
            int power = hit + (int)attacker.EquipWeapon.Value;

            int guard = defender.TotalDefence / 2 + defender.Status.luckJudge();
            return damageCulc(power, guard, attacker, defender, GameConst.ELEMENT.Material, BuffTran.TYPE.ATK, BuffTran.TYPE.DEF);
        }

        return 0;
    }

    /// <summary>
    /// 魔法ダメージ
    /// </summary>
    /// <param name="skill"></param>
    /// <param name="attacker"></param>
    /// <param name="defender"></param>
    /// <returns></returns>
    private static int magicDamage(SkillMast skill, UnitStatusTran attacker, UnitStatusTran defender) {
        int rand_pow = (int)Random.Range(skill.PowMin, skill.PowMax);
        int base_pow = attacker.Status.getParam(skill.BaseParam) + attacker.Status.luckJudge();
        int power = base_pow + rand_pow + (int)attacker.EquipWeapon.SubValue;

        return powerDamage(skill, power, attacker, defender);

        //int guard = (int)(defender.Status.Men + defender.EquipArmor.SubValue) / 2 + defender.Status.luckJudge ();

        //      int damage = damageCulc(power, guard, attacker, defender, skill.Element, BuffTran.TYPE.MATK_BUFF, BuffTran.TYPE.MDEF_BUFF);

        //      if (damage > 0) {
        //          //ダメージ量により0～50%のバフボーナス
        //          var bonus = 50f;
        //          if (defender.IsCrash) {
        //              bonus = 100f;
        //          } else if (guard > 0) {
        //              bonus = Mathf.Clamp( bonus * ( (float)(power - guard) / guard), 0f, 50f);
        //          }
        //          addStatusEffect(skill, attacker, defender, true, (int)bonus);
        //      }

        //      BattleAction.Def.Add(defender);
        //      BattleAction.Dmg.Add(damage);
        //      BattleAction.IsHit.Add(true);
        //      BattleAction.Skill = skill;

        //      return damage;
    }

    /// <summary>
    /// 魔法・アイテムダメージ
    /// </summary>
    /// <param name="mst"></param>
    /// <param name="val"></param>
    /// <param name="attacker"></param>
    /// <param name="defender"></param>
    /// <returns></returns>
    private static int powerDamage(PowerMast mst, int val, UnitStatusTran attacker, UnitStatusTran defender) {

        int guard = (int)(defender.Status.Men + defender.EquipArmor.SubValue) / 2 + defender.Status.luckJudge();

        int damage = damageCulc(val, guard, attacker, defender, mst.Element, BuffTran.TYPE.MAG, BuffTran.TYPE.REG);

        if (damage > 0) {
            //ダメージ量により0～50%のバフボーナス
            var bonus = 50f;
            if (defender.IsCrash) {
                bonus = 100f;
            } else if (guard > 0) {
                bonus = Mathf.Clamp(bonus * ((float)(val - guard) / guard), 0f, 50f);
            }
            addStatusEffect(mst, attacker, defender, true, (int)bonus);
        }

        //BattleAction.Def.Add(defender);
        BattleAction.Dmg.Add(damage);
        BattleAction.IsHit.Add(true);
        //BattleAction.Skill = skill;

        return damage;
    }

    /// <summary>
    /// 魔法ダメージ・複数
    /// </summary>
    /// <param name="skill"></param>
    /// <param name="attacker"></param>
    /// <param name="defenders"></param>
    /// <returns></returns>
    private static float magicDamages(SkillMast skill, UnitStatusTran attacker, ref List<UnitStatusTran> defenders) {

        if (defenders.Count == 0) {
            return 0;
        }

        float damage = 0;

        for (int i = 0; i < defenders.Count; i++) {
            damage += (float)magicDamage(skill, attacker, defenders[i]);
        }
        damage /= (float)defenders.Count;
        float avg_damage = (float)System.Math.Round(damage, 2);
        return avg_damage;
    }

    /// <summary>
    /// 弱点・抵抗属性判定
    /// </summary>
    /// <param name="power"></param>
    /// <param name="ele"></param>
    /// <param name="def"></param>
    /// <returns></returns>
    private static int elementCulc(int power, GameConst.ELEMENT ele, UnitStatusTran def) {

        if (USE_ELEMENT) {

            int at_el = (int)ele;
            int def_el = (int)def.Element;

            //無属性でない、属性異なる場合
            if (at_el != 0 && def_el != 0 && at_el != def_el) {

                //攻撃属性の前の属性か、攻撃属性最初、かつ防御属性最後、か
                if (def_el == (at_el - 1) || (at_el == 1 && def_el == (int)GameConst.ELEMENT.All - 1)) {
                    power = (int)((float)power * ELEMENT_ADD_CULC);
                } else if (at_el == (def_el - 1) || (def_el == 1 && at_el == (int)GameConst.ELEMENT.All - 1)) {
                    power = (int)((float)power * ELEMENT_SUB_CULC);
                }
            }
        } else if (USE_WEAK_SYSTEM) {
            ///TODO defが防御中なら弱点無視
            if (def.Mst.Weak.Any(it => it == ele)) {
                power = (int)((float)power * ELEMENT_ADD_CULC);
            } else if (def.Mst.Regist.Any(it => it == ele)) {
                power = (int)((float)power * ELEMENT_SUB_CULC);
            }
        }

        return power;

    }


    /// <summary>
    /// バフ・デバフ付与
    /// </summary>
    /// <param name="pow"></param>
    /// <param name="val"></param>
    /// <param name="target"></param>
    /// <param name="is_debuff">デバフ（相手に損害与える）か？</param>
    /// <returns></returns>
    private static bool addStatusEffect(PowerMast pow, UnitStatusTran val, UnitStatusTran target, bool is_debuff, int bonus = 0) {

        if (is_debuff) {
            //デバフの場合、判定に成功するか、相手がバーストブレイク状態の場合、効果を発揮
            var rand = Random.Range(0, 100);
            if (rand >= pow.BuffPercent + bonus && !target.IsCrash) {
                return false;
            }
        }

        if (pow.BuffType != BuffTran.TYPE.NON) {
            if (target.Buff.Exists(it => it.Type == pow.BuffType)) {

                var bf = target.Buff.Find(it => it.Type == pow.BuffType);
                if (Mathf.Sign(bf.Value) != Mathf.Sign(pow.BuffPower)) {
                    //対抗の場合削除
                    target.Buff.Remove(bf);
                } else if (bf.Value <= pow.BuffPower) {
                    //既に同系統のバフ・デバフがかかっている場合、効果が上のものを適用
                    target.Buff.Add(new BuffTran(pow.BuffType, BuffTran.FIELD_TYPE.BATTLE, pow.EffectTime, pow.BuffPower));
                } else {
                    return false;
                }
            } else {
                target.Buff.Add(new BuffTran(pow.BuffType, BuffTran.FIELD_TYPE.BATTLE, pow.EffectTime, pow.BuffPower));
                BattleAction.addBuff = true;
            }
            return true;
        }

        return false;
    }

    private static bool skillHeal(SkillMast skill, UnitStatusTran val, List<UnitStatusTran> targets) {

        if (skill.Target == SkillMast.TARGET.ANYTHING) {
            for (int i = 0; i < targets.Count; i++) {
                int rand = (int)Random.Range(skill.PowMin, skill.PowMax);

            }
        } else {

        }

        return false;
    }

    private static bool isDead(UnitStatusTran val, UnitStatusTran atk, bool is_log = true) {
        if (val.Status.Hp <= 0) {
            if (val.Type == UnitMast.TYPE.PLAYER) {
                Quest.ActiveParty.Remove(val);
            } else if (val.Type == UnitMast.TYPE.ENEMY) {
                //var enemy = val.getEnemyMast();
                Quest.DestroyNum++;

                //経験値
                var exp = val.Exp;
                Reward.RuneBonusExp = (int)(exp * ((float)atk.GetRune(RuneMast.KIND.UP_EXP) / 100f));
                Reward.GetExp += exp + Reward.RuneBonusExp; //今回の敵のEXP

                //お金
                var money = val.Money;
                Reward.RuneBonusMoney += (int)(money * ((float)atk.GetRune(RuneMast.KIND.UP_MONEY) / 100f));
                Reward.GetMoney += money + Reward.RuneBonusMoney; //今回の敵のmoney

                //アイテム
                var drop_item = ItemTran.getRandomItem(val.Lv);
                if (drop_item != null) {
                    Reward.GetItems.Add(drop_item);
                }
                Quest.Enemys.Remove((UnitStatusTran)val);
                //LogProc.reportAddParam(ReportTran.PARAMS.DESTROY_NUMS);
            }
            //if( is_log ){
            //	LogProc.addLog( LogListTran.GROUP.ACTION_APPEND, LogListTran.LOG_TPL.BTL_DOWN, LogListTran.ICON.NON, new string[] { val.Name } );
            //}
            //if( val.Type == UnitMast.TYPE.PLAYER ){
            //	int floor_num = Quest.NowFloorNum + 1;
            //             LogProc.setTopic(ReportTran.TOPIC_KIND.DEAD_CHARA, new string[] { floor_num.ToString(), val.Name, term });
            //}

            return true;
        }
        return false;
    }

    /// <summary>
    /// ダメージ計算
    /// </summary>
    /// <param name="power"></param>
    /// <param name="defender"></param>
    /// <param name="BuffType"></param>
    /// <returns></returns>
    private static int damageCulc(int power, int guard, UnitStatusTran attacker, UnitStatusTran defender, GameConst.ELEMENT elemnt, BuffTran.TYPE atkBuff, BuffTran.TYPE defBuff) {

        power = calcBuffBonus(power, attacker, atkBuff);
        power = calcRuneBonus(power, attacker, RuneMast.KIND.P_ATK_ADD);
        power = calcRuneBonus(power, attacker, RuneMast.KIND.ATTACK_ALL); //全体攻撃の弱体化

        int no_element_power = power;
        power = elementCulc(power, elemnt, defender);

        if (power == no_element_power || defender.IsGuard) {
            //通常または防御側がガード中
            defender.addBurst(-NORMAL_CRASH_ADD_SUB);
        } else if (power > no_element_power) {
            //素値(no_element_power)が補正後の値より大きい（弱点）
            defender.addBurst(-SPECIAL_CRASH_ADD_SUB);
        }

        //if (power > no_element_power) {
        //    //素値(no_element_power)が補正後の値より大きい（弱点）
        //    if (defender.IsGuard) { //ガード中は通常
        //        defender.addBurst(-NORMAL_CRASH_ADD_SUB);
        //    } else {
        //        defender.addBurst(-SPECIAL_CRASH_ADD_SUB);
        //    }
        //} else if (power == no_element_power) {
        //    defender.addBurst(-NORMAL_CRASH_ADD_SUB);
        //}

        if (defender.CrashPower <= 0 && !defender.IsCrash) {
            defender.Buff.Add(new BuffTran(BuffTran.TYPE.CRASH_OUT, BuffTran.FIELD_TYPE.BATTLE, 1, 1f));
        }

        guard = calcBuffBonus(guard, defender, defBuff);
        guard = defender.IsCrash ? guard / 4 : guard; //クラッシュ時防御 1/4
        int damage = defender.damage(power, guard);

        damage = defender.IsGuard ? damage / 2 : damage; //ガード中は被ダメージ1/2

        if (defender.IsCrash) {
            damage *= DAMAGE_CRASH_ADJ;
        }
        return damage;
    }

    public static int directDamage(int power, UnitStatusTran defender) {
        int guard = defender.Status.luckJudge();
        int damage = defender.damage(power, guard);
        return damage;
    }

    public static int freeDamage(int power, UnitStatusTran defender) {
        int damage = defender.Status.damage(power, StatusMast.TYPE.HP);
        return damage;
    }

    public static void freeCrash(int num, UnitStatusTran defender) {
        defender.addBurst(num);
    }


    /// <summary>
    /// ボーナス値計算（ボーナスは100分率）
    /// </summary>
    /// <param name="param"></param>
    /// <param name="unit"></param>
    /// <param name="type"></param>
    /// <returns></returns>
    private static int calcBuffBonus(int param, UnitStatusTran unit, BuffTran.TYPE type) {
        if (unit.isBuff(type)) {
            var bf = unit.getBuff(type);
            param = param + (int)(param * (bf.Value / 100));
        }
        return param;
    }

    /// <summary>
    /// ルーンボーナス値計算（ボーナスは100分率）
    /// </summary>
    /// <param name="param"></param>
    /// <param name="unit"></param>
    /// <param name="type"></param>
    /// <returns></returns>
    private static int calcRuneBonus(int param, UnitStatusTran atk, RuneMast.KIND kind) {
        param = param + (int)(param * (atk.GetRune(kind) / 100));
        return param;
    }

    private static int getTarget(UnitStatusTran attacker, int unit_num) {
        int index = -1;
        int rate = unit_num - 1;
        if (rate < 0) {
            return index;
        }

        if (rate >= TARGET_PERCENT.Length) {
            rate = TARGET_PERCENT.Length - 1;
        }

        if (attacker.EquipWeapon.Range == ItemMast.RANGE.LONG) {
            index = Random.Range(0, unit_num);
        } else {
            index = UtilToolLib.getRateRandom(0, 100, TARGET_PERCENT[rate]);
        }

        return index;

    }

    private static List<int[]> getBaseSpeed() {

        List<int[]> base_speeds = new List<int[]>();

        for (int h = 0; h < 2; h++) {

            List<UnitStatusTran> list = new List<UnitStatusTran>();

            if (h == 0) {
                list = Quest.ActiveParty;
            } else {
                foreach (UnitStatusTran val in Quest.Enemys) {
                    list.Add(val);
                }
            }

            for (int i = 0; i < list.Count; i++) {
                int[] speed = new int[(int)SPEED_INDEX.ALL];
                speed[(int)SPEED_INDEX.PLAYER_OR_ENEMY] = h;
                speed[(int)SPEED_INDEX.PARTY_INDEX] = i;
                speed[(int)SPEED_INDEX.AGI] = list[i].Status.Agi;
                speed[(int)SPEED_INDEX.LUCK] = list[i].Status.Luk;
                speed[(int)SPEED_INDEX.SPEED_FACTER] = 0;
                base_speeds.Add(speed);
            }
        }

        return base_speeds;
    }

}
