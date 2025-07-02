using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TacticsBoardMng : MonoBehaviour {

    [SerializeField]
    public Button[] Tactics = new Button[(int)AiProc.TACTICS.ALL];

    [SerializeField]
    public Text TacticInfo;

    [SerializeField]
    public GameObject UnitBase;

    [SerializeField]
    public Image UnitIcon;

    [SerializeField]
    public Text UnitName;

    [SerializeField]
    public Text UnitHp;

    [SerializeField]
    public Text UnitMp;

    [SerializeField]
    public GameObject TeamInfo;

    public static UnitStatusTran Unit;

    public void init( UnitStatusTran unit = null ) {
        this.gameObject.SetActive(true);
        Unit = unit;

        UtilToolLib.AllButtonOnOff(Tactics, true);

        if (Unit != null) {
            Tactics[(int)Unit.Tactics].interactable = false;
            setInfo((int)Unit.Tactics);

            UnitIcon.sprite = Unit.getIconImage();
            UnitName.text = Unit.Name;
            UnitHp.text = $"{Unit.Hp}/{Unit.MaxHp}";
            UnitMp.text = $"{Unit.Mp}/{Unit.MaxMp}";

        } else {
            setInfo("");
        }

        UnitBase.SetActive(Unit != null);
        TeamInfo.SetActive(!UnitBase.activeSelf);
    }

    [EnumAction(typeof(AiProc.TACTICS))]
    public void pushTactics(int tac )
    {
        CommonProcess.playClickSe();

        UtilToolLib.AllButtonOnOff(Tactics, true);

        if (Unit != null) {
            Unit.Tactics = (AiProc.TACTICS)tac;
        } else {
            SaveMng.Units.ForEach( it=>it.Tactics = (AiProc.TACTICS)tac );
        }
        SaveMng.UnitData.save();

        Tactics[tac].interactable = false;
        setInfo(tac);
    }

    private void setInfo(int tac) {
        setInfo(AiProc.TacticsInfo[tac]);
    }
    private void setInfo(string txt) {
        TacticInfo.text = txt;
    }

    public void close() {
        CommonProcess.playCanselSe();
        hide();
    }

    public void hide() {
        PartyPlatesMng.Singleton.updatePlate();
        this.gameObject.SetActive(false);
    }

    public void pushEscape() {
        CommonProcess.playClickSe();
        CommonProcess.showConfirm("撤退しますか？\n経験値と宝箱を得ることはできません", escapeExec);
    }

    public void escapeExec(object obj) {
        SaveMng.Quest.ActiveParty = new List<UnitStatusTran>();
        SceneManagerWrap.LoadScene(CmnConst.SCENE.HomeScene);
    }

}
