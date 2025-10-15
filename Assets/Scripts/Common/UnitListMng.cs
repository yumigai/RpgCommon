using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class UnitListMng : MonoBehaviour
{
    [SerializeField]
    public CharaImgGroupMng ListItems;

    [SerializeField]
    private GamePadListRecivMng Recive;

    [SerializeField]
    public UnitDetailMng Info;

    [SerializeField]
    public SkillBoardMng SkillBoard;

    public System.Action<CharaImgGaugeMng> PushUnitCallback;

    public System.Action CloseCallback;
    //public System.Action PushCanselCallback;

    private int ChangeIndex = -1;

    private void Start() {
        //ListItems.CreateGroup(SaveMng.Units);
        //Recive.initSetup(true);
    }

    private void OnEnable() {
        //OnEnable処理をStartの後に行う
        StartCoroutine(initListProcess());
    }

    IEnumerator initListProcess() {
        ListItems.CreateGroup(SaveMng.GetActiveAllUnits());
        yield return new WaitForEndOfFrame(); //リスト更新のためフレーム終了まで待つ
        Recive.initSetupWithFrameEnd(true);
    }

    //public void initList() {
    //    StartCoroutine(initListProcess());
    //}

    public void changeUnitSelect(CharaImgGaugeMng gauge) {
        var unit = SaveMng.Units.Where(it => it.Id == gauge.UnitTranId).FirstOrDefault();
        Info.setParams(unit);
    }

    public void pushSelectUnit(CharaImgGaugeMng gauge) {
        PushUnitCallback(gauge);
    }

    public void pushUnitSkill(CharaImgGaugeMng gauge) {

        var unit = SaveMng.Units.Where(it => it.Id == gauge.UnitTranId).FirstOrDefault();
        SkillBoard.init(this.transform).changeUnit(unit);

    }

    //public void pushCansel() {
    //    PushCanselCallback();
    //}

    /// <summary>
    /// メンバーチェンジ開始
    /// </summary>
    /// <param name="index"></param>
    public void pushMemberChange(int index)
    {
        CommonProcess.playClickSe();
        ChangeIndex = index;
    }

    /// <summary>
    /// メンバーチェンジ実行
    /// </summary>
    /// <param name="mng"></param>
    public void pushChangeExec(CharaImgGaugeMng unit)
    {
        var before_index = System.Array.IndexOf(SaveMng.Status.ActiveMember, unit.UnitTranId);
        if (before_index >= 0)
        {
            SaveMng.Status.ActiveMember[before_index] = SaveMng.Status.ActiveMember[ChangeIndex];
        }

        SaveMng.Status.ActiveMember[ChangeIndex] = unit.UnitTranId;
        SaveMng.Status.save();

        if(CloseCallback == null){
            StartCoroutine(initListProcess());
        }else{
            CloseCallback();
        }
    }
}
