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
		ATTACK,
		DEFENCE,
		HEAL,
		RESURRECT,
		FIND_TREASURE,
		AVOID_TRAP,
		AVOID_ENEMY,
		STATUS_BUFF,
		STATUS_DEBUFF,
		BUFF_REMOVE,
		DEBUFF_REMOVE,
		ENCHANT_ELEMENT,
		STAN,
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

	public int PhysicsPower;

	public int MagicPower;

	public float PowerRandom;

	public float Inhibit;

	public int UseCost;

	public USE_TIMING UseTiming;

	public SIDE Side;

	public SPEC Spec;

	public GameConst.ELEMENT Element;

	public TARGET Target;

	public RANGE Range;

	public BuffTran.TYPE BuffType;

	public int BuffPercent;

	public int BuffPower;

	public int EffectTime; //効果時間

	//public GameConst.EffectVisual Effect; //エフェクトビジュアル

	public string Effect; //独自エフェクト（基本的に使わない）

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
			case SPEC.STAN:
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
	public void usePower(USE_TIMING timing) {
		if (UseTiming == USE_TIMING.DUAL || UseTiming == timing) {
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

}
