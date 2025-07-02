using UnityEngine;
using System.Linq;
using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public class ShopItemMast : MasterCmn {

	public int Id;
	public int ShopId;
	public int ItemId;
	public int SellPrice;
	public int SellLv;
	public int SellClearStageNum; //購入可能な進行度（店が一つしかない系のゲームで使用する）

    private ItemMast _Mast;
	
	public static ShopItemMast[] List;

    public ItemMast ItmMst {
        get {
            if (_Mast == null) {
                _Mast = ItemMast.getItem(ItemId);
            }
            return _Mast;
        }
    }

    public static void load(){
		List = load<ShopItemMast> ();
	}

    /// <summary>
    /// カテゴリ別ショップアイテム取得
    /// </summary>
    /// <param name="category"></param>
    /// <returns></returns>
    public static ShopItemMast[] getCategoryShopItems(ItemMast.CATEGORY category) {
        return System.Array.FindAll(List, it => it.ItmMst != null && it.ItmMst.Category == category);
    }
}
