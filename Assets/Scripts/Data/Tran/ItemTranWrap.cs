using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ItemTranWrap : SaveMng.SaveClass
{

    public List<ItemTran> DataList = new List<ItemTran>();
    public ItemTran getData(int id) {
        return DataList.Find(d => d.Id == id);
    }
    public ItemTranWrap() {
        DataList = new List<ItemTran>();
    }

    public ItemTran getTranData(int mast_id) {

        ItemMast mst = ItemMast.getItem(mast_id);
        ItemTran itm = new ItemTran(mst);

        //itm.Level = mst.getInitBonus();
        return itm;
    }

    /// <summary>
    /// アイテム取得（マスタ）
    /// </summary>
    /// <param name="mast_id"></param>
    /// <param name="num"></param>
    /// <returns></returns>
    public bool addItem(int mast_id, int num = 1) {
        ItemTran tran = getTranData(mast_id);
        return addItem(tran, num);
    }

    /// <summary>
    /// アイテム取得
    /// </summary>
    /// <param name="itm"></param>
    /// <param name="num"></param>
    /// <returns></returns>
    public bool addItem(ItemTran itm, int num = 1) {

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

    private int getNewId() {
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

    public bool useItem(int id) {
        var item = SaveMng.ItemData.getData(id);
        return useItem(item);
    }

    public bool useItem(ItemTran itm) {
        if (itm != null) {
            if (itm.Mst.Category == ItemMast.CATEGORY.CONSUMABLE) {
                //現状は消耗品のみ

                lostItem(itm, 1);

                return true;
            }
        }
        return false;
    }

    public bool lostItem(ItemTran itm, int num = 1) {
        int index = SaveMng.Items.IndexOf(itm);
        if (index >= 0) {
            if (SaveMng.Items[index].Num > num) {
                SaveMng.Items[index].Num -= num;
            } else {
                SaveMng.Items.RemoveAt(index);
            }
            return true;
        }
        return false;
    }

    /// <summary>
    /// アイテム購入
    /// </summary>
    /// <param name="mast"></param>
    /// <param name="num"></param>
    /// <returns></returns>
    public bool buyItem(ShopItemMast mast, int num = 1) {
        var tran = SaveMng.Items.FirstOrDefault(it => it.MasterId == mast.ItemId);
        if (tran == null || tran.Num + num <= GameConst.MAX_ITEM_NUM) {
            int price = mast.SellPrice * num;
            SaveMng.Status.subMoney(price);
            SaveMng.ItemData.addItem(mast.ItemId, num);
            return true;
        }
        return false;
    }

    /// <summary>
    /// アイテム売却
    /// </summary>
    /// <param name="itm"></param>
    /// <param name="num"></param>
    /// <returns></returns>
    public bool sellItem(ItemTran itm, int num) {
        if (itm.Num >= num) {
            int price = itm.Mst.Cost * num;
            SaveMng.Status.addMoney(price);
            SaveMng.ItemData.lostItem(itm, num);
            return true;
        }
        return false;
    }

    /// <summary>
    /// カテゴリ全売却
    /// </summary>
    /// <param name="cate"></param>
    public void allSell(ItemMast.CATEGORY cate) {
        List<ItemTran> itms = SaveMng.Items.FindAll(it => it.Mst.Category == cate);
        for (int i = 0; i < itms.Count; i++) {
            sellItem(itms[i], itms[i].Num);
        }
    }

    /// <summary>
    /// 一括売却時の価格取得用
    /// </summary>
    /// <param name="cate"></param>
    /// <returns></returns>
    public int totalPrice(ItemMast.CATEGORY cate) {
        int total = SaveMng.Items.Where(it => it.Mst.Category == cate).Sum(item => (item.Mst.Cost * item.Num));
        return total;

    }

    public bool exchangeItem(ItemTran itm, int num) {
        return false;
    }

    public List<ItemTran> getMasterItem(int master_id) {
        return SaveMng.Items.FindAll(it => it.MasterId == master_id);
    }

    public void sortCategory() {
        SaveMng.Items.Sort((it1, it2) => (int)it1.Mst.Category - (int)it2.Mst.Category);
    }
    public void sortRarity() {
        SaveMng.Items.Sort((it1, it2) => (int)it2.Rarity - (int)it1.Rarity);
    }
    public int getOverRideIndex(ItemTran itm) {

        if (itm.Mst.checkOverRideNum()) {
            return SaveMng.Items.FindIndex(it => it.MasterId == itm.MasterId);
        }
        return -1;

    }
}