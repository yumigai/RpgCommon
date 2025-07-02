using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class QuestTran : CmnSaveProc.SaveClass {

	public class BattleTran{
		public int Turn;
		public int ActionRound;
	}
	
	public int						StageIndex;
	public List<UnitStatusTran>		ActiveParty = new List<UnitStatusTran>();
	public List<UnitStatusTran> 	Enemys = new List<UnitStatusTran>();
    public List<UnitStatusTran>     Battlers = new List<UnitStatusTran>();
    public List<ItemTran>			CarryBag = new List<ItemTran>();
	public int DestroyNum;
	public int GetExp;
	public int GetMoney;
	
	public int NowFloorNum = 0;

	public BattleTran Battle;
	
	public StageMast Stage{ get{ return StageMast.List[StageIndex];}}

	public bool IsBattle{ get { return (Enemys != null && Enemys.Count > 0); } }

	//消したい
    public bool IsGameOver { get { return ActiveParty.Count == 0; } }

	//クエスト中か
	public bool IsQuest { get { return ActiveParty.Count > 0; } }


	//QuestTran(int stageId, List<UnitStatusTran> party) {
	//	StageIndex = System.Array.FindIndex(StageMast.List, it=>it.Id==stageId);
	//	ActiveParty = party;
	//}

	public int Init(int stageId, List<UnitStatusTran> party) {
		StageIndex = System.Array.FindIndex(StageMast.List, it => it.Id == stageId);
		ActiveParty = party;
		CarryBag = new List<ItemTran>();
		NowFloorNum = 0;
		return StageIndex;
	}

	public int Continue(int stageId) {
		StageIndex = StageMast.getStageIndex(stageId);// System.Array.FindIndex(StageMast.List, it => it.Id == stageId);

		NowFloorNum = 0;
		return StageIndex;
	}

	public List<UnitStatusTran> getTarget( UnitMast.TARGET target ){

		if ( ActiveParty == null || ActiveParty.Count == 0) {
			return null;
		}

		List<UnitStatusTran> target_list = new List<UnitStatusTran>();

		switch (target) {
			case UnitMast.TARGET.RANDOM:
				int index = Random.Range( 0, ActiveParty.Count );
				target_list.Add( ActiveParty[index] );
				break;
			case UnitMast.TARGET.PARTY:
				target_list = ActiveParty;
				break;
		}

		return target_list;
	}

    public void MemberHeal( int num, UnitStatusTran target, StatusMast.TYPE type) {

        int max = 0;//  target.BaseStatus.getParam(type);
        switch (type) {
            case StatusMast.TYPE.HP:
                max = target.MaxHp;
            break;
            case StatusMast.TYPE.MP:
                max = target.MaxMp;
            break;
            default:
                max = target.BaseStatus.getParam(type);
            break;
        }

        target.Status.heal(num, max, type);
    }

    public void MemberHeal(int num, StatusMast.TYPE type) {
        for (int i = 0; i < ActiveParty.Count; i++) {
            MemberHeal(num, ActiveParty[i], type);
        }
    }

    public void MemberHeal( float per, StatusMast.TYPE type ) {
		for( int i = 0; i < ActiveParty.Count; i++) {
			int heal = Mathf.CeilToInt( (float)ActiveParty[i].BaseStatus.getParam(type) * per );
            MemberHeal(heal,ActiveParty[i],type);
		}
	}

	public void MemberStatusHeal( int type, UnitStatusTran target ){
		for (int i = 0,b=1; i < (int)StatusMast.TYPE.ALL; i++, b=b*2) {

			int bit = b & type;

			if (bit == b) {
				MemberHeal (StatusMast.MAX_STATUS, target, (StatusMast.TYPE)i);
			}
		}
	}

	public void MemberStatusHeal( int type ){
		for( int i = 0; i < ActiveParty.Count; i++) {
			MemberStatusHeal(type,ActiveParty[i]);
		}
	}
	
	public bool isActiveMember( UnitStatusTran unit ){
		if (ActiveParty != null && ActiveParty.FindIndex(it=>it.Id == unit.Id) >= 0) {
			return true;
		}
		return false;
	}

	public bool partyMemberSort(UnitStatusTran unit1, UnitStatusTran unit2) {

		if (unit1 != unit2) {
			var index1 = ActiveParty.IndexOf(unit1);
			var index2 = ActiveParty.IndexOf(unit2);
			if (index1 >= 0 && index2 >= 0) {
				ActiveParty[index1] = unit2;
				ActiveParty[index2] = unit1;
				save();
				return true;
			}
		}
		return false;
	}

}
