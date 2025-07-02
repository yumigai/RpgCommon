using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SaveBoardMng : MonoBehaviour
{
    [SerializeField]
    private Text AutoSaveSlot;

    [SerializeField]
    private Text[] SaveSlots;

    [SerializeField]
    private GameObject LoadTitle;

    [SerializeField]
    private GameObject SaveTitle;

    public static bool IsLoad = false;

    public static GameObject InstanceObject; 

    private void Awake() {
        InstanceObject = this.gameObject;
    }

    private void Start() {

    }

    private void OnEnable() {

        LoadTitle.SetActive(IsLoad);
        SaveTitle.SetActive(!IsLoad);

        AutoSaveSlot.text = SaveMng.SysDt.getSaveDate().ToString();

        for (int i = 0; i < SaveSlots.Length; i++) {
            int slot = i + 1;
            if (SaveMng.isEmpty(slot)) {
                SaveSlots[i].text = "Empty";
            } else {
                var sys = SaveMng.load<SaveMng.SystemTran>(true, slot);
                SaveSlots[i].text = sys.getSaveDate().ToString();
            }
        }
    }

    /// <summary>
    /// セーブ/ロードボタン
    /// </summary>
    /// <param name="slot"></param>
    public void pushSave(int slot) {
        if( IsLoad) {
            if (CmnSaveProc.isEmpty(slot)) {
                CommonProcess.showMessage("データがありません");
            } else {
                ConfirmWindowCmn wnd = CommonProcess.showConfirm("load", (obj) => { loadExec(slot); });
            }
        } else {
            if (CmnSaveProc.isEmpty(slot)) {
                saveExec(slot);
            } else {
                ConfirmWindowCmn wnd = CommonProcess.showConfirm("save", (obj) => { saveExec(slot); });
            } 
        }
        
    }

    private void saveExec(int slot) {
        SaveMng.saveAll(slot);
        var sys = SaveMng.load<SaveMng.SystemTran>(true, slot);
        SaveSlots[slot-1].text = sys.getSaveDate().ToString();
    }

    private void loadExec(int slot) {
        SaveMng.loadAll(slot);
        SceneManagerWrap.clearBefore();
        SceneManagerWrap.LoadScene(CmnConst.SCENE.InitScene, false);
    }

    public void closeWindow() {
        Destroy(InstanceObject);
    }

    /// <summary>
    /// Loadボード表示
    /// </summary>
    /// <param name="parent"></param>
    public void showLoadBoard(Transform parent) {
        IsLoad = true;
        showBoard(this.gameObject, parent);
    }

    /// <summary>
    /// Saveボード表示
    /// </summary>
    /// <param name="parent"></param>
    public void showSaveBoard(Transform parent) {
        IsLoad = false;
        showBoard(this.gameObject, parent);
    }
    private static void showBoard(GameObject go, Transform parent) {
        if (InstanceObject == null) {
            var saveBoard = Instantiate(go);
            saveBoard.transform.SetParent(parent);
            saveBoard.transform.localPosition = Vector3.zero;
        }
    }

    public static bool IsShowSaveBoard() {
        if (InstanceObject != null) {
            return InstanceObject.activeSelf;
        }
        return false;
    }

    //public static void ShowBoard( bool val ) {
    //    if (Singleton != null && Singleton.gameObject != null) {
    //        Singleton.gameObject.SetActive(val);
    //    }
    //}

    //public static bool IsShowBoard() {
    //    return (Singleton != null && Singleton.gameObject.activeSelf);
    //}

}
