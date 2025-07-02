using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;

[System.Serializable]
public class UnitStatusTran
{

    public const int MAX_ACCESSORY = 2;

    public enum EQUIP
    {
        WEAPON,
        ARMOR,
        ACCESSORY1,
        ACCESSORY2,
        ALL
    }

    public int Id;
    public int MasterId;

    public ItemTran[] Equips = new ItemTran[(int)EQUIP.ALL];

    public ItemTran EquipWeapon {get {return getEquip(EQUIP.WEAPON);}set {Equips[(int)EQUIP.WEAPON] = value;}}
    public ItemTran EquipArmor {get {return getEquip(EQUIP.ARMOR);}set {Equips[(int)EQUIP.ARMOR] = value;}}
    public ItemTran EquipAccessory1 {get {return getEquip(EQUIP.ACCESSORY1);}set {Equips[(int)EQUIP.ACCESSORY1] = value;}}
    public ItemTran EquipAccessory2 {get {return getEquip(EQUIP.ACCESSORY2);}set {Equips[(int)EQUIP.ACCESSORY2] = value;}}

    public int Lv;

    public string Name;
    public string Img { get { return Mst.Img; } }
    public UnitMast.JOB Job { get { return Mst.Job; } }
    public AiProc.TACTICS Tactics;
    public GameConst.ELEMENT Element { get { return Mst.Element; } }
    public GameConst.RARITY[] Potentials { get { return Mst.Potentials; } }
    public List<int> SkillIds = new List<int>();
    public UnitMast.TYPE Type;
    public int Exp;
    public int Money;

    public StatusMast BaseStatus;
    public StatusMast Status;
    
    public Dictionary<RuneMast.KIND, int> Runes = new Dictionary<RuneMast.KIND, int>();

    //public List<BuffTran> DeBuff = new List<BuffTran>();
    public bool SellLock = false;
    public int UseCount = 0;
    public int FriendShip = 0;
    //public int UseSecond = 0;

    #region　NonSerialized:一時パラメータ

    [System.NonSerialized]
    public UnitMast _Mst;

    [System.NonSerialized]
    public int speedDice;

    [System.NonSerialized]
    public SkillMast[] _Skills;

    /// <summary>
    /// 一時的効果・バフ（デバフを含む）
    /// </summary>
    [System.NonSerialized]
	public List<BuffTran> Buff = new List<BuffTran>();

    [System.NonSerialized]
    public int CrashPower = 10;

    //現在の戦闘行動
    [System.NonSerialized]
    public AiProc.ACTION ActionPlan = AiProc.ACTION.READY;

    #endregion

    public UnitMast Mst { get { return getMast(); } }

    public int LvNum { get { return Lv + 1; } }
    public SkillMast[] Skills { get { return _Skills == null ? updateSkill() : _Skills; } }
    public bool CanHeal { get; set; }
    public bool CanRessurect { get; set; }
    public bool CanCureStun { get; set; }
    public bool CanCurePoison { get; set; }
    public bool CanRemoveBuff { get; set; }
    public bool CanRemoveDebuff { get; set; }
    public int speedFacter { get { return speedDice + Status.Agi; } }
    public List<SkillMast.SPEC> CanSupport { get; set; }
    public int Hp { get { return Status.Hp; } }
    public int MaxHp { get { return Status.MaxHp; } }
    public int Mp { get { return Status.Mp; } }
    public int MaxMp { get { return Status.MaxMp; } }
    public int TotalAttack { get { return Status.Str + EquipWeapon.Value; } }
    public int TotalDefence { get { return Status.Con + EquipArmor.Value + EquipAccessory1.Value + EquipAccessory2.Value; } }
    public int TotalMagic { get { return Status.Mag + EquipWeapon.SubValue; } }
    public int TotalRegister { get { return Status.Men + EquipArmor.SubValue + EquipAccessory1.SubValue + EquipAccessory2.SubValue; } }
    public bool IsAlive { get { return Hp > 0; } }
    public bool IsGuard { get { return ActionPlan == AiProc.ACTION.GUARD; } }

    public void setLevel() {
        setLevel(Lv);
    }

    public void setLevel(int lv) {
        Lv = lv;
        BaseStatus = UnitMast.makeStatus(Lv, Job, Potentials);
        Status = new StatusMast();
        Status.Param = BaseStatus.ParamCopy();
        addSkill();
    }

    public void setBuff(BuffTran.TYPE type, BuffTran.FIELD_TYPE field_type, int turn, float value) {
        BuffTran buff = new BuffTran(type, field_type, turn, value);
        Buff.Add(buff);
    }
    //public void setDeBuff( BuffTran.TYPE type, BuffTran.FIELD_TYPE field_type, int turn, float value ){
    //	BuffTran buff = new BuffTran( type, field_type, turn, value );
    //	DeBuff.Add( buff );
    //}

