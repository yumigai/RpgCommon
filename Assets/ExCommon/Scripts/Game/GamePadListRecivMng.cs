using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityStandardAssets.CrossPlatformInput;

/// <summary>
/// 非推奨クラス
/// </summary>
//重複防止
[DisallowMultipleComponent]
public class GamePadListRecivMng : MonoBehaviour {

    public enum LoopType {
        Stop,
        Loop,
    }

    [SerializeField]
    public List<GamePadButtonMng> Buttons;

    //[SerializeField]
    //public string[] GuidTextsJp;

    //[SerializeField]
    //public string[] GuidTextsEn;

    [SerializeField]
    public Text GuidMessage;

    [SerializeField]
    public GamePadButtonMng.AxisType Type = GamePadButtonMng.AxisType.Vertical;

    [SerializeField]
    public LoopType Loop;

    //[SerializeField]
    //public UnityEvent CancelEvent;

    [SerializeField]
    public ScrollPageControllMng PageController;

    [SerializeField,Header("対象のオブジェクトがdisableにならない場合はtrue")]
    private bool StartAndInit = false;

    [System.NonSerialized]
    public System.Action Callback;

    //[System.NonSerialized]
    //public System.Action CallbackCancel;

    [System.NonSerialized]
    private int Selecter = 0;

    [System.NonSerialized]
    private GamePadListRecivMng _beforeGamePadList = null;

    public static GamePadListRecivMng _activeGamePadList = null;

    public static GamePadListRecivMng _justBeforeGamePadList = null; //直前のアクティブ（GamePadListRecivMngをnullにした場合の戻り先）

    //public static bool IsReadyCancel = true;

    public static GamePadListRecivMng ActiveGamePadList { get { return _activeGamePadList; } set {changeActiveList(value);} }

    public bool IsActive { get{ return this == ActiveGamePadList;  } }

    public int NowSelector {get{return Selecter;}}

    /// <summary>
    /// リストアイテムのサイズが同一である前提
    /// 複数サイズにする場合、配列化してスクロール移動する際にsumして算出
    /// </summary>
    [System.NonSerialized]
    private Vector2 ListSize = new Vector2();

    private void Awake() {
        PageController = PageController == null ? this.GetComponent<ScrollPageControllMng>() : PageController;
    }

    private void Start() {
        if (this.gameObject.GetComponent<MultiUseScrollMng>() == null && StartAndInit) {
            //MultiUseScrollMngがnullの場合、リストは固定と想定
            initSetup();
        }
    }

    /// <summary>
    /// 入力権を移す処理を１フレーム分待つ
    /// </summary>
    /// <param name="over_ride"></param>
    public void setupInputRecive(bool over_ride = false) {
        StartCoroutine(setupInputReciveProcess(over_ride));
    }

    public IEnumerator setupInputReciveProcess(bool over_ride = false) {
        yield return new WaitForEndOfFrame();
        initSetup(over_ride);
    }

    //OnEnableで入力受付を更新すると、リストの更新タイミングとずれてフォーカスを失う可能性が高いので避ける
    //private void OnEnable() {
    //    StartCoroutine(activeOn());
    //}

    //IEnumerator activeOn() {
    //    yield return new WaitForEndOfFrame();
    //    initSetup();
    //}

