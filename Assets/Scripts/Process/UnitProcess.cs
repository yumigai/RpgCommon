using System.Collections;
using System.Collections.Generic;
using System.Linq;

public static class UnitProcess
{
    public static bool addFriendShip(string unitTag, int val) {
        var unit = SaveMng.Units.Find(it => it.Mst.Tag == unitTag);
        if (unit != null) {
            unit.FriendShip += val;
            return true;
        }
        return false;
    }

    public static int getFriendShip(string unitTag) {
        var unit = SaveMng.Units.Find(it => it.Mst.Tag == unitTag);
        if (unit == null) {
            return 0;
        }
        return unit.FriendShip;
    }

    public static UnitStatusTran getAllyMember(string unitTag) {
        if (SaveMng.Units != null) {
            return SaveMng.Units.Find(it => it.Mst.Tag == unitTag);
        }
        return null;
    }

    public static UnitStatusTran getPartyMember(string unitTag) {
        if (SaveMng.ActiveUnits != null) {
            return SaveMng.ActiveUnits.Find(it => it.Mst.Tag == unitTag);
        }
        return null;
    }

    public static bool addMember(string unitTag) {
        if (SaveMng.Units != null && !SaveMng.Units.Exists(it => it.Mst.Tag == unitTag)) {
            var mst = UnitMast.List.FirstOrDefault(it => it.Tag == unitTag);
            var unit = UnitMast.getUnit(mst);
            SaveMng.Units.Add(unit);
            return true;
        }
        return false;
    }

    public static bool removeMember(string unitTag) {
        var tran = getAllyMember(unitTag);
        if (tran != null) {
            SaveMng.Units.Remove(tran);
            return true;
        }
        return false;
    }

    public static List<string> LeanSkills(UnitStatusTran unit) {
        var leand = SkillLeanMast.List.Where(it =>
            it.Type == SkillLeanMast.TYPE.UNIT
            && it.UserTag == unit.Mst.Tag
            && it.LeanLevel <= unit.Lv)
            .Select(it => it.SkillTag).ToList();
        return leand;
    }

    public static void addExp(int exp) {
        if (SaveMng.ActiveUnits != null) {
            foreach (var unit in SaveMng.ActiveUnits) {
                unit.addExp(exp);
            }
        }
        SaveMng.UnitData.save();
    }

    public static void memberAllDamage(int num, bool isPercent = true) {
        if (SaveMng.ActiveUnits != null) {
            foreach (var unit in SaveMng.ActiveUnits) {
                int dam = isPercent ? unit.MaxHp * num / 100 : num;
                unit.damage(dam);
                //現状、0にはしない
                unit.Status.Hp = unit.Hp <= 0 ? 1 : unit.Hp;
            }
        }
    }

}