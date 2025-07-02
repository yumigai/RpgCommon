using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulkItemDeclutter
{
    public void allDrop(GameConst.RARITY rarity, ItemMast.CATEGORY category) {
        allItemProcess(rarity, category, (ItemTran itm)=>{
            SaveMng.ItemData.lostItem(itm,itm.Num);
        });
    }

    public void allSell(GameConst.RARITY rarity, ItemMast.CATEGORY category) {
        allItemProcess(rarity, category, (ItemTran itm)=>{
            SaveMng.ItemData.sellItem(itm,itm.Num);
        });
    }

    public void allExchange(GameConst.RARITY rarity, ItemMast.CATEGORY category) {
        allItemProcess(rarity, category, (ItemTran itm)=>{
            SaveMng.ItemData.exchangeItem(itm, itm.Num);
        });
    }

    private void allItemProcess(GameConst.RARITY rarity, ItemMast.CATEGORY category, System.Action<ItemTran> callback) {
        var items = getItems(rarity, category);
        foreach (var itm in items) {
            callback(itm);
        }
    }

    private List<ItemTran> getItems(GameConst.RARITY rarity, ItemMast.CATEGORY category) {
        return SaveMng.Items.FindAll(it => it.Rarity == (int)rarity && it.Mst.Category == category);
    }

    public void allDropEquip(GameConst.RARITY rarity) {
        allDrop(rarity, ItemMast.CATEGORY.WEAPON);
        allDrop(rarity, ItemMast.CATEGORY.ARMOR);
        allDrop(rarity, ItemMast.CATEGORY.ACCESSORY);
    }



}