    /// <summary>
    /// 初期グループ設定(AwakeでやるとSelectedObjectが外れるから注意）
    /// 対象リストをInstantiateやdestroyする場合、生成でラグが発生するので、
    /// initSetupする前にyield return new WaitForEndOfFrame()すること
    /// </summary>
    /// <param name="over_ride">ボタン情報取り直し</param>
    protected void initSetup(bool over_ride = false) {

        if (Buttons == null || over_ride) {
            Buttons = new List<GamePadButtonMng>(); 
        }

        // GetComponentsInChildrenで取ると、自分と同じ階層のGamePadButtonMngも取ってくるのでchildで回す
        if (Buttons.Count == 0) {
            for (int i = 0; i < this.transform.childCount; i++) {
                GamePadButtonMng child = this.transform.GetChild(i).GetComponent<GamePadButtonMng>();
                if ( child != null && child.gameObject.activeSelf) {
                    Buttons.Add(child);
                }
            }
        }

        if (Buttons.Count > 0) {
            int button_index = 0;
            Buttons.ForEach(it => it.ListIndex=button_index++);
            ListSize = Buttons[0].GetComponent<RectTransform>().sizeDelta;
            //ActiveGamePadList = this;
        }

        //選択状態を初期化
        Selecter = 0;

        ActiveGamePadList = this;

        Canvas.ForceUpdateCanvases();//Canvas更新

        for (int i = 0; i < Buttons.Count; i++) {

            var posi = Buttons[i].transform.position;
            Buttons[i].ListRecive = this;
            Navigation navi = Buttons[i].navigation;
            navi.mode = Navigation.Mode.Explicit;

            switch (Type) {
                case GamePadButtonMng.AxisType.Dual:
                    var buttons = Buttons.FindAll(it => it.transform.position.y > posi.y && it.transform.position.x == posi.x);
                    if (buttons.Count > 0) {
                        var min = buttons.Min(it=>it.transform.position.y);
                        navi.selectOnUp = buttons.Find(it => it.transform.position.y==min);
                    }
                    buttons = Buttons.FindAll(it => it.transform.position.y < posi.y && it.transform.position.x == posi.x);
                    if (buttons.Count > 0) {
                        var max = buttons.Max(it => it.transform.position.y);
                        navi.selectOnDown = buttons.Find(it => it.transform.position.y == max);
                    }
                    buttons = Buttons.FindAll(it => it.transform.position.x < posi.x && it.transform.position.y == posi.y);
                    if (buttons.Count > 0) {
                        var min = buttons.Min(it => it.transform.position.x);
                        navi.selectOnLeft = buttons.Find(it => it.transform.position.x == min);
                    }
                    buttons = Buttons.FindAll(it => it.transform.position.x > posi.x && it.transform.position.y == posi.y);
                    if (buttons.Count > 0) {
                        var max = buttons.Max(it => it.transform.position.x);
                        navi.selectOnRight = buttons.Find(it => it.transform.position.x == max);
                    }

                    break;
                case GamePadButtonMng.AxisType.Horizontal:
                    if (i > 0) {
                        navi.selectOnLeft = Buttons[i - 1];
                    }
                    if (i < Buttons.Count - 1) {
                        navi.selectOnRight = Buttons[i + 1];
                    }
                    break;
                case GamePadButtonMng.AxisType.Vertical:
                    if (i > 0) {
                        navi.selectOnUp = Buttons[i - 1];
                    }
                    if (i < Buttons.Count - 1) {
                        navi.selectOnDown = Buttons[i + 1];
                    }
                    break;
            }
            
            Buttons[i].navigation = navi;
        }

        if (Buttons != null && Buttons.Count > 0) {
            Buttons[0].SeletedButton();
        }

        PageController?.init();

    }

    /// <summary>
    /// フレーム終了時にボタン設定
    /// </summary>
    /// <param name="over_ride"></param>
    public void initSetupWithFrameEnd(bool over_ride = false) {
        TimeInvokeMng.FrameEndAction(this.gameObject, () => { initSetup(over_ride); });
    }

    private void Update() {
        //Debug.Log("active : " + _activeGamePadList.transform.parent.parent.name);
        //Debug.Log("true : " + EventSystem.current.currentSelectedGameObject);
        inputAction();
    }

    /// <summary>
    /// 共通入力
    /// </summary>
    private void inputAction() {
        if (IsActive) {
            //if (IsReadyCancel && CrossPlatformInputManager.GetButtonDown(CmnConfig.GamePadButton.Cancel.ToString())) {
            //    CancelEvent?.Invoke();
            //    IsReadyCancel = false;
            //} else 
            if (GamePadButtonMng.GetAxisInt(GamePadButtonMng.AnalogRawType.DUAL, Type, true) != 0) {
                //フォーカスが外れた場合の復帰処理（消すな）
                if (EventSystem.current.currentSelectedGameObject == null) {
                    setSelectedGameObject(this);
                } else {
                    GameObject btn = getActiveButton();
                    var btnmng = btn.GetComponent<GamePadButtonMng>();
                    if (btnmng != null) {
                        Selecter = btnmng.ListIndex;
                    }
                }
            } 
            //else {
            //    IsReadyCancel = true;
            //}
        }
    }

