using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class ItemMast : PowerMast
{
    /// <summary>
    /// 分類
    /// </summary>
    public enum CATEGORY
    {
        UNNOWN,
        WEAPON,
        ARMOR,
        ACCESSORY,
        CONSUMABLE, //使用・消耗品
 //		EVENT, //貴重品・イベントアイテム（売却管理とか面倒だから別枠で管理）
//		COLLECTION, //ゲーム進行に関係ない収集アイテム（売却管理とか面倒だから別枠で管理）
        MATERIAL, //素材・売却用アイテム
        ANY, //いずれか（全てのアイテムを含む）
        ALL
    }

    public enum MATERIAL_TYPE
    {
        NON,
        METAL,
        WOOD,
        LEATHER,
        CLOTH,
        NOBLE_METAL,
    }

    ///// <summary>
    ///// レアリティ
    ///// </summary>
    //public enum RARITY
    //{
    //    N,
    //    R,
    //    SR,
    //    SSR,
    //    UR,
    //    ALL
    //}

    public static readonly string[] CATEGORY_JP = new string[(int)CATEGORY.ALL] {
        "不明",
        "武器",
        "防具",
        "装飾品",
        "消耗品", //使用・消耗品
//		"貴重品", //貴重品・イベントアイテム
//		"コレクション", //ゲーム進行に関係ない収集アイテム
		"雑貨", //素材・売却用アイテム
		"全て", //いずれか（全てのアイテムを含む）
    };

    public static readonly string[] CATEGORY_EN = new string[(int)CATEGORY.ALL] {
        "UNNOWN",
        "WEAPON",
        "ARMOR",
        "ACCESSORY",
        "CONSUMABLE", //使用・消耗品
//		"EVENT", //貴重品・イベントアイテム
//		"COLLECTION", //ゲーム進行に関係ない収集アイテム
		"SUNDRIES", //素材・売却用アイテム
		"ANY", //いずれか（全てのアイテムを含む）
    };

    public int Lv; //推奨・対象となるレベル帯

    public int Rarity;
    public int MaxEnhance;
    public float BaseValue {
        get {
            return Values[0];
        }
    }
    public float SubValue {
        get {
            return Values[1];
        }
    }
    public float[] Values; //基本的に直接使用しない方向で
    public int Cost;
    public CATEGORY Category;

    public UnitMast.JOB[] EquipJob; // 装備・使用可能ジョブ
    public string CategoryDetailName;
    //public int MaxNum; 使わんかな？
    public string SkillTag; //特殊な効果を持つアイテム（攻撃スキルや毒付与など）
    public bool CanDrop = true; //捨てれるor売れるか

    public MATERIAL_TYPE MaterialType;

    private SkillMast _Skill;
    private bool SkillCheckd;

    public static IReadOnlyList<ItemMast> List;

    public SkillMast Skill {
        get {
            if (_Skill == null && !SkillCheckd) {
                _Skill = FindByTag(SkillMast.List, SkillTag);
            }
            SkillCheckd = true;
            return _Skill;
        }
    }


    public bool isEquip {
        get {
            switch (Category) {
                case CATEGORY.WEAPON:
                case CATEGORY.ARMOR:
                case CATEGORY.ACCESSORY:
                return true;
                default:
                return false;
            }
        }
    }

    public static void load() {
        List = load<ItemMast>();
    }

    /// <summary>
    /// 最初から高レベルアイテムをランダムで付与する場合
    /// ※基本的に使わない
    /// </summary>
    /// <returns></returns>
    public int getInitBonus() {
        int[] rates = Enumerable.Range(0, MaxEnhance).ToArray();
        int total = rates.Sum();
        return UtilToolLib.getRateRandom(0, total, rates, 0);
    }

    public static ItemMast getItem(int id) {
        return FindById(List, id);
    }

    public static ItemMast getItem(string tag) {
        return FindByTag(List, tag);
    }

    /// <summary>
    /// 数値管理できるか
    /// </summary>
    /// <returns></returns>
    public bool checkOverRideNum() {
        return !isEquip; // 現状、装備品以外は数値管理可能
        //if (System.Array.IndexOf(CanOverRideNum, Category) >= 0) {
        //    return true;
        //}
        //return false;
    }

    /// <summary>
    /// 効果タイプ名取得
    /// </summary>
    /// <returns></returns>
    public string getEffectName(bool is_main = true) {
        switch (Spec) {
            case SPEC.ATTACK:
            if (is_main) {
                return "攻撃力";
            } else {
                return "魔法攻撃力";
            }
            case SPEC.DEFENCE:
            if (is_main) {
                return "防御力";
            } else {
                return "魔法防御力";
            }
            case SPEC.HEAL:
            //if (Skill != null) {
            //    return Skill.getEffectName();
            //}
            break;
        }
        return "";
    }

    /// <summary>
    /// カテゴリ名取得
    /// </summary>
    /// <param name="category"></param>
    /// <returns></returns>
    public static string GetCategoryName(CATEGORY category) {
        if (CmnSaveProc.IsJp) {
            return CATEGORY_JP[(int)category];
        } else {
            return CATEGORY_EN[(int)category];
        }
    }
}