using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class AiProc 
{

	public const float LOW_HP = 4f;
	public const float LIFE_SAVE_HP = 2f;

    //温存量
    public const float NO_SUPPESS = 1f;
	public const float KEEP_MP_DEFAULT = 0.2f;
	public const float KEEP_MP_SUPPRESS = 0.4f;

    //使用確率
    public const float USE_FULL_POWER = 100f;
	public const float USE_SKILL_DEFAULT = 40f;
	public const float USE_SKILL_SUPPRESS = 20f;


	public const float LV_COEF = 10f;	//レベル差係数
	public const float PINCHI_COEF = 5f;

	public enum TACTICS{
		FREE,
		SUPPRESS_MP,
		DONT_USE_MP,
		FULL_POWER,
		LIFE_SAVE,
		SUPPORT,
		COMMAND,
		ALL
	}

	public enum ACTION{
		READY,
		ATTACK,
		SKILL,
        GUARD,
        ITEM,

	}

    public static readonly string[] TacticsJp = new string[(int)TACTICS.ALL] {
        "自由戦闘",
        "魔力温存",
        "魔力制限",
        "全力攻撃",
        "回復重視",
        "支援戦闘",
        "命令受付",
    };
    public static readonly string[] TacticsInfo = new string[(int)TACTICS.ALL] {
        "自由に各自の判断で戦闘します",
        "魔力消費を最小限にして戦闘します",
        "魔力を一切消費せずに戦闘します",
        "消費を気にせず全力で攻撃します",
        "回復・防御に専念します",
        "味方の支援を優先して戦闘します",
        "プレイヤーが直接行動を決めます",
    };

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

	public ACTION JudgeAction = ACTION.READY;
	public SkillMast UseSkill;
    public List<UnitStatusTran> ActionTargets; //スキル・攻撃の対象

	public int UnitIndex = 0;
	
	protected UnitStatusTran _Unit;
	
	protected List<UnitStatusTran> Friends;//{ get{ return ; } }
	protected List<UnitStatusTran> Targets;//{ get{ return ; } } //対象グループ全体

	public AiProc( List<UnitStatusTran> fr, List<UnitStatusTran> ta ){
		Friends = fr;
		Targets = ta;
	}

	public void think( UnitStatusTran uni ){
		_Unit = uni;
		tactics();
	}

	private void tactics( ){

		UseSkill = null;

        //デフォルトは攻撃（スキル使う場合は後で上書き）
        JudgeAction = _Unit.Tactics == TACTICS.COMMAND ? ACTION.READY : ACTION.ATTACK;

		if( _Unit.Tactics == TACTICS.COMMAND || _Unit.Tactics == TACTICS.DONT_USE_MP ){
			return;
		}

		//緊急判定
		switch ( _Unit.Tactics ){
		case TACTICS.DONT_USE_MP:
			break;
		case TACTICS.LIFE_SAVE:
			if(judgeHeal(LIFE_SAVE_HP)){
				return;
			}
			break;
		default:
			if(judgeHeal(LOW_HP)){
				return;
			}
			break;
		}

		var use_skill = false;

		//通常判定
		if (_Unit.Type == UnitMast.TYPE.PLAYER) {
			switch (_Unit.Tactics) {
				case TACTICS.FREE:
				use_skill = judgeAttackSkill(USE_SKILL_DEFAULT, KEEP_MP_DEFAULT);
				break;
				case TACTICS.SUPPRESS_MP:
				case TACTICS.LIFE_SAVE:
				use_skill = judgeAttackSkill(USE_SKILL_SUPPRESS, KEEP_MP_SUPPRESS);
				break;
				case TACTICS.FULL_POWER:
				use_skill = judgeAttackSkill(USE_FULL_POWER, NO_SUPPESS);
				break;
				case TACTICS.SUPPORT:
				use_skill = judgeSupportSkill(USE_SKILL_SUPPRESS, KEEP_MP_DEFAULT);
				break;
			}
		} else {
			use_skill = fullPowerSkill(USE_SKILL_DEFAULT);
		}

        if (!use_skill || ActionTargets == null) {
			ActionTargets = new List<UnitStatusTran>();
		}

        switch (JudgeAction) {
			case ACTION.ATTACK:
				List<UnitStatusTran> list = Targets.FindAll(it => it.Hp > 0);
				var index = getRandomTarget(_Unit, list.Count);
				if (index >= 0) {
					ActionTargets.Add(list[index]);
				}
			break;
			case ACTION.GUARD:
				ActionTargets.Add(_Unit);
			break;
        }
	}


	private bool judgeHeal( float low_judge ){

		if( _Unit.CanHeal ){
		
			var list = Friends.FindAll( it=>it.Hp < it.MaxHp / low_judge && it.Hp > 0 );
			
			if( list.Count > 0 ){

				SkillMast[] skills = getRangeSkill(list.Count, SkillMast.SPEC.HEAL );

				if( skills.Length > 0 ){

					list.Sort( (it1,it2)=> (it1.MaxHp - it1.Hp) - (it2.MaxHp - it2.Hp) );

					System.Array.Sort( skills, (it1, ite2 )=> ite2.Cost - it1.Cost );

					int use_index = 0;
					
					for( int i = 0; i < skills.Length; i++ ){
						use_index = i;
						if(list[0].MaxHp - list[0].Hp < skills[i].AvrPow ){
							break;
						}
					}

					UseSkill = skills[use_index];
					JudgeAction = ACTION.SKILL;
					return true;
				}

			}
		}
		return false;

	}

    /// <summary>
    /// 攻撃スキル判定
    /// </summary>
    /// <param name="use_per">使用確率</param>
    /// <param name="keep_mp">温存量(1:全使用～0:不使用)</param>
    /// <returns></returns>
	private bool judgeAttackSkill( float use_per, float keep_mp ){
		return judgeToTargetSkill( use_per, keep_mp, PowerMast.SPEC.ATTACK, true, true );
	}

	private bool judgeSupportSkill( float use_per, float keep_mp ){

		if( _Unit.CanSupport.Count > 0 ){
			int index = Random.Range( 0, _Unit.CanSupport.Count );
			switch( _Unit.CanSupport[index] ){

			}
		}

		return false;
	}

    private static int getRandomTarget(UnitStatusTran val, int unit_num) {

		int index = -1;

		if (val.Status.Hp > 0) {

			if (val.EquipWeapon.Range == PowerMast.RANGE.LONG) {
				index = Random.Range(0, unit_num);
			} else {
				int rate = unit_num - 1;
				if (rate < 0) {
					return index;
				}

				if (rate >= TARGET_PERCENT.Length) {
					rate = TARGET_PERCENT.Length - 1;
				}
				index = UtilToolLib.getRateRandom(0, 100, TARGET_PERCENT[rate]);
			}
		}

        return index;

    }

    //private bool judgeBuffSkill( float use_per, float keep_mp ){

    //	SkillTargets = Friends.FindAll( it=>it.Hp > 0 && it.Buff.Count == 0 );

    //	return judgeToTarggetSkill( ref Friends, use_per, keep_mp, SkillMast.KIND.STATUS_BUFF, false, false );

    //	//ENCHANT_ELEMENT,
    //	//CURE_STAN,
    //	//CURE_POISON,
    //}

    //private bool judgedDebuffSkill( float use_per, float keep_mp ){

    //	SkillTargets = Targets.FindAll( it=>it.Hp > 0 && it.DeBuff.Count == 0 );

    //	return judgeToTarggetSkill( ref Targets, use_per, keep_mp, SkillMast.KIND.STATUS_DEBUFF, false, true );

    //	//return judgeToTarggetSkill( user_per, mp, SkillMst.KIND.STAN );

    //	//return judgeToTarggetSkill( user_per, mp, SkillMst.KIND.POISON );

    //}


    private bool isKeepMp( float keep_mp ){
		if( _Unit.Mp / _Unit.MaxMp < keep_mp ){
			return true;
		}
		return false;
	}


	private bool judgeToTargetSkill( float use_per, float keep_mp, SkillMast.SPEC kind, bool judge_tage_hp = false, bool judge_tage_lv = false, bool judge_pinchi = true ){

		List<UnitStatusTran> list = Targets.FindAll(it => it.Hp > 0);
		float adj = use_per;

		if( isKeepMp(keep_mp) ){
			return false;
		}

		SkillMast[] skills = getRangeSkill(list.Count, kind );

		if( skills.Length == 0 ){
			return false;
		}

		if( judge_tage_hp ){

			list.Sort((a, b) => b.Hp - a.Hp );
			int max_hp = list[0].Hp;

			SkillMast[] use_skills = System.Array.FindAll( skills, it=> (int)it.PowMin <= max_hp );
			if( use_skills.Length > 0 ){
				skills = use_skills;
			}
		}

		if( judge_tage_lv ){

			list.Sort((a, b) => b.Lv - a.Lv );
			int max_lv = list[0].Lv;

			float diff_lv = (float)((max_lv+10) / (_Unit.Lv+10));

			adj += (diff_lv * LV_COEF)-LV_COEF;
		}

		if( judge_pinchi ){
			int pinch_num = Friends.FindAll( it=> it.Hp / it.MaxHp < 0.5f ).Count;
			adj += ((float)pinch_num * PINCHI_COEF );
		}

		float judge = Random.Range( 0f, 100f );

		if( judge < adj ){
			skills.OrderByDescending( it=>it.AvrPow );
			UseSkill = skills[0];
			JudgeAction = ACTION.SKILL;
			ActionTargets = list; //ターゲット選択しないといけないか
			return true;
		}

		return false;
	}

	/// <summary>
	/// 温存なし攻撃（主に敵）
	/// </summary>
	/// <param name="use_per">使用確率</param>
	/// <param name="is_random">対象ランダムか</param>
	/// <returns></returns>
	private bool fullPowerSkill(float use_per = 100f, bool is_random = true) {

        SkillMast[] skills = getRangeSkill(Targets.Count, PowerMast.SPEC.ATTACK);
		List<UnitStatusTran> list = Targets.FindAll(it => it.Hp > 0);

		float judge = Random.Range(0f, 100f);

		if (skills.Length > 0 && list.Count > 0  && judge <= use_per) {

			int index = 0;
			if (is_random) {
				index = Random.Range(0, skills.Length);
			} else {
				System.Array.Sort(skills, (ite1, ite2) => (int)ite2.AvrPow - (int)ite1.AvrPow);
			}
			UseSkill = skills[index];
            JudgeAction = ACTION.SKILL;
			if (UseSkill.Target == PowerMast.TARGET.ANYTHING) {
				ActionTargets = list;
			} else {
				int tage = Random.Range(0, list.Count);
				ActionTargets = new List<UnitStatusTran>() { list[tage] };
			}
            return true;
        }
        return false;
    }

	private SkillMast[] getRangeSkill( int count, SkillMast.SPEC kind ){

		SkillMast[] skills = new SkillMast[0];

		if( count == 1 ){
			skills = getUseSkill( kind, SkillMast.TARGET.ONE );
			if( skills.Length == 0 ){
				skills = getUseSkill( kind, SkillMast.TARGET.ANYTHING );
			}
		}else{
			skills = getUseSkill( kind, SkillMast.TARGET.ANYTHING );
			if( skills.Length == 0 ){
				skills = getUseSkill( kind, SkillMast.TARGET.ONE );
			}
		}

		return skills;
	}

	private SkillMast[] getUseSkill( SkillMast.SPEC kind, SkillMast.TARGET target ){
		return System.Array.FindAll (_Unit.Skills, it=> it.Spec == kind && it.Target == target && it.Cost <= _Unit.Mp );
	}

}