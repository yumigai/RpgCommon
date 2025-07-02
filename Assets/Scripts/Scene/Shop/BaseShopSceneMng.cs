using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class BaseShopSceneMng : Base2DSceneMng {

    [SerializeField]
    private GamePadListRecivMng CategoryReciv;

    [SerializeField]
    private GamePadListRecivMng ShopItemListReciv;

    [SerializeField]
    private MultiUseScrollMng ShopItemScroll;

    [SerializeField]
    private Text HeadInfo;

    [SerializeField]
    public ConfirmAdvanceWindowCmn ConfirmWnd;

    private ShopItemMast SelectedItem;

    private void Awake() {
        
        
    }

    private void Start() {
        base.Start();
        init();
    }

    private void init() {

        ConfirmWnd.gameObject.SetActive(false);
        toSelectCategory();
    }

    public void toSelectCategory() {
        HeadInfo.text = "";
        GamePadListRecivMng.ActiveGamePadList = CategoryReciv;
    }

    [EnumAction(typeof(ItemMast.CATEGORY))]
    public void pushCategory(int category) {

        ShopItemScroll.clear();

        ItemMast.CATEGORY cate = (ItemMast.CATEGORY)category;

        ShopItemMast[] shop_items = ShopItemMast.getCategoryShopItems(cate);

        for (int i = 0; i < shop_items.Length; i++) {
            MultiUseListMng mng = ShopItemScroll.makeListItem(i, shop_items[i].Id, shop_items[i].ItmMst);
            string param = shop_items[i].ItmMst.BaseValue == 0 ? "" : shop_items[i].ItmMst.BaseValue.ToString();
            mng.ExtraText1 = shop_items[i].ItmMst.getEffectName() + param;
            mng.ExtraText2 = shop_items[i].SellPrice.ToString();
            mng.Callback = pushItem;
        }
        ShopItemScroll.gameObject.SetActive(true);
        ShopItemListReciv.initSetupWithFrameEnd(true);
        
    }

    /// <summary>
    /// アイテムリスト押下
    /// </summary>
    /// <param name="mng"></param>
    public void pushItem(MultiUseListMng mng) {
        SelectedItem = System.Array.Find(ShopItemMast.List, it => it.Id == mng.Id);
        int maxNum = SaveMng.Status.Money / SelectedItem.SellPrice;
        ItemTran item = SaveMng.Items.Find(it => it.MasterId == SelectedItem.ItemId);
        int nowNum = item == null ? 0 : item.Num;

        //ButtonGuidSetMng.initButton(
        //    new CmnConfig.GamePadButton[] { CmnConfig.GamePadButton.Decision, CmnConfig.GamePadButton.Cancel, CmnConfig.GamePadButton.Horizontal },
        //    new string[] { "決定", "キャンセル", "数量増減" }
        //    );

        ConfirmWnd.show(SelectedItem.ItmMst.Detail, "購入しますか？", maxNum, SelectedItem.SellPrice, nowNum, execBuy, closeBuy);
    }

    public void execBuy(object obj) {
        ConfirmAdvanceWindowCmn win = (ConfirmAdvanceWindowCmn)obj;
        int num = win.InputNum;

        int totalPrice = SelectedItem.SellPrice * num;
        if (SaveMng.Status.Money >= totalPrice) {
            SaveMng.Status.subMoney(totalPrice);
            SaveMng.ItemData.addItem(SelectedItem.ItemId, num);
            SaveMng.Status.save();
            SaveMng.ItemData.save();
        }
        
    }

    public void closeBuy(object obj) {

    }

}