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
        ALL
    }

    public MODE Mode = MODE.VIEW;

    new private void Awake() {
        base.Awake();
        Board = this;
    }

    new private void Start() {
        base.Start();
    }

    new protected void OnEnable() {
        base.OnEnable();
        unitSetting();
    }

    void unitSetting() {
        UserGroup.GroupBase.SetActive(true);
        UserGroup.CreateGroup();
        UserGroup.SettingButtonInvoke(selectOnUnit);
        UserGroup.SettingSelectInvoke(updateSkillList);
    }

    public SkillBoardMng init(Transform parent) {
        if (SaveMng.Quest.IsQuest) {
            Mode = MODE.USE;
        } else {
            showViewMode();
        }
        if (Board == null) {
            Board = (SkillBoardMng)base.Init(parent);
        } else {
            Board.gameObject.SetActive(true);
        }
        return Board;
    }

    public static bool showViewMode(Transform source = null) {
        if (Board == null) {
            return false;
        }
        Board.showViewMode();

        Board.updateSibiling(source);

        return true;
    }

    public void showViewMode() {
        Mode = MODE.VIEW;
        gameObject.SetActive(true);
        TargetGroup.GroupBase.SetActive(false);
        UserGroup.GroupBase.SetActive(false);
    }

    public void setUnitSelectRecv() {
        UserGroup.setInputReciv(true);
    }

    public void updateSkillList(int unitTranId) {
        var unit = SaveMng.UnitData.getData(unitTranId);
        updateSkillList(unit);
    }

    /// <summary>
    /// スキルリスト更新
    /// </summary>
    /// <param name="unit"></param>
    public void changeUnit(UnitStatusTran unit) {
        updateSkillList(unit);
        UserGroup.SetCheckMark(unit.Id, true);
        Scroll.Recive.initSetupWithFrameEnd(true);
    }

    private void updateSkillList(UnitStatusTran unit) {
        
        SkillMast[] skills = null;

        if (Mode == MODE.USE) {
            skills = unit.Skills.Where(it => it.canUse()).ToArray();
            //if (SaveMng.Quest.IsBattle) {
            //    skills = unit.Skills.Where(it => it.canUse(PowerMast.USE_TIMING.BATTLE)).ToArray();
            //} else {
            //    skills = unit.Skills.Where(it => it.UseTiming == PowerMast.USE_TIMING.DUAL || it.UseTiming == PowerMast.USE_TIMING.FIELD).ToArray();
            //}
        } else {
            skills = unit.Skills;
        }

        Scroll.makeList(skills);
    }

    public void selectOnUnit(int unitTranId) {
        var unit = SaveMng.UnitData.getData(unitTranId);
        changeUnit(unit);
    }

    public void changeSelectInfo(MultiUseListMng list) {
        var mast = SkillMast.List.FindById(list.Id);
        Scroll.Recive.setGuidMessage(mast.Detail);
    }


    public void changeSelectDetail(MultiUseListMng list) {
        var mast = SkillMast.List.FindById(list.Id);
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
                readyUseTarget(skill);
                break;
            }
        }
    }

    public override CmnConst.BOARD_STATUS closeWindow() {

        if (ActiveBase) {
            if (TargetGroup.closeWindow()) {
                if (Scroll.Recive.IsActive) {
                    UserGroup.SetCheckMark(false);
                    UserGroup.InputReciv.initSetupWithFrameEnd();
                } else {
                    ActiveBase = false;
                    return CmnConst.BOARD_STATUS.CLOSING;
                }
                return CmnConst.BOARD_STATUS.OPEN;
            }
        }
        return CmnConst.BOARD_STATUS.CLOSED;;
    }
}
