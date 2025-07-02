using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class EventActionMast : MasterCmn
{
    public enum ACTION
    {
        JUDGE,
        CONFIRM,
        CONFIRM_HOLD,
        ADD,
        REMOVE,
        SCENARIO,
        MESSAGE,
    }

    public enum TYPE
    {
        FLAG,
        FRIEND_SHIP,
        ITEM,
        ALL_UNIT,
        PARTY_UNIT,
        DISCOVERY_STAGE,
    }

    public int Id;

    public string Tag;

    public TYPE Type;

    public ACTION Act;

    public string TargetTag;

    public string Param;

    public int Value {
        get {
            return UtilToolLib.ParseInt(Param);
        }
    }

    public static bool execEvents(string tag, bool is_and = true) {
        bool result = is_and;
        var judge = List.Where(it => it.Tag == tag && it.Act == ACTION.JUDGE);
        foreach (var eve in judge) {
            bool rtn = eve.judgeEvent();
            if (is_and) {
                result = result ? rtn : false;
            } else {
                result = result ? true : rtn;
            }
        }

        if (result) {

            var confirm = List.FirstOrDefault(it => it.Tag == tag
                && (it.Act == ACTION.CONFIRM || it.Act == ACTION.CONFIRM_HOLD));

            if (confirm != null) {
                if (confirm.Act == ACTION.CONFIRM_HOLD) {
                    CommonProcess.showConfirm(confirm.Param, (obj) => { eventAction(tag); }, null, true);
                } else {
                    CommonProcess.showConfirm(confirm.Param, (obj) => { eventAction(tag); });
                }
            } else {
                result = eventAction(tag);
            }
        }
        return result;
    }

    public static bool eventAction(string tag) {
        var act_result = true;
        var add = List.Where(it => it.Tag == tag && it.Act == ACTION.ADD);
        foreach (var eve in add) {
            act_result = eve.execAddEvent() ? act_result : false;
        }
        var rem = List.Where(it => it.Tag == tag && it.Act == ACTION.REMOVE);
        foreach (var eve in rem) {
            act_result = eve.execRemoveEvent() ? act_result : false;
        }
        if (act_result) {
            SaveMng.Status.save();
            SaveMng.UnitData.save();
            SaveMng.ItemData.save();
        }

        var mess = List.FirstOrDefault(it => it.Tag == tag && it.Act == ACTION.MESSAGE);
        if (mess != null) {
            CommonProcess.showMessage(mess.Param);
        }

        return act_result;
    }

    public bool judgeEvent() {
        switch (Type) {
            case TYPE.FLAG:
            return SaveMng.AdvData.getFlag(TargetTag) >= Value;
            case TYPE.FRIEND_SHIP:
            return UnitProcess.getFriendShip(TargetTag) >= Value;
            case TYPE.ITEM:
            return ItemProcess.getTranData(TargetTag).Num >= Value;
            case TYPE.ALL_UNIT:
            return UnitProcess.getAllyMember(TargetTag) != null;
            case TYPE.PARTY_UNIT:
            return UnitProcess.getPartyMember(TargetTag) != null;
            case TYPE.DISCOVERY_STAGE:
            return SaveMng.Status.DiscoveryStage.Exists(it => it.ToString() == TargetTag);
        }

        return false;
    }

    public bool execAddEvent() {
        switch (Type) {
            case TYPE.FLAG:
            SaveMng.AdvData.addFlag(TargetTag, Value);
            return true;
            case TYPE.FRIEND_SHIP:
            UnitProcess.addFriendShip(TargetTag, Value);
            return true;
            case TYPE.ITEM:
            return ItemProcess.addItem(TargetTag, Value);
            case TYPE.PARTY_UNIT:
            break;
            case TYPE.ALL_UNIT:
            UnitProcess.addMember(TargetTag);
            break;
            case TYPE.DISCOVERY_STAGE:
            SaveMng.Status.DiscoveryStage.Add(Value);
            break;
        }

        return false;
    }

    public bool execRemoveEvent() {
        switch (Type) {
            case TYPE.FLAG:
            //SaveMng.AdvData.addFlag(TargetTag, Value);
            return true;
            case TYPE.ITEM:
            //return ItemProcess.addItem(TargetTag, Value);
            case TYPE.PARTY_UNIT:
            break;
            case TYPE.ALL_UNIT:
            UnitProcess.removeMember(TargetTag);
            break;
        }
        return false;
    }

    public static IEnumerable<EventActionMast> List;

    public static void load() {
        List = load<EventActionMast>();
    }
}

