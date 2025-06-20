using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityStandardAssets.CrossPlatformInput;

public class GamePadButtonMng : Button { //, IPointerClickHandler {

    /// <summary>
    /// GetAxisRaw代わりのしきい値
    /// </summary>
    public const float RawThresholdMin = 0.2f;

    public const float RawThresholdMax = 0.8f;

    public enum PushStatus
    {
        Off,
        Push,
        Hold,
        HoldEnd,
    }

    public enum AnimeTrriger {
        On,
        Off,
    }

    public enum AxisType {
        Horizontal,
        Vertical,
        Dual,
    }

    /// <summary>
    /// アナログ・デジタル・両方
    /// </summary>
    public enum AnalogRawType {
        ANALOG,
        DIGITAL,
        DUAL,
    }

    [SerializeField,Header("Decisionを指定する場合、OnClickかPadPushEventのどちらか片方のみ設定")]
    public CmnConfig.GamePadButton DealButton;

    /// <summary>
    /// OnClickイベントと別にしないと、ボタンをマウスクリックした場合に重複してイベント発生するので分けてる
    /// </summary>
    //[SerializeField]
    //public UnityEvent PadPushEvent;

    //[SerializeField]
    //public Animator Anime;

    //[SerializeField,Tooltip("長押し状態キャンセル")]
    //public UnityEvent HoldOutEvent;

    [SerializeField, Tooltip("長押し完了イベント")]
    public UnityEvent HoldFinishEvent;

    [SerializeField]
    public GamePadListRecivMng ListRecive;

    [SerializeField]
    protected bool IsHold = false;

    [SerializeField]
    public Image HoldGauge;

    [SerializeField,Tooltip("長押し完了までの時間")]
    public float FinishHoldTime = 2f;

    [SerializeField, Tooltip("非アクティブ時非表示")]
    public bool IsNonActiveHide = false;

    //[SerializeField]
    //public CmnConfig.Axis PadAxis;

    //[SerializeField]
    //public GameObject CheckMark;

    /// <summary>
    /// GamePadListRecivMngに対するindex
    /// </summary>
    public int ListIndex = 0;

    private string _dealStr = "";

    //private bool IsBeforePush = false;

    /// <summary>
    /// 長押しマウス用（パッドと別にしておかないと、どちらかの長押しキャンセルを拾ってしまう）
    /// </summary>
    private bool HoldingMouseDown = false;

    /// <summary>
    /// 長押しパッド用
    /// </summary>
    private bool HoldingPadDown = false;

    /// <summary>
    /// キー長押し時間
    /// </summary>
    private float HoldKeyTime = 0f;

    private LanguageStaticTextMng LangText;

    private static bool IsAxisRest = true;

    public static int AxisHorizontal = 0;

    public static int AxisVertical = 0;

    public static Dictionary<CmnConfig.GamePadButton, List<GamePadButtonMng>> Standby
    = new Dictionary<CmnConfig.GamePadButton, List<GamePadButtonMng>>();

    public List<GamePadButtonMng> StandByList {
        get {
            if (!Standby.ContainsKey(DealButton)) {
                Standby[DealButton] = new List<GamePadButtonMng>();
            }
            return Standby[DealButton];
        }
    }

    public string JpGuid {
        get {
            return LangText == null ? "" : LangText.JpText;
        }
    }

    public string EnGuid {
        get {
            return LangText == null ? "" : LangText.EngText;
        }
    }

    protected string DealStr{ get{
            if (_dealStr.Length == 0) {
                //_dealStr = CmnSaveProc.getKeyStr(DealButton);
                _dealStr = DealButton.ToString();
            } return _dealStr; } }

    protected override void Awake() {
        base.Awake();
        LangText = LangText == null ? GetComponent<LanguageStaticTextMng>() : LangText;
        InitReflesh();
    }

    void InitReflesh() {
        if (Standby.ContainsKey(DealButton)) {
            Standby[DealButton].RemoveAll(it => it == null || it.gameObject.activeSelf == false);
        }
    }

    protected new void OnEnable() {
        base.OnEnable();
        updateActive(false);
        StandByList.Add(this);
    }

    protected new void OnDisable() {
        base.OnDisable();
        StandByList.Remove(this);
        updateActive(true);
    }

    protected void updateActive(bool val) {
        if (IsNonActiveHide && StandByList.Count() > 0 && ListRecive == null) {
            StandByList.Last().gameObject.SetActive(val);
        }
    }


