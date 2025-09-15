using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class SaveMng : CmnSaveProc
{

    public class UnitWrap : SaveClass
    {
        public List<UnitStatusTran> DataList;
        public UnitStatusTran getData(int id) {
            return DataList.Find(d => d.Id == id);
        }

        public void refleshAll() {
            foreach (UnitStatusTran uni in DataList) {
                uni.refresh();
            }
            saveUnit();
        }

        public UnitWrap() {
            DataList = new List<UnitStatusTran>();
        }
    }

    //public class LogHistoryWrap : SaveClass {
    //    public List<LogListTran> DataList;
    //    public LogHistoryWrap() {
    //        DataList = new List<LogListTran>();
    //    }
    //}
    //public class ReportWrap : SaveClass {
    //    public List<ReportTran> DataList;
    //    public ReportWrap() {
    //        DataList = new List<ReportTran>();
    //    }
    //}

    public class SystemTran : SaveClass
    {
        public int LoginNum;
        public long LastLogin;
        public long SaveDate;
        public System.DateTime getLoginDate() {
            return System.DateTime.FromBinary(LastLogin);
        }
        public System.DateTime getSaveDate() {
            return System.DateTime.FromBinary(SaveDate);
        }
        public void saveLoginDate() {
            LastLogin = System.DateTime.Now.ToBinary();
            LoginNum++;
            save();
        }
        public void saveSaveDate() {
            SaveDate = System.DateTime.Now.ToBinary();
            save();
        }

        public bool login() {
            System.DateTime lastlogin = getLoginDate();
            if (System.DateTime.Now.Date > lastlogin.Date) {
                saveLoginDate();
                return true;
            }
            return false;
        }

    }

    public static PlayerStatusTran Status;
    public static SystemTran SysDt;
    public static UnitWrap UnitData;
    public static QuestTran Quest;
    public static ItemTranWrap ItemData;

    public static AdvStatusTran AdvData;

    public static List<UnitStatusTran> Units {
        get {
            return UnitData.DataList;
        }
        set {
            UnitData.DataList = value;
        }
    }

    public static List<UnitStatusTran> ActiveUnits {
        get {
            if (Quest != null && Quest.ActiveParty != null) {
                return Quest.ActiveParty;
            }
            return null;
        }
    }

    public static List<ItemTran> Items {
        get {
            return ItemData.DataList;
        }
        set {
            ItemData.DataList = value;
        }
    }
    //public static List<LogListTran> LogHistory {
    //    get {
    //        return LogData.DataList;
    //    }
    //}

    //public static List<ReportTran> Reports {
    //    get {
    //        return ReportData.DataList;
    //    }
    //    set {
    //        ReportData.DataList = value;
    //    }
    //}

    public static void initSave() {
        if (CmnSaveProc.Conf.Standby == false) {
            CmnSaveProc.Conf.Standby = true;
            CmnSaveProc.Conf.PlayerName = GameConst.DEFAULT_PLAYER_NAME;
            saveConfig();
        }
    }

    new public static void loadAll() {

        CmnSaveProc.loadAll();
        initSave();
        UnitData = load<UnitWrap>();
        ItemData = load<ItemTranWrap>();
        Status = load<PlayerStatusTran>();
        Quest = load<QuestTran>();
        SysDt = load<SystemTran>();
        AdvData = load<AdvStatusTran>();

        //破損チェック
        repairBrokenData(ref UnitData);
        repairBrokenData(ref ItemData);
        repairBrokenData(ref Status);
        repairBrokenData(ref Quest);
        repairBrokenData(ref SysDt);
        repairBrokenData(ref AdvData);
    }

    new public static void resetSlotAll(int slot = -1) {
        CmnSaveProc.resetSlotAll(slot);
        Key.reset(slot);
        Conf.reset(slot);
        UnitData.reset(slot);
        ItemData.reset(slot);
        Status.reset(slot);
        Quest.reset(slot);
        SysDt.reset(slot);
        loadAll();
    }


    public static void saveUnit() {
        UnitData.save();
    }

    public static void saveStatus() {
        Status.save();
    }

    /// <summary>
    /// スロットセーブ
    /// </summary>
    /// <param name="slot"></param>
    public static void saveAll(int slot = 0) {
        Key.save(slot);
        Conf.save(slot);
        UnitData.save(slot);
        ItemData.save(slot);
        Status.save(slot);
        Quest.save(slot);
        SysDt.saveSaveDate();
        SysDt.save(slot);

    }

    public static void loadAll(int slot) {
        Key = load<KeyConfigTran>(true, slot);
        Conf = load<GameConfig>(true, slot);
        UnitData = load<UnitWrap>(true, slot);
        ItemData = load<ItemTranWrap>(true, slot);
        Status = load<PlayerStatusTran>(true, slot);
        Quest = load<QuestTran>(true, slot);
        SysDt = load<SystemTran>(true, slot);
        saveAll();
    }

    public static List<UnitStatusTran> GetActiveAllUnits() {
        return (Quest != null && Quest.IsQuest)
            ? Quest.ActiveParty : Units;
    }

    public static List<UnitStatusTran> GetActivePartyUnits() {
        return (Quest != null && Quest.IsQuest)
            ? Quest.ActiveParty : Status.getActiveMembers(false);
    }
}