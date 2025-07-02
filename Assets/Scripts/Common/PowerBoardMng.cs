using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PowerBoardMng : MonoBehaviour
{
    [SerializeField]
    protected CharaImgGroupMng TargetGroup;

    [SerializeField]
    protected MultiUseScrollMng Scroll;

    [System.NonSerialized]
    public PowerMast SelectedPower;

    [System.NonSerialized]
    protected List<UnitStatusTran> TargetUnits;

    [System.NonSerialized]
    public System.Action CallbackClose;

    public bool ActiveBase {
        get {
            return this.gameObject.activeSelf;
        }
        set {
            this.gameObject.SetActive(value);
        }
    }

    protected void Awake() {
        if (Scroll == null) {
            Scroll = this.GetComponentInChildren<MultiUseScrollMng>();
        }
    }


    protected void Start() {
    }

    protected void OnEnable() {
        TargetGroup.GroupBase.SetActive(false);
    }

    public void readyUseTarget() {
        TargetGroup.GroupBase.SetActive(true);
        TargetGroup.gameObject.SetActive(true);
        bool check = SelectedPower.Target == PowerMast.TARGET.ANYTHING;
        TargetGroup.CreateGroup(check);
        TargetGroup.InputReciv.initSetupWithFrameEnd(true);
    }
    public void selectUnit(CharaImgGaugeMng chara) {
        if (SelectedPower.Target == PowerMast.TARGET.ANYTHING) {
            TargetUnits = SaveMng.Quest.ActiveParty;
        } else {
            TargetUnits = new List<UnitStatusTran>();
            var unit = SaveMng.Units.Find(it => it.Id == chara.UnitTranId);
            if (unit != null) {
                TargetUnits.Add(unit);
            }
        }

        PowerProcess.execPower(SelectedPower, TargetUnits);

        StartCoroutine(showEffects(SelectedPower.Effect));
    }

    protected IEnumerator showEffects(string effect) {

        for (var i = 0; i < TargetUnits.Count(); i++) {
            var posi = TargetGroup.Members.Find(it => it.UnitTranId == TargetUnits[i].Id).transform.position;
            CommonProcess.show2DEffect(effect, posi);
        }
        yield return new WaitForSeconds(1f);

        TargetGroup.closeWindow();
    }

    public virtual bool closeWindow() {
        if (ActiveBase) {
            if (TargetGroup.closeWindow()) {
                if (CallbackClose != null) {
                    CallbackClose();
                }
                ActiveBase = false;
            }
            return false;
        }
        return true;
    }
}
