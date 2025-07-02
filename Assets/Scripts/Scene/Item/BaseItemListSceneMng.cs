using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BaseItemListSceneMng : ItemBoardMng {

    [SerializeField]
    public ItemDetailWindowMng DetailWnd;

    [SerializeField]
    public FreeImgAndTxtMng BeforeEquip;

    [SerializeField]
    public Button BeforeEquipButton;

    public const string CHASHIR = "chashir";

    public static BaseItemListSceneMng Singleton;

    void Awake(){
        DetailWnd.gameObject.SetActive(false);
        BeforeEquip.gameObject.SetActive(false);
        Singleton = this;
	}

    private void Start() {
        if (Mode == MODE.EQUIP && EquipUnit != null) {
            BeforeEquip.gameObject.SetActive(true);
            //BeforeEquip.setImage(new string[] { EquipUnit.getIconPath(), EquipUnit.Equips[(int)EquipPosi].Icon });
            //BeforeEquip.setText(new string[] { EquipUnit.Equips[(int)EquipPosi].Name, EquipUnit.Equips[(int)EquipPosi].Detail });
            if (EquipUnit.Equips[(int)EquipPosi].Mst == null) {
                BeforeEquipButton.gameObject.SetActive(false);
            }
        }

        if(Mode == MODE.USE || Mode == MODE.EQUIP) {
            //Array.ForEach(CategoryButtons, it => { if (it != null) { it.gameObject.SetActive(false); } });
            //CategoryButtons[(int)ItemCategory].gameObject.SetActive(true);
        }
    }

    public void pushChangeMode( int mode )
    {
        CommonProcess.playClickSe();
		Mode = (MODE)mode;
        SceneManagerWrap.reload();
    }


    override public void pushList(MultiUseListMng mng) {

        SelectedItem = ShowItems[mng.Index];

        if (Mode == MODE.EQUIP) {

            showConfirm(mng);

        } else {
            CommonProcess.playClickSe();
            DetailWnd.gameObject.SetActive(true);
        }

    }

    public void sortCategory() {
		CommonProcess.playClickSe ();
        SaveMng.ItemData.sortCategory();
        reload();
    }

    public void sortRarity() {
		CommonProcess.playClickSe ();
        SaveMng.ItemData.sortRarity();
        reload();
    }


}
