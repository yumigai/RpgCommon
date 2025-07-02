using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PartyListItemMng : MultiUseListMng {

	[SerializeField]
	public Image Img;

    [SerializeField]
    public Text Lv;

    [SerializeField]
    public Text Atk;

    [SerializeField]
    public Text Def;

    [SerializeField]
    public Text Mgc;

    [SerializeField]
    public Text Reg;

    [SerializeField]
    public Text Agi;

	[System.NonSerialized]
	public int ThisIndex;

	public void setInfo( int index, UnitStatusTran unit ) {

		ThisIndex = index;

        Name.text = unit.Name;
        Lv.text = unit.Lv.ToString();
        Atk.text = unit.TotalAttack.ToString();
        Def.text = unit.TotalDefence.ToString();
        Mgc.text = unit.TotalMagic.ToString();
        Reg.text = unit.TotalRegister.ToString();
        Agi.text = unit.Status.Agi.ToString();

        string img_path = CmnConst.Path.IMG_CHARA_BUSTUP + unit.Mst.Tag;

        Img.sprite = Resources.Load<Sprite> (img_path);
	}

    public void pushUnit() {
        PartyEditSceneMng.Singleton.selectChara(ThisIndex);
    }
		
}
