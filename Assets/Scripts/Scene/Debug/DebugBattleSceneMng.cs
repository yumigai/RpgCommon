using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Assertions.Must;

public class DebugBattleSceneMng : MonoBehaviour
{

    public int PlayCount = 1;

    public string UnitIds = "";

    public int UnitLv = 0;

    public string EnemyIds = "";

    public int MapId = 0;

    public int FieldSize = 6;

    public bool IsPlayerReflesh;

    private List<UnitStatusTran> Players;
    private List<UnitStatusTran> Enemys;

    private int WinCount = 0;

    private BattleProc.ActionData Action {
        get {
            return BattleProc.BattleAction;
        }
    }

    // Start is called before the first frame update
    void Start() {

    }

    public void pushStart() {
        WinCount = 0;

        for (var h = 0; h < PlayCount; h++) {
            Setting();

            for (int i = 0; i < 100; i++) {
                if (turn()) {
                    break;
                }
            }

            if (SaveMng.Quest.IsGameOver) {
                break;
            }
        }

        Debug.Log("WinCount : " + WinCount);

    }

    private void Setting() {
        if (MapId > 0) {
            Enemys = EnemyEncountMast.encount(MapId, FieldSize);
            EnemyIds = "";
            foreach (var ene in Enemys) {
                EnemyIds += ene.Mst.Id.ToString() + ",";
            }
            EnemyIds = EnemyIds.Substring(0, EnemyIds.Length - 1);
        }

        reflesh();

        if (Enemys.Count == 0) {
            Debug.LogError("Enemys is not match");
        }



    }

    private void reflesh() {
        if (Players == null) {
            Players = new List<UnitStatusTran>();
            var pids = UtilToolLib.purgeNumber(UnitIds);
            foreach (var id in pids) {
                var pl = UnitMast.getUnit(id);
                pl.Tactics = AiProc.TACTICS.FREE;
                pl.setLevel(UnitLv);
                Players.Add(pl);
            }
            SaveMng.Quest.ActiveParty = Players;
        }

        if (IsPlayerReflesh) {
            foreach (var pl in Players) {
                pl.Status.heal(pl.Status.MaxHp, pl.Status.MaxHp, StatusMast.TYPE.HP);
                pl.Status.heal(pl.Status.MaxMp, pl.Status.MaxMp, StatusMast.TYPE.MP);
            }
        }

        playerEquip();

        Enemys = new List<UnitStatusTran>();

        var ids = UtilToolLib.purgeNumber(EnemyIds);
        var enemy_names = "";
        foreach (var id in ids) {
            var ene = EnemyMast.getEnemy(id);
            Enemys.Add(ene);
            ene.Tactics = AiProc.TACTICS.FREE;
            enemy_names += "p:" + ene.Name + " (lv:" + ene.Lv + ") ";
        }
        Debug.Log(enemy_names);
        SaveMng.Quest.Enemys = Enemys;

        BattleProc.Init();
        BattleProc.ready();
    }

    private void playerEquip() {
        foreach (var pl in Players) {
            pl.EquipWeapon = randomEquip(ItemMast.CATEGORY.WEAPON);
            pl.EquipArmor = randomEquip(ItemMast.CATEGORY.ARMOR);
            pl.EquipAccessory1 = randomEquip(ItemMast.CATEGORY.ACCESSORY);
            pl.EquipAccessory2 = randomEquip(ItemMast.CATEGORY.ACCESSORY);
            //Debug.Log("equip : " + pl.Name + " w:" + pl.EquipWeapon.Name + " a:" + pl.EquipArmor.Name);
            Debug.Log(string.Format("equip : {0} w:{1}({2}) a:{3}({4})",
            pl.Name, pl.EquipWeapon.Name, pl.EquipWeapon.BaseLv, pl.EquipArmor.Name, pl.EquipArmor.BaseLv));
        }
    }

    private ItemTran randomEquip(ItemMast.CATEGORY category) {
        return ItemTran.getRandomItem(UnitLv, (int)category, false);
    }

    public bool turn() {
        BattleProc.RESULT result;

        if (BattleProc.ActionUnit.Hp > 0) {
            result = BattleProc.battle();
            if (Action.Skill != null) {
                Debug.Log(Action.Skill.Name);
            }
            for (var i = 0; i < Action.Def.Count && i < Action.Dmg.Count; i++) {
                Debug.Log("at : " + Action.Atk.Name + " → "
            + Action.Def[i].Name + " = " + Action.Dmg[i]);
            }

        } else {
            result = BattleProc.endProcess();
        }

        if (result != BattleProc.RESULT.CONTINUE) {
            if (result == BattleProc.RESULT.WIN) {
                WinCount++;
            }
            Debug.Log(result);
            return true;
        }
        if (BattleProc.isTurnStart()) {
            BattleProc.ready();
        }
        return false;
    }
}
