using UnityEngine;
using System.Collections;
using System.Linq;
using System.Collections.Generic;

public class UnitMast : MasterCmn {

	public int Id;
    public string Tag;
    public string Name;
    public int BaseLv;
    public int MaxLv;
    public string Img;
    public JOB Job;
    public GameConst.ELEMENT Element;
    public GameConst.RARITY[] Potentials;
    //public string[] SkillTags = new string[0];
    //public int[] SkillLeans = new int[0];
    public string AtkEffect;

	public FEATURE[] Feature = new FEATURE[0]; //特徴

	public GameConst.ELEMENT[] Weak = new GameConst.ELEMENT[0];
    public GameConst.ELEMENT[] Regist = new GameConst.ELEMENT[0];

    //public const int MAX_POTENTIAL = (int)GameConst.RARITY.ALL; //才能とレアリティを一致させる
	//public const int MAX_LV = 99;
    public const int MAX_CRASH = 10;

	public enum TYPE{
		PLAYER,
		ENEMY,
		ALL
	}
	
	public enum JOB{
		NON,
		HERO, //特別
		FIGHTER, //攻撃力高い
		MAGE, //魔力高い
		KNIGHT, //防御力高い
		HEALER, //ヒーラー、抵抗力高い
		ROGUE,
		HUNTER,
		ALL
	}

	public enum TARGET
	{
		NON,
		RANDOM,
		PARTY,
	}

	public enum FEATURE //特徴
    {
		NON,//基本的に使用しない
		HERO, //主人公
		INIT_JOIN, //最初から加入
		INIT_PARTY, //最初から加入＋初期パーティ
		DROP_AND_SELL, //捨てる・売却が可能
    }

	public static readonly float[,] CLASS_BOOST = new float[(int)JOB.ALL,(int)StatusMast.TYPE.ALL]{
		{0.5f,0.5f,0.5f,0.5f,1f,0.5f,0.5f,1f,1f},
		{1f,1f,1f,1f,1f,1f,1f,1f,1f},
		{1.5f,1.1f,1f,1f,1f,0.5f,0.75f,1f,1f},
		{0.75f,0.75f,1f,0.9f,1f,1.5f,1.25f,1f,1f},
		{1.25f,1.25f,0.75f,1f,1f,0.5f,0.75f,1f,1f},
		{0.8f,1f,0.8f,1f,1f,1.25f,1.5f,1f,1f},
		{1f,0.9f,1.5f,1f,1f,1f,1f,1f,1f},
		{1.25f,0.9f,1.25f,1.1f,1f,1f,1f,1f,1f},
	};
	
	public static readonly string[] POTENTIAL_NAME = new string[(int)GameConst.RARITY.ALL]{
		"鈍才","平凡","非凡","天才","入神",
	};

	//public static readonly string[] JOB_NAME = new string[ (int)JOB.ALL ]{
	//	"なし","界導師","戦士","魔術師","騎士","盗賊","僧侶","狩人"
	//};

    public static IReadOnlyList<UnitMast> List;


    public int MaxCrash { get { return MAX_CRASH; } } //基本的に成長しない

    public static void load() {
        List = load<UnitMast>();
    }

    public static StatusMast makeStatus( int lv, JOB job, GameConst.RARITY[] potentials ){

		int param_index = lv > LevelMast.MAX_LV ? LevelMast.MAX_LV - 1 : lv-1;
		param_index = param_index < 0 ? 0 : param_index;

		StatusMast status = new StatusMast ();
		for (int i = 0; i < potentials.Length && i < (int)StatusMast.TYPE.ALL; i++) {

			int pote = (int)potentials[i] < (int)GameConst.RARITY.ALL ? (int)potentials[i] : (int)GameConst.RARITY.ALL - 1;

            status.Param[i] = LevelMast.List[param_index].Potential[pote]; // LVUP_PARAM[ param_index, pote];

			status.Param[i] = (int)(status.Param[i] * CLASS_BOOST[(int)job, i ]);

		}

		status.Hp = status.MaxHp;
		status.Mp = status.MaxMp;

		return status;
	}

    public static UnitStatusTran getUnit(int id) {

		UnitMast mast = List.FirstOrDefault(it => it.Id == id);
		UnitStatusTran tran = getUnit(mast);
        return tran;
    }

    public static UnitStatusTran getUnit(UnitMast mast) {
        UnitStatusTran tran = getUnit(mast, mast.BaseLv, TYPE.PLAYER);
        return tran;
    }

    protected static UnitStatusTran getUnit(UnitMast uni, int lv, TYPE type ) {

        UnitStatusTran tran = new UnitStatusTran();

		tran.Id = uni.Id; //デフォルト。同一種類がある場合、後で採番しなおす
        tran.MasterId = uni.Id;
        tran.Type = type;
        tran.Name = uni.Name;
        tran.Exp = 0;
        tran.Lv = lv;
        tran.CrashPower = uni.MaxCrash;
		tran.Tactics = type == TYPE.PLAYER ? AiProc.TACTICS.COMMAND : AiProc.TACTICS.FREE;

		tran.setLevel();
        tran.addSkill();

        return tran;
    }



}