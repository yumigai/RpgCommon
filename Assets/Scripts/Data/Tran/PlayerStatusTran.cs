using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class PlayerStatusTran : CmnSaveProc.SaveClass
{
    public const int MAX_UNIT_NUM = 30;

    public const int MAX_MONEY = 999999999;

    public enum STAGE_STATE
    {
        UNKNOWN,
        DISCOVERY,
        BOSS_ENCOUNT,
        BOSS_DESTROY, //予備
        CLEAR,
    }

    public string Name;
    public int Lv;  //プレイヤーレベル（ユニットとは別）
    public int Exp; //プレイヤーExp（ユニットとは別）
    public int Money;
    public int RestTimeLimit; //ゲームオーバーまでの残り時間

    public bool IsFinishInitSetting;
    public bool IsFinishOpening;
    public int Charisma = 10;
    private int PartyIndex = 0;

    public List<int> Tutorial = new List<int>();
    public List<int> DiscoveryStage = new List<int>();
    public List<int> ClearStage = new List<int>();
    public List<int> Archives = new List<int>();
    public List<int> Readed = new List<int>();
    //public Dictionary<string,int> Flags = new Dictionary<string, int>();

    //public List<UnitStatusTran> ActiveParty {
    //    get {
    //        return SaveMng.Quest.ActiveParty;
    //    }
    //}
    public int[] ActiveMember {
        get {
            return Parties[NowParty].Members;
        }
    }

    public PartyTran[] Parties = new PartyTran[PartyTran.MAX_PARTY] { new PartyTran(), new PartyTran(), new PartyTran(), new PartyTran() };

    public bool isAllClear {
        get {
            return ClearStage.Count >= StageMast.List.Length;
        }
    }

    public int NowParty {
        get {
            return PartyIndex;
        }
        set {
            PartyIndex = Mathf.Clamp(value, 0, PartyTran.MAX_PARTY - 1);
        }
    }

    /// <summary>
    /// 全パーティ初期化
    /// </summary>
    /// <param name="unit_tran_id"></param>
    public void resetPartyAll(int unit_tran_id) {
        Parties = new PartyTran[PartyTran.MAX_PARTY] { new PartyTran(), new PartyTran(), new PartyTran(), new PartyTran() };
        for (var i = 0; i < Parties.Length; i++) {
            resetParty(i, unit_tran_id);
        }
    }

    /// <summary>
    /// パーティ初期化
    /// </summary>
    /// <param name="i"></param>
    /// <param name="unit_tran_id"></param>
    public void resetParty(int i, int unit_tran_id) {
        Parties[i] = new PartyTran(unit_tran_id);
    }

    public STAGE_STATE stageState(int stage_id) {
        return stageState(stage_id, ClearStage, DiscoveryStage);
    }

    public STAGE_STATE stageState(int id, List<int> clear, List<int> discovery) {
        if (clear.IndexOf(id) >= 0) {
            return STAGE_STATE.CLEAR;
        }
        if (discovery.IndexOf(id) >= 0) {
            return STAGE_STATE.DISCOVERY;
        }
        return STAGE_STATE.UNKNOWN;
    }

    /// <summary>
    /// アーカイブ取得
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    public bool addArchive(int id) {
        return UtilToolLib.addId(id, ref Archives);
    }

    /// <summary>
    /// アーカイブ閲覧済み
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    public bool addReaded(int id) {
        return UtilToolLib.addId(id, ref Readed);
    }



    public void addMoney(int num) {
        Money = Mathf.Clamp(Money + num, 0, MAX_MONEY);
    }
    public void subMoney(int num) {
        addMoney(-num);
    }

    /// <summary>
    /// アクティブメンバー取得
    /// </summary>
    /// <param name="isSetOnly">メンバーが配置されている場合のみ</param>
    /// <returns></returns>
    public List<UnitStatusTran> getActiveMembers(bool isSetOnly = true) {
        List<UnitStatusTran> list = new List<UnitStatusTran>();
        for (int i = 0; i < ActiveMember.Length; i++) {
            if (ActiveMember[i] >= 0 || !isSetOnly) {
                list.Add(SaveMng.UnitData.getData(ActiveMember[i]));
            }
        }

        return list;
    }

    public UnitStatusTran getActiveMember(int index) {
        return SaveMng.UnitData.getData(ActiveMember[index]);
    }

    public bool ClearAndNext(StageMast stg) {
        return ClearAndNext(ref ClearStage, ref DiscoveryStage, stg.NextIds, stg.Id);
    }

    private bool ClearAndNext(ref List<int> clear, ref List<int> discovery, int[] next_ids, int id) {
        if (clear != null && clear.IndexOf(id) < 0) {
            for (int i = 0; next_ids != null && i < next_ids.Length; i++) {
                if (next_ids[i] > 0 && discovery.IndexOf(next_ids[i]) < 0) {
                    discovery.Add(next_ids[i]);
                }
            }
            clear.Add(id);
            return true;
        }
        return false;
    }

    // public void addFlag(string key, int val){
    //     if(Flags.ContainsKey(key)){
    //         Flags[key] += val;
    //     }else{
    //         Flags.Add(key,val);
    //     }
    // }

    // public int getFlag(string key){
    //     if(Flags.ContainsKey(key)){
    //         return Flags[key];
    //     }
    //     return 0;
    // }

}