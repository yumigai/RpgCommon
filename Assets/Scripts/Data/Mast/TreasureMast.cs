using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class TreasureMast : MasterCmn
{
	public int Id;
    public int TableId; //アイテムテーブル
    public string DropTableTab; //アイテムテーブルのタブ用
	public int ItemId;
    public float DropPer; //出現確率

	public static TreasureMast[] List;

    public ItemMast ItmMst {
        get {
            return ItemMast.getItem(ItemId);
        }
    }

    public enum DROP_CATEGORY {
        HEAL,
        SELL,
        ANY,
        ALL
    }
    public static readonly int[] DROP_PER = new int[(int)DROP_CATEGORY.ALL] {
        20,
        40,
        30
    };

 //   public static TreasureMast getItemRate( QuestTran quest ){

	//	int max_rate = quest.Stage.TreasureMin +  (int)( quest.Stage.TreasurUpd * (float)quest.NowFloorNum );
	//	int get_rate = Random.Range( quest.Stage.TreasureMin, max_rate );

	//	TreasureMast[] list = System.Array.FindAll (TreasureMast.List, tl => quest.Stage.TreasureMin >= tl.Rate && tl.Rate <= max_rate && ( tl.DungeonId <= 0 || tl.DungeonId == quest.Stage.Id ) );

 //       int cate_index = UtilToolLib.getRateRandom(DROP_PER);
 //       if(cate_index != (int)DROP_CATEGORY.ANY) {
 //           ItemMast.CATEGORY cate = cate_index == (int)DROP_CATEGORY.HEAL ? ItemMast.CATEGORY.HEAL : ItemMast.CATEGORY.SELL;
 //           list = System.Array.FindAll(list, it => it.ItmMst.Category == cate);
 //       }

	//	if (list == null || list.Length == 0) {
	//		return null;
	//	}

	//	int index = Random.Range (0, list.Length);

	//	return list[index];

	//}

    public static void load()
    {
        List = load<TreasureMast>();
    }


}

