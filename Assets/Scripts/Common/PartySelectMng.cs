using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class PartySelectMng : MonoBehaviour
{

    [SerializeField]
    private UnitDetailMng[] PartyUnits;

    [SerializeField]
    private Text PartyName;

    [SerializeField]
    private GameObject LeftArrow;

    [SerializeField]
    private GameObject RightArrow;

    [SerializeField]
    private UnitListMng ChangeUnitList;

    [System.NonSerialized]
    public System.Action CallbackCancel;

    private int ChangeIndex = -1;

    private void Start() {
        if (ChangeUnitList != null) {
            ChangeUnitList.PushUnitCallback = pushChangeExec;
        }
        changeParty(0);
    }

    public void setupUnits( ) {
        var units = SaveMng.Status.getActiveMembers(false);
        for (int i = 0; i < PartyUnits.Length; i++) {
            if(i < units.Count && units[i] != null) {
                PartyUnits[i].setParams(units[i]);
            } else {
                PartyUnits[i].setClear();
            }
        }
    }

    /// <summary>
    /// パーティ変更
    /// </summary>
    /// <param name="add"></param>
    public void changeParty( int add ) {
        if (add != 0) {
            SaveMng.Status.NowParty += add;
            SaveMng.Status.save();
        }
        setupUnits();
        PartyName.text = "PARTY " + (SaveMng.Status.NowParty + 1).ToString();

        if(SaveMng.Status.NowParty <= 0) {
            LeftArrow.SetActive(false);
        } else {
            LeftArrow.SetActive(true);
        }
        if (SaveMng.Status.NowParty >= PartyTran.MAX_PARTY-1) {
            RightArrow.SetActive(false);
        } else {
            RightArrow.SetActive(true);
        }
    }

    /// <summary>
    /// チーム編成へ遷移
    /// （既にチーム編成画面なら何もしないので問題なし）
    /// </summary>
    public void pushJumpPartyEdit() {
        CommonProcess.playClickSe();
        SceneManagerWrap.loadScene(CmnConst.SCENE.TeamEditScene);
    }

    /// <summary>
    /// 戻る
    /// </summary>
    public void returnBack() {
        CommonProcess.playCanselSe();
        SceneManagerWrap.loadBefore();
    }

    public void pushCancel() {
        if (CallbackCancel == null) {
            returnBack();
        } else {
            CallbackCancel();
        }
    }

    /// <summary>
    /// メンバーチェンジ開始
    /// </summary>
    /// <param name="index"></param>
    public void pushMemberChange(UnitDetailMng unit) {
        CommonProcess.playClickSe();

        ChangeIndex = System.Array.IndexOf(PartyUnits,unit);

        if (ChangeUnitList != null) {
            ChangeUnitList.gameObject.SetActive(true);
        }

        //ButtonGuidMng.initButton(
        //    new CmnConfig.GamePadButton[] {
        //            CmnConfig.GamePadButton.Decision, CmnConfig.GamePadButton.Cancel },
        //            new string[] { "Choose", "return" },
        //            new string[] { "選択", "戻る" }
        //);
    }

    /// <summary>
    /// メンバーチェンジ実行
    /// </summary>
    /// <param name="mng"></param>
    public void pushChangeExec(CharaImgGaugeMng unit) {

        var before_index = System.Array.IndexOf(SaveMng.Status.ActiveMember, unit.UnitTranId);
        if (before_index >= 0) {
            SaveMng.Status.ActiveMember[before_index] = SaveMng.Status.ActiveMember[ChangeIndex];
        }

        SaveMng.Status.ActiveMember[ChangeIndex] = unit.UnitTranId;
        SaveMng.Status.save();

        ChangeUnitList?.gameObject?.SetActive(false);

        setupUnits();
    }

}