    public void endBattle() {
        Buff.RemoveAll(b => b.FieldType == BuffTran.FIELD_TYPE.BATTLE);
    }

    public bool battleTurn() {
        return buffTurn(ref Buff, BuffTran.FIELD_TYPE.BATTLE);
    }

    public bool dungeonTurn() {
        return buffTurn(ref Buff, BuffTran.FIELD_TYPE.DUNGEON);
    }

    private bool buffTurn(ref List<BuffTran> buf, BuffTran.FIELD_TYPE field_type) {
        var result = false;
        for (int i = buf.Count - 1; i >= 0; i--) {
            int rest_turn = buf[i].passTurn(field_type);
            if (rest_turn <= 0) {
                buf.RemoveAt(i);
                result = true;
            }
        }
        return result;
    }

    public int addExp(int add) {
        Exp += add;
        return lvUp();
    }

    public int lvUp() {

        int lvup_cnt = 0;

        for (int i = Lv; i < LevelMast.List.Length - 1; i++) {
            if (Exp >= LevelMast.List[i].Exp) {
                Exp -= LevelMast.List[i].Exp;
                Lv = i + 1;
                lvup_cnt++;
                setLevel();
                addSkill();
                initAiTerm();

            }
        }

        return lvup_cnt;
    }

    /// <summary>
    /// 現在のLv以下で習得できるスキルを獲得
    /// </summary>
    public void addSkill() {
        var skills = UnitProcess.LeanSkills(this);
        skills.ForEach(it => addSkill(it));
    }

    public void addSkill(string tag) {

        SkillMast skill = Array.Find(SkillMast.List, it => it.Tag == tag);

        if (skill != null) {

            if (SkillIds.IndexOf(skill.Id) < 0) {
                SkillIds.Add(skill.Id);
                updateSkill();
            }
        }
    }

    public SkillMast[] updateSkill() {
        if (SkillIds == null) {
            return new SkillMast[0];
        }

        ///TODO スキルレベルに対応する
        _Skills = new SkillMast[SkillIds.Count];

        for (int i = 0; i < _Skills.Length; i++) {
            _Skills[i] = System.Array.Find<SkillMast>(SkillMast.List, it => it.Id == SkillIds[i]);
        }

        return _Skills;
    }

    //public SkillMast[] getAllSkill() {
    //    equip
    //}


    //初期AI定数セット
    public void initAiTerm() {
        int can = System.Array.FindIndex(Skills, it => it.Spec == SkillMast.SPEC.HEAL);
        CanHeal = can < 0 ? false : true;
        can = System.Array.FindIndex(Skills, it => it.Spec == SkillMast.SPEC.RESURRECT);
        CanRessurect = can < 0 ? false : true;
        can = System.Array.FindIndex(Skills, it => it.Spec == SkillMast.SPEC.CURE_STAN);
        CanCureStun = can < 0 ? false : true;
        can = System.Array.FindIndex(Skills, it => it.Spec == SkillMast.SPEC.CURE_POISON);
        CanCurePoison = can < 0 ? false : true;
        can = System.Array.FindIndex(Skills, it => it.Spec == SkillMast.SPEC.BUFF_REMOVE);
        CanRemoveBuff = can < 0 ? false : true;
        can = System.Array.FindIndex(Skills, it => it.Spec == SkillMast.SPEC.DEBUFF_REMOVE);
        CanRemoveDebuff = can < 0 ? false : true;
    }

    public void refresh() {
        Status.Param = BaseStatus.ParamCopy();
    }

    public UnitMast getMast() {

        if (_Mst == null) {
            if (Type == UnitMast.TYPE.PLAYER) {
                _Mst = UnitMast.List.First(it => it.Id == MasterId);
            } else {
                _Mst = EnemyMast.List.First(it => it.Id == MasterId);
            }
        }

        return _Mst;

    }

    /// <summary>
    /// 敵マスタ取得（敵ユニットのみ）
    /// </summary>
    /// <returns></returns>
    public EnemyMast getEnemyMast() {

        if (Type == UnitMast.TYPE.ENEMY) {
            return EnemyMast.List.First( it => it.Id == MasterId);
        }

        return null;
    }

    //public string getIconPath() {
    //    return CmnConst.Path.IMG_CHARA_ICON + Img;
    //}
    //public string getBustUpPath() {
    //    return CmnConst.Path.IMG_CHARA_BUSTUP + Img;
    //}
    public Sprite getIconImage() {
        string path = CmnConst.Path.IMG_CHARA_ICON + Img;
        return Resources.Load<Sprite>(path);
    }
    public Sprite getBustUpImage() {
        string path = CmnConst.Path.IMG_CHARA_BUSTUP + Img;
        return Resources.Load<Sprite>(path);
    }
    public Sprite getStandImage() {
        string path = CmnConst.Path.IMG_CHARA_STAND + Img;
        return Resources.Load<Sprite>(path);
    }

