using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ItemDetailWindowMng : ConfirmWindowCmn {

    public enum BUTTON_TYPE {
        USE,
        SELL,
        POWER_UP,
        EQUIP,
        ALL
    }

    [SerializeField]
    public Text NowNum;

    [SerializeField]
    public Button[] Buttons = new Button[(int)BUTTON_TYPE.ALL];

    private ItemTran Item { get { return BaseItemListSceneMng.Singleton.SelectedItem; } }
    private BaseItemListSceneMng Board { get { return BaseItemListSceneMng.Singleton; } }

	void OnEnable(){

        for( int i = 0; i < Buttons.Length; i++) {
            Buttons[i].gameObject.SetActive(false);
        }

        switch (Board.Mode) {
            case ItemBoardMng.MODE.VIEW:
                Buttons[(int)BUTTON_TYPE.SELL].gameObject.SetActive(true);
                break;
            case ItemBoardMng.MODE.USE:
                Buttons[(int)BUTTON_TYPE.USE].gameObject.SetActive(true);
                break;
        }

        switch (Item.Mst.Category) {
            case ItemMast.CATEGORY.WEAPON:
            case ItemMast.CATEGORY.ARMOR:
            case ItemMast.CATEGORY.ACCESSORY:
                if (Board.Mode == ItemBoardMng.MODE.POWER_UP || Board.Mode == ItemBoardMng.MODE.VIEW) {
                    Buttons[(int)BUTTON_TYPE.POWER_UP].gameObject.SetActive(true);
                }
                break;
        }

        setFullInfo(Item.Name, Item.Mst.Detail, CmnConst.Path.ICON_ITEM_PATH + Item.Mst.Icon, Item.BonusText );
        NowNum.text = Item.Num.ToString();

    }

	public void Close(){
        CommonProcess.playCanselSe();
        this.gameObject.SetActive (false);
	}

}
