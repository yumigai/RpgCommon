using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// ItemとSkillの基底クラス
/// </summary>
abstract public class PowerMast : MulitiUseListMast
{
	public enum SPEC
	{
		NON, //効果なし
		ATTACK,
		DEFENCE, //未使用
		HEAL,
		RESURRECT,
		FIND_TREASURE, //未使用
		AVOID_TRAP, //未使用
		AVOID_ENEMY, //未使用
		STATUS_BUFF, //未使用
		STATUS_DEBUFF, //未使用
		BUFF_REMOVE,
		DEBUFF_REMOVE,
		ENCHANT_ELEMENT, //未使用
		PARALYZE,
		CURE_STAN,
		POISON,
		CURE_POISON,
		PANIC,
		CURE_PANIC,
		CURSE,
		CURE_CURSE,
		BAD,
		CURE_BAD, //毒・麻痺回復
		HEAL_CURE,
		BUFF,
		DEBUFF,
	}

	public enum USE_TIMING
	{
		NON, //使用不可
		BATTLE,
		FIELD,
		DUAL,
	}

	public enum SIDE
	{
		FRIEND,
		TARGET,
		DUAL,
	}

	public enum TARGET
	{
		ONE,
		ANYTHING,
		RANDOM,
	}

	public enum RANGE
	{
		SHORT,
		LONG,
		ALL,
	}

	public int PhysicsPower; //物理（Physicsとすると予約語と被るから注意）

	public int MagicPower; //魔法

	//public float PowerRandom;

	//public float Inhibit;

	public int Cost; //消費MPや価格など

	public GameConst.TIME UseTiming;

	public SIDE Side;

	public SPEC Spec;

	public GameConst.ELEMENT Element;

	public TARGET Target;

	public RANGE Range;

	public BuffTran.TYPE BuffType;

	public int BuffPower;

	public int EffectTime; //効果時間

	//public GameConst.EffectVisual Effect; //エフェクトビジュアル

	public string Effect; //独自エフェクト（基本的に使わない）

	public bool isTargettAll { get { return Target == PowerMast.TARGET.ANYTHING;} }

	/// <summary>
	/// 効果タイプ名取得
	/// </summary>
	/// <returns></returns>
	public string getEffectName() {
		switch (Spec) {
			case SPEC.ATTACK:
			return "攻撃";
			case SPEC.HEAL:
			return "回復";
			case SPEC.RESURRECT:
			return "復活";
			case SPEC.FIND_TREASURE:
			return "宝箱発見";
			case SPEC.AVOID_TRAP:
			return "罠回避";
			case SPEC.AVOID_ENEMY:
			return "敵回避";
			case SPEC.STATUS_BUFF:
			return "強化";
			case SPEC.STATUS_DEBUFF:
			return "弱体";
			case SPEC.BUFF_REMOVE:
			return "強化消去";
			case SPEC.DEBUFF_REMOVE:
			return "弱体消去";
			case SPEC.ENCHANT_ELEMENT:
			return "属性付与";
			case SPEC.PARALYZE:
			return "麻痺攻撃";
			case SPEC.CURE_STAN:
			return "麻痺回復";
			case SPEC.POISON:
			return "毒攻撃";
			case SPEC.CURE_POISON:
			return "毒回復";
			case SPEC.CURE_BAD:
			return "状態異常回復";
		}
		return "";
	}

	/// <summary>
	/// 効力発揮
	/// </summary>
	public void usePower(GameConst.TIME timing) {
		if (canUse(timing)) {
			switch (Spec) {
				case SPEC.HEAL:
				break;
				case SPEC.CURE_POISON:
				break;
				case SPEC.CURE_STAN:
				break;
				case SPEC.CURE_BAD:
				break;
			}
		}
    }

	public bool canUse() {
		return SaveMng.Quest.IsBattle ? canUse(GameConst.TIME.BATTLE) : canUse(GameConst.TIME.FIELD);
	}

	public bool canUse(GameConst.TIME timing) {
		return (UseTiming == GameConst.TIME.DUAL || UseTiming == timing) && Spec != SPEC.NON;
	}


}
