using UnityEngine;
using System.Collections;

public class BaseConfigSceneMng : CmnConfigMng {

    public const float VolumeChangeUnit = 0.01f;

    [SerializeField]
    public InputNameMng InputNameObj;

    [SerializeField]
    public LangageSelectMng LangSelect;

    [SerializeField]
    public SaveBoardMng SaveBoard;

    [SerializeField]
    public Transform Parent;

    private void Awake() {
        InputNameObj?.gameObject.SetActive(false);
        LangSelect?.gameObject.SetActive(false);
    }

    protected new void Start() {
        base.Start();
        SaveMng.SysDt.saveSaveDate();
        SaveMng.SysDt.save();
    }

    protected new void Update() {

        VolumeSet();

        if (ReadySeCheck) {
            float add = Input.GetAxisRaw("Horizontal");
            if (add == 0) {
                SoundMng.Instance.playSE(SeCheck);
                ReadySeCheck = false;
            }
        }
    }

    public void pushChangeName()
    {
        CommonProcess.playClickSe();
        GameObject obj = Instantiate(InputNameObj.gameObject) as GameObject;
        if (Parent == null)
        {
            obj.transform.parent = this.transform;
            obj.transform.localPosition = InputNameObj.transform.localPosition;
            obj.transform.localScale = InputNameObj.transform.localScale;
        }
    }

    public void updateBgm() {
        GameObject pushBtn = UnityEngine.EventSystems.EventSystem.current.currentSelectedGameObject;
        if (pushBtn.name == "Bgm") {
            float add = GamePadButtonMng.GetAxisFloat(GamePadButtonMng.AnalogRawType.DUAL, GamePadButtonMng.AxisType.Horizontal);
            sliderBgm.value += VolumeChangeUnit * add;
        }
    }

    public void updateSe() {
        GameObject pushBtn = UnityEngine.EventSystems.EventSystem.current.currentSelectedGameObject;
        if (pushBtn.name == "Se") {
            float add = Input.GetAxisRaw("Horizontal");
            sliderSe.value += VolumeChangeUnit * add;
            ReadySeCheck = true;
        }
    
    }

    public void pushSaveData() {
        SaveBoard.showSaveBoard(Parent);
    }

    public void pushLoadData() {
        SaveBoard.showLoadBoard(Parent);
    }

    public void pushSaveConfig() {
        saveSound();
        SceneManagerWrap.loadBefore();
    }

    public void pushKeyConfig() {
        SceneManagerWrap.LoadScene(CmnConst.SCENE.KeyConfigScene);
    }

    public void pushLanguageSelect() {
        LangSelect.show();
        //LangageSelectMng.show(LangSelectObj.gameObject, this.Parent);
    }

    /// <summary>
    /// セーブ状態の表示状況
    /// </summary>
    /// <returns></returns>
    public bool IsShowSaveBoard() {
        return SaveBoardMng.IsShowSaveBoard();
        //if (SaveBoardMng.InstanceObject != null) {
        //    return SaveBoardMng.InstanceObject.activeSelf;
        //}
        //return false;
    }
}
