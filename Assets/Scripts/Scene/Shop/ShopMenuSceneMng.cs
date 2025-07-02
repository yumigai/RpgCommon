using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ShopMenuSceneMng : Base2DSceneMng
{
    [SerializeField]
    private Text Money;

    [SerializeField]
    private AudioClip Chashir;

    // Start is called before the first frame update
    new protected void Start() {
        base.Start();
        Money.text = SaveMng.Status.Money.ToString();
    }

    public void pushBuy() {
        ItemBoardMng.OrderMode = ItemBoardMng.MODE.BUY;
        SceneManagerWrap.loadScene(CmnConst.SCENE.ShopScene);
    }
    public void pushSell() {
        ItemBoardMng.OrderMode = ItemBoardMng.MODE.SELL;
        SceneManagerWrap.loadScene(CmnConst.SCENE.ItemScene);
    }
    public void pushAllSell() {
        CommonProcess.playClickSe();
        int price = SaveMng.ItemData.totalPrice(ItemMast.CATEGORY.MATERIAL);
        CommonProcess.showConfirm($"換金アイテムを全て売却します。よろしいですか？\n計 : {price}{GameConst.MONEY_CURRENCY}", sellAllMoneyItemExe);
    }

    public void sellAllMoneyItemExe(object obj) {
        SoundMng.Instance.playSE(Chashir);
        SaveMng.ItemData.allSell(ItemMast.CATEGORY.MATERIAL);
    }
}
