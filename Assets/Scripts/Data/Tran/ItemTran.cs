using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

[System.Serializable]
public class ItemTran {

    private const float LV_ADJ = 0.25f;

    private const float RANDOM_MIN_LV_ADJ = 0.75f;

    public int Id;
    public int Num;
    public int MasterId;
    public int BaseLv;//レベル（取得時固定値。ローグ系で同型の武器でも高レベル時に取得した場合は強いアレ）
    public int EnhanceLv; //強化レベル
    public int Value;
    public int SubValue;

    public List<RuneTran> Runes = new List<RuneTran>();

    private ItemMast _Mst;

    public ItemTran() {

    }
    public ItemTran(ItemMast mst, int lv = 0) {
        MasterId = mst.Id;
        Num = 1;
        BaseLv = lv;
        Value = getValue(mst.BaseValue);
        SubValue = getValue(mst.SubValue);
    }

    public string Name {
        get {
            if (Mst == null) {
                return "";
            } else {
                return Mst.Name;
            }
        }
    }
    public string NameAndBonus {
        get {
            if (Mst == null) {
                return "";
            } else {
                return string.Format("{0}+{1}",Mst.Name,EnhanceLv);
            }
        }
    }

    public int Rarity {
        get {
            if (Mst == null) {
                return 0;
            } else {
                return Mst.Rarity;
            }
        }
    }


    public string Icon {
        get {
            if (Mst == null) {
                return "";
            } else {
                return Mst.Icon;
            }
        }
    }
    //public ItemMast.WEIGHT Weight {
    //    get {
    //        if (Mst == null) {
    //            return ItemMast.WEIGHT.LIGHT;
    //        } else {
    //            return Mst.Weight;
    //        }
    //    }
    //}
    public ItemMast.RANGE Range {
        get {
            if (Mst == null) {
                return ItemMast.RANGE.SHORT;
            } else {
                return Mst.Range;
            }
        }
    }


    public string BonusText {
        get {
            if (EnhanceLv > 0) {
                return "+" + EnhanceLv;
            } else if( EnhanceLv < 0 ){
                return "-" + EnhanceLv;
            } else {
                return "±";
            }
        }
    }
    public string Detail {
        get {
            if (Mst == null) {
                return "";
            } else {
                return Mst.Detail;
            }
        }
    }
    public string CategoryDetailName {
        get {
            if (Mst == null) {
                return "";
            } else {
                return Mst.CategoryDetailName;
            }
        }
    }

    public ItemMast Mst {
        get {
            if(_Mst == null) {
                _Mst = ItemMast.getItem(MasterId);// System.Array.Find(ItemMast.List, it => it.Id == MasterId);
            }
            return _Mst;
        }
    }

    /// <summary>
    /// 数値設定
    /// </summary>
    /// <param name="base_value"></param>
    private int getValue( float base_value ) {
        //var val = (base_value + EnhanceLv) + (base_value * ENHNCE_ADJ * EnhanceLv);
        var val = base_value;
        if(base_value > 0) {
            int lv = Mathf.Clamp( BaseLv + EnhanceLv, 0, LevelMast.MAX_LV);
            val += LevelMast.GetParam(lv, Rarity) * LV_ADJ;
        }
        return (int)val;
    }

    public static ItemTran getRandomItem(int maxlv, bool addRune = true) {
        var category = UtilToolLib.getRateRandom(GameConst.DROP_CATEGORY_PERCENT);
        return getRandomItem(maxlv, category, addRune);
    }

    /// <summary>
    /// ランダム取得
    /// </summary>
    /// <param name="lv"></param>
    /// <param name="category"></param>
    /// <param name="addRune"></param>
    /// <returns></returns>
    public static ItemTran getRandomItem(int maxlv, int category,  bool addRune = true) {

        int rare = UtilToolLib.getRateRandom(GameConst.RARITY_PERCENT);

        int lv = Random.Range((int)(maxlv * RANDOM_MIN_LV_ADJ), maxlv);
        var targets = ItemMast.List.Where(r => r.Lv <= lv && rare >= r.Rarity && (int)r.Category == category);

        if (targets.Count() == 0) {
            return null;
        }

        int get_index = Random.Range(0, targets.Count());
        var mst = targets.ElementAt(get_index);

        return getItemTran(mst,lv,addRune);
    }

    /// <summary>
    /// アイテム取得
    /// </summary>
    /// <param name="mst"></param>
    /// <param name="rare"></param>
    /// <param name="addRune"></param>
    /// <returns></returns>
    public static ItemTran getItemTran(ItemMast mst, int lv, bool addRune = true) {

        var tran = new ItemTran(mst, lv);

        if (mst.isEquip && addRune && tran.Rarity >= (int)GameConst.RUNE_MINIMAM_RARITY) {
            for (int i = 0; i < GameConst.MAX_RUNE_EQUIP; i++) {
                if (i > 0 && tran.Rarity <= (int)GameConst.RUNE_MULTI_RARITY) {
                    //複数ルーンは指定レアリティ以上
                    break;
                }
                var rune = RuneTran.getRandomRune(mst.Lv);
                if (rune != null && !tran.Runes.Exists(it => it.MasterId == rune.MasterId)) {
                    //ルーン抽選に成功し、既存のルーンに被ってない場合、追加
                    tran.Runes.Add(rune);
                }
            }
        }
        return tran;
    }
}