    public Sprite getImage(string dire) {
        string path = dire + Img;
        return Resources.Load<Sprite>(path);
    }

    public int getNextLvupExp() {
        return LevelMast.GetNextExp(Lv);
    }

    /// <summary>
    /// HPダメージ
    /// </summary>
    /// <param name="power"></param>
    /// <param name="guard"></param>
    public int damage(int power, int guard = 0) {
        return Status.damage(power, StatusMast.TYPE.HP, guard);
    }

    /// <summary>
    /// バースト状態かどうか
    /// </summary>
    public bool IsCrash {
        get {
            return isBuff(BuffTran.TYPE.CRASH_OUT);
            //return Buff.Exists(it => it.Type == BuffTran.TYPE.BREAK_BURST);
        }
    }

    /// <summary>
    /// バースト回復/ダメージ
    /// </summary>
    /// <param name="num"></param>
    public void addBurst(int num) {
        CrashPower = Mathf.Clamp(CrashPower + num, 0, Mst.MaxCrash);
    }

    /// <summary>
    /// バフ判定
    /// </summary>
    /// <param name="type"></param>
    /// <returns></returns>
    public bool isBuff(BuffTran.TYPE type) {
        return Buff.Exists(it => it.Type == type);
    }

    /// <summary>
    /// バフ取得
    /// </summary>
    /// <param name="type"></param>
    /// <returns></returns>
    public BuffTran getBuff(BuffTran.TYPE type) {
        return Buff.Find(it => it.Type == type);
    }

    public void addBuff(BuffTran buff)
    {
        var before = getBuff(buff.Type);
        if(buff.Turn > before.Turn)
        {
            Buff.Remove(before);
            Buff.Add(buff);
        }
    }
    public void removeBuff(BuffTran.TYPE type)
    {
        var bf = getBuff(type);
        Buff.Remove(bf);
    }

    /// <summary>
    /// 装備
    /// </summary>
    /// <param name="item">装備アイテム</param>
    /// <returns>外したアイテム</returns>
    public ItemTran Equip(ItemTran item, EQUIP posi) {

        //カテゴリチェック
        if (CheckEquipCategory(item, posi)) {
            var eject = Equips[(int)posi];
            Equips[(int)posi] = item;

            //ルーン設定
            UpdateRune();

            return eject;
        }

        return null;

    }
    public bool CheckEquipCategory(ItemTran item, EQUIP posi) {
        if (item != null) {
            switch (item.Mst.Category) {
                case ItemMast.CATEGORY.WEAPON:
                if (posi != EQUIP.WEAPON) {
                    return false;
                }
                break;
                case ItemMast.CATEGORY.ARMOR:
                if (posi != EQUIP.ARMOR) {
                    return false;
                }
                break;
                case ItemMast.CATEGORY.ACCESSORY:
                if (posi != EQUIP.ACCESSORY1 && posi != EQUIP.ACCESSORY2) {
                    return false;
                }
                break;
            }
        }
        return true;
    }

    /// <summary>
    /// ルーン更新
    /// </summary>
    public void UpdateRune() {
        Runes = new Dictionary<RuneMast.KIND, int>(); //一旦リセット
        foreach( var equip in Equips) {
            foreach(var rune in equip.Runes) {
                if(Runes.ContainsKey( rune.Mst.Kind)) {
                    if(rune.Value > Runes[rune.Mst.Kind]) {
                        //既存より効果が高ければ更新
                        Runes[rune.Mst.Kind] = rune.Value;
                    }
                } else {
                    Runes.Add(rune.Mst.Kind, rune.Value);
                }
            }
        }
    }

    /// <summary>
    /// スキルマスタ取得
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    public SkillMast getSkill(int id) {
        return Skills.FirstOrDefault(it => it.Id == id);
    }

    /// <summary>
    /// ルーンチェック＆ルーン値取得
    /// </summary>
    /// <param name="kind"></param>
    /// <returns></returns>
    public int GetRune( RuneMast.KIND kind ) {
        if (Runes.ContainsKey(kind)) {
            return Runes[kind];
        }
        return 0;
    }

    public ItemTran getEquip(EQUIP posi) {
        if (Type == UnitMast.TYPE.PLAYER) {
            return Equips[(int)posi] == null ? new ItemTran() : Equips[(int)posi];
        } else {
            if (Equips[(int)posi] == null) {
                var item = new ItemTran();

                switch (posi) {
                    case EQUIP.WEAPON:
                    item.Value = Status.Str;
                    item.SubValue = Status.Mag;
                    break;
                    case EQUIP.ARMOR:
                    item.Value = Status.Con;
                    item.SubValue = Status.Men;
                    break;
                }

                Equips[(int)posi] = item;
            }

            return Equips[(int)posi];

        }
    }
}