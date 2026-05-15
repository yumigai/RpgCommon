using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class PowerBoardMng : MonoBehaviour
{
    [SerializeField]
    protected CharaImgGroupMng TargetGroup;

    [SerializeField]
    protected MultiUseScrollMng Scroll;

    [SerializeField]
    public Text DetailName;

    [SerializeField]
    public Text DetailInfo;

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
        if (TargetGroup.GroupBase != null) {
            TargetGroup.GroupBase.SetActive(false);
        }
    }

    public PowerBoardMng Init(Transform parent) {

        var bd = Instantiate(this);
        bd.transform.SetParent(parent);
        bd.transform.localPosition = Vector3.zero;
        bd.gameObject.SetActive(true);
        return bd;
    }

    public void readyUseTarget(MultiUseListMng mng) {
        if (SelectedPower.canUse()) {
            mng.check(true);
            TargetGroup.GroupBase.SetActive(true);
            TargetGroup.gameObject.SetActive(true);
            bool check = SelectedPower.Target == PowerMast.TARGET.ANYTHING;
            TargetGroup.CreateGroup(check);
            TargetGroup.InputReciv.initSetupWithFrameEnd(true);
        }
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

    public void showDetail(PowerMast mst) {
        string name = mst == null ? "" : mst.Name;
        string info = mst == null ? "" : mst.Detail;
        if (DetailName != null) {
            DetailName.text = name;
        }
        if (DetailInfo != null) {
            DetailInfo.text = info;
        }
    }

    protected IEnumerator showEffects(string effect) {

        for (var i = 0; i < TargetUnits.Count(); i++) {
            var posi = TargetGroup.Members.Find(it => it.UnitTranId == TargetUnits[i].Id).transform.position;
            CommonProcess.show2DEffect(effect, posi);
        }
        yield return new WaitForSeconds(1f);

        TargetGroup.closeWindow();
    }

    protected void updateSibiling( Transform source) {
        if (source != null
                && this.transform.parent == source.parent
                && this.transform.GetSiblingIndex() < source.GetSiblingIndex()) {
            this.transform.SetSiblingIndex(source.GetSiblingIndex() + 1);
        }
    }

    protected void execPower(PowerMast pow, List<UnitStatusTran> trans) {
        foreach (var tran in trans ) {
            PowerProcess.execPower(pow, TargetUnits);
            var unit = TargetGroup.Members.Find(it => it.UnitTranId == tran.Id);
            unit.updateStatus();
            unit.effect(EffectMng.Key.Heal);
        }
        
    }

    public virtual CmnConst.BOARD_STATUS closeWindow() {
        if (ActiveBase) {
            if (TargetGroup.closeWindow()) {
                if (CallbackClose != null) {
                    CallbackClose();
                }
                ActiveBase = false;
                return CmnConst.BOARD_STATUS.CLOSING;
            }
            return CmnConst.BOARD_STATUS.OPEN;
        }
        return CmnConst.BOARD_STATUS.CLOSED;
    }

}
