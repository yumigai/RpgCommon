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

    public float HpBonus;
    public float ExpBonus;
    public float MoneyBonus;

    public float MajiMode = 0f;
    public string HenshinImg;
    public string[] SpecialSkills = new string[] { };
    public int[] SpecialCounts = new int[] { };

    public float UseSkillPercent;

    new public static IReadOnlyList<EnemyMast> List;

    public int MaxSpecialCount { get { return SpecialCounts.Max(); } }

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
        return getEnemy(ene, ene.BaseLv);
    }

    public static UnitStatusTran getEnemy(string tag){
        EnemyMast ene = List.First(it => it.Tag == tag);
        return getEnemy(ene, ene.BaseLv);
    }

    public new static UnitStatusTran getUnit(int id) {
        return getEnemy(id);
    }

    public static UnitStatusTran getEnemy(EnemyMast ene, int lv) {

        UnitStatusTran tran = getUnit(ene, lv, TYPE.ENEMY);

        //敵のユニットIDはデフォルトでランダム・念のためプレイヤーと被らないように+10000
        tran.Id = UnityEngine.Random.Range(10000, int.MaxValue);

        if( ene.SpecialSkills != null) {
            for( int i = 0; i < ene.SpecialSkills.Length; i++) {
                tran.addSkill(ene.SpecialSkills[i]);
            }
        }

        if (ene.HpBonus > 0) {
            tran.Status.MaxHp = (int)(tran.Status.MaxHp * ene.HpBonus);
            tran.Status.Hp = tran.Status.MaxHp;
        }

        //tran.Status.Lif = (int)((float)tran.Status.Lif  * ene.HpBonus);
        //      tran.Status.Hp = tran.Status.MaxHp;
        tran.Exp = ene.getExp(lv);
        tran.Money = ene.getMoney(lv);

        tran.Name = ene.Name;
        tran.NameEn = ene.NameEn;

        //if( lv >= (ene.BaseLv + ene.EvoLv) && !UtilToolLib.IsNullOrEmpty(ene.EvoNames) && ene.EvoLv > 0) {
        //    int add_index = (lv - ene.BaseLv) / ene.EvoLv;
        //    add_index = Mathf.Clamp(add_index, 0, ene.EvoNames.Length - 1);
        //    tran.Name = ene.EvoNames[add_index];
        //}

        tran.Type = TYPE.ENEMY;

        return tran;
    }

    /// <summary>
    /// free encount 
    /// </summary>
    /// <param name="lv"></param>
    /// <param name="max_num"></param>
    public static List<UnitStatusTran> freeEncount( int lv, int field_size) {

        var max_lv = List.Where(it => it.BaseLv <= lv).Max(it => it.BaseLv);
        var enemys = List.Where(it => it.BaseLv == max_lv).ToList();

        var units = new List<UnitStatusTran>();

        int fill = 0;

        for ( int i = 0; i < field_size && fill < field_size; i++){
            var index = UnityEngine.Random.Range(0, enemys.Count());
            var tran = EnemyMast.getEnemy(enemys[index], lv);
            units.Add(tran);
            fill += enemys[index].Size;
        }
        return units;
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