    /// <summary>
    /// アクティブボタン取得
    /// </summary>
    /// <returns></returns>
    public GameObject getActiveButton() {

        if (Buttons == null || EventSystem.current == null) {
            return null;
        }

        return EventSystem.current.currentSelectedGameObject;

    }

    /// <summary>
    /// アクティブボタンの有効切り替え
    /// </summary>
    /// <param name="val"></param>
    //public void setActiveButtonInteractable(bool val) {
    //    GameObject btn = getActiveButton();
    //    if (btn != null) {
    //        btn.GetComponent<GamePadButtonMng>().changeActive(val);
    //    }
    //}

    public static void changeActiveList( GamePadListRecivMng list, bool record_before = true ) {

        if (_activeGamePadList != null && record_before && list != null && _activeGamePadList != list) {
            list._beforeGamePadList = _activeGamePadList;
        }

        if(list == null) {
           _justBeforeGamePadList = _activeGamePadList;
        }
           
        _activeGamePadList = list;

        setSelectedGameObject(list);
    }


    private static void setSelectedGameObject(GamePadListRecivMng list) {
        if (list != null) {
            list.setSelectedButton();
        }
    }

    public void setSelectedButton() {
        if (Buttons != null && Buttons.Count > 0) {
            //Buttons.ForEach(it => it.setCheckMark(false));
            if(Buttons.Count >= Selecter) {
                Selecter = 0;
            }
            var btn = Buttons[Selecter];
            if (btn != null) {
                if (EventSystem.current != null) {
                    EventSystem.current.SetSelectedGameObject(btn.gameObject);
                }
            } else {
                //init時のエラーでButtonsにnullが混じった場合、削除
                Buttons.RemoveAt(Selecter);
            }
        } else {
        }
    }

    /// <summary>
    /// 現在アクティブになっているのが本当にこのリストか？（マウス操作などで他のボタンにフォーカス移った時）
    /// </summary>
    /// <returns></returns>
    public bool isActiveThis() {
        var reciv = EventSystem.current.currentSelectedGameObject.GetComponentInParent<GamePadListRecivMng>();
        return reciv == this;
    }

    /// <summary>
    /// ボタンのアクティブ状態を変更する
    /// </summary>
    /// <param name="cng"></param>
    public void changeButtonsOnOff(bool cng) {
        Buttons.ForEach(it => it.interactable = cng);
    }

    /// <summary>
    /// シーン戻る
    /// </summary>
    public void toBeforeScene() {
        SceneManagerWrap.loadBefore();
    }

    public void OnDisable() {
        returnBeforeGamePad();
    }

    public void OnDestroy() {
        returnBeforeGamePad();
    }
    private void returnBeforeGamePad() {
        if (_beforeGamePadList != null) {
            //Debug.Log("CahgeActive:" + ActiveGamePadList.transform.parent.parent );
            //Debug.Log("CahgeActive Next:" + _beforeGamePadList.transform.parent.parent);
            changeActiveList(_beforeGamePadList, false);
        }
    }

    /// <summary>
    /// 外部からのアクティブ戻し（確認ウインドウなど、activeGamePadがnullの場合に使う）
    /// </summary>
    public static void returnJustBeforeGamePad() {
        changeActiveList(_justBeforeGamePadList, false);
    }

    public void CallbackAction() {
        Callback();
    }

    //public void CallbackCancelAction() {
    //    if (CallbackCancel == null) {
    //        toBeforeScene();
    //    } else {
    //        CallbackCancel();
    //    }
    //}

    public int getButtonIndex( GamePadButtonMng btn ) {
        return Buttons.IndexOf(btn);
    }

    public void active() {
        ActiveGamePadList = this;
    }
}
