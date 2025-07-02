using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

/// <summary>
/// ドロップアイテム
/// 管理面倒だし使わんかも（lvによるランダムの方が楽）
/// </summary>
public class DropItemMast : MasterCmn
{
    public int Id;
    public int DropTableId;
    public int ItemId;
    public float Percent;

    public static DropItemMast[] List;

    public static void load() {
        List = load<DropItemMast>();
    }

    /// <summary>
    /// ドロップアイテム判定
    /// </summary>
    /// <param name="table_id"></param>
    /// <returns></returns>
    public static ItemMast judgeDropItem(int table_id) {

        var category = UtilToolLib.getRateRandom(GameConst.DROP_CATEGORY_PERCENT);

        var drops = System.Array.FindAll(List, it => it.DropTableId == table_id);

        drops.Select(it => ItemMast.getItem(it.ItemId));

        var items = ItemMast.List.Where(it => (int)it.Category == category).Where(it => drops.Any(it2 => it.Id == it2.ItemId));
        var count = items.Count();
        if (count <= 0) {
            return null;
        }

        var index = UnityEngine.Random.Range(0, count);

        return items.ElementAt(index);
    }
}