    protected void Update() {

        if (!IsInputPriority()) {
            return;
        }

        //ゲームパッド・キーボードチェック
        //リストの場合
        if (ListRecive == null) {
            input();
        } else if (ListRecive.getActiveButton() == this.gameObject && DealButton != CmnConfig.GamePadButton.Decision) {
            //このボタンが選択状態、かつ、決定ボタンでない場合（決定はEventSystemによって発火させる）
            input();
        }
        
        //マウス入力チェック
        if (IsHold && HoldingMouseDown) {
            //マウスの長押し状態
            if (Input.GetMouseButton(0) && interactable) {
                hold();
                onClick.Invoke();
            } else {
                HoldingMouseDown = false;
                updHoldTime(0);
            }
        }

        if(IsHold && ( HoldingMouseDown || HoldingPadDown)) {
            updGauge();
        }
    }

    /// <summary>
    /// パッドボタン入力実行
    /// </summary>
    protected bool input() {

        if (interactable && (ListRecive == null || ListRecive.IsActive)) {

            if (IsHold) {
                var holding = inputKeyAction();
                if (!holding) {
                    holding = inputAxisAction();
                }

                if (holding) {
                    hold();
                } else if (HoldingPadDown) {
                    //長押し解除
                    updHoldTime(0);
                }

                HoldingPadDown = holding;

            } else {
                if (Input.anyKeyDown) {
                    inputKeyAction();
                }

                if(IsAxisDeal()) {
                    //方向入力は一旦素通しする
                    inputAxisAction();
                }
            }
        }

        return false;
    }

    /// <summary>
    /// 長押し状態
    /// </summary>
    void hold() {
        HoldKeyTime += Time.fixedDeltaTime;
        updHoldTime(HoldKeyTime);
        if (HoldKeyTime >= FinishHoldTime) {
            HoldFinishEvent.Invoke();
        }
    }

    /// <summary>
    /// パッド・キー入力
    /// </summary>
    bool inputKeyAction() {
        if (checkKeyAction()) {
            onClick.Invoke();
            return true;
        }
        return false;
        //if (DealButton < CmnConfig.GamePadButton.KeyAll && CrossPlatformInputManager.GetButton(DealStr)) {
        //    //パッドキー
        //    if (DealButton == CmnConfig.GamePadButton.Decision || DealButton == CmnConfig.GamePadButton.Cancel) {
        //        if (ListRecive == null && EventSystem.current.currentSelectedGameObject != this.gameObject) {
        //            //リストではなく、決定・キャンセルの対象の場合、選択状態でない場合も発火させるため、EventSystemから漏れた分をフォローする
        //            onClick.Invoke();
        //            return true;
        //        }
        //    } else {
        //        onClick.Invoke();

        //        return true;
        //    }
        //}
        //return false;
    }

    protected bool checkKeyAction() {
        if (DealButton < CmnConfig.GamePadButton.KeyAll && CrossPlatformInputManager.GetButton(DealStr)) {
            //パッドキー
            if (DealButton == CmnConfig.GamePadButton.Decision || DealButton == CmnConfig.GamePadButton.Cancel) {
                if (ListRecive == null && EventSystem.current.currentSelectedGameObject != this.gameObject) {
                    //リストではなく、決定・キャンセルの対象の場合、選択状態でない場合も発火させるため、EventSystemから漏れた分をフォローする
                    return true;
                }
            } else {
                return true;
            }
        }
        return false;
    }

    /// <summary>
    /// パッド・方向入力
    /// </summary>
    /// <returns></returns>
    protected bool inputAxisAction() {

        if (checkAxisAction()) {
            onClick.Invoke();
            return true;
        }
        return false;

        //if (IsAxisDeal()) {
        //    //方向キー
        //    int axis = 0;

        //    if (DealButton == CmnConfig.GamePadButton.Horizontal) {
        //        axis = GamePadButtonMng.GetAxisInt(GamePadButtonMng.AnalogRawType.DUAL, AxisType.Horizontal, IsHold);
        //        AxisHorizontal = axis;
        //    } else if (DealButton == CmnConfig.GamePadButton.Vertical) {
        //        axis = GamePadButtonMng.GetAxisInt(GamePadButtonMng.AnalogRawType.DUAL, AxisType.Vertical, IsHold);
        //        AxisVertical = axis;
        //    }
        //    if (axis != 0) {
        //        onClick.Invoke();
        //        return true;
        //    }
        //}
        //return false;
    }

