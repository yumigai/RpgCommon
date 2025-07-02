using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UnitDetailMng : MonoBehaviour
{
    [SerializeField]
    CharaImgGaugeMng Chara;

    [SerializeField]
    private Text MaxHp;

    [SerializeField]
    private Text Hp;

    [SerializeField]
    private Text MaxMp;

    [SerializeField]
    private Text Mp;

    [SerializeField]
    private Text Atk;

    [SerializeField]
    private Text Def;

    [SerializeField]
    private Text Mag;

    [SerializeField]
    private Text Reg;

    [SerializeField]
    private Text Str;

    [SerializeField]
    private Text Mys;

    [SerializeField]
    private Text Dex;

    [SerializeField]
    private Text Con;

    [SerializeField]
    private Text Pow;

    [SerializeField]
    private Text Men;

    [SerializeField]
    private Text Luc;

    [SerializeField]
    private Text Lv;

    [SerializeField]
    private GaugeBarMng HpGauge;

    [SerializeField]
    private GaugeBarMng MpGauge;

    [SerializeField]
    private GaugeBarMng ExpGauge;

    [SerializeField]
    private MultiUseListMng Weapon;

    [SerializeField]
    private MultiUseListMng Armor;

    [SerializeField]
    private MultiUseListMng Accessory1;

    [SerializeField]
    private MultiUseListMng Accessory2;

    [SerializeField]
    private GamePadListRecivMng Reciv;

    [SerializeField]
    private ItemBoardMng ItemBoardPrefab;

    private ItemBoardMng _InstanceItemBoard;


    public void setParams( UnitStatusTran unit, bool recive_update = false ) {
        Chara.UnitTranId = unit.Id;
        Chara.Name.text = unit.Name;
        Chara.setImage(unit.Img);
        setParam(MaxHp, unit.MaxHp);
        setParam(Hp, unit.Hp);
        setParam(MaxMp, unit.MaxMp);
        setParam(Mp, unit.Mp);
        setParam(Atk, unit.TotalAttack);
        setParam(Def, unit.TotalDefence);
        setParam(Mag, unit.TotalMagic);
        setParam(Reg, unit.TotalRegister);
        setParam(Lv, unit.Lv);
        setParam(Str, unit.Status.Str);
        setParam(Dex, unit.Status.Agi);
        setParam(Con, unit.Status.Con);
        setParam(Pow, unit.Status.Mag);
        setParam(Men, unit.Status.Men);
        setParam(Luc, unit.Status.Luk);

        setGauge(HpGauge, unit.MaxHp, unit.Hp);
        setGauge(MpGauge, unit.MaxMp, unit.Mp);
        setGauge(ExpGauge, unit.getNextLvupExp(), unit.Exp);

        setItem(Weapon, unit.EquipWeapon);
        setItem(Armor, unit.EquipArmor);
        setItem(Accessory1, unit.EquipAccessory1);
        setItem(Accessory2, unit.EquipAccessory2);

        ItemBoardMng.EquipUnit = unit;

        Reciv?.initSetupWithFrameEnd();
    }

    private void setParam( Text txt, int num) {
        setParam(txt, num.ToString());
    }

    private void setParam( Text txt, string str = "") {
        if (txt != null) {
            txt.text = str;
        }
    }

    public void setClear() {
        Chara.UnitTranId = 0;
        Chara.Name.text = "";
        Chara.setImage("");
        setParam(MaxHp);
        setParam(Hp);
        setParam(MaxMp);
        setParam(Mp);
        setParam(Atk);
        setParam(Def);
        setParam(Mag);
        setParam(Reg);
        setParam(Lv);
        setParam(Str);
        setParam(Dex);
        setParam(Con);
        setParam(Pow);
        setParam(Men);
        setParam(Luc);

        setGauge(HpGauge, 0, 1);
        setGauge(MpGauge, 0, 1);
        setGauge(ExpGauge, 0,1);

        setItem(Weapon, null);
        setItem(Armor, null);
        setItem(Accessory1, null);
        setItem(Accessory2, null);

        ItemBoardMng.EquipUnit = null;

        Reciv?.initSetupWithFrameEnd();
    }

    private void setGauge( GaugeBarMng gauge, int max, int val ) {
        if(gauge != null) {
            gauge.init(max, val);
        }
    }

    private void setItem(MultiUseListMng list, ItemTran item ) {
        if(list != null) {
            list.Name.text = item.Name;
            list.Value.text = item.Value.ToString();
            list.ExtraText1 = item.SubValue.ToString();
        }
    }

    public void closeWindow() {
        this.gameObject.SetActive(false);
    }

    public void pushEquipWepon() {
        ItemBoardMng.EquipPosi = UnitStatusTran.EQUIP.WEAPON;
        pushEquip(ItemMast.CATEGORY.WEAPON);
    }

    public void pushEquipArmor() {
        ItemBoardMng.EquipPosi = UnitStatusTran.EQUIP.ARMOR;
        pushEquip(ItemMast.CATEGORY.ARMOR);
    }

    public void pushEquipAccessory1() {
        ItemBoardMng.EquipPosi = UnitStatusTran.EQUIP.ACCESSORY1;
        pushEquip(ItemMast.CATEGORY.ACCESSORY);
    }

    public void pushEquipAccessory2() {
        ItemBoardMng.EquipPosi = UnitStatusTran.EQUIP.ACCESSORY2;
        pushEquip(ItemMast.CATEGORY.ACCESSORY);
    }

    public void pushEquip(ItemMast.CATEGORY category) {
        ItemBoardMng.OrderCategory = category;
        ItemBoardMng.OrderMode = ItemBoardMng.MODE.EQUIP;
        if (_InstanceItemBoard == null) {
            _InstanceItemBoard = Instantiate(ItemBoardPrefab.gameObject).GetComponent<ItemBoardMng>();
            _InstanceItemBoard.transform.SetParent(this.transform, false);
            _InstanceItemBoard.CallbackClose = backFromEquip;
        } else {
            _InstanceItemBoard.gameObject.SetActive(true);
        }
        _InstanceItemBoard.setShowItemList();
    }

    public void backFromEquip() {

        switch (ItemBoardMng.EquipPosi) {
            case UnitStatusTran.EQUIP.WEAPON:
            setItem(Weapon, ItemBoardMng.EquipUnit.EquipWeapon);
            break;
            case UnitStatusTran.EQUIP.ARMOR:
            setItem(Armor, ItemBoardMng.EquipUnit.EquipArmor);
            break;
            case UnitStatusTran.EQUIP.ACCESSORY1:
            setItem(Accessory1, ItemBoardMng.EquipUnit.EquipAccessory1);
            break;
            case UnitStatusTran.EQUIP.ACCESSORY2:
            setItem(Accessory2, ItemBoardMng.EquipUnit.EquipAccessory2);
            break;
        }
    }

    /// <summary>
    /// アイテムボードを閉じる
    /// </summary>
    /// <returns></returns>
    public bool CloseItemBoard() {

        bool is_open = false;

        if (_InstanceItemBoard != null) {
            is_open = _InstanceItemBoard.gameObject.activeSelf;
            _InstanceItemBoard.gameObject.SetActive(false);
        }

        return is_open;
    }
}
