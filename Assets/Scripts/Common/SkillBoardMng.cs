using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class SkillBoardMng : PowerBoardMng
{
    [SerializeField]
    GamePadListRecivMng Recive;

    [System.NonSerialized]
    public SkillMast SelectedSkill;

    public static SkillBoardMng BoardInstance;

    public enum MODE
    {
        VIEW,
        USE,
        BATTLE,//現状未使用
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
    }

    public SkillBoardMng init(Transform parent) {
        if (BoardInstance == null) {
            BoardInstance = Instantiate(this);
            BoardInstance.transform.SetParent(parent);
            BoardInstance.transform.localPosition = Vector3.zero;
        }
        BoardInstance.gameObject.SetActive(true);
        return BoardInstance;
    }

    /// <summary>
    /// スキルリスト更新
    /// </summary>
    /// <param name="unit"></param>
    public void changeUnit(UnitStatusTran unit) {

        Scroll.makeList(unit.Skills);

        Recive.initSetupWithFrameEnd(true);

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