    protected bool checkAxisAction() {
        if (IsAxisDeal()) {
            //方向キー
            int axis = 0;

            if (DealButton == CmnConfig.GamePadButton.Horizontal) {
                axis = GetAxisInt(GamePadButtonMng.AnalogRawType.DUAL, AxisType.Horizontal, IsHold);
                AxisHorizontal = axis;
            } else if (DealButton == CmnConfig.GamePadButton.Vertical) {
                axis = GetAxisInt(GamePadButtonMng.AnalogRawType.DUAL, AxisType.Vertical, IsHold);
                AxisVertical = axis;
            }
            if (axis != 0) {
                return true;
            }
        }
        return false;
    }



    /// <summary>
    /// マウス用のイベントリスナ（長押し用アクション）
    /// </summary>
    public void OnKeyDown() {
        if (IsHold) {
            HoldingMouseDown = true;
        }
    }

    /// <summary>
    /// ボタン離した時のアクション
    /// </summary>
    //void UpKeyAction() {
    //    if (IsBeforePush) {
    //        PadUpEvent?.Invoke();
    //        IsBeforePush = false;
    //    }
    //}

    /// <summary>
    /// キー判定
    /// </summary>
    /// <returns></returns>
    //bool checkKey() {

    //    if (DealButton < CmnConfig.GamePadButton.KeyAll && CrossPlatformInputManager.GetButton(DealStr)) {
    //        return true;
    //    }else if(IsAxisDeal()) {
    //        //方向キーの場合true(inputKeyActionで値取得と同時にチェックする
    //        return true;
    //    }
        
    //    //else if (DealButton == CmnConfig.GamePadButton.Horizontal) {
    //    //    //クリック用のチェックは既に行っているが、Hold用にGetAxisしている
    //    //    return CrossPlatformInputManager.GetAxis("Horizontal") != 0;
    //    //} else if (DealButton == CmnConfig.GamePadButton.Vertical) {
    //    //    return CrossPlatformInputManager.GetAxis("Vertical") != 0;
    //    //}
    //    return false;
    //}

    /// <summary>
    /// 方向キー入力
    /// 長押し未対応
    /// </summary>
    //private void inputAxis() {

    //    if (DealButton == CmnConfig.GamePadButton.Horizontal || DealButton == CmnConfig.GamePadButton.Vertical) {
    //        if (checkKey()) {
    //            PadPushEvent.Invoke();
    //        }
    //    }
    //}

    /// <summary>
    /// アクティブ状態切り替わり時動作
    /// </summary>
    /// <param name="val"></param>
    //public void changeActive( bool val ) {
    //    //interactable = val;
    //    //setCheckMark(val);
    //    string trriger = val ? AnimeTrriger.On.ToString() : AnimeTrriger.Off.ToString();
    //    if(Anime != null) { //Anime?.SetTrigger(trriger)だとバグる
    //        Anime.SetTrigger(trriger);
    //    }
    //}

    /// <summary>
    /// ボタン選択状態変更（EventTrigger.Selectで指定）
    /// </summary>
    public void SeletedButton() {
        if ( ListRecive != null && ListRecive.GuidMessage != null) {
            if (CmnSaveProc.IsJp) {
                ListRecive.GuidMessage.text = JpGuid;
            } else {
                ListRecive.GuidMessage.text = EnGuid;
            }
        }
    }

    /// <summary>
    /// 長押し用ゲージの更新
    /// </summary>
    /// <param name="val"></param>
    private void updGauge() {
        if(HoldGauge != null && FinishHoldTime > 0) {
            var val = Mathf.Clamp(HoldKeyTime / FinishHoldTime, 0, 1);
            HoldGauge.fillAmount = val;
        }
    }

    /// <summary>
    /// 長押しタイム更新
    /// </summary>
    /// <param name="val"></param>
    private void updHoldTime( float val ) {
        HoldKeyTime = val;
        updGauge();
    }


    /// <summary>
    /// 方向キー取得(int)
    /// </summary>
    /// <param name="ana_raw"></param>
    /// <param name="type"></param>
    /// <param name="isHold">長押し可か</param>
    /// <param name="axis_order">方向指定（正負が一致しないなら０を返す）</param>
    /// <returns></returns>
    public static int GetAxisInt(AnalogRawType ana_raw, AxisType type, bool isHold = false, float axis_order = 0f) {

        float axis = GetAxisFloat(ana_raw, type);

        if (Mathf.Abs(axis_order) > 0) {
            if ( Mathf.Sign(axis_order) != Mathf.Sign(axis) ) {
                return 0;
            }
        }

        if (isHold) {
            return Mathf.RoundToInt(axis);
        }
        return GetIntervalAxisProcess(axis);
    }

