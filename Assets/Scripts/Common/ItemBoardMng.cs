using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class ItemBoardMng : PowerBoardMng
{

    private const string GuidWord = "できるアイテムのみ表示しています";
    private const string ConfirmWord = "しますか？";
    private const string NothingWord = "できるアイテムがありません";
    private const string MoreMoney = "所持金が不足しています";
    private const string HoldItemNum = "所持数 : ";
    private const string SellPrice = "\n売値 : ";
    private const string PowerUpPrice = "強化費 : ";

    public enum MODE
    {
        VIEW,
        SELL,
        EQUIP,
        USE,
        POWER_UP,
        BUY,
        ALL

    }

    [System.NonSerialized]
    public static readonly string[] CategoryWords = new string[(int)MODE.ALL] {
         "詳細",
         "売却",
         "装備",
         "使用",
         "強化",
         "購入",
     };

    [SerializeField]
    public Text GuidInfo;

    [SerializeField]
    protected GamePadListRecivMng Recive;

    [SerializeField]
    protected Text ItemDetailName;

    [SerializeField]
    protected Text ItemDetailInfo;

    [SerializeField]
    protected MultiUseScrollMng ItemDetailSpecs;

    [SerializeField]
    protected GameObject EquipBoard;

    [SerializeField]
    protected Text HaveItemNum;

    [SerializeField]
    protected Text MoneyNum;

    [SerializeField]
    protected Text CategoryName;

    [System.NonSerialized]
    public ItemTran SelectedItem;

    [System.NonSerialized]
    protected List<ItemTran> ShowItems;

    [System.NonSerialized]
    public MODE Mode = MODE.VIEW;

    [System.NonSerialized]
    public ItemMast.CATEGORY ItemCategory = ItemMast.CATEGORY.ANY;

    //protected GamePadListRecivMng UnitRecive;

    private int SelectItemNum = 1;

    private List<ItemMast.CATEGORY> ShopCategoryList = new List<ItemMast.CATEGORY>();

    //Order　まだインスタンスが存在しない場合に指定
    public static MODE OrderMode = MODE.ALL; //何も設定されていない値としてALLを使用

    public static ItemMast.CATEGORY OrderCategory = ItemMast.CATEGORY.ALL; //何も設定されていない値としてALLを使用

    //public static List<UnitStatusTran> TargetUnits;

    public static UnitStatusTran EquipUnit;
    public static UnitStatusTran.EQUIP EquipPosi;

    public static int ShopId = 0;

    //public bool ActiveBase{ get{ return this.gameObject.activeSelf;} set{ this.gameObject.SetActive(value); } }

    protected new void Awake() {
        base.Awake();
    }

    new protected void OnEnable() {

        base.OnEnable();

        if (OrderMode != MODE.ALL) {
            Mode = OrderMode;
            OrderMode = MODE.ALL;
        }
        if (OrderCategory != ItemMast.CATEGORY.ALL) {
            ItemCategory = OrderCategory;
            OrderCategory = ItemMast.CATEGORY.ALL;
        }

        if (Mode == MODE.BUY) {
            TargetGroup.GroupBase.SetActive(false);
            ShopCategoryList = GetShopItemList().Select(it => it.ItmMst.Category).Distinct().OrderBy(it => it).ToList();
        } else {
            setShowItemList();
        }

        CategoryName.text = ItemMast.GetCategoryName(ItemCategory);
        showDetailClear();
        updateInfo();
    }

    public void setShowItemList() {
        string nothing_txt = "";

        if (SaveMng.ItemData == null || SaveMng.Items.Count == 0) {
            ShowItems = new List<ItemTran>();
        } else {
            switch (Mode) {
                case MODE.VIEW:
                ShowItems = SaveMng.Items.FindAll(it => (ItemCategory == ItemMast.CATEGORY.ANY || ItemCategory == it.Mst.Category));
                break;
                case MODE.EQUIP:
                ShowItems = SaveMng.Items.FindAll(it => (ItemCategory == it.Mst.Category) && it.Mst.EquipJob != null);
                ShowItems = ShowItems.FindAll(it => ((System.Array.IndexOf(it.Mst.EquipJob, UnitMast.JOB.NON) >= 0 || System.Array.IndexOf(it.Mst.EquipJob, EquipUnit.Job) >= 0)));
                break;
                case MODE.SELL:
                ShowItems = SaveMng.Items.FindAll(it => (ItemCategory == ItemMast.CATEGORY.ANY || ItemCategory == it.Mst.Category) && it.Mst.Cost > 0);
                break;
                case MODE.USE:
                ShowItems = SaveMng.Items.FindAll(it => (ItemCategory == it.Mst.Category));
                break;
                case MODE.POWER_UP:
                ShowItems = SaveMng.Items.FindAll(it => (ItemCategory == ItemMast.CATEGORY.ANY || ItemCategory == it.Mst.Category) && (ItemMast.CATEGORY.WEAPON == it.Mst.Category || ItemMast.CATEGORY.ARMOR == it.Mst.Category || ItemMast.CATEGORY.ACCESSORY == it.Mst.Category));
                break;
            }
        }

        if (GuidInfo != null) {
            GuidInfo.text = "";
        }

        if (Mode == MODE.VIEW) {

        } else if (ShowItems.Count == 0) {
            nothing_txt = CategoryWords[(int)Mode] + NothingWord;
            GuidInfo.text = nothing_txt;
        }

        Scroll.clear();

        for (int i = 0; i < ShowItems.Count; i++) {
            var mst = ShowItems[i].Mst;

            var list = Scroll.makeListItem(i, ShowItems[i].Id, mst);

            if (Mode == MODE.SELL) {
                list.Value.text = string.Format("x{0} {1}{2}", ShowItems[i].Num, GameConst.MONEY_CURRENCY, mst.Cost);
            } else {
                if (mst.isEquip) {
                    list.Value.text = "";
                } else {
                    list.Value.text = string.Format("{0}/{1}", ShowItems[i].Num, GameConst.MAX_ITEM_NUM);
                }
            }
        }

        Recive.initSetupWithFrameEnd(true);
    }

    /// <summary>
    /// ショップアイテム表示
    /// </summary>
    public void showShopItems() {

        Scroll.clear();

        if (GuidInfo != null) {
            GuidInfo.text = "";
        }

        var shop_items = GetShopItemList().Where(
            it => ItemCategory == ItemMast.CATEGORY.ANY
            || ItemCategory == it.ItmMst.Category).ToList();


        for (int i = 0; i < shop_items.Count; i++) {

            Scroll.IconDirectory = GameConst.Path.IMG_ITEM_ICON;
            var list = Scroll.makeListItem(i, shop_items[i].Id, shop_items[i].ItmMst);

            if (shop_items[i].ItmMst.isEquip) {
                //装備品の場合は数量を記載しない
                list.Value.text = string.Format("{0}{1}", GameConst.MONEY_CURRENCY, shop_items[i].SellPrice);
            } else {
                var tran = SaveMng.Items.Find(it => it.MasterId == shop_items[i].ItemId);
                int num = 0;
                if (tran != null) {
                    num = tran.Num;
                }
                list.Value.text = string.Format("{0}/{1} {2}{3}", num, GameConst.MAX_ITEM_NUM, GameConst.MONEY_CURRENCY, shop_items[i].SellPrice);
            }
        }

        Recive.initSetupWithFrameEnd(true);

    }

    /// <summary>
    /// ショップアイテムリスト取得
    /// </summary>
    /// <returns></returns>
    private IEnumerable<ShopItemMast> GetShopItemList() {
        return ShopItemMast.List.Where(it =>
            SaveMng.Status.ClearStage.Count >= it.SellClearStageNum
            && ShopId == it.ShopId
            && SaveMng.Status.Lv >= it.SellLv
            && it.ItmMst != null
        );
    }


    virtual public void pushList(MultiUseListMng mng) {

        if (Mode != MODE.BUY) {
            var tran = ItemProcess.getTranData(mng.Id);
            SelectedPower = tran.Mst;
        }

        switch (Mode) {
            case MODE.EQUIP:
            equip(mng.Id);
            closeWindow();
            break;
            case MODE.BUY:
            case MODE.SELL:
            showConfirm(mng);
            break;
            case MODE.USE:
            readyUseTarget();
            break;
        }

    }

    public void showConfirm(MultiUseListMng mng) {

        //MultiUseListMngで受け取るのは、中身がItemTranの場合と、ShopItemMastの場合があるから

        string info = "";
        var itemName = "";
        var price = 0;

        ItemTran item = null;

        if (Mode != MODE.BUY) {
            item = SaveMng.Items.Find(it => it.Id == mng.Id);
        }

        switch (Mode) {
            case MODE.SELL:
            info = string.Format("{0}\nを売却しますか？\n価格 : {1}{2}\n所持金：{3}{4}", item.Name, GameConst.MONEY_CURRENCY, item.Mst.Cost, GameConst.MONEY_CURRENCY, SaveMng.Status.Money);
            CommonProcess.showConfirm(info, sellItem, item);
            break;
            case MODE.POWER_UP:
            info = PowerUpPrice + getPowerUpCost().ToString() + GameConst.MONEY_CURRENCY;
            break;
            case MODE.BUY:
            var shopItem = ShopItemMast.List.FirstOrDefault(it => it.Id == mng.Id);
            itemName = shopItem.ItmMst.checkOverRideNum() ? string.Format("{0}x{1}", shopItem.ItmMst.Name, SelectItemNum) : shopItem.ItmMst.Name;
            price = shopItem.SellPrice * SelectItemNum;
            info = string.Format("{0}\nを購入しますか？\n価格 : {1}{2}\n所持金：{3}{4}", itemName, GameConst.MONEY_CURRENCY, price, GameConst.MONEY_CURRENCY, SaveMng.Status.Money);
            CommonProcess.showConfirm(info, buyItem, shopItem);
            break;
            case MODE.USE:
            info = string.Format("{0}\nを使用しますか？", item.Name);
            CommonProcess.showConfirm(info, useItem, item);
            break;
            default:
            break;
        }
    }

    public void equip(int Id) {
        ItemTran old = EquipUnit.Equips[(int)EquipPosi];
        ItemTran item = SaveMng.Items.Find(it => it.Id == Id);
        EquipUnit.Equips[(int)EquipPosi] = item;
        SaveMng.ItemData.lostItem(item, 1);
        if (old != null && old.Mst != null) {
            SaveMng.ItemData.addItem(old);
        }
        SaveMng.ItemData.save();
        SaveMng.saveUnit();
    }
    public void equipOut() {
        SelectedItem = null;
        equip(0);
    }

    public void powerUp(object obj) {

        int price = getPowerUpCost();

        if (price > 0 && price <= SaveMng.Status.Money && SelectedItem.EnhanceLv < SelectedItem.Mst.MaxEnhance) {
            SaveMng.Status.subMoney(price);
            SaveMng.Items.Find(it => it == SelectedItem).EnhanceLv++;
            reload();
        } else {
            CommonProcess.showMessage(MoreMoney);
        }
    }

    public int getPowerUpCost() {
        int pow = (int)Mathf.Pow(2, SelectedItem.EnhanceLv + 1);
        int price = SelectedItem.Mst.Cost * pow;
        return price;
    }

    public void buyItem(object obj) {
        var shopItem = (ShopItemMast)obj;

        int totalPrice = shopItem.SellPrice * SelectItemNum;
        if (SaveMng.Status.Money >= totalPrice) {
            if (SaveMng.ItemData.addItem(shopItem.ItemId, SelectItemNum)) {
                SaveMng.Status.subMoney(totalPrice);
                SaveMng.Status.save();
                SaveMng.ItemData.save();
                showShopItems();
                updateInfo();
            }
        }
    }

    public void sellItem(object obj) {

        var num = 1;

        var item = obj as ItemTran;

        if (item == null) {
            return;
        }

        if (item.Num >= num) {
            SaveMng.ItemData.sellItem(item, num);
            reload();
            updateInfo();
        }
    }

    public void useItem(object obj) {
        if (SaveMng.ItemData.useItem(SelectedItem)) {

            PowerProcess.execPower(SelectedItem.Mst, TargetUnits);

            SaveMng.ItemData.save();
            SaveMng.Status.save();
        }

        closeWindow();
    }

    public void reload() {
        SaveMng.ItemData.save();
        SaveMng.Status.save();
        OrderCategory = ItemCategory;
        if (Mode != MODE.BUY) {
            setShowItemList();
        }
    }

    /// <summary>
    /// カテゴリ変更
    /// </summary>
    /// <param name="add"></param>
    public void pushChangeCategory(int add) {
        CommonProcess.playClickSe();

        if (Mode == MODE.BUY) {

            if (ShopCategoryList.Count <= 1) {
                ItemCategory = ItemMast.CATEGORY.ANY;
            } else if (ItemCategory == ItemMast.CATEGORY.ANY) {
                if (add < 0) {
                    ItemCategory = ShopCategoryList.Last();
                } else {
                    ItemCategory = ShopCategoryList.First();
                }
            } else {
                var index = ShopCategoryList.IndexOf(ItemCategory);
                if (index + add < 0 || index + add >= ShopCategoryList.Count) {
                    ItemCategory = ItemMast.CATEGORY.ANY;
                } else {
                    ItemCategory = ShopCategoryList[index + add];
                }
            }
            showShopItems();
        } else {
            switch (Mode) {
                case MODE.USE:
                ItemCategory = ItemMast.CATEGORY.CONSUMABLE;
                break;
                default:
                ItemCategory = (ItemMast.CATEGORY)Mathf.Repeat((int)ItemCategory + add, (int)ItemMast.CATEGORY.ALL);
                break;
            }
            setShowItemList();
        }

        CategoryName.text = ItemMast.GetCategoryName(ItemCategory);

    }

    public override bool closeWindow() {
        var result = base.closeWindow();
        if (result) {
            if (SceneManagerWrap.NowScheneIs(CmnConst.SCENE.ItemScene)) {
                SceneManagerWrap.loadBefore();
            } else {
                ActiveBase = false;
            }
        }
        return result;
    }

    public void changeSelect(MultiUseListMng list) {

        ItemDetailSpecs.clear();

        if (Mode == MODE.BUY) {
            var shop = ShopItemMast.List.FirstOrDefault(it => it.Id == list.Id);
            var mst = shop.ItmMst;
            showDetailMast(mst, (int)mst.BaseValue, (int)mst.SubValue);
        } else {
            var item = SaveMng.Items.FirstOrDefault(it => it.Id == list.Id);
            if (item != null) {
                showDetailMast(item.Mst, item.Value, item.SubValue);
                foreach (var rune in item.Runes) {
                    var add = ItemDetailSpecs.makeListItem();
                    add.Id = rune.Id;
                    if (rune.Mst.Name.Length > 0) {
                        add.Name.text = rune.Mst.Name;
                    } else {
                        //add.Name.text = rune.Mst
                    }

                    add.Value.text = rune.getInfo();
                }
            }
        }
    }

    private void showDetailMast(ItemMast mst, int mainValue, int subValue) {

        ItemDetailSpecs.clear();

        if (mst != null) {

            ItemDetailName.text = mst.Name;
            ItemDetailInfo.text = mst.Detail;

            EquipBoard.SetActive(mst.isEquip);

            switch (mst.Category) {
                case ItemMast.CATEGORY.WEAPON:
                case ItemMast.CATEGORY.ARMOR:
                case ItemMast.CATEGORY.ACCESSORY:
                case ItemMast.CATEGORY.CONSUMABLE: {
                        if (mainValue > 0) {
                            var main_param = ItemDetailSpecs.makeListItem();
                            main_param.Name.text = mst.getEffectName(true);
                            main_param.Value.text = mainValue.ToString();
                        }
                        if (mst.isEquip && subValue > 0) {
                            var sub_param = ItemDetailSpecs.makeListItem();
                            sub_param.Name.text = mst.getEffectName(false);
                            sub_param.Value.text = subValue.ToString();
                        }
                    }
                    break;
            }
        }
    }

    private void showDetailClear() {
        ItemDetailSpecs.clear();
        ItemDetailName.text = "";
        ItemDetailInfo.text = "";
        EquipBoard.SetActive(false);
    }

    private void updateInfo() {
        //基本的にnulllにはならない（シーン直接起動のデバッグ時のみ発生）
        if (SaveMng.ItemData == null) {
            StartCoroutine(delayUpdateInfo());
        } else {
            MoneyNum.text = SaveMng.Status?.Money.ToString();
            HaveItemNum.text = string.Format("{0}/{1}", SaveMng.Items?.Count, GameConst.MAX_ITEM_KIND_NUM);
        }
    }

    private IEnumerator delayUpdateInfo() {
        yield return new WaitForEndOfFrame();
        MoneyNum.text = SaveMng.Status?.Money.ToString();
        HaveItemNum.text = string.Format("{0}/{1}", SaveMng.Items?.Count, GameConst.MAX_ITEM_KIND_NUM);
    }



}
