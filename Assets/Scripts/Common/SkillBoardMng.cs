using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class SkillBoardMng : PowerBoardMng
{

    [SerializeField]
    protected CharaImgGroupMng UserGroup;

    [System.NonSerialized]
    public SkillMast SelectedSkill;

    public static SkillBoardMng Board;

    public enum MODE
    {
        VIEW,
        USE,
        //BATTLE,
        ALL

    }

    public MODE Mode = MODE.VIEW;

    new private void Awake() {
        base.Awake();
    }

    new private void Start() {
        base.Start();
    }

    new protected void OnEnable() {
        base.OnEnable();
        UserGroup.CreateGroup();
    }

    public SkillBoardMng init(Transform parent) {
        //if (Board == null) {
        //    Board = Instantiate(this);
        //    Board.transform.SetParent(parent);
        //    Board.transform.localPosition = Vector3.zero;
        //}
        //Board.gameObject.SetActive(true);
        //return Board;
        return (SkillBoardMng)base.Init(parent, Board);
    }

    /// <summary>
    /// スキルリスト更新
    /// </summary>
    /// <param name="unit"></param>
    public void changeUnit(UnitStatusTran unit) {

        SkillMast[] skills = null;

        if(Mode == MODE.USE){
            if (SaveMng.Quest.IsBattle){
                skills = unit.Skills.Where(it => it.UseTiming == PowerMast.USE_TIMING.DUAL || it.UseTiming == PowerMast.USE_TIMING.BATTLE).ToArray();
            }else{
                skills = unit.Skills.Where(it=>it.UseTiming == PowerMast.USE_TIMING.DUAL || it.UseTiming == PowerMast.USE_TIMING.FIELD).ToArray();
            }
        }else{
            skills = unit.Skills;
        }
        
        Scroll.makeList(skills);

        Scroll.Rcv.initSetupWithFrameEnd(true);

    }

    public void changeSelect(MultiUseListMng list) {

        var mast = SkillMast.List.Where(it => it.Id == list.Id).FirstOrDefault();
        showDetail(mast);
    }

    public void selectSkill(MultiUseListMng skill) {

        var mast = SkillMast.List.Where(it => it.Id == skill.Id).FirstOrDefault();
        SelectedSkill = mast;

        if (mast != null) {
            switch (Mode) {
                case MODE.VIEW:
                CommonProcess.showMessage(mast.Name, mast.Info);
                break;
                case MODE.USE:
                readyUseTarget();
                break;
            }
        }
    }

    public override bool closeWindow() {
        return base.closeWindow();
    }
}