    public static Vector2 GetAxisVector(AnalogRawType analog_raw = AnalogRawType.DUAL) {

        Vector2 axis = new Vector2();

        if (analog_raw == AnalogRawType.DIGITAL || analog_raw == AnalogRawType.DUAL) {
            axis.x = Input.GetAxisRaw(CmnConfig.GamePadButton.Horizontal.ToString());
            axis.y = Input.GetAxisRaw(CmnConfig.GamePadButton.Vertical.ToString());
        }

        if (analog_raw == AnalogRawType.ANALOG || (analog_raw == AnalogRawType.DUAL && axis == Vector2.zero )) {
            axis.x = Input.GetAxis(CmnConfig.GamePadButton.Horizontal.ToString());
            axis.y = Input.GetAxis(CmnConfig.GamePadButton.Vertical.ToString());
        }

        return axis;
    }

    /// <summary>
    /// 方向取得
    /// </summary>
    /// <param name="analog_raw"></param>
    /// <param name="type"></param>
    /// <returns></returns>
    public static float GetAxisFloat( AnalogRawType analog_raw, AxisType type) {

        float axis = 0f;
        float axis_raw = 0f;

        //if (analog_raw == AnalogRawType.ANALOG || analog_raw == AnalogRawType.DUAL) {
            if (type == AxisType.Dual) {
                axis = Input.GetAxisRaw(CmnConfig.GamePadButton.Horizontal.ToString());
                if (axis == 0) {
                    axis = Input.GetAxisRaw(CmnConfig.GamePadButton.Vertical.ToString());
                }
            } else {
                string line = type == AxisType.Horizontal ?
                CmnConfig.GamePadButton.Horizontal.ToString() : CmnConfig.GamePadButton.Vertical.ToString();
                axis = Input.GetAxisRaw(line);
            }
            
        //}

        //if (analog_raw == AnalogRawType.DIGITAL || analog_raw == AnalogRawType.DUAL) {
        //    if (type == AxisType.Dual) {
        //        axis_raw = Input.GetAxisRaw(CmnConfig.GamePadButton.HorizontalRaw.ToString());
        //        if (axis == 0) {
        //            axis_raw = Input.GetAxisRaw(CmnConfig.GamePadButton.VerticalRaw.ToString());
        //        }
        //    } else {
        //        string line_raw = type == AxisType.Horizontal ?
        //        CmnConfig.GamePadButton.HorizontalRaw.ToString() : CmnConfig.GamePadButton.VerticalRaw.ToString();
        //        axis_raw = Input.GetAxisRaw(line_raw);
        //    }
        //}

        return axis == 0 ? axis_raw : axis;
    }


    /// <summary>
    /// 方向入力共通処理（連続入力防止）
    /// </summary>
    /// <param name="axis"></param>
    /// <returns></returns>
    private static int GetIntervalAxisProcess(float axis) {

        if (IsAxisRest && Mathf.Abs(axis) > RawThresholdMax) {
            IsAxisRest = false;
            return Mathf.RoundToInt(axis);
        }
        if (Mathf.Abs(axis) < RawThresholdMin) {
            IsAxisRest = true;
        }
        return 0;
    }

    /// <summary>
    /// チェックマーク切り替え
    /// Toggleと同様の使い方
    /// </summary>
    /// <param name="val"></param>
    //public void setCheckMark(bool val) {
    ////    if (CheckMark != null) {
    ////        CheckMark.SetActive(val);
    ////    }
    //}

    public bool IsAxisDeal() {
        switch (DealButton) {
            case CmnConfig.GamePadButton.Horizontal:
            case CmnConfig.GamePadButton.Vertical:
            //case CmnConfig.GamePadButton.HorizontalRaw:
            //case CmnConfig.GamePadButton.VerticalRaw:
            case CmnConfig.GamePadButton.DualAxis:
            //case CmnConfig.GamePadButton.AnalogAxis:
            //case CmnConfig.GamePadButton.DigitalAxis:
                return true;
        }
        return false;
    }

    public void setInteractable(bool val){
        interactable = val;
    }

    public void changeHoldType(bool hold) {
        IsHold = hold;
        HoldGauge.gameObject.SetActive(hold);
        updGauge();
    }

    private bool IsInputPriority() {
        if (StandByList.Last() == this && ListRecive == null) {
            return true;
        }
        return false;
    }

}
