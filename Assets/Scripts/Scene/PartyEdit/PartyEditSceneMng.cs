using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PartyEditSceneMng : MonoBehaviour {

	public const int FONT_SIZE = 22;

    public const int SKILL_LIST_COST_INDEX = 0;

	public enum MODE{
		CHARA_VIEW,
		PARTY_EDIT,
	}

    [SerializeField]
    public Transform MainCanvas;

    //   [SerializeField]
    //public GameObject DatailList;

    //[SerializeField]
    //public Transform DetailListBase;

    //[SerializeField]
    //public InfiniteScroll DetailScroll;

    //[SerializeField]
    //public UnitDetailMng Detail;

	[SerializeField]
	public GameObject ListPrefab;

    [SerializeField]
    public GameObject SkillBase;

    [SerializeField]
    public MultiUseScrollMng SkillScroll;

    [System.NonSerialized]
	public PartyListItemMng[] ListItems;

	[System.NonSerialized]
	public MODE ListMode = MODE.CHARA_VIEW;

    public static int SelectedDetail = -1;

	public static MODE ModeOrder = MODE.CHARA_VIEW;
	public static int ChangeBeforeMember = -1;


	public static PartyEditSceneMng Singleton;

	//private static Vector2 DetailListBasePosi;
	//private static Vector2 DetailSize;
	//private static float DetailAdj;

    private int ChangeMember { get { return SaveMng.Status.ActiveMember[ChangeBeforeMember]; } set { SaveMng.Status.ActiveMember[ChangeBeforeMember] = value; } }

	void Awake(){
		Singleton = this;
		//DetailListBasePosi = DetailListBase.localPosition;
        //DetailSize = DatailList.transform.localScale;
		//DetailAdj = DetailSize.x * DatailList.transform.parent.localScale.x;
	}

	// Use this for initialization
	void Start () {
        //ButtonGuidMng.initButton(
        //    new CmnConfig.GamePadButton[] {
        //        CmnConfig.GamePadButton.Decision, CmnConfig.GamePadButton.Cancel,CmnConfig.GamePadButton.Y,
        //        CmnConfig.GamePadButton.Vertical, CmnConfig.GamePadButton.R1, CmnConfig.GamePadButton.L1 },
        //    new string[] { "決定", "戻る", "自動装備", "装備選択", "次", "前" }
        //    );
        closeDetail ();
        closeSkill();
		init ();

	}

	void init(){

		ListItems = new PartyListItemMng[SaveMng.Units.Count];

		ListMode = ModeOrder;

		for (int i = 0; i < SaveMng.Units.Count; i++) {
            PartyListItemMng mng = addList();
			setValue ( i, mng, SaveMng.Units [i]);
			ListItems [i] = mng;
		}

        nextMember();

		ModeOrder = MODE.CHARA_VIEW;

		Destroy (ListPrefab);

        if (SelectedDetail >= 0)
        {
            selectChara(SelectedDetail);
            SelectedDetail = -1;
        }
	}

    PartyListItemMng addList() {
        GameObject list = Instantiate(ListPrefab) as GameObject;
        list.transform.SetParent(ListPrefab.transform.parent);
        list.transform.localScale = ListPrefab.transform.localScale;
        return list.GetComponent<PartyListItemMng>();
    }

	void setValue( int index, PartyListItemMng mng, UnitStatusTran unit ){

		mng.setInfo( index, unit);

		//mng.PartyInedIcon.SetActive (false);

		//if (ListMode == MODE.PARTY_EDIT ) {

  //          if (System.Array.IndexOf<int>(SaveMng.Status.ActiveMember, SaveMng.Units[index].Id) >= 0){
  //              mng.PartyInedIcon.SetActive(true);
  //          }

  //          if (ChangeBeforeMember >= 0) {
		//		if (unit.Id == SaveMng.Status.ActiveMember [ChangeBeforeMember]) {
		//			mng.ButtonText.text = "外す";
					
		//		} else {
		//			mng.ButtonText.text = "入れ替え";
		//		}
		//	} else {
		//		mng.ButtonText.text = "加える";
		//	}	
		//}

	}

	public void selectChara( int index = 0 ){
		CommonProcess.playClickSe ();
		if (ListMode == MODE.CHARA_VIEW ) {
			openDetail (index);
		} else {
			changeMember (index);
		}
	}

	public void changeMember( int index ){

        if(ChangeMember == SaveMng.Units[index].Id){
            ChangeMember = -1;
        }else{
            int swap = System.Array.IndexOf<int>(SaveMng.Status.ActiveMember, SaveMng.Units[index].Id);
            if (swap >= 0){
                int b_id = ChangeMember;
                SaveMng.Status.ActiveMember[swap] = b_id;
            }
            ChangeMember = SaveMng.Units[index].Id;
        }
		     
        ChangeBeforeMember = -1;
        SaveMng.saveStatus();
        SceneManagerWrap.loadBefore ();
	}

	public void openDetail( int index ){

        //Detail.show(index);
		//DatailList.transform.localScale = DetailSize;
		//DetailListBase.localPosition = new Vector2 (index * -DetailScroll.itemScale, DetailListBasePosi.y);
	}

	public void pushCloseDetail(){
		CommonProcess.playCanselSe ();
		closeDetail ();
	}

	public void closeDetail(){
        //Detail.gameObject.SetActive(false);
        //DatailList.transform.localScale = Vector3.zero;
    }

    public void openSkill( UnitStatusTran unit ) {
        for (int i = 0; i < unit.Skills.Length; i++) {
            MultiUseListMng item = SkillScroll.makeListItemMasterId(i, unit.Skills[i], "", MultiUseListMng.BUTTON.HIDE );
            item.ExtraTxts[SKILL_LIST_COST_INDEX].text = unit.Skills[i].Cost.ToString();
            item.gameObject.SetActive(true);
        }
        SkillScroll.ListItem.SetActive(false);
        SkillBase.SetActive(true);
    }

    public void pushCloseSkill() {
        CommonProcess.playCanselSe();
        closeSkill();
    }

    public void closeSkill() {
        SkillBase.SetActive(false);
    }


    public void pushReturnHome(){
		SceneManagerWrap.loadBefore ();// (CmnConst.SCENE.HomeScene);
	}

    void nextMember() {

        if (!GameConst.IS_ASSIGN_TIME) {
            return;
        }

        //if (ListMode == MODE.CHARA_VIEW && SaveMng.Units.Count < UnitMast.List.Length) {

        //    UnitMast mst = UnitMast.List[SaveMng.Units.Count];

        //    PartyListItemMng mng = addList();
        //    mng.Btn.gameObject.SetActive(false);
        //    mng.PartyInedIcon.SetActive(false);
        //    System.DateTime lastlog = SaveMng.SysDt.getLoginDate().Date;
        //    lastlog = lastlog.AddDays(1);
        //    double diff_hour = UtilToolLib.diffHourTime(lastlog, System.DateTime.Now);
        //    int hour_num = (int)Mathf.Ceil((float)diff_hour);

        //    string text = mst.Name;
        //    text += "\n"+UnitMast.JOB_NAME[(int)mst.Job];
        //    text += $"\n\nこの{GameConst.HEROS_NAME}は、あと\n{hour_num }時間\nで復活します";
  
        //    mng.Info.text = text;

        //}
    }

		
}

