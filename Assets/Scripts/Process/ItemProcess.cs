using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ItemProcess
{
	//�J�e�S���Ⴂ�̕K�v�f��
    // J e S   Ⴂ ̕K v f  
    public const int NEED_MATERIAL_OTHER_CATEGORY = 4;

    /// <summary>
    ///          ۂ̓   f  
    /// </summary>
    /// <param name="tran"></param>
    /// <returns></returns>
    public static Dictionary<int, int> getMaterials(ItemTran tran) {
        var mNums = new Dictionary<int, int>();

        var mates = ItemMast.List.Where(it =>
        it.Category == ItemMast.CATEGORY.MATERIAL
        && it.Rarity <= tran.Rarity
        && it.MaterialType != ItemMast.MATERIAL_TYPE.NON
        && it.MaterialType == tran.Mst.MaterialType);

        foreach (var m in mates) {
            var diff = tran.Rarity - m.Rarity + 1;
            var num = Random.Range(1, tran.BaseLv) * diff;
            mNums.Add(m.Id, num);
        }

        return mNums;
    }

    /// <summary>
    ///  K v f ނ̌v Z
    /// </summary>
    /// <param name="tran"></param>
    /// <returns></returns>
    public static Dictionary<int, int> needMaterials(ItemTran tran) {
        var mNums = new Dictionary<int, int>();

        var mates = ItemMast.List.Where(it =>
        it.Category == ItemMast.CATEGORY.MATERIAL
        && it.Rarity <= tran.Rarity);

        foreach (var m in mates) {
            var lv = tran.BaseLv;
            var num = Mathf.FloorToInt(lv + (lv * (lv / (tran.BaseLv + 1))));
            if (tran.Mst.Category != m.Category) {
                num /= NEED_MATERIAL_OTHER_CATEGORY;
            }
            mNums.Add(m.Id, num);
        }

        return mNums;
    }

    public static int addItem(List<ItemTran> bag, List<ItemTran> adds) {

        int count = 0;

        foreach (var item in adds) {
            int index = bag.FindIndex(it => it.Mst.checkOverRideNum() && it.MasterId == item.MasterId);
            if (index >= 0) {
                if (bag[index].Num < GameConst.MAX_ITEM_NUM) {
                    bag[index].Num += item.Num;
                    count++;
                }
            } else {
                bag.Add(item);
                count++;
            }
        }

        return count;
    }

    public static bool addItem(string tag, int num = 1) {
        ItemTran tran = getTranData(tag);
        return addItem(tran, num);
    }

    /// <summary>
    /// アイテム取得（マスタ）
    /// </summary>
    /// <param name="mast_id"></param>
    /// <param name="num"></param>
    /// <returns></returns>
    public static bool addItem(int mast_id, int num = 1) {
        ItemTran tran = getTranFromMasterId(mast_id);
        return addItem(tran, num);
    }

    /// <summary>
    /// アイテム取得
    /// </summary>
    /// <param name="itm"></param>
    /// <param name="num"></param>
    /// <returns></returns>
    public static bool addItem(ItemTran itm, int num = 1) {

        if (SaveMng.Items.Count >= GameConst.MAX_ITEM_KIND_NUM) {
            return false;
        }

        itm.Id = getNewId();

        int index = getOverRideIndex(itm);
        if (index >= 0) {
            if (SaveMng.Items[index].Num + num > GameConst.MAX_ITEM_NUM) {
                return false;
            }
            SaveMng.Items[index].Num += num;
        } else {
            SaveMng.Items.Add(itm);
        }

        return true;
    }

    private static int getNewId() {
        int new_id = 1;
        if (SaveMng.Items.Count > 0) {
            new_id = SaveMng.Items.Select(s => s.Id).Max();
            if (new_id < int.MaxValue - 1) {
                new_id++;
            } else {
                new_id = Random.Range(1, int.MaxValue);
            }
        }
        return new_id;

    }

    public static int getOverRideIndex(ItemTran itm) {

        if (itm.Mst.checkOverRideNum()) {
            return SaveMng.Items.FindIndex(it => it.MasterId == itm.MasterId);
        }
        return -1;

    }

    public static ItemTran getTranFromMasterId(int mast_id) {
        ItemMast mst = ItemMast.getItem(mast_id);
        return new ItemTran(mst);
    }

    public static ItemTran getTranData(string tag) {
        ItemMast mst = ItemMast.getItem(tag);
        return new ItemTran(mst);
    }


    #region BulkItemDeclutter

    public static void allDrop(GameConst.RARITY rarity, ItemMast.CATEGORY category) {
        allItemProcess(rarity, category, (ItemTran itm) => {
            SaveMng.ItemData.lostItem(itm, itm.Num);
        });
    }
    public static void allSell(GameConst.RARITY rarity, ItemMast.CATEGORY category) {
        allItemProcess(rarity, category, (ItemTran itm) => {
            SaveMng.ItemData.sellItem(itm, itm.Num);
        });
    }

    public static void allExchange(GameConst.RARITY rarity, ItemMast.CATEGORY category) {
        allItemProcess(rarity, category, (ItemTran itm) => {
            SaveMng.ItemData.exchangeItem(itm, itm.Num);
        });
    }

    private static void allItemProcess(GameConst.RARITY rarity, ItemMast.CATEGORY category, System.Action<ItemTran> callback) {
        var items = getItems(rarity, category);
        foreach (var itm in items) {
            callback(itm);
        }
    }

    private static List<ItemTran> getItems(GameConst.RARITY rarity, ItemMast.CATEGORY category) {
        return SaveMng.Items.FindAll(it => it.Rarity == (int)rarity && it.Mst.Category == category);
    }

    public static void allDropEquip(GameConst.RARITY rarity) {
        allDrop(rarity, ItemMast.CATEGORY.WEAPON);
        allDrop(rarity, ItemMast.CATEGORY.ARMOR);
        allDrop(rarity, ItemMast.CATEGORY.ACCESSORY);
    }
    #endregion

}