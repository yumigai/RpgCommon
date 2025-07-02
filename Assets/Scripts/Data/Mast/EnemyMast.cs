using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using System.Linq;

public class EnemyMast : UnitMast
{

    public int Size;
    //public int EvoLv;
    //public string[] EvoNames;

    //public int DropTableId; //撃破時のアイテムドロップテーブル
    public float DropPercent; //アイテムドロップ自体の確率

    public float ExpBonus;
    public float MoneyBonus;

    new public static IReadOnlyList<EnemyMast> List;

    new public static void load() {
        List = load<EnemyMast>();
    }


    //public static UnitStatusTran makeTran(int lv, int posi, int world_id, int element) {

    //    if (lv > MAX_LV) {
    //        lv = MAX_LV;
    //    }

    //    EnemyMast[] masts = Array.FindAll(List, it =>
    //    (int)it.Element == element
    //    && lv >= it.BaseLv && lv <= it.MaxLv && posi >= it.Size
    //    && Array.Exists(it.WorldIds, wid => wid == world_id || wid == 0)
    //    );

    //    if (masts.Length == 0) {
    //        return null;
    //    }

    //    int idx = UnityEngine.Random.Range(0, masts.Length);

    //    UnitStatusTran tran = getEnemy(masts[idx], lv);

    //    return tran;
    //}

    public static UnitStatusTran getEnemy(int id) {

        EnemyMast ene = List.First(it => it.Id == id);
        UnitStatusTran tran = getEnemy(ene, ene.BaseLv);

        return tran;
    }

    public new static UnitStatusTran getUnit(int id) {
        return getEnemy(id);
    }


    public static UnitStatusTran getEnemy(EnemyMast ene, int lv) {

        UnitStatusTran tran = getUnit(ene, lv, TYPE.ENEMY);

        //敵のユニットIDはデフォルトでランダム・念のためプレイヤーと被らないように+10000
        tran.Id = UnityEngine.Random.Range(10000, int.MaxValue);

        tran.addSkill();
        //tran.Status.Lif = (int)((float)tran.Status.Lif  * ene.HpBonus);
        //      tran.Status.Hp = tran.Status.MaxHp;
        tran.Exp = ene.getExp(lv);
        tran.Money = ene.getMoney(lv);

        tran.Name = ene.Name;

        //if( lv >= (ene.BaseLv + ene.EvoLv) && !UtilToolLib.IsNullOrEmpty(ene.EvoNames) && ene.EvoLv > 0) {
        //    int add_index = (lv - ene.BaseLv) / ene.EvoLv;
        //    add_index = Mathf.Clamp(add_index, 0, ene.EvoNames.Length - 1);
        //    tran.Name = ene.EvoNames[add_index];
        //}

        tran.Type = TYPE.ENEMY;

        return tran;
    }

    /// <summary>
    /// 獲得経験値計算
    /// </summary>
    /// <param name="lv"></param>
    /// <returns></returns>
    public int getExp(int lv) {
        int index = Mathf.Clamp(lv, 0, LevelMast.List.Length);
        return (int)(LevelMast.List[index].GetExp * ExpBonus);
    }

    /// <summary>
    /// 獲得金額計算
    /// </summary>
    /// <param name="lv"></param>
    /// <returns></returns>
    public int getMoney(int lv) {
        int index = Mathf.Clamp(lv, 0, LevelMast.List.Length);
        return (int)(LevelMast.List[index].GetMoney * MoneyBonus);
    }

}