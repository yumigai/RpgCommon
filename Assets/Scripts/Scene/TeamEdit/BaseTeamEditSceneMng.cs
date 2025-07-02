using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.UI;

public class BaseTeamEditSceneMng : Base2DSceneMng
{

    public const string NOT_PARTY = "パーティーメンバーが設定されていません";

    public enum SET_TYPE{
        NORMAL_EDIT,
        QUEST_READY
    }

    [SerializeField]
    private UnitListMng ChangeUnitList;

    [SerializeField]
    private PartySelectMng PartySelect;

    public static SET_TYPE Type = SET_TYPE.QUEST_READY;
	
	public static BaseTeamEditSceneMng Singleton;

	void Awake(){
		
        Singleton = this;
	}
	
	new public void Start(){

        base.Start();

        PartySelect.CallbackCancel = SceneManagerWrap.loadBefore;

        if (ChangeUnitList != null) {
            //ChangeUnitList.PushUnitCallback = pushSelectUnit;
            ChangeUnitList.PushCanselCallback = pushClose;
            ChangeUnitList.gameObject.SetActive(false);
        }
        init();
        refleshParty();

    }

    private void init() {

    }

    public void refleshParty()
    {
        //ButtonGuidMng.initButton(
        //new CmnConfig.GamePadButton[] {
        //            CmnConfig.GamePadButton.Decision, CmnConfig.GamePadButton.Cancel, CmnConfig.GamePadButton.Vertical, CmnConfig.GamePadButton.R1, CmnConfig.GamePadButton.L1 },
        //        new string[] { "member change", "return", "select", "party switching", "" },
        //        new string[] { "メンバー入替", "戻る", "選択", "パーティ切替", "" }
        //);
    }

    /// <summary>
    /// メンバー解除
    /// </summary>
    /// <param name="index"></param>
    public void removeMember(int index) {
        SaveMng.Status.ActiveMember[index] = -1;
        SaveMng.Status.save();
        refleshParty();
    }

    public void pushClose() {
        if (ChangeUnitList != null) {
            if (ChangeUnitList.gameObject.activeSelf) {
                ChangeUnitList.gameObject.SetActive(false);
            } else {
                SceneManagerWrap.loadBefore();
            }
        }
    }
}


