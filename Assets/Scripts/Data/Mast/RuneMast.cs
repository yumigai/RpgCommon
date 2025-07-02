using UnityEngine;
using System.Linq;
using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public class RuneMast : MasterCmn {

	public enum KIND{
		
		//STR_ADD,
		//CON_ADD,
		//AGI_ADD,
		//LIF_ADD,
		//MEN_ADD,
		//MAG_ADD,
		//LUK_ADD,
		
		UP_EXP,
		UP_MONEY,
		
		P_ATK_ADD,
		M_ATK_ADD,
		P_DEF_ADD,
		M_DEF_ADD,
		
		GUARD_POISON,
		GUARD_STAN,
		GUARD_BAD,
		
		MP_DISCOUNT,

		ELEMENT_ATTACK,
		ELEMENT_GUARD,

		HP_HEAL_TURN_END,
		HP_HEAL_BATTLE_END,
		HP_STEAL,

		ATTACK_ALL,

		CRASH_RECOVER,
		CRASH_DAMAGE,
		

		ALL
	}

	///// <summary>
	///// レアリティ
	///// </summary>
	//public enum RARITY
	//{
	//	N,
	//	R,
	//	SR,
	//	SSR,
	//	UR,
	//	ALL
	//}

	/// <summary>
	/// 使用できるのは戦闘中かフィールドか
	/// </summary>
	//public enum USE_TIMING{
	//	BATTLE,
	//	FIELD,
	//	DUAL,
	//}

	public enum GROUP{
		WEAPON,
		ARMOR,
		ACCESSORY,
		ALL
	}

	public static readonly string[] DEFAULT_NAME = new string[(int)KIND.ALL]{

		//"筋力上昇",
		//"耐久力上昇",
		//"敏捷力上昇",
		//"生命力上昇",
		//"精神力上昇",
		//"魔力上昇",
		//"幸運上昇",

		"獲得経験値上昇",
		"獲得金銭上昇",

		"物理攻撃力上昇",
		"魔法攻撃力上昇。",
		"物理防御力上昇",
		"魔法防御力上昇",

		"毒耐性",
		"麻痺耐性",
		"状態異常耐性",

		"消費MP低減",

		"{0}属性攻撃",
		"{0}属性防御",

		"HP再生",
		"戦闘後HP回復",

		"HP吸収",
		"全体攻撃",
		"クラッシュ回復",
		"クラッシュ強化",
	};

	public static readonly string[] INFO_TEMPLATE = new string[(int)KIND.ALL]{

		//"筋力が{0}%強化される。",
		//"耐久力が{0}%強化される。",
		//"敏捷力が{0}%強化される。",
		//"生命力が{0}%強化される。",
		//"精神力が{0}%強化される。",
		//"魔力が{0}%強化される。",
		//"幸運が{0}%強化される。",
		
		"獲得経験値が{0}%増加する。",
		"獲得金銭が{0}%増加する。",
		
		"武器の物理攻撃力が{0}%増加する。",
		"武器の魔法攻撃力が{0}%増加する。",
		"防具の物理防御力が{0}%増加する。",
		"防具の魔法防御力が{0}%増加する。",

		"毒を{0}％防ぐ。",
		"麻痺を{0}％防ぐ。",
		"状態異常を{0}％防ぐ",
		
		"消費MPが{0}%低下する。",

		"{0}の属性による攻撃を{1}%上昇させる。",
		"{0}の属性によるダメージを{1}%低下させる。",

		"戦闘中、毎ターンHPが最大HPの{0}％回復する。",
		"戦闘終了後、HPが最大HPの{0}％回復する。",

		"通常攻撃をすると、HPが与えたダメージの{0}％回復する。",
		"通常攻撃が全体攻撃になるが、{0}％攻撃力が低下する。",
		"ターン終了時のクラッシュゲージ回復が{0}増加する。",
		"通常攻撃で与えるクラッシュダメージが{0}増加する。",
	};

	public const string StartInfo = "戦闘開始後、{0}に効果が発動。";
	public const string EndoInfo = "{0}ターン持続";
	
	public int Id;

	public int Lv;
	
	public int Rarity;
	
	public string Name;

	public KIND Kind;
	
//	public USE_TIMING UseTiming;
	
	public int RandValueMin;
	public int RandValueMax;
	
	public string UniqueInfo;

	public GameConst.ELEMENT Element;

	public bool IsRandom; //ランダムで付与されるか（実質有効・無効フラグ）
	
	public static RuneMast[] List;

	public bool IsElement() {
		switch (Kind) {
			case KIND.ELEMENT_ATTACK:
			case KIND.ELEMENT_GUARD:
			return true;
			default:
			return false;
		}
	}

	public static void load(){
		List = load<RuneMast> ();
	}
	
	
